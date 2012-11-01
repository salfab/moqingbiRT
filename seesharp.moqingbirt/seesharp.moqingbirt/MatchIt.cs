namespace seesharp.moqingbirt
{
    using System;
    using System.Collections.Generic;

    static public class MatchIt
    {
        static MatchIt()
        {
            LastMatch = new List<Func<object, bool>>();
            RecordMatches = false;
        }    

        public static List<Func<object, bool>> LastMatch { get; private set; }

        public static bool RecordMatches { get; set; }
    }
}