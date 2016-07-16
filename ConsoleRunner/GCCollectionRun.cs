using System;
using System.Threading;
using Experiments.GarbageCollection;

namespace ConsoleRunner
{
    internal class GCCollectionRun
    {
        private const int _sleepTime = 5*1000;
        private GCCollectionTime _gcCollectionTime = new GCCollectionTime(Gen0Ended, Gen1Ended, Gen2Ended);
        private static DateTime _start;

        public void Run()
        {
            _start = DateTime.Now;
            ExecuteStep(_gcCollectionTime.Setup, "Setup");
            TestEnded();

            ExecuteStep(_gcCollectionTime.Gen0Collection, "Gen0Collection");
            TestEnded();
            ExecuteStep(_gcCollectionTime.Gen1Collection, "Gen1Collection");
            TestEnded();
            ExecuteStep(_gcCollectionTime.Gen2Collection, "Gen2Collection");
            TestEnded();
            ExecuteStep(_gcCollectionTime.Gen2CollectionWithNothing, "Gen2CollectionWithNothing");
        }

        private static void Gen0Ended()
        {
            GenEnded(0);
        }

        private static void Gen1Ended()
        {
            GenEnded(1);
        }

        private static void Gen2Ended()
        {
            GenEnded(2);
        }

        private void ExecuteStep(Action step, string stepName)
        {
            Console.WriteLine("---------Begin:" + stepName + " seconds from start" + (DateTime.Now.Subtract(_start).TotalSeconds));
            step();
            Console.WriteLine("---------End:" + stepName+" seconds from start"+(DateTime.Now.Subtract(_start).TotalSeconds));
        }

        private static void GenEnded(int generationNumber)
        {
            Console.WriteLine("Generation "+ generationNumber+" collection ended. Seconds:"+ (DateTime.Now.Subtract(_start).TotalSeconds));
            Thread.Sleep(_sleepTime);
        }
        private static void TestEnded()
        {
            Thread.Sleep(_sleepTime);
            GC.Collect(2, GCCollectionMode.Forced, true, true);
            Thread.Sleep(_sleepTime);
            Console.WriteLine();
            Console.WriteLine();
            
        }
    }
}