namespace seesharp.moqingbirt
{
    using System;

    public class Times
    {
        private Func<int, bool> timesComparer;

        private Times(Func<int, bool> timesComparer)
        {
            this.timesComparer = timesComparer;
        }

        public static Times AtLeast(int timesCalled)
        {
            return new Times(comparison => comparison >= timesCalled);
        }

        public static Times Exactly(int timesCalled)
        {
            return new Times(comparison => comparison == timesCalled);            
        }

        public bool MatchTimes(int actualCallsCount)
        {
            return timesComparer(actualCallsCount);
        }

        public static Times Once()
        {
            return Times.Exactly(1);
        }

        public static Times Never()
        {
            return Times.Exactly(0);
        }
    }
}