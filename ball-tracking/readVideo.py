import numpy
import cv2
import math

cap = cv2.VideoCapture("vid.mp4")
frame_rate = cap.get(5)
print(frame_rate)
x=1

while cap.isOpened():
    frame_id = cap.get(1)
    ret, frame = cap.read()

    if (frame_id != 0.0 and frame_id % math.floor(frame_id) == 0):
        filename = './image' + str(int(x)) + ".jpg";x+=1
        cv2.imwrite(filename, frame)
    gray = cv2.cvtColor(frame, cv2.COLOR_BGR2GRAY)

    cv2.imshow('frame', gray)
    if cv2.waitKey(50) & 0xFF == ord('q'):
        break

cap.release()
cv2.destroyAllWindows()
