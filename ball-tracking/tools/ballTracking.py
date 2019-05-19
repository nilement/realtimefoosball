import cv2 as cv
import numpy as np
import time
import random
import asyncio
import argparse
import queue
import math

from houghBarsHorizontal import detection as horizontalDetection
from houghBarsVideo import findAllPlayers
from numpy.linalg import inv
from playsound import playsound
from multiprocessing import Process, Pool

lowerBallMaskHSV = np.array([15, 120, 130], dtype="uint8")
upperBallMaskHSV = np.array([20, 255, 255], dtype="uint8")
redGoalLineStart = [26, 218]
redGoalLineEnd = [26, 355]
blueGoalLineStart = [905, 218]
blueGoalLineEnd = [905, 355]
showVideo = False

class KalmanFilter:
    kf = cv.KalmanFilter(4,2)
    kf.measurementMatrix = np.array([[1, 0, 0, 0], [0, 1, 0, 0]], np.float32)
    kf.transitionMatrix = np.array([[1, 0, 1, 0], [0, 1, 0, 1], [0, 0, 1, 0], [0, 0, 0,1]], np.float32)

    def Estimate(self, coordX, coordY):
        measured = np.array([[np.float32(coordX)], [np.float32(coordY)]])
        self.kf.correct(measured)
        predicted = self.kf.predict()
        return predicted

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
    def MatchTemplate(self, image, template):
        result = cv.matchTemplate(image, template, cv.TM_CCOEFF)
        return cv.minMaxLoc(result)

    def Track(self, video):
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
                
                self.CheckForGoal(ballX, ballY)
                # goal = random.randint(0,100)
                # queue.put("R")
                # if goal < 10:
                # print("putting")
                # asyncio.create_task(callback("R"))
                # elif goal > 10 and goal < 20:
                # await callback("B")


    def DebugTrack(self, video, goalsQueue, eventsQueue):
        width = int(video.get(3))
        height = int(video.get(4))
        # template = cv.imread('./data/ballTemplate.png')
        if showVideo:
            cv.namedWindow("Input")
            cv.setMouseCallback("Input", getMousePosition)
        adaptiveBoxSizing = 100 # total square box edge = * 2

        kfObj = MyKalmanFilter()
        predictedCoords = np.zeros((2,1), np.float32)

        tryAdaptive = False
        ballDetected = False
        usePrediction = False

        ballX = 0
        ballY = 0

        topSpeed = 0

        tableLeftX = 160
        tableRightX = 1150
        tableTopY = 80
        tableBottomY = 650

        groupSize = 13
        playerWidth = 5

        adaptiveFrames = 0
        fullTableFrames = 0
        totalNotFoundFrames = 0
        notFoundFrames = 0
        predictedFrames = 0

        start_time = time.time()
        x = 1 
        counter = 0

        rc, frame = video.read()

        frame = frame[tableTopY:tableBottomY, tableLeftX:tableRightX]

        lines = horizontalDetection(frame)

        with Pool(5) as pool:
            while(video.isOpened()):
                counter+=1
                if (time.time() - start_time) > x :
                    print("FPS: {} , full frames: {} , adaptive: {} , not found: {}".format(counter / (time.time() - start_time), fullTableFrames, adaptiveFrames, totalNotFoundFrames))
                    counter = 0
                    start_time = time.time()

                rc, frame = video.read()

                if (rc == True):
                    frame = frame[tableTopY:tableBottomY, tableLeftX:tableRightX]
                    players = findAllPlayers(frame, lines, groupSize, pool)
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
                            ballDetected = True
                            adaptiveFrames += 1
                            notFoundFrames = 0
                            [ballX, ballY] = coords
                        else:
                            fullScan = self.DetectBall(frame)
                            if (fullScan):
                                ballDetected = True
                                fullTableFrames += 1
                                notFoundFrames = 0
                                [ballX, ballY] = fullScan
                            else:
                                # ballX = int(kfObj.Xk[0])
                                # ballY = int(kfObj.Xk[2])
                                ballDetected = False
                                totalNotFoundFrames += 1
                                notFoundFrames += 1
                                predictedFrames += 1
                                if notFoundFrames > 10:
                                    notFoundFrames = 0
                                    # tryAdaptive = False
                                    # usePrediction = False
                    else:
                        vals = self.DetectBall(frame)
                        if (len(vals) == 2):
                            ballDetected = True
                            fullTableFrames += 1
                            tryAdaptive = True
                            usePrediction = True
                            [ballX, ballY] = vals
                        else:
                            ballDetected = False
                            totalNotFoundFrames += 1
                            notFoundFrames += 1
                            if notFoundFrames > 10:
                                notFoundFrames = 0
                                # usePrediction = False


                    kfObj.Estimate(ballX, ballY) 
                    predictedCoords.append(kfObj.Xk[0])
                    predictedCoords.append(kfObj.Xk[2])

                    goalSide = self.CheckForGoal(ballX, ballY)
                    if goalSide is not None:
                        goalsQueue.put(goalSide)
                    #self.RegisterGoal(callback)

                    if showVideo:
                        cv.rectangle(frame, (predictedCoords[0]-adaptiveBoxSizing, predictedCoords[1]+adaptiveBoxSizing), (predictedCoords[0]+adaptiveBoxSizing, predictedCoords[1]-adaptiveBoxSizing), [255, 0, 0], 5)
                        xabs = abs(kfObj.Xk[1])
                        yabs = abs(kfObj.Xk[3])
                        nowspeed = math.sqrt(xabs**2 + yabs**2)
                        if nowspeed > topSpeed:
                            topSpeed = nowspeed
                        speedStringX = "X Speed is: {}".format(xabs)
                        speedStringY = "Y Speed is: {}".format(yabs)
                        cv.putText(frame, speedStringX, (10, 10), cv.FONT_HERSHEY_SIMPLEX, 0.5, [0, 0, 0])
                        cv.putText(frame, speedStringY, (10, 30), cv.FONT_HERSHEY_SIMPLEX, 0.5, [0, 0, 0])
                        cv.putText(frame, "Top speed: {}".format(topSpeed), (10, 50), cv.FONT_HERSHEY_SIMPLEX, 0.5, [0, 0, 0])
                        if ballDetected:
                            self.drawBallCircle(frame, ballX, ballY, predictedCoords[0], predictedCoords[1], adaptiveBoxSizing)
                        if usePrediction:
                            cv.circle(frame, (predictedCoords[0], predictedCoords[1]), 20, [0,255,255], 2, 8)
                            cv.line(frame, (predictedCoords[0] + 16, predictedCoords[1] - 15), (predictedCoords[0] + 50, predictedCoords[1] - 30), [100, 10, 255], 2, 8)
                            cv.putText(frame, "Predicted", (predictedCoords[0] + 50, predictedCoords[1] - 30), cv.FONT_HERSHEY_SIMPLEX, 0.5, [50, 200, 250])
                        self.drawGoalLines(frame)
                        self.drawBarLines(frame, lines, groupSize)
                        for i in range(len(players)):
                            bar = players[i].get()
                            for y in bar:
                                xIdx = lines[i][0] * groupSize
                                cv.rectangle(frame, (xIdx - 5, y*playerWidth-10), (xIdx + 5, y*playerWidth+10), [0, 0, 255], 5)
                        cv.imshow('Input', frame)
                    if (cv.waitKey(1) & 0xFF == ord('q')):
                        break
                    
                    if eventsQueue.empty():
                        pass
                    else:
                        message = eventsQueue.get()
                        if message == "stop":
                            video.release()
                            cv.destroyAllWindows()
                            return "stop"
                        elif message == "exit":
                            video.release()
                            cv.destroyAllWindows()
                            return "exit"  
        
        video.release()
        cv.destroyAllWindows()

    def drawBarLines(self, image, lines, groupSize):
        for i in lines:
            cv.line(image, (i[0] * groupSize, 0), (i[0] * groupSize, 553), (0,0,255), 2)

    def drawBallCircle(self, image, x, y, kalmanX, kalmanY, adaptiveSize):
        cv.circle(image, (x, y), 20, [0,0,255], 2, 8)
        cv.line(image, (x, y + 20), (x + 50, y + 20), [100, 100, 255], 2, 8)
        cv.putText(image, "Actual", (x + 50, y + 20), cv.FONT_HERSHEY_SIMPLEX, 0.5, [50,200,250])

    def drawGoalLines(self, image):
        cv.line(image, (redGoalLineStart[0], redGoalLineStart[1]), (redGoalLineEnd[0], redGoalLineEnd[1]), [100, 0, 0], 2, 8)
        cv.line(image, (blueGoalLineStart[0], blueGoalLineStart[1]), (blueGoalLineEnd[0], blueGoalLineEnd[1]), [100, 0, 0], 2, 8)

    def CheckForGoal(self, ballX, ballY):
        xRangeDiff = 20
        if (ballX > redGoalLineStart[0] - xRangeDiff and ballX < redGoalLineEnd[0] + xRangeDiff  and ballY > redGoalLineStart[1] and ballY < redGoalLineEnd[1]):
            # playsound("../data/goal.mp3")
            return "B"
        elif (ballX > blueGoalLineStart[0] - xRangeDiff and ballX < blueGoalLineEnd[0] + xRangeDiff  and ballY > blueGoalLineStart[1] and ballY < blueGoalLineEnd[1]):
            # playsound("../data/goal.mp3")
            return "R"
        else:
            return None

    def DetectBall(self, image):
        hsvframe = cv.cvtColor(image, cv.COLOR_BGR2HSV)
        ballMask = cv.inRange(hsvframe, lowerBallMaskHSV, upperBallMaskHSV)
        kernel = np.ones((3,3), np.uint8)
        orangeMaskDilated = cv.erode(ballMask, kernel, 1)
        cv.imshow('thresholdedfull', orangeMaskDilated)

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

        hsvframe = cv.cvtColor(croped, cv.COLOR_BGR2HSV)
        ballMask = cv.inRange(hsvframe, lowerBallMaskHSV, upperBallMaskHSV)
        kernel = np.ones((3,3), np.uint8)
        orangeMaskDilated = cv.erode(ballMask, kernel)
        cv.imshow('thresholded', orangeMaskDilated)

        [nlabels, labels, stats, centroids] = cv.connectedComponentsWithStats(orangeMaskDilated, 8, cv.CV_32S)

        if (stats.shape[0] == 1):
            return []

        stats = np.delete(stats, (0), axis = 0)
        maxBlobIdx_i, maxBlobIdx_j = np.unravel_index(stats.argmax(), stats.shape)

        ballX = stats[maxBlobIdx_i, 0] + (stats[maxBlobIdx_i, 2]/2) + leftX # adaptive window offset
        ballY = stats[maxBlobIdx_i, 1] + (stats[maxBlobIdx_i, 3]/2) + topY

        # print("Adaptive, BallX: {} BallY: {}".format(ballX, ballY))

        return [int(ballX), int(ballY)]

    def DetectWithTemplateMatching(self, image, leftX, topY, sizing, template):
        rightX = leftX + sizing*2
        bottomY = topY + sizing*2
        croped = image[topY:bottomY, leftX:rightX]
        if showVideo:
            cv.imshow('adaptiveWindow', croped)

        ballMask = cv.inRange(croped, lowerBallMaskHSV, upperBallMaskHSV)
        kernel = np.ones((5,5), np.uint8)
        orangeMaskDilated = cv.dilate(ballMask, kernel)

        res = cv.matchTemplate(croped, template, cv.TM_CCOEFF)
        min_val, max_val, min_loc, max_loc = cv.minMaxLoc(res)
        top_left = max_loc
        return [top_left[0] + leftX, top_left[1] + topY]
        # print(max_val)

