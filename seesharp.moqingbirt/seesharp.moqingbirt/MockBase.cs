namespace seesharp.moqingbirt
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public abstract class MockBase : IMock
    {
        protected MockBase()
        {
            this.Calls = new List<Tuple<string, object[]>>();
            this.PropertyGetCalls = new List<string>();
            this.PropertySetCalls = new List<Tuple<string, object>>();
            this.SetupsPropertySet = new List<Tuple<string, Func<object, bool>>>();
            this.Setups = new List<Tuple<string, object[], object>>();
        }

        protected bool IsVerifySetInProgess { get; private set; }

        public List<Tuple<string, object>> PropertySetCalls { get; private set; }

        public List<string> PropertyGetCalls { get; private set; }

        public List<Tuple<string, object[]>> Calls { get; private set; }

        public List<Tuple<string, object[], object>> Setups { get; private set; }

        public List<Tuple<string, Func<object, bool>>> SetupsPropertySet { get; private set; }

        protected int LastVerifySetMatchesCount { get; set; }
        
        protected string LastVerifySetPropertyName { get; set; }

        public void StartVerifySet<T>(T mockedOject, Action<T> expression)
        {
            if (IsVerifySetInProgess)
            {
                throw new InvalidOperationException("There is already a verification pending for a property set. There might be a bug in Moqingbirt. Were you using multithreading by any chance ?");
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

        public void ApplySetupSet<T>(Action<T> expression)
        {
            if (this.IsApplySetupSetInProgess)
            {
                throw new InvalidOperationException("There is already an evaluation of the arguments pending for a property set. There might be a bug in Moqingbirt. Were you using multithreading by any chance ?");
            }

            this.IsApplySetupSetInProgess = true;

            // var setupCounts = this.SetupsPropertySet.Count;
            expression.DynamicInvoke(new object[] {this});
            //var newSetupCounts = this.SetupsPropertySet.Count;

            //if (newSetupCounts - setupCounts != 1)
            //{
            //    throw new InvalidOperationException("It seems that calling ApplySetupGet did in fact create more than one setup. There might be a bug in Moqingbirt. Were you using multithreading by any chance ?");
            //}

            this.IsApplySetupSetInProgess = false;

            //return new SetupSet<T>(this.SetupsPropertySet.Last());
        }

        protected bool IsApplySetupSetInProgess { get; set; }
    }
}