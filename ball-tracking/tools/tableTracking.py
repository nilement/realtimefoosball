import cv2 as cv
import numpy as np
import time
import random
import asyncio
import argparse
import sys
from numpy.linalg import inv
from houghBarsVideo import detection, playersInFrame
import multiprocessing
from multiprocessing import Process, Pool

lowerBallMaskHSV = np.array([0, 89, 159], dtype="uint8")
upperBallMaskHSV = np.array([63, 222, 255], dtype="uint8")
redGoalLineStart = [250, 230]
redGoalLineEnd = [250, 480]
blueGoalLineStart = [940, 300]
blueGoalLineEnd = [940, 500]
showVideo = False

class MyKalmanFilter:
    sd = 1e-5
    ax = 0
    ay = 0
    Xk = np.array([[0, 0, 0, 0]]).transpose()
    uk = np.array([[ax], [ay]])
    dt = 1
    A = np.array([[1, dt, 0, 0], [0, 1, 0, 0], [0, 0, 1, dt], [0, 0, 0, 1]], np.float32)
    B = np.array([[dt*dt/2, 0], [dt*dt/2, 0], [1, 0], [0, 1]])
    P = np.identity(4) * sd
    R = np.identity(2) * sd
    H = np.array([[1,0,0,0],[0,0,1,0]])
    Q = np.identity(4) * 0.2
    def instantiate(self, x, xv, y, yv):
        self.Xk[0] = x
        self.Xk[1] = xv
        self.Xk[2] = y
        self.Xk[3] = yv
    def Estimate(self, coordX, coordY):
        self.correct([coordX, coordY])
        self.predict()

    def predict(self):
        self.Xk = self.A.dot(self.Xk)
        self.P = self.A.dot(self.P).dot(self.A.T) + self.Q 
    def correct(self, data):
        data = np.array([data]).T
        S = self.H.dot(self.P).dot(self.H.T) + self.R
        self.K = self.P.dot(self.H.T).dot(inv(S))
        self.Xk = self.Xk + self.K.dot(data - self.H.dot(self.Xk)) 
        I = np.identity(len(self.K))
        self.P = (I - self.K.dot(self.H)).dot(self.P)

