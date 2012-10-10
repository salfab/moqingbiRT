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
        public List<Tuple<string, object>> PropertySetCalls { get; private set; }

        public List<string> PropertyGetCalls { get; private set; }

        public List<Tuple<string, object[]>> Calls { get; private set; }

        public List<Tuple<string, object[], object>> Setups { get; private set; }
    }
}