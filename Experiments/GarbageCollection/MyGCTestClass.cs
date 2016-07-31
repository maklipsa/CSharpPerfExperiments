using System;

namespace Experiments.GarbageCollection
{
    public class MyGCTestClass
    {
        private readonly string Text;
        private readonly Guid Id;

        public MyGCTestClass(string text)
        {
            Text = text;
        }

        public MyGCTestClass(Guid guid)
        {
            Id = guid;
        }

        public MyGCTestClass(MyGCTestClass source)
        {
            Text = source.Text;
        }
    }
}