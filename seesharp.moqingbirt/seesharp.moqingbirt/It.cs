namespace seesharp.moqingbirt
{
    using System;
    using System.Linq;

    public class It
    {
        public static T IsAny<T>()
        {
            var delayedCompute = new Func<T>(() =>
                {
                    if (MatchIt<T>.LastMatch.Any())
                    {
                        throw new InvalidOperationException("There is already a matching pending (It.Is<T>() or It.IsAny<T>(). There might be a bug in Moqingbirt. Were you using multithreading by any chance ?");
                    }

                    // always match.
                    MatchIt<T>.LastMatch.Add( x => true);

                    return default(T);
                });

            return delayedCompute();
        }

    }
}