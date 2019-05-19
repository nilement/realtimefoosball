import websockets
import asyncio
import json

async def responder(websocket, path):
    print("waiting for key")
    key = await websocket.recv()
    acceptedEvent = {
        "event": "key accepted"
    }
    await websocket.send(json.dumps(acceptedEvent))
    while True:
        try:
            message = await websocket.recv()
            print("message received: {}".format(message))
        except websockets.exceptions.ConnectionClosed:
            print("Closing!")
            break
            
    print('fuckk')
    loop = asyncio.get_event_loop()
    loop.call_soon_threadsafe(loop.stop)
    # await acceptMessage(websocket, eventsQueue, goalsQueue)
    # loop = asyncio.get_event_loop()
    # loop.call_soon_threadsafe(loop.stop)

start_server = websockets.serve(responder, '0.0.0.0', 42069)    
print('Server running on port 42069!')
asyncio.get_event_loop().run_until_complete(start_server)
asyncio.get_event_loop().run_forever()