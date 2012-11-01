namespace seesharp.moqingbirt
{
    using System;
    using System.Collections.Generic;

    static public class MatchIt<T>
    {
        static MatchIt()
        {
            LastMatch = new List<Func<T, bool>>();
        }    

        public static List<Func<T, bool>> LastMatch { get; private set; }
    }
}