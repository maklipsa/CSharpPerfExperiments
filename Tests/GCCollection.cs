using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using NBench;

namespace Tests
{
    public class GCCollection
    {
        private IList<string> _list;
        [PerfSetup]
        public void Setup(BenchmarkContext context)
        {
            _list = GenerateObjects(20*1000*1000);
        }

        [PerfBenchmark(Description = "Gen 0 collection",NumberOfIterations = 1, RunMode = RunMode.Iterations,TestMode = TestMode.Test)]
        [GcTotalAssertion(GcMetric.TotalCollections, GcGeneration.Gen0, MustBe.ExactlyEqualTo, 1d)]
        [GcTotalAssertion(GcMetric.TotalCollections, GcGeneration.Gen1, MustBe.ExactlyEqualTo, 0.0d)]
        [GcTotalAssertion(GcMetric.TotalCollections, GcGeneration.Gen2, MustBe.ExactlyEqualTo, 0.0d)]
        public void Gen0Collection()
        {
            var testList = _list.ToList();
            testList = null;
            var sw = Stopwatch.StartNew();

            GC.Collect(0);
            GC.WaitForFullGCComplete();
            sw.Stop();
            Console.WriteLine("StopWatch time:"+sw.ElapsedMilliseconds);
        }


        [PerfBenchmark(Description = "Gen 1 collection", NumberOfIterations = 1, RunMode = RunMode.Iterations, TestMode = TestMode.Test)]
        [GcTotalAssertion(GcMetric.TotalCollections, GcGeneration.Gen0, MustBe.ExactlyEqualTo, 2.0d)]
        [GcTotalAssertion(GcMetric.TotalCollections, GcGeneration.Gen1, MustBe.ExactlyEqualTo, 1.0d)]
        [GcTotalAssertion(GcMetric.TotalCollections, GcGeneration.Gen2, MustBe.ExactlyEqualTo, 0.0d)]
        public void Gen1Collection()
        {
            var testList = _list.ToList();
            var sw = Stopwatch.StartNew();
            GC.Collect(0);
            Console.WriteLine("Gen0 StopWatch time:" + sw.ElapsedMilliseconds);

            testList = null;
            GC.Collect(1);
            sw.Stop();
            Console.WriteLine("Gen 0+1 StopWatch time:" + sw.ElapsedMilliseconds);
        }

        private IList<string> GenerateObjects(long count)
        {
            var ret = new List<string>();
            for (int i = 0; i < count; i++)
            {
                ret.Add(Guid.NewGuid().ToString());
            }
            return ret;
        }
    }
}
