namespace seesharp.moqingbirt
{
    using System;
    using System.Collections.Generic;

    public abstract class MockBase : IMock
    {
        protected MockBase()
        {
            this.Calls = new List<Tuple<string, object[]>>();
            this.PropertyGetCalls = new List<string>();
            this.PropertySetCalls = new List<Tuple<string, object>>();
            this.Setups = new List<Tuple<string, object[], object>>();
        }

        protected bool IsVerifySetInProgess { get; private set; }

        public List<Tuple<string, object>> PropertySetCalls { get; private set; }

        public List<string> PropertyGetCalls { get; private set; }

        public List<Tuple<string, object[]>> Calls { get; private set; }

        public List<Tuple<string, object[], object>> Setups { get; private set; }

        protected int LastVerifySetMatchesCount { get; set; }
        
        protected string LastVerifySetPropertyName { get; set; }

        public void StartVerifySet<T>(T mockedOject, Action<T> expression)
        {
            if (IsVerifySetInProgess)
            {
                throw new InvalidOperationException("There is already a verification pending. There might be a bug in Moqingbirt. Were you using multithreading by any chance ?");
            }

            this.IsVerifySetInProgess = true;

            expression.Invoke(mockedOject);
        }

        public Tuple<string, int> StopVerifySet<T>(T mockedOject, Action<T> expression)
        {
            int lastVerifySetMatchesCount = this.LastVerifySetMatchesCount;

            MatchIt<T>.LastMatch = null;
            this.IsVerifySetInProgess = false;

            return new Tuple<string, int>(LastVerifySetPropertyName, lastVerifySetMatchesCount);
        }

    }
}