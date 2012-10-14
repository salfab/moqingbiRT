namespace seesharp.moqingbirt
{
    using System;

    static public class MatchIt<T>
    {
        public static Func<T, bool> LastMatch { get; set; }
    }
}