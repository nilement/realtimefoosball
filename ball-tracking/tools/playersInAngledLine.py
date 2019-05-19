import cv2
import numpy as np
import scipy.signal as sp
from matplotlib import pyplot

def getPlayersAlongAngledLine(imgCopy, barYstart, barYend, color, playerCnt):
    blueMaskRGB = np.array([33, 54, 120]) / 255
    redMaskRGB = np.array([165, 30, 30]) / 255
    if color == "B":
        colorMaskRGB = blueMaskRGB
    else:
        colorMaskRGB = redMaskRGB

    # print("Bar Y: {}".format(barYcoord))
    print("Color : {}".format(color))
    playerWidth = 20
    barRoi = imgCopy[barYstart:barYend, :]
    cv2.imshow('barRoi', barRoi)
    gaussBlured = cv2.GaussianBlur(barRoi, (11,11), cv2.BORDER_DEFAULT)
    roiShape = gaussBlured.shape
    print("ROI shape: ", roiShape)
    distances = np.zeros(gaussBlured.shape[1] // playerWidth + 1)
    print("Distances shape: ", distances.shape)
    for i in range(len(gaussBlured)):
        for y in range(len(gaussBlured[i])):
            bar = gaussBlured[i, y]
            barRGB = np.array([bar[2], bar[1], bar[0]], dtype="uint8") / 255
            colorDifference = np.linalg.norm(barRGB - colorMaskRGB)
            distanceIndex = y // playerWidth
            distances[distanceIndex] += colorDifference

    distances = distances * -1
    pyplot.plot(distances)
    pyplot.ylabel("Mean difference to color mask")
    pyplot.show()
    peaks = sp.find_peaks(distances, height=-150, width=1)
    players = peaks[0][:]
    print("Player coords: {}".format(players))
    return players
    # displayRoi = imgCopy[barYcoord-5:barYcoord+5, :]

    # for i in players:
    #     cv2.rectangle(displayRoi, ((i * playerWidth) - playerWidth // 2, 0), ((i * playerWidth) + playerWidth // 2, 30), [0, 0, 255], 5)
    # cv2.imshow('players', displayRoi)
    # cv2.waitKey(0)