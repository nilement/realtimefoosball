import os
import multiprocessing
from multiprocessing import Pool, Queue, Process

# class NoDaemonProcess(multiprocessing.Process):
#     @property
#     def daemon(self):
#         return False

#     @daemon.setter
#     def daemon(self, value):
#         pass


# class NoDaemonContext(type(multiprocessing.get_context())):
#     Process = NoDaemonProcess

# # We sub-class multiprocessing.pool.Pool instead of multiprocessing.Pool
# # because the latter is only a wrapper function, not a proper class.
# class MyPool(multiprocessing.pool.Pool):
#     def __init__(self, *args, **kwargs):
#         kwargs['context'] = NoDaemonContext()
#         super(MyPool, self).__init__(*args, **kwargs)

def printers():
    print(os.getpid())

def child1(arg):
    with Pool(4) as pool:
        luls = []
        for i in range(4):
            task = pool.apply_async(printers)
            luls.append(task)
        for i in range(4):
            print(arg)
            luls[i].get()

if __name__ == '__main__':
    # with MyPool(2) as pool:
        # luls = []
    p1 = Process(target=child1, args=("lul",))
    p2 = Process(target=child1, args=("bab",))
    p1.daemon = False
    p2.daemon = False
    p1.start()
    p2.start()
    # p1.join()
    # p2.join()
        # for i in range(2):
        #     task = pool.apply_async(child1)
        #     luls.append(task)
        # for i in range(2):
        #     luls[i].get()
            