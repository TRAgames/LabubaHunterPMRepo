//------------------------------------------------------------//
// yanfei 2025.01.17
// FARM KING GOOD, MY LORD
//------------------------------------------------------------//
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading;
using System.Collections.Concurrent;

namespace QuickFinder.ThreadUtility
{
    public class ConcurrentQueueProcessor<TData>
    {
        public ConcurrentQueueProcessor(IEnumerable<TData> query_, int threadCount = 5)
        {
            query = query_;
            threadMaxCount = Math.Min(32, Math.Max(1, threadCount));
        }

        public void Do(Func<TData, bool> proccessor, int millisecondsTimeout = int.MaxValue)
        {
            var dataQueue = new ConcurrentQueue<TData>(query);

            var threadEvent = new ManualResetEvent(false);

            for (int i = 0; i < threadMaxCount; i++)
            {
                ThreadPool.QueueUserWorkItem((outsideParam) =>
                {
                    while (dataQueue.TryDequeue(out var data))
                    {
#if ASSETCONCURRENTPROCCESSOR_DEBUG
                        try
                        {
#endif
                        /////////////
                        var r = proccessor.Invoke(data);
                        if (!r)
                        {
                            break;
                        }
                        /////////////
#if ASSETCONCURRENTPROCCESSOR_DEBUG
                        }
                        catch (Exception e)
                        {
                            Debug.LogError($"{data} {e}");
                        }
#endif
                    }
                    ((ManualResetEvent)outsideParam).Set();
                },
                threadEvent);
            }

            threadEvent.WaitOne(millisecondsTimeout);
        }

        private int threadMaxCount = 5;
        private IEnumerable<TData> query;
    }

////    public class ConcurrentManualWaitingProcessor<TData>
////    {
////        ConcurrentQueue<TData> dataQueue = new ConcurrentQueue<TData>();
////        int threadMaxCount = 5;
////        bool exitableSwitch = false;
////        Func<TData, bool> processor;

////        public ConcurrentManualWaitingProcessor(int threadCount_, Func<TData, bool> proccessor_)
////        {
////            threadMaxCount = Math.Min(32, Math.Max(1, threadCount_));
////            processor = proccessor_;
////        }

////        public void Enqueue(TData data)
////        {
////            dataQueue.Enqueue(data);
////        }

////        public void Enqueue(IEnumerable<TData> datas)
////        {
////            foreach (var data in datas)
////            {
////                dataQueue.Enqueue(data);
////            }
////        }

////        public void Do(int millisecondsTimeout = int.MaxValue)
////        {
////            var threadEvent = new ManualResetEvent(false);

////            for (int i = 0; i < threadMaxCount; i++)
////            {
////                ThreadPool.QueueUserWorkItem((outsideParam) =>
////                {
////                    while (dataQueue.TryDequeue(out var data))
////                    {
////#if ASSETCONCURRENTPROCCESSOR_DEBUG
////                        try
////                        {
////#endif
////                        /////////////
////                        var r = processor.Invoke(data);
////                        if (!r)
////                        {
////                            break;
////                        }
////                        /////////////
////#if ASSETCONCURRENTPROCCESSOR_DEBUG
////                        }
////                        catch (Exception e)
////                        {
////                            Debug.LogError($"{data} {e}");
////                        }
////#endif
////                    }
////                    ((ManualResetEvent)outsideParam).Set();
////                },
////                threadEvent);
////            }
////            threadEvent.WaitOne(millisecondsTimeout);
////        }

////        //public void Do(TData data)
////        //{

////        //}

////        //public void Do(IEnumerable<TData> datas)
////        //{

////        //}
////    }

}
