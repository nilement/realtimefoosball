import cv2
import numpy as np
import colorsys
import time
import scipy.signal as sp
from matplotlib import pyplot

try:
    from playersInVerticalAngledLine import getPlayersAlongVerticalAngledLine
    from playersInVerticalStraightLine import getPlayersAlongVerticalStraightLine
except ModuleNotFoundError:
    from playersInVerticalAngledLine import getPlayersAlongVerticalAngledLine
    from playersInVerticalStraightLine import getPlayersAlongVerticalStraightLine

blueMaskRGB = np.array([0, 0, 255]) / 255
redMaskRGB = np.array([255, 0, 0]) / 255

formation = [
    (1, blueMaskRGB, "B"),
    (2, blueMaskRGB, "B"),
    (3, redMaskRGB, "R"),
    (5, blueMaskRGB, "B"),
    (5, redMaskRGB, "R"),
    (3, blueMaskRGB, "B"),
    (2, redMaskRGB, "R"),
    (1, redMaskRGB, "R")
]

formationInverted = [
    (1, "R"),
    (2, "R"),
    (3, "B"),
    (5, "R"),
    (5, "B"),
    (3, "R"),
    (2, "B"),
    (1, "B")
]

blueRed = [
    (5, "B"),
    (5, "R")
]


def joinAdjacentMiddle(peaksList):
    print("Length: {}".format(len(peaksList)))
    started = False
    idx = 0
    xs = []
    for i in range(len(peaksList)):
        # if (peaksList[i] < 15):
        #     peaksList[i] = 0
        if peaksList[i] > 0:
            if started == False:
                idx = i
                started = True
            if (peaksList[i-1] != 0 and peaksList[i] != 0):
                peaksList[i] += peaksList[i-1]
                peaksList[i-1] = 0
        else:
            if started == True:
                started = False
                middle = (i + idx) // 2
                peaksList[middle] = peaksList[i-1]
                peaksList[i-1] = 0
                xs.append([middle])
    return xs

def joinAdjacentStartAndEnd(peaksList):
    started = False
    idx = 0
    xs = []
    for i in range(len(peaksList)):
        if (peaksList[i] < 15):
            peaksList[i] = 0
        if peaksList[i] > 0:
            if started == False:
                idx = i
                started = True
            if (peaksList[i-1] != 0 and peaksList[i] != 0):
                peaksList[i] += peaksList[i-1]
                peaksList[i-1] = 0
        else:
            if started == True:
                started = False
                middle = i - idx // 2
                peaksList[middle] = peaksList[i-1]
                peaksList[i-1] = 0
                xs.append([idx, i])
    return xs

