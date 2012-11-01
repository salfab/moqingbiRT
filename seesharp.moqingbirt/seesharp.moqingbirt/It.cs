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
                    // always match.
                    if (MatchIt.RecordMatches == true)
                    {
                        MatchIt.LastMatch.Add( x => true);
                    }

                    return default(T);
                });

            return delayedCompute();
        }

    }
}