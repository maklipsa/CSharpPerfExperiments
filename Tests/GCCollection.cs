using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Experiments;
using Experiments.GarbageCollection;
using NBench;

namespace Tests
{
    public class GCCollection
    {
        private readonly int _objectNumber = 20*1000;
        //private Experiments.
        [PerfSetup]
        public void Setup(BenchmarkContext context)
        {
            Console.WriteLine("Objects generated.");
            GCHelper.GenerateObjects(_objectNumber*2);
            GC.Collect(2, GCCollectionMode.Forced, true, true);
        }

        [PerfBenchmark(Description = "Gen 2 collection with nothing", NumberOfIterations = 1, RunMode = RunMode.Iterations, TestMode = TestMode.Test)]
        [GcTotalAssertion(GcMetric.TotalCollections, GcGeneration.Gen0, MustBe.ExactlyEqualTo, 3d)]
        [GcTotalAssertion(GcMetric.TotalCollections, GcGeneration.Gen1, MustBe.ExactlyEqualTo, 2d)]
        [GcTotalAssertion(GcMetric.TotalCollections, GcGeneration.Gen2, MustBe.ExactlyEqualTo, 1d)]
        public void Gen2CollectionWithNothing()
        {
            GC.Collect(2,GCCollectionMode.Forced,true,true);

            var sw = Stopwatch.StartNew();

            GCHelper.RunGCAndCheck(0, sw);/* Run collect for gen 0 - collect everything.*/

            GCHelper.RunGCAndCheck(1, sw);/* Nothing should be here.*/

            GCHelper.RunGCAndCheck(2, sw);/* Nothing should be here.*/
        }

        [PerfBenchmark(Description = "Gen 0 collection", NumberOfIterations = 1, RunMode = RunMode.Iterations,TestMode = TestMode.Test)]
        [GcTotalAssertion(GcMetric.TotalCollections, GcGeneration.Gen0, MustBe.ExactlyEqualTo, 1d)]
        [GcTotalAssertion(GcMetric.TotalCollections, GcGeneration.Gen1, MustBe.ExactlyEqualTo, 0.0d)]
        [GcTotalAssertion(GcMetric.TotalCollections, GcGeneration.Gen2, MustBe.ExactlyEqualTo, 0.0d)]
        public void Gen0Collection()
        {
            var list = GCHelper.GenerateObjects(_objectNumber);
            list = null;

            var sw = Stopwatch.StartNew();

            GCHelper.RunGCAndCheck(0, sw);/* Run collect for gen 0 - collect everything.*/
        }


        [PerfBenchmark(Description = "Gen 1 collection", NumberOfIterations = 1, RunMode = RunMode.Iterations,TestMode = TestMode.Test)]
        [GcTotalAssertion(GcMetric.TotalCollections, GcGeneration.Gen0, MustBe.ExactlyEqualTo, 2.0d)]
        [GcTotalAssertion(GcMetric.TotalCollections, GcGeneration.Gen1, MustBe.ExactlyEqualTo, 1.0d)]
        [GcTotalAssertion(GcMetric.TotalCollections, GcGeneration.Gen2, MustBe.ExactlyEqualTo, 0.0d)]
        public void Gen1Collection()
        {
            var list = GCHelper.GenerateObjects(_objectNumber);
            var sw = Stopwatch.StartNew();
            GCHelper.RunGCAndCheck(0, sw);/* Can't collect anything. Move it to gen 1.*/

            list = null;
            GCHelper.RunGCAndCheck(1, sw);/* Run collect for gen 0 - it is empty. Run gen 1 collection - collect everything.*/
        }

        [PerfBenchmark(Description = "Gen 2 collection", NumberOfIterations = 1, RunMode = RunMode.Iterations,TestMode = TestMode.Test)]
        [GcTotalAssertion(GcMetric.TotalCollections, GcGeneration.Gen0, MustBe.ExactlyEqualTo, 3.0d)]
        [GcTotalAssertion(GcMetric.TotalCollections, GcGeneration.Gen1, MustBe.ExactlyEqualTo, 2.0d)]
        [GcTotalAssertion(GcMetric.TotalCollections, GcGeneration.Gen2, MustBe.ExactlyEqualTo, 1.0d)]
        public void Gen2Collection()
        {
            var list = GCHelper.GenerateObjects(_objectNumber);
            var sw = Stopwatch.StartNew();

            GCHelper.RunGCAndCheck(0, sw);/* Can't collect anything. Move it to gen 1.*/

            GCHelper.RunGCAndCheck(1, sw);/* Run collect for gen 0 - it is empty. Run gen 1 collection - collect the list elements.*/

            list = null;
            GCHelper.RunGCAndCheck(2, sw);/* Run collect for generation 0 - it is empty. Run collect for generation 1 - it is empty. Run collect for generation 2 - collect everything.*/
        }
    }
}