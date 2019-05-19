import cv2
import numpy as np
import colorsys
import time
import scipy.signal as sp
import multiprocessing

from matplotlib import pyplot
from multiprocessing import Process, Pool

try:
    from playersInVerticalAngledLine import getPlayersAlongVerticalAngledLine
    from playersInVerticalStraightLine import getPlayersAlongVerticalStraightLine
    from houghBarsHorizontal import detection as horizontalDetection
except ModuleNotFoundError:
    from playersInVerticalAngledLine import getPlayersAlongVerticalAngledLine
    from playersInVerticalStraightLine import getPlayersAlongVerticalStraightLine
    from houghBarsHorizontal import detection as horizontalDetection

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

# blueMaskRGB = np.array([33, 54, 120])
# redMaskRGB = np.array([165, 30, 30])

def findAllPlayers(frame, lines, groupSize, pool):
    strips = []
    # print(lines)
    for i in range(0, 8):
        strips.append(frame[:, lines[i][0] * groupSize])

    players = []
    for i in range(0, 8):
        players.append(pool.apply_async(getPlayersAlongVerticalStraightLine, args=(strips[i], 0, formation[i][1])))

    return players

def main():
    video = cv2.VideoCapture('../data/officeRelease.mp4')
    tableLeftX = 160
    tableRightX = 1150
    tableTopY = 80
    tableBottomY = 650
    groupSize = 13
    playerWidth = 5
    # tableFileName = '../data/tablehorizontalcrop.png'
    # img = cv2.imread(tableFileName)
    # video = cv2.VideoCapture(0)
    # video.set(cv2.CAP_PROP_FOURCC, cv2.VideoWriter_fourcc('M','J','P','G'))
    # video.set(cv2.CAP_PROP_FRAME_WIDTH, 1280)
    # video.set(cv2.CAP_PROP_FRAME_HEIGHT, 720)
    # video.set(cv2.CAP_PROP_FPS, 60)
    cv2.namedWindow("Input")

    # frame = []
    # while True:
    #     rec, img = video.read()
    #     cv2.imshow("g", img)
    #     if (cv2.waitKey(1) & 0xFF == ord('q')):
    #         frame = img
    #         break
    rc, frame = video.read()

    frame = frame[tableTopY:tableBottomY, tableLeftX:tableRightX]

    lines = horizontalDetection(frame)

    for i in lines:
        cv2.line(frame,(i[0] * groupSize, 0),(i[0] * groupSize, 553),(0,0,255),2)

    start_time = time.time()
    x = 1
    counter = 0

    cv2.imshow('Input', frame)
    cv2.waitKey(0)

    with Pool(4) as pool:
        while(video.isOpened()):
            rc, frame = video.read()
            if (rc == True):
                frame = frame[tableTopY:tableBottomY, tableLeftX:tableRightX]
                counter += 1
                if (time.time() - start_time) > x :
                    print("FPS: {} ".format(counter / (time.time() - start_time)))
                    counter = 0
                    start_time = time.time()
                strips = []
                for i in range(0, 8):
                    strips.append(frame[:, lines[i][0] * groupSize])

                players = []
                for i in range(0, 8):
                    players.append(pool.apply_async(getPlayersAlongVerticalStraightLine, args=(strips[i], 0, formation[i][1])))

                for i in range(len(players)):
                    bar = players[i].get()
                    for y in bar:
                        xIdx = lines[i][0] * groupSize
                        cv2.rectangle(frame, (xIdx - 5, y*playerWidth-10), (xIdx + 5, y*playerWidth+10), [0, 0, 255], 5)

            cv2.imshow('Input', frame)
            if (cv2.waitKey(1) & 0xFF == ord('q')):
                break

    video.release()
    cv2.destroyAllWindows()

if __name__ == '__main__':
    main()