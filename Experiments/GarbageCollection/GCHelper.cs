using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Experiments.GarbageCollection
{
    public class GCHelper
    {
        private static readonly string[] _messages =
        {
            "Gen 0 ticks:       ",
            "Gen 0+1 ticks:     ",
            "Gen 0+1+2 ticks:   "
        };
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static List<MyGCTestClass> GenerateObjects(long count)
        {
            var ret = new List<MyGCTestClass>();
            for (var i = 0; i < count; i++)
            {
                ret.Add(new MyGCTestClass(Guid.NewGuid().ToString()));
            }
            return ret;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void RunGCAndCheck(int generationNumber, Stopwatch sw)
        {
            sw.Restart();
            GC.Collect(generationNumber, GCCollectionMode.Forced, true, true);
            Console.WriteLine(_messages[generationNumber] + sw.ElapsedTicks);
        }
    }
}