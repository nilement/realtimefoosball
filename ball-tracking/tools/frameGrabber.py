import numpy as np
import argparse
import cv2

def main():
    parser = argparse.ArgumentParser()
    parser.add_argument('second', type=int)
    parser.add_argument('file', type=str)
    args = parser.parse_args()
    vid = cv2.VideoCapture(args.file)
    fps = vid.get(cv2.CAP_PROP_FPS)
    
    frame = int(fps) * args.second + 1
    # print("Frame n: {}".format(frame))

    frameRead = vid.read()
    while frameRead:
        frameId = int(round(vid.get(1)))
        # print(frameId)
        frameRead, img = vid.read()

        if frameId == frame:
            # print("written")
            cv2.imwrite('capture.jpg', img)
            break

    vid.release()

main()