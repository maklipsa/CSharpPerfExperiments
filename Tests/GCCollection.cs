using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using NBench;

namespace Tests
{
    public class GCCollection
    {
        private readonly string[] _messages =
        {
            "Gen 0 ticks:       ",
            "Gen 0+1 ticks:     ",
            "Gen 0+1+2 ticks:   "
        };

        private List<MyGCTestClass> _list;

        [PerfSetup]
        public void Setup(BenchmarkContext context)
        {
            _list = GenerateObjects(20*1000*1000);
            Console.WriteLine("Objects generated.");
            GC.Collect(2, GCCollectionMode.Forced, true,true);
        }

        [PerfBenchmark(Description = "Gen 2 collection with nothing", NumberOfIterations = 1, RunMode = RunMode.Iterations, TestMode = TestMode.Test)]
        [GcTotalAssertion(GcMetric.TotalCollections, GcGeneration.Gen0, MustBe.ExactlyEqualTo, 3d)]
        [GcTotalAssertion(GcMetric.TotalCollections, GcGeneration.Gen1, MustBe.ExactlyEqualTo, 2d)]
        [GcTotalAssertion(GcMetric.TotalCollections, GcGeneration.Gen2, MustBe.ExactlyEqualTo, 1d)]
        public void Gen2CollectionWithNothing()
        {
            _list = null;

            var sw = Stopwatch.StartNew();

            RunGCAndCheck(0, sw);/* Run collect for gen 0 - collect everything.*/

            RunGCAndCheck(1, sw);/* Nothing should be here.*/

            RunGCAndCheck(2, sw);/* Nothing should be here.*/
        }

        [PerfBenchmark(Description = "Gen 0 collection", NumberOfIterations = 1, RunMode = RunMode.Iterations,TestMode = TestMode.Test)]
        [GcTotalAssertion(GcMetric.TotalCollections, GcGeneration.Gen0, MustBe.ExactlyEqualTo, 1d)]
        [GcTotalAssertion(GcMetric.TotalCollections, GcGeneration.Gen1, MustBe.ExactlyEqualTo, 0.0d)]
        [GcTotalAssertion(GcMetric.TotalCollections, GcGeneration.Gen2, MustBe.ExactlyEqualTo, 0.0d)]
        public void Gen0Collection()
        {
            _list = null;

            var sw = Stopwatch.StartNew();

            RunGCAndCheck(0, sw);/* Run collect for gen 0 - collect everything.*/
        }


        [PerfBenchmark(Description = "Gen 1 collection", NumberOfIterations = 1, RunMode = RunMode.Iterations,TestMode = TestMode.Test)]
        [GcTotalAssertion(GcMetric.TotalCollections, GcGeneration.Gen0, MustBe.ExactlyEqualTo, 2.0d)]
        [GcTotalAssertion(GcMetric.TotalCollections, GcGeneration.Gen1, MustBe.ExactlyEqualTo, 1.0d)]
        [GcTotalAssertion(GcMetric.TotalCollections, GcGeneration.Gen2, MustBe.ExactlyEqualTo, 0.0d)]
        public void Gen1Collection()
        {
            var sw = Stopwatch.StartNew();
            RunGCAndCheck(0, sw);/* Can't collect anything. Move it to gen 1.*/

            _list = null;
            RunGCAndCheck(1, sw);/* Run collect for gen 0 - it is empty. Run gen 1 collection - collect everything.*/
        }

        [PerfBenchmark(Description = "Gen 2 collection", NumberOfIterations = 1, RunMode = RunMode.Iterations,TestMode = TestMode.Test)]
        [GcTotalAssertion(GcMetric.TotalCollections, GcGeneration.Gen0, MustBe.ExactlyEqualTo, 3.0d)]
        [GcTotalAssertion(GcMetric.TotalCollections, GcGeneration.Gen1, MustBe.ExactlyEqualTo, 2.0d)]
        [GcTotalAssertion(GcMetric.TotalCollections, GcGeneration.Gen2, MustBe.ExactlyEqualTo, 1.0d)]
        public void Gen2Collection()
        {
            var sw = Stopwatch.StartNew();

            RunGCAndCheck(0, sw);/* Can't collect anything. Move it to gen 1.*/

            RunGCAndCheck(1, sw);/* Run collect for gen 0 - it is empty. Run gen 1 collection - collect the list elements.*/

            _list = null;
            RunGCAndCheck(2, sw);/* Run collect for generation 0 - it is empty. Run collect for generation 1 - it is empty. Run collect for generation 2 - collect everything.*/
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void RunGCAndCheck(int generationNumber, Stopwatch sw)
        {
            sw.Restart();
            GC.Collect(generationNumber, GCCollectionMode.Forced, true,true);
            Console.WriteLine(_messages[generationNumber] + sw.ElapsedTicks);
        }

        private List<MyGCTestClass> GenerateObjects(long count)
        {
            var ret = new List<MyGCTestClass>();
            for (var i = 0; i < count; i++)
            {
                ret.Add(new MyGCTestClass(Guid.NewGuid().ToString()));
            }
            return ret;
        }

        private class MyGCTestClass
        {
            private readonly string Text;

            public MyGCTestClass(string text)
            {
                Text = text;
            }

            public MyGCTestClass(MyGCTestClass source)
            {
                Text = source.Text;
            }
        }
    }
}