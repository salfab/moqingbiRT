namespace seesharp.moqingbirt
{
    using System;

    public class Times
    {
        public static Func<int, bool> AtLeast(int timesCalled)
        {
            return comparison => comparison >= timesCalled;
        }

        public static Func<int, bool> Exactly(int timesCalled)
        {
            return comparison => comparison == timesCalled;
        }
    }
}