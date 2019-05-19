import cv2
import numpy as np
import colorsys
import pickle
import time
import scipy.signal as sp
from matplotlib import pyplot
from jenksBreaks import getBreaks
from playersInAngledLine import getPlayersAlongAngledLine

formation = [
    (1, "B"),
    (2, "B"),
    (3, "R"),
    (5, "B"),
    (5, "R"),
    (3, "B"),
    (2, "R"),
    (1, "R")
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

def writeIgnoredRegionsToFile(regions):
    regionsName = 'ignored' + tableFileName
    with open("../data/" + regionsName, 'wb') as f:
        pickle.dump(regions, f)

def joinAdjacentMiddle(peaksList):
    started = False
    idx = 0
    ys = []
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
                ys.append([i, idx])
    return ys

def joinAdjacent(peaksList):
    for i in range(len(peaksList)):
        if (peaksList[i] < 15):
            peaksList[i] = 0
        if (i > 0):
            if (peaksList[i-1] != 0 and peaksList[i] != 0):
                peaksList[i] += peaksList[i-1]
                peaksList[i-1] = 0

def getPlayersAlongLine(barYcoord, color, playerCnt):
    blueMaskRGB = np.array([33, 54, 120]) / 255
    redMaskRGB = np.array([165, 30, 30]) / 255
    if color == "B":
        colorMaskRGB = blueMaskRGB
    else:
        colorMaskRGB = redMaskRGB
    print("Bar Y: {}".format(barYcoord))
    print("Color : {}".format(color))
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
            colorDifference = np.linalg.norm(colorMaskRGB - barRGB)
            distanceIndex = i // playerWidth
            distances[distanceIndex] += colorDifference

    distances = distances * -1
    # pyplot.plot(distances)
    # pyplot.ylabel("Mean difference to color mask")
    # pyplot.show()
    peaks = sp.find_peaks(distances, height=-5, width=2)
    players = peaks[0][:]
    print("Player coords: {}".format(players))
    return players
    # displayRoi = imgCopy[barYcoord-5:barYcoord+5, :]

    # for i in players:
    #     cv2.rectangle(displayRoi, ((i * playerWidth) - playerWidth // 2, 0), ((i * playerWidth) + playerWidth // 2, 30), [0, 0, 255], 5)
    # cv2.imshow('players', displayRoi)
    # cv2.waitKey(0)


def findPlayersAlongLine(barYcoord, width, color, playerCnt):
    redMask = np.array([0, 0.83, 1])
    blueMask = np.array([0.5689, 0.6198, 0.9490])

    blueMaskRGB = np.array([33, 54, 120]) / 255
    redMaskRGB = np.array([165, 30, 30]) / 255
    print("Bar Y: {}".format(barYcoord))
    print("Color : {}".format(color))
    print("Playerscnt: {}".format(playerCnt))
    # blueMaskUpper = np.array([110, 190, 200], dtype="uint16")
    # blueMaskMean = ((blueMaskLower + blueMaskUpper) / 2) / 255
        # print(blueMaskMean)
    # if color == "B":
    #     colorMaskRGB = colorsys.hsv_to_rgb(blueMask[0], blueMask[1], blueMask[2])
    # else:
    #     colorMaskRGB = colorsys.hsv_to_rgb(redMask[0], redMask[1], redMask[2])
    playerWidth = 20
    barRoi = imgCopy[barYcoord, :]
    # print(barRoi)
    gaussBlured = cv2.GaussianBlur(barRoi, (11,11), cv2.BORDER_DEFAULT)
    roiShape = gaussBlured.shape
    print("ROI shape: ", roiShape)
    distances = np.zeros(gaussBlured.shape[0] // playerWidth + 1)
    print("Distances shape: ", distances.shape)
    # print("Colormask: {}".format(colorMaskRGB))
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


def findPlayers(barYcoordStart, barYcoordEnd, width, color, playerCnt):
    #Image is in BGR
    print("Color: {}".format(color))
    print("Players: {}".format(playerCnt))
    # redMaskLower = np.array([121, 152,130], dtype="uint16")
    # redMaskUpper = np.array([213, 255, 232], dtype="uint16")
    redMask = np.array([0, 0.83, 1])
    blueMask = np.array([0.5689, 0.6198, 0.9490])
    # blueMaskUpper = np.array([110, 190, 200], dtype="uint16")
    # blueMaskMean = ((blueMaskLower + blueMaskUpper) / 2) / 255
    # print(blueMaskMean)
    if color == "B":
        colorMaskRGB = colorsys.hsv_to_rgb(blueMask[0], blueMask[1], blueMask[2])
    else:
        colorMaskRGB = colorsys.hsv_to_rgb(redMask[0], redMask[1], redMask[2])
    playerWidth = 10
    barRoi = imgCopy[barYcoordStart:barYcoordEnd, 0:width]
    roiShape = barRoi.shape
    gaussBlured = cv2.GaussianBlur(barRoi, (11,11), cv2.BORDER_DEFAULT)
    print("ROI shape: ", roiShape)
    distances = np.zeros(barRoi.shape[1] // playerWidth + 1)
    print("Distances shape: ", distances.shape)
    print("Length of strip: {}".format(len(barRoi[0])))
    print("Colormask: {}".format(colorMaskRGB))
    for i in range(len(barRoi)):
        for pixel in range(len(barRoi[i])):
            # if pixel % playerWidth == 0:
                # print(pixel)
                # barRoi[i, pixel] = [0, 0, 0]
            bar = barRoi[i, pixel]
            barRGB = np.array([bar[2], bar[1], bar[0]], dtype="uint8") / 255
            colorDifference = np.linalg.norm(barRGB - colorMaskRGB)
            distanceIndex = pixel // playerWidth
            distances[distanceIndex] += colorDifference

    # print("Pixel at: {}".format(barRoi[1][200]))
    # print(distances)
    # cv.circle(barRoi, (barRoi[1][20][0], predictedCoords[1]), 20, [0,255,255], 2, 8)
    # cv2.rectangle(imgCopy, (1, barYcoordStart), (1080, barYcoordEnd), [0, 0, 255], 5)
    distances = distances * -1
    pyplot.plot(distances)
    pyplot.ylabel("Mean difference to color mask")
    pyplot.show()
    peaks = sp.find_peaks(distances, height=-250, width=2)
    players = peaks[0][0:playerCnt]
    print("Player coords: {}".format(players))
    for i in players:
        cv2.rectangle(barRoi, ((i * playerWidth) - playerWidth // 2, 0), ((i * playerWidth) + playerWidth // 2, 30), [0, 0, 255], 5)
    cv2.imshow('players', barRoi)
    cv2.waitKey(0)

# tableFileName = '../data/table/tablenew/tablenewcroped.png'
# tableFileName = '../data/table/decentcroped.png'
video = cv2.VideoCapture(0)
# time.sleep(5)
img = []
while True:
    rec, img2 = video.read()
    cv2.imshow("g", img2)
    if (cv2.waitKey(1) & 0xFF == ord('q')):
        img = img2
        break



# img = cv2.imread(tableFileName)
imgCopy = img.copy()
height, width, _ = img.shape

cv2.waitKey(0)

# ignoredRegions = [[25, 1330, 1055, 77],[196, 986, 6, 67], [218, 893, 118, 201], [214, 952, 254, 335], [155, 944, 577, 671], [266, 312, 1030, 1055], [312, 516, 1040, 1058], [636, 825, 1038, 1065], [554, 648, 1039, 1063], [307, 794, 1211, 1266], [255, 360, 1303, 1376], [658, 746, 1326, 1370], [749, 981, 1125, 1245], [0, 0, 0, 0], [2, 1, 1078, 60]]
# ignoredRegions = [[121, 999, 1179, 1215], [3, 1001, 0, 66], [339, 956, 49, 91], [723, 946, 66, 107], [262, 678, 154, 215], [191, 777, 295, 356], [70, 986, 609, 664], [187, 778, 902, 963], [217, 767, 1038, 1099], [320, 1003, 1171, 1215], [0, 0, 0, 0]] #tableCroped
# ignoredRegions = [[199, 792, 302, 384], [290, 701, 166, 233], [240, 940, 15, 84], [14, 437, 461, 520], [9, 1006, 602, 666], [164, 799, 900, 962], [198, 788, 1041, 1093], [305, 1015, 1161, 1215], [0, 0, 0, 0], [0, 999, 0, 70], [596, 936, 43, 98], [339, 631, 31, 96], [0, 0, 0, 0]]
# ignoredRegions = [[215, 650, 75, 126], [152, 717, 201, 249], [55, 814, 451, 527], [158, 722, 771, 819], [156, 723, 894, 961], [250, 920, 1022, 1070], [0, 0, 0, 0]]
ignoredRegions = []

newImg = np.zeros((height, width))
gray = cv2.cvtColor(img,cv2.COLOR_BGR2GRAY)


edges = cv2.Canny(gray,50,100,apertureSize = 3)

# Selection of ignored regions
while True:
    roi = cv2.selectROI(edges) #ROI selection on table
    # print(roi)
    ignoredRegions.append([roi[0], roi[0] + roi[2], roi[1], roi[1] + roi[3]])
    k = cv2.waitKey(0) & 0xFF
    if (k == 113):  # q is pressed
      break

# print(ignoredRegions)

# writeIgnoredRegionsToFile(ignoredRegions)
# roiCrop = edges[int(roi[1]):int(roi[1]+roi[3]), int(roi[0]):int(roi[0]+roi[2])]
# cropped = cv2.imshow("cropped", roiCrop)
# cv2.waitKey(0)


# print(ignoredRegions)

minLineLength = 100
maxLineGap = 10
lines = cv2.HoughLinesP(edges,0.1,np.pi/180,100,minLineLength,maxLineGap)
xses = []
yses = []
angles = []
cnt = 0
for x in range(0, len(lines)):
    for x1,y1,x2,y2 in lines[x]:
        cnt += 1
        angle = np.arctan2(y2-y1, x2-x1) * 180. / np.pi
        if (angle < 10 and angle > -10):
            accepted = True
            for region in ignoredRegions:
                if (x1 > region[0] and x1 < region[1] and y1> region[2] and y1< region[3]):
                    accepted = False
                    break
            if (accepted):
                xses.append(x1)
                yses.append(y1)
                angles.append(angle)
                cv2.line(newImg,(x1,y1),(x2,y2),(255,255,255),2)

cv2.imshow("lines", newImg)
cv2.waitKey(0) & 0xFF
bin = 120
groupSize = int(height // bin)

print("Photo height: ", height)
print("Photo width: ", width)
print("Group count: ", bin)
print("Bar location group size: ", groupSize)
print("Group size: ", groupSize, "px")
print("Y1's found: ", len(yses))
peaks = []
groups = np.zeros((height // groupSize + 1))
for i in range(len(yses)):
    idx = yses[i] // groupSize
    groups[idx] += 1

histTuples = np.zeros((len(yses), 2))

for i in range(len(yses)):
    histTuples[i, 0] = xses[i]
    histTuples[i, 1] = yses[i]

# print(groups)
lines = joinAdjacentMiddle(groups)
print("Lines: {}".format(lines))

IDX = []
for i in range(len(groups)):
    if groups[i] > 0:
        IDX.append(i*groupSize)
        
# print(IDX)
# for i in lines:
#     cv2.line(img,(0,i[0] * groupSize),(1080,i[1] * groupSize),(255,255,255),2)

# cv2.imshow('hough', img)
# cv2.waitKey(0)

# pyplot.hist(histTuples, bins=bin)
#pyplot.hist(angles)
# pyplot.show()
# cv2.imshow('idx', img)
# cv2.waitKey(0)

# jenksBreaks = getBreaks(yses, 7)
# print(jenksBreaks)
# for i in IDX:
#     i = int(i) + 1 
#     cv2.line(img, (1, i), (1080, i), [0, 0, 255], 5)
# cv2.imshow('hough', img)
# cv2.waitKey(0)

# barCoords = int(jenksBreaks[3])
# i=4
# findPlayers(IDX[i]-20, IDX[i]+20, width, formation[i][1], formation[i][0])
playerWidth = 20
for i in range(len(lines)):
    heightlocal = lines[i]
    # print("Lines 0: {} Lines 1: {}".format(lines[i][0], lines[i][1]))
    # players = getPlayersAlongAngledLine(imgCopy, lines[i][0], lines[i][1], formationInverted[i][1], formationInverted[i][0])
    players = getPlayersAlongAngledLine(imgCopy, lines[i][1] * groupSize, lines[i][0] * groupSize, formation[i][1], 5)
    print("Players: {}".format(players))
    for y in players:
        print("Y: {}".format(y))
        print("Height: {}".format(height))
        cv2.rectangle(img, (y*playerWidth-10, heightlocal[1] * groupSize), (y*playerWidth+10, heightlocal[0] * groupSize), [0, 0, 255], 5)

cv2.imshow('players', img)
cv2.waitKey(0)

# findPlayersAlongLine(IDX[0], width, formation[0][1], formation[0][0])

# for i in range(len(IDX)):