class Processor:
    def Track(self, video):
        print("reading")
        width = int(video.get(3))
        height = int(video.get(4))
        adaptiveBoxSizing = 200 # total square box edge = * 2
        kfObj = MyKalmanFilter()
        predictedCoords = np.zeros((2,1), np.float32)
        tryAdaptive = False
        ballX = 0
        ballY = 0

        while(video.isOpened()):
            rc, frame = video.read()
            if (rc == True):
                predictedCoords = []
                if (tryAdaptive):
                    leftX = int(kfObj.Xk[0]) - adaptiveBoxSizing
                    topY = int(kfObj.Xk[2]) - adaptiveBoxSizing
                    if (leftX < 0):
                        leftX = 0
                    if (topY < 0):
                        topY = 0
                    coords = self.DetectBallInAdaptiveBox(frame, leftX, topY, adaptiveBoxSizing)
                    if (coords):
                        [ballX, ballY] = coords
                    else:
                        fullScan = self.DetectBall(frame)
                        if (fullScan):
                            [ballX, ballY] = fullScan
                        else:
                            ballX = int(kfObj.Xk[0])
                            ballY = int(kfObj.Xk[2])
                else:
                    vals = self.DetectBall(frame)
                    if (len(vals) == 2):
                        tryAdaptive = True
                        [ballX, ballY] = vals
                kfObj.Estimate(ballX, ballY) 
                predictedCoords.append(kfObj.Xk[0])
                predictedCoords.append(kfObj.Xk[2])
                
                playersInFrame(frame, lines)

                self.CheckForGoal(ballX, ballY)
                # goal = random.randint(0,100)
                # queue.put("R")
                # if goal < 10:
                # print("putting")
                # asyncio.create_task(callback("R"))
                # elif goal > 10 and goal < 20:
                # await callback("B")

    async def DebugTrack(self, video, callback):
        width = int(video.get(3))
        height = int(video.get(4))
        if showVideo:
            cv.namedWindow("Input")
        adaptiveBoxSizing = 100 # total square box edge = * 2

        kfObj = MyKalmanFilter()
        predictedCoords = np.zeros((2,1), np.float32)
        tryAdaptive = False
        ballDetected = True
        ballX = 0
        ballY = 0
        adaptiveFrames = 0
        fullTableFrames = 0
        notFoundFrames = 0

        ballCoords = []

        start_time = time.time()
        ballcoordsTime = time.time()
        x = 1 
        counter = 0

        r1, frame1 = video.read()
        lines = detection(frame1)

        with Pool(2) as pool:
            while(video.isOpened()):
                counter+=1
                if (time.time() - start_time) > x :
                    print("FPS: {} , full frames: {} , adaptive: {} , not found: {}".format(counter / (time.time() - start_time), fullTableFrames, adaptiveFrames, notFoundFrames))
                    counter = 0
                    start_time = time.time()

                rc, frame = video.read()

                if (rc == True):
                    # players = pool.apply_async(playersInFrame, args=(frame, lines,))
                    predictedCoords = []
                    if (tryAdaptive):
                        leftX = int(kfObj.Xk[0]) - adaptiveBoxSizing
                        topY = int(kfObj.Xk[2]) - adaptiveBoxSizing
                        if (leftX < 0):
                            leftX = 0
                        if (topY < 0):
                            topY = 0
                        coords = self.DetectBallInAdaptiveBox(frame, leftX, topY, adaptiveBoxSizing)
                        if (coords):
                            adaptiveFrames += 1
                            [ballX, ballY] = coords
                        else:
                            fullScan = self.DetectBall(frame)
                            if (fullScan):
                                fullTableFrames += 1
                                [ballX, ballY] = fullScan
                            else:
                                notFoundFrames += 1
                                ballX = int(kfObj.Xk[0])
                                ballY = int(kfObj.Xk[2])
                    else:
                        fullTableFrames += 1
                        vals = self.DetectBall(frame)
                        if (len(vals) == 2):
                            tryAdaptive = True
                            [ballX, ballY] = vals


                    kfObj.Estimate(ballX, ballY) 
                    predictedCoords.append(kfObj.Xk[0])
                    predictedCoords.append(kfObj.Xk[2])

                    self.CheckForGoal(ballX, ballY)
                    await self.RegisterGoal(callback)

                    if (time.time() - ballcoordsTime) > 0.1: 
                        ballCoords.append([ballX, ballY])
                        ballcoordsTime = time.time()

                    # players.get()

                    if showVideo:
                        cv.circle(frame, (ballX, ballY), 20, [0,0,255], 2, 8)
                        cv.line(frame, (ballX, ballY + 20), (ballX + 50, ballY + 20), [100, 100, 255], 2, 8)
                        cv.rectangle(frame, (predictedCoords[0]-adaptiveBoxSizing, predictedCoords[1]+adaptiveBoxSizing), (predictedCoords[0]+adaptiveBoxSizing, predictedCoords[1]-adaptiveBoxSizing), [255, 0, 0], 5)
                        cv.putText(frame, "Actual", (ballX + 50, ballY + 20), cv.FONT_HERSHEY_SIMPLEX,0.5, [50,200,250])

                        cv.circle(frame, (predictedCoords[0], predictedCoords[1]), 20, [0,255,255], 2, 8)
                        cv.line(frame, (predictedCoords[0] + 16, predictedCoords[1] - 15), (predictedCoords[0] + 50, predictedCoords[1] - 30), [100, 10, 255], 2, 8)
                        # cv.line(frame, (redGoalLineStart[0], redGoalLineStart[1]), (redGoalLineEnd[0], redGoalLineEnd[1]), [100, 0, 0], 2, 8)
                        # cv.line(frame, (blueGoalLineStart[0], blueGoalLineStart[1]), (blueGoalLineEnd[0], blueGoalLineEnd[1]), [100, 0, 0], 2, 8)
                        cv.putText(frame, "Predicted", (predictedCoords[0] + 50, predictedCoords[1] - 30), cv.FONT_HERSHEY_SIMPLEX, 0.5, [50, 200, 250])
                        cv.imshow('Input', frame)
                        #matching = self.MatchTemplate(frame, ball)
                        #cv.circle(frame, (int(matching[3][0]), int(matching[3][1])), 20, [0,255,255], 2, 8)
                    if (cv.waitKey(10) & 0xFF == ord('q')):
                        print(ballCoords)
                        break
        
        video.release()
        cv.destroyAllWindows()

    def ballDetection():
        predictedCoords = []
        if (tryAdaptive):
            leftX = int(kfObj.Xk[0]) - adaptiveBoxSizing
            topY = int(kfObj.Xk[2]) - adaptiveBoxSizing
            if (leftX < 0):
                leftX = 0
            if (topY < 0):
                topY = 0
            coords = self.DetectBallInAdaptiveBox(frame, leftX, topY, adaptiveBoxSizing)
            if (coords):
                adaptiveFrames += 1
                [ballX, ballY] = coords
            else:
                fullScan = self.DetectBall(frame)
                if (fullScan):
                    fullTableFrames += 1
                    [ballX, ballY] = fullScan
                else:
                    notFoundFrames += 1
                    ballX = int(kfObj.Xk[0])
                    ballY = int(kfObj.Xk[2])
        else:
            fullTableFrames += 1
            vals = self.DetectBall(frame)
            if (len(vals) == 2):
                tryAdaptive = True
                [ballX, ballY] = vals


        kfObj.Estimate(ballX, ballY) 
        predictedCoords.append(kfObj.Xk[0])
        predictedCoords.append(kfObj.Xk[2])

    async def RegisterGoal(self, callback):
        goal = random.randint(0,3000)
        if goal < 10:
            print("putting")
            asyncio.create_task(callback("R"))
            await asyncio.sleep(0)
        elif goal > 10 and goal < 20:
            asyncio.create_task(callback("B"))
            await asyncio.sleep(0)

    def CheckForGoal(self, ballX, ballY):
        xRangeDiff = 20
        # print("Ball X: {} Ball Y: {} RedGoalStart X: {} RedGoalStartY: {} RedGoalEnd X: {} RedGoalEnd Y: {}".format(ballX, ballY, redGoalLineStart[0], redGoalLineStart[1], redGoalLineEnd[0], redGoalLineEnd[1]))
        if (ballX > redGoalLineStart[0] - xRangeDiff and ballX < redGoalLineStart[0] + xRangeDiff  and ballY > redGoalLineStart[1] and ballY < redGoalLineEnd[1]):
            print("Blue goal")

    def DetectBall(self, image):
        ballMask = cv.inRange(image, lowerBallMaskHSV, upperBallMaskHSV)
        kernel = np.ones((5,5), np.uint8)
        orangeMaskDilated = cv.dilate(ballMask, kernel)
        if showVideo:
            cv.imshow('thresholded', orangeMaskDilated)

        [nlabels, labels, stats, centroids] = cv.connectedComponentsWithStats(orangeMaskDilated, 8, cv.CV_32S)

        if (stats.shape[0] == 1):
            return []

        stats = np.delete(stats, (0), axis = 0)
        maxBlobIdx_i, maxBlobIdx_j = np.unravel_index(stats.argmax(), stats.shape)

        ballX = stats[maxBlobIdx_i, 0] + (stats[maxBlobIdx_i, 2]/2)
        ballY = stats[maxBlobIdx_i, 1] + (stats[maxBlobIdx_i, 3]/2)

        return [int(ballX), int(ballY)]

    def DetectBallInAdaptiveBox(self, image, leftX, topY, sizing):
        rightX = leftX + sizing*2
        bottomY = topY + sizing*2
        croped = image[topY:bottomY, leftX:rightX]
        if croped.size <= 0:
            return []

        if showVideo:
            cv.imshow('adaptiveWindow', croped)

        ballMask = cv.inRange(croped, lowerBallMaskHSV, upperBallMaskHSV)
        kernel = np.ones((5,5), np.uint8)
        orangeMaskDilated = cv.dilate(ballMask, kernel)

        [nlabels, labels, stats, centroids] = cv.connectedComponentsWithStats(orangeMaskDilated, 8, cv.CV_32S)

        if (stats.shape[0] == 1):
            return []

        stats = np.delete(stats, (0), axis = 0)
        maxBlobIdx_i, maxBlobIdx_j = np.unravel_index(stats.argmax(), stats.shape)

        ballX = stats[maxBlobIdx_i, 0] + (stats[maxBlobIdx_i, 2]/2) + leftX # adaptive window offset
        ballY = stats[maxBlobIdx_i, 1] + (stats[maxBlobIdx_i, 3]/2) + topY

        return [int(ballX), int(ballY)]

async def main(callback):
    parser = argparse.ArgumentParser()
    parser.add_argument("--showVideo", action="store_true")
    args, leftovers = parser.parse_known_args()
    if args.showVideo is True:
        global showVideo
        showVideo = True
    vid = cv.VideoCapture(0)
    vid.set(cv.CAP_PROP_FPS, 60)
    # vid = cv.VideoCapture('../data/vid.mp4')
    processor = Processor()
    if __debug__:
        await processor.DebugTrack(vid, callback)
    else:
        processor.Track(vid)
 

def getMousePosition(event, x, y, flags, param):
    if event == cv.EVENT_LBUTTONDOWN:
        # print
        print("x: {} y: {}".format(x, y))

async def fakeCallback(arg):
    pass

def fakeMain():
    asyncio.get_event_loop().run_until_complete(main(fakeCallback))

if __name__ == '__main__':
    fakeMain()