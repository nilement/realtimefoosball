import cv2
import time
import numpy as np
import scipy.signal as sp
import random
import os

from matplotlib import pyplot

def getPlayersAlongVerticalStraightLineDebug(imgCopy, barX, colorMaskRGB):
    playerHeight = 20
    barX = barX * 10
    barRoi = imgCopy[:, barX]
    cv2.imshow('barRoi', barRoi)
    cv2.waitKey(0)
    # gaussBlured = cv2.GaussianBlur(barRoi, (11,11), cv2.BORDER_DEFAULT)
    roiShape = barRoi.shape
    distances = np.zeros(barRoi.shape[0] // playerHeight + 1)
    for i in range(len(barRoi)):
        bar = barRoi[i]
        barRGB = np.array([bar[2], bar[1], bar[0]], dtype="uint8") / 255
        colorDifference = np.linalg.norm(barRGB - colorMaskRGB)
        distanceIndex = i // playerHeight
        distances[distanceIndex] += colorDifference

    distances = distances * -1

    pyplot.plot(distances)
    pyplot.ylabel("Mean difference to color mask")
    pyplot.show()

    peaks = sp.find_peaks(distances, height=-150, width=1)
    players = peaks[0][:]
    return players


def getPlayersAlongVerticalStraightLine(imgCopy, barX, colorMaskRGB):
    playerHeight = 5
    # roiShape = imgCopy.shape
    distances = np.zeros(imgCopy.shape[0] // playerHeight + 1)

    # color0 = colorMaskRGB[0]
    # color1 = colorMaskRGB[1]
    # color2 = colorMaskRGB[2]
    for i in range(0, len(imgCopy)):
        bar = imgCopy[i] / 255
        # print(bar)
        # barRGB = np.array([bar[2], bar[1], bar[0]], dtype="uint8") / 255
        newbar = [bar[2], bar[1], bar[0]] 
        colorDifference = np.linalg.norm(newbar - colorMaskRGB)
        # colorDifference = bar - colorMaskRGB # fuck
        # pixel = imgCopy[i]
        # colorDifference = imgCopy[i][0] - color0 + (imgCopy[i][1] - color1) + (imgCopy[i][2] - color2)
        # a = i - one
        # # if color == "R":
        # #     colorDifference = bar[2] + bar[1] + bar[0] - colorMaskRGB[0]
        # # else:
        # #     colorDifference = bar[2] + bar[1] + bar[0] - colorMaskRGB[1]
        distanceIndex = i // playerHeight
        distances[distanceIndex] += colorDifference

    distances = distances * -1

    # barRoi = imgCopy[:, barX-50:barX+50]
    # cv2.line(barRoi,(50, 0),(50, 553),(0,0,255),2)

    # cv2.imshow('barRoi', barRoi)
    # cv2.waitKey(0)

    # pyplot.plot(distances)
    # pyplot.ylabel("Mean difference to color mask")
    # pyplot.show()

    peaks = sp.find_peaks(distances, height=-3.5, width=2)
    return peaks[0]
    # return []
