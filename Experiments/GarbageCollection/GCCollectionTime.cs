using System;
using System.Diagnostics;
using System.Threading;

namespace Experiments.GarbageCollection
{
    public class GCCollectionTime
    {
        private readonly Action _gen0Ended;
        private readonly Action _gen1Ended;
        private readonly Action _gen2Ended;
        private readonly int _objectNumber = 20*1000;

        public GCCollectionTime()
        {
        }

        public GCCollectionTime(Action gen0Ended, Action gen1Ended, Action gen2Ended)
        {
            _gen0Ended = gen0Ended;
            _gen1Ended = gen1Ended;
            _gen2Ended = gen2Ended;
        }

        public void Setup()
        {
            Console.WriteLine("Objects generated.");
            var list = GCHelper.GenerateObjects(_objectNumber*2);
            list = null;
            Thread.Sleep(5000);
            GC.Collect(2, GCCollectionMode.Forced, true, true);
        }


        public void Gen2CollectionWithNothing()
        {
            var list = GCHelper.GenerateObjects(_objectNumber);
            list = null;

            var sw = Stopwatch.StartNew();

            GCHelper.RunGCAndCheck(0, sw); /* Run collect for gen 0 - collect everything.*/
            _gen0Ended?.Invoke();

            GCHelper.RunGCAndCheck(1, sw); /* Nothing should be here.*/
            _gen1Ended?.Invoke();

            GCHelper.RunGCAndCheck(2, sw); /* Nothing should be here.*/
            _gen2Ended?.Invoke();
        }

        public void Gen0Collection()
        {
            var list = GCHelper.GenerateObjects(_objectNumber);
            list = null;

            var sw = Stopwatch.StartNew();

            GCHelper.RunGCAndCheck(0, sw); /* Run collect for gen 0 - collect everything.*/
            _gen0Ended?.Invoke();
        }


        public void Gen1Collection()
        {
            var list = GCHelper.GenerateObjects(_objectNumber);
            var sw = Stopwatch.StartNew();

            GCHelper.RunGCAndCheck(0, sw); /* Can't collect anything. Move it to gen 1.*/
            _gen0Ended?.Invoke();

            list = null;
            GCHelper.RunGCAndCheck(1, sw);/* Run collect for gen 0 - it is empty. Run gen 1 collection - collect everything.*/
            _gen1Ended?.Invoke();
        }

        public void Gen2Collection()
        {
            var list = GCHelper.GenerateObjects(_objectNumber);
            var sw = Stopwatch.StartNew();

            GCHelper.RunGCAndCheck(0, sw); /* Can't collect anything. Move it to gen 1.*/
            _gen0Ended?.Invoke();

            GCHelper.RunGCAndCheck(1, sw);
            /* Run collect for gen 0 - it is empty. Run gen 1 collection - collect the list elements.*/
            _gen1Ended?.Invoke();

            list = null;
            GCHelper.RunGCAndCheck(2, sw);
            /* Run collect for generation 0 - it is empty. Run collect for generation 1 - it is empty. Run collect for generation 2 - collect everything.*/
            _gen2Ended?.Invoke();
        }
    }
}