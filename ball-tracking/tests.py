import argparse
import glob

from ballTracking import Processor
from houghBarsHorizontal import detection as horizontalDetection

def testImageDataset():
    processor = Processor()
    images = glob.glob('./*.jpg') + glob.glob('./*.png')

def testVideoDataset():
    print("testing")

if __name__ == '__main__':
    parser = argparse.ArgumentParser()
    parser.add_argument('images')
    parser.add_argument('videos')

    args, leftovers = parser.parse_known_args()
    if args.images:
        testImageDataset()
    elif args.videos:
        testVideoDataset()
    else: 
        print("Enter --images or --videos to choose dataset to test")