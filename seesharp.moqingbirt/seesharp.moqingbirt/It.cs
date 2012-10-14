namespace seesharp.moqingbirt
{
    using System;

    public class It
    {
        public static T IsAny<T>()
        {
            var delayedCompute = new Func<T>(() =>
                {
                    if (MatchIt<T>.LastMatch != null)
                    {
                        throw new InvalidOperationException("There is already a matching pending (It.Is<T>() or It.IsAny<T>(). There might be a bug in Moqingbirt. Were you using multithreading by any chance ?");
                    }

                    // always match.
                    MatchIt<T>.LastMatch = x =>
                    {
                        MatchIt<T>.LastMatch = null;
                        return true;
                    };

                    return default(T);
                });

            return delayedCompute();
        }

    }
}