def main(goalsQueue, eventsQueue):
    parser = argparse.ArgumentParser()
    parser.add_argument("--showVideo", action="store_true")
    args, leftovers = parser.parse_known_args()
    # global showVideo
    # showVideo = True
    if args.showVideo is True:
        global showVideo
        showVideo = True
    vid = cv.VideoCapture('../data/officeRelease.mp4')
    # vid = cv.VideoCapture(0)
    # vid.set(cv.CAP_PROP_FOURCC, cv.VideoWriter_fourcc('M','J','P','G'))
    # vid.set(cv.CAP_PROP_FRAME_WIDTH, 1280)
    # vid.set(cv.CAP_PROP_FRAME_HEIGHT, 720)
    # vid.set(cv.CAP_PROP_FPS, 60)
    processor = Processor()
    if __debug__:
        return processor.DebugTrack(vid, goalsQueue, eventsQueue)
    else:
        processor.Track(vid)


def testingFromFile():
    coordsFile = "locations.txt"
    data = []
    with open(coordsFile) as f:
        for line in f:
            coords = line.strip('\n').split(' ')
            coords = list(map(int, coords))
            data.append(coords)
    filter = MyKalmanFilter()
    openCvFilter = KalmanFilter()
    filter.instantiate(data[0][0], 1, data[0][1], 1)
    cnter = 0
    for coord in data[1:]:
        cvPrediction = openCvFilter.Estimate(coord[0], coord[1])
        filter.correct(coord)
        filter.predict()
        print("Actual x: ", coord[0], " y: ", coord[1])
        print("Predicted x: ", filter.Xk[0], " y: ", filter.Xk[2])
        print("OpenCv x: ", cvPrediction[0], " y: ", cvPrediction[1])        

def getMousePosition(event, x, y, flags, param):
    if event == cv.EVENT_LBUTTONDOWN:
        print("x: {} y: {}".format(x, y))

async def fakeCallback(arg):
    pass

def fakeMain():
    eventsQueue = queue.Queue()
    goalsQueue = queue.Queue()
    main(goalsQueue, eventsQueue)

if __name__ == '__main__':
    fakeMain()