namespace Mocks
{
    using System;

    using seesharp.moqingbirt;

    public class It
    {
        public static T IsAny<T>()
        {
            var delayedCompute = new Func<T>(() =>
                {
                    if (MatchIt<T>.LastMatch != null)
                    {
                        throw new InvalidOperationException("There is already a matching pending (It.Is<T>() or It.IsAny<T>89. There might be a bug in Moqingbirt. Were you using multithreading by any chance ?");
                    }

                    // always match.
                    MatchIt<T>.LastMatch = x => true;

                    return default(T);
                });

            return delayedCompute();
        }

    }
}