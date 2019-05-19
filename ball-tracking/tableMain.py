import asyncio
import datetime
import websockets
import json
import functools
import time
import multiprocessing
import random

from tools.ballTracking import main as startTracking
from multiprocessing import Pool, Queue, Process
from enum import Enum

class trackerStates(Enum):
    WAITING=1
    TRACK=2

class listenerStates(Enum):
    WAITING=1
    LISTENING=2
    EXITING=3
    

async def acceptMessage(websocket, eventsQueue, goalsQueue):
    asyncState = type('', (), {})()
    asyncState.state = listenerStates.WAITING

    async def res(event):    
        eventjson = {
            "event": "G",
            "side": event,
        }
        await socketWrapper(eventjson)

    async def socketWrapper(eventJson):
        await websocket.send(json.dumps(eventJson))
    
    async def listener():
        try:
            message = await websocket.recv()
            print("message received: {}".format(message))
            if message == "track":
                asyncState.state = listenerStates.LISTENING
            if message == "exit":
                print("setting state to exiting")
                asyncState.state = listenerStates.EXITING
            eventsQueue.put(message)
        except websockets.exceptions.ConnectionClosed:
            asyncState.state = listenerStates.EXITING
            eventsQueue.put("exit")

    async def goalsResponder():
        while True:
            if asyncState.state == listenerStates.LISTENING:
                if goalsQueue.empty():
                    await asyncio.sleep(0.1)
                    pass
                else:
                    goal = goalsQueue.get()
                    await res(goal)
            else:
                break

    while True:
        listener_task = asyncio.ensure_future(listener())
        goalsResponder_task = asyncio.ensure_future(goalsResponder())
        done, pending = await asyncio.wait(
            [listener_task, goalsResponder_task],
            return_when=asyncio.FIRST_COMPLETED,
        )
        if asyncState.state == listenerStates.EXITING:
            print("breaking")
            break

    # while True:
    #     responder_task = asyncio.ensure_future(responder())
    #     if state == listenerStates.WAITING:
    #     if message == "exit":
    #         break

def tracker(eventsQueue, goalsQueue):
    state = trackerStates.WAITING
    while True:
        if state is trackerStates.WAITING:
            if eventsQueue.empty():
                print('empty')
                time.sleep(2)
            else:
                item = eventsQueue.get()
                if item == "track":
                    print("found in queue: {}".format(item))
                    state = trackerStates.TRACK
                elif item == "exit":
                    print("exiting2")
                    break    
        elif state is trackerStates.TRACK:
            newState = startTracking(goalsQueue, eventsQueue)
            # print("tracking")
            # goal = random.randint(0,30)
            # if goal < 5:
            #     print('redGoal')
            #     goalsQueue.put('R')
            # elif goal < 10:
            #     print('blueGoal')
            #     goalsQueue.put('B')
            # time.sleep(2)
            if newState == "stop":
                print("stopping")
                state = trackerStates.WAITING
            elif newState == "exit":
                print("exiting")
                break        

async def setUsage(lock, mutex):
    await lock.acquire()
    try:
        if not mutex.InUse:
            mutex.InUse = True
            return True
        else:
            return False
    finally: lock.release()

async def unsetUsage(lock, mutex):
    await lock.acquire()
    try:
        mutex.InUse = False
    finally: lock.release()

async def responder(websocket, path, eventsQueue, goalsQueue, mutex, lock):
    try:
        print("lock is: {}, mutex is: {}".format(lock, mutex))
        mutexAvailable = await asyncio.wait_for(setUsage(lock, mutex), timeout=1.0)
        if not mutexAvailable:
            print("denied unavailable")
            await websocket.send(json.dumps({"event": "analyser in use"}))
            return             
    except asyncio.TimeoutError:
        print("someone's locked out")
        await websocket.send(json.dumps({"event": "analyser in use"}))
        return

    print("waiting for key")
    try:
        key = await websocket.recv()
    except websockets.exceptions.ConnectionClosed:
        print("closing while waiting for key")
        await unsetUsage(lock, mutex)
        return

    if key != "420":
        await unsetUsage(lock, mutex)
        print("denied wrong pass")
        await websocket.send(json.dumps({"event": "invalid key"}))
        return

    acceptedEvent = {
        "event": "key accepted"
    }
    await websocket.send(json.dumps(acceptedEvent))
    await acceptMessage(websocket, eventsQueue, goalsQueue)
    await unsetUsage(lock, mutex)
    loop = asyncio.get_event_loop()
    loop.call_soon_threadsafe(loop.stop)

def startServer(q, g):
    lock = asyncio.Lock()
    mutex = type('', (), {})()
    mutex.InUse = False
    start_server = websockets.serve(functools.partial(responder, eventsQueue=q, goalsQueue=g, mutex=mutex, lock=lock), '0.0.0.0', 42069)    
    print('Server running on port 42069!')
    asyncio.get_event_loop().run_until_complete(start_server)
    asyncio.get_event_loop().run_forever()


if __name__ == '__main__':
    m = multiprocessing.Manager()
    eventsQueue = m.Queue()
    goalsQueue = m.Queue()
    try:
        task1 = Process(target=startServer, args=(eventsQueue, goalsQueue,))
        task2 = Process(target=tracker, args=(eventsQueue, goalsQueue,))
        task1.daemon = False
        task2.daemon = False
        task1.start()
        task2.start()
        task1.join()
        task2.join()
    except:
        raise
