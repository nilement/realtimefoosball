import cv2

img = cv2.imread('../data/table/redplayer.png')
for i in img:
    for pixel in i:
        print(pixel)