#!/usr/bin/env python
 
import cv2
import time

def weird():
    if __name__ == '__main__' :
    
        # Start default camera
        video = cv2.VideoCapture(0);
        
        # Find OpenCV version
        (major_ver, minor_ver, subminor_ver) = (cv2.__version__).split('.')
        
        # With webcam get(CV_CAP_PROP_FPS) does not work.
        # Let's see for ourselves.
        
        if int(major_ver)  < 3 :
            fps = video.get(cv2.cv.CV_CAP_PROP_FPS)
            print("Frames per second using video.get(cv2.cv.CV_CAP_PROP_FPS): {0}".format(fps))
        else :
            fps = video.get(cv2.CAP_PROP_FPS)
            print("Frames per second using video.get(cv2.CAP_PROP_FPS) : {0}".format(fps))
        
    
        # Number of frames to capture
        num_frames = 120;
        
        
        print("Capturing {0} frames".format(num_frames))
    
        # Start time
        start = time.time()
        
        # Grab a few frames
        for i in range(0, num_frames) :
            ret, frame = video.read()
    
        
        # End time
        end = time.time()
    
        # Time elapsed
        seconds = end - start
        print("Time taken : {0} seconds".format(seconds))
    
        # Calculate frames per second
        fps  = num_frames / seconds
        print("Estimated frames per second : {0}".format(fps))
 
def custom():
    vid = cv2.VideoCapture(0)
    # vid = cv2.VideoCapture(1 + cv2.CAP_DSHOW)
    vid.set(cv2.CAP_PROP_FOURCC, cv2.VideoWriter_fourcc('M','J','P','G'))
    # print(vid.get(cv2.CV_CAP_PROP_FOURCC))
    # vid.set(cv2.CAP_PROP_FRAME_WIDTH, 1280)
    # vid.set(cv2.CAP_PROP_FRAME_HEIGHT, 720)
    vid.set(cv2.CAP_PROP_FPS, 60)
    # vid.set(cv2.CAP_PROP_EXPOSURE, 0)
    counter=0
    x=1
    start_time = time.time()
    while(vid.isOpened()):
        rc, frame = vid.read()
        if rc == True:
            counter+=1

        if (time.time() - start_time) > x :
            print("FPS: {} ".format(counter / (time.time() - start_time)))
            width = int(vid.get(3))
            height = int(vid.get(4))
            print("width: {} height: {}".format(width, height))
            counter = 0
            start_time = time.time()
        cv2.imshow("wtf", frame)
        if cv2.waitKey(1) & 0xFF is ord('q'):
            break

custom()