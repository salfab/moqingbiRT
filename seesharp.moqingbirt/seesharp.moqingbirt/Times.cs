namespace seesharp.moqingbirt
{
    using System;

    public class Times
    {
        public enum TimesKind
        {
            AtLeast,
            Exactly,
            AtMost
        }

        private TimesKind timesKind;

        private int count;

        private Func<int, bool> timesComparer;

        private Times(Func<int, bool> timesComparer, TimesKind kindOfCountMatching, int expectedCallsCount)
        {
            this.timesComparer = timesComparer;
            this.timesKind = kindOfCountMatching;
            this.count = expectedCallsCount;
        }

        public static Times AtLeast(int timesCalled)
        {
            return new Times(comparison => comparison >= timesCalled, TimesKind.AtLeast, timesCalled);
        }

        public static Times Exactly(int timesCalled)
        {
            return new Times(comparison => comparison == timesCalled, TimesKind.Exactly, timesCalled);            
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

        public override string ToString()
        {
            var timesSpelling = this.count > 1 ? " times." : " time.";
            switch (this.timesKind)
            {
                case TimesKind.AtLeast :
                    return "at least " + this.count + timesSpelling;                  
                case TimesKind.AtMost:
                    return "at most " + this.count + timesSpelling;
                case TimesKind.Exactly:
                    return "exactly " + this.count + timesSpelling;

            }
            return base.ToString();
        }
    }
}