def findPlayersAlongLine(barYcoord, width, color, playerCnt):
    redMask = np.array([0, 0.83, 1])
    blueMask = np.array([0.5689, 0.6198, 0.9490])

    blueMaskRGB = np.array([33, 54, 120]) / 255
    redMaskRGB = np.array([165, 30, 30]) / 255
    print("Bar Y: {}".format(barYcoord))
    print("Color : {}".format(color))
    print("Playerscnt: {}".format(playerCnt))
    playerWidth = 20
    barRoi = imgCopy[barYcoord, :]
    gaussBlured = cv2.GaussianBlur(barRoi, (11,11), cv2.BORDER_DEFAULT)
    roiShape = gaussBlured.shape
    print("ROI shape: ", roiShape)
    distances = np.zeros(gaussBlured.shape[0] // playerWidth + 1)
    print("Distances shape: ", distances.shape)
    for i in range(len(barRoi)):
            pixel = barRoi[i]
            barRGB = np.array([pixel[2], pixel[1], pixel[0]], dtype="uint8") / 255
            # print("BarRGB: {}".format(barRGB))            
            # print("ColorMaskRGB: {}".format(colorMaskRGB))
            colorDifference = np.linalg.norm(redMaskRGB - barRGB)
            # print("Difference: {}".format(colorMaskRGB))
            distanceIndex = i // playerWidth
            distances[distanceIndex] += colorDifference

    distances = distances * -1
    pyplot.plot(distances)
    pyplot.ylabel("Mean difference to color mask")
    pyplot.show()
    peaks = sp.find_peaks(distances, height=-5, width=2)
    players = peaks[0][:]
    print("Player coords: {}".format(players))

    displayRoi = imgCopy[barYcoord-5:barYcoord+5, :]

    for i in players:
        cv2.rectangle(displayRoi, ((i * playerWidth) - playerWidth // 2, 0), ((i * playerWidth) + playerWidth // 2, 30), [0, 0, 255], 5)
    cv2.imshow('players', displayRoi)
    cv2.waitKey(0)


def detection(img):
    height, width, _ = img.shape

    newImg = np.zeros((height, width))
    gray = cv2.cvtColor(img,cv2.COLOR_BGR2GRAY)
    edges = cv2.Canny(gray,50,100,apertureSize = 3)

    ignoredRegions = [[88, 167, 109, 476], [216, 286, 116, 460], [0, 53, 2, 570], [459, 509, 29, 570], [703, 748, 108, 463], [795, 862, 106, 463], [909, 990, 0, 570], [0, 0, 0, 0]]
    # while True:
    #     roi = cv2.selectROI(edges) #ROI selection on table
    #     ignoredRegions.append([roi[0], roi[0] + roi[2], roi[1], roi[1] + roi[3]])
    #     k = cv2.waitKey(0) & 0xFF
    #     if (k == 113):  # q is pressed
    #         break

    # print(ignoredRegions)

    minLineLength = 100
    maxLineGap = 10
    lines = cv2.HoughLinesP(edges,1,np.pi/180,100,minLineLength,maxLineGap)
    xses = []
    yses = []
    angles = []
    cnt = 0
    for x in range(0, len(lines)):
        for x1,y1,x2,y2 in lines[x]:
            cnt += 1
            angle = np.arctan2(y2-y1, x2-x1) * 180. / np.pi
            if (angle < -80 and angle > -100 or angle > 80 and angle < 100):
                accepted = True
                for region in ignoredRegions:
                    if (x1 > region[0] and x1 < region[1] and y1> region[2] and y1< region[3]):
                        accepted = False
                        break
                if (accepted):
                    xses.append(x1)
                    yses.append(y1)
                    cv2.line(newImg,(x1,y1),(x2,y2),(255,255,255),2)

    # cv2.imshow("lines", newImg)
    # cv2.waitKey(0) & 0xFF
    
    bin = 75
    groupSize = int(width // bin)

    print("Photo height: ", height)
    print("Photo width: ", width)
    print("Group count: ", bin)
    print("Bar location group size: ", groupSize)
    print("Group size: ", groupSize, "px")
    print("X1's found: ", len(xses))
    peaks = []
    groups = np.zeros((width // groupSize + 1))
    for i in range(len(xses)):
        idx = xses[i] // groupSize
        groups[idx] += 1

    return joinAdjacentMiddle(groups)


def main():
    # video = cv2.VideoCapture(0)
    # video.set(cv2.CAP_PROP_FOURCC, cv2.VideoWriter_fourcc('M','J','P','G'))
    # video.set(cv2.CAP_PROP_FRAME_WIDTH, 1280)
    # video.set(cv2.CAP_PROP_FRAME_HEIGHT, 720)
    # video.set(cv2.CAP_PROP_FPS, 60)
    # time.sleep(5)
    # img = []
    # while True:
    #     rec, img2 = video.read()
    #     cv2.imshow("g", img2)
    #     if (cv2.waitKey(1) & 0xFF == ord('q')):
    #         img = img2
    #         break
    tableLeftX = 160
    tableRightX = 1150
    tableTopY = 80
    tableBottomY = 650

    img = cv2.imread("../test_data/capture.jpg")
    img = img[tableTopY:tableBottomY, tableLeftX:tableRightX]

    detection(img)

    # groupSize = 12
    # lines = detection(img)
    # print(lines)
    # playerWidth = 5
    
    # imgCopy = img.copy()

    # for i in lines:
    #     cv2.line(imgCopy,(i[0] * groupSize, 0),(i[0] * groupSize, 553),(0,0,255),2)

    # cv2.imshow('Input', imgCopy)
    # cv2.waitKey(0)

    # for i in range(len(lines)):
    #     heightlocal = lines[i]
    #     # print(heightlocal[0] * groupSize)
    #     # players = getPlayersAlongVerticalAngledLine(img, lines[i][0] * groupSize, lines[i][0] * groupSize + 10, formation[i][1])
    #     players = getPlayersAlongVerticalStraightLine(img, heightlocal[0] * groupSize, formation[i][1])
    #     print(players)
    #     for y in players:
    #         cv2.rectangle(img, (heightlocal[0] * groupSize, y*playerWidth-10), (heightlocal[0] * groupSize, y*playerWidth+10), [0, 0, 255], 5)

    # cv2.imshow('Input', img)
    # cv2.waitKey(0)


main()