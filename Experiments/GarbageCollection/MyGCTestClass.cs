namespace Experiments.GarbageCollection
{
    public class MyGCTestClass
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