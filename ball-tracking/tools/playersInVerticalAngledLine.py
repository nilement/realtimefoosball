import cv2
import time
import numpy as np
import scipy.signal as sp
from matplotlib import pyplot

def getPlayersAlongVerticalAngledLine(imgCopy, barXstart, barXend, colorMaskRGB):
    playerWidth = 20
    barRoi = imgCopy[:, barXstart:barXend]
    cv2.imshow('barRoi', barRoi)
    cv2.waitKey(0)
    gaussBlured = cv2.GaussianBlur(barRoi, (11,11), cv2.BORDER_DEFAULT)
    roiShape = barRoi.shape
    print("ROI shape: ", roiShape)
    distances = np.zeros(gaussBlured.shape[0] // playerWidth + 1)
    print("Distances shape: ", distances.shape)
    for i in range(len(gaussBlured)):
        for y in range(len(gaussBlured[i])):
            bar = gaussBlured[i, y]
            barRGB = np.array([bar[2], bar[1], bar[0]], dtype="uint8") / 255
            colorDifference = np.linalg.norm(barRGB - colorMaskRGB)
            distanceIndex = i // playerWidth
            distances[distanceIndex] += colorDifference

    distances = distances * -1

    pyplot.plot(distances)
    pyplot.ylabel("Mean difference to color mask")
    pyplot.show()

    peaks = sp.find_peaks(distances, height=-150, width=1)
    players = peaks[0][:]
    return players