namespace seesharp.moqingbirt
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    public abstract class MockBase : IMock
    {
        protected MockBase()
        {
            this.Calls = new List<Tuple<string, object[]>>();
            this.PropertyGetCalls = new List<string>();
            this.PropertySetCalls = new List<Tuple<string, object>>();
            this.SetupsPropertySet = new List<Tuple<string, Func<object, bool>>>();
            this.Setups = new List<Tuple<string, Func<object, bool>[], object>>();
        }

        protected bool IsVerifySetInProgess { get; private set; }

        public List<Tuple<string, object>> PropertySetCalls { get; private set; }

        public List<string> PropertyGetCalls { get; private set; }

        public List<Tuple<string, object[]>> Calls { get; private set; }

        public List<Tuple<string, Func<object, bool>[], object>> Setups { get; private set; }

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

            MatchIt.LastMatch.Clear();
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
            MatchIt.RecordMatches = true;
            expression.DynamicInvoke(new object[] {this});
            MatchIt.RecordMatches = false;
            //var newSetupCounts = this.SetupsPropertySet.Count;

            //if (newSetupCounts - setupCounts != 1)
            //{
            //    throw new InvalidOperationException("It seems that calling ApplySetupGet did in fact create more than one setup. There might be a bug in Moqingbirt. Were you using multithreading by any chance ?");
            //}

            MatchIt.LastMatch.Clear();

            this.IsApplySetupSetInProgess = false;

            //return new SetupSet<T>(this.SetupsPropertySet.Last());
        }

        public void ApplySetupReturns<T, TReturn>(Expression<Func<T, TReturn>> expression, TReturn returnValue)
        {
            // we already know we're not null here. courtesy of the caller.
            var methodCallExpression = expression.Body as MethodCallExpression;
            
            var methodName = methodCallExpression.Method.Name;
            var arguments = methodCallExpression.Arguments.ToArray();

            if (this.IsApplySetupReturnsInProgess)
            {
                throw new InvalidOperationException("There is already an evaluation of the arguments pending for a property set. There might be a bug in Moqingbirt. Were you using multithreading by any chance ?");
            }

            this.IsApplySetupReturnsInProgess = true;

            // build a list of predicated of all the available parameters.
            MatchIt.RecordMatches = true;
            foreach (var argument in arguments)
            {
                var recordedMatches = MatchIt.LastMatch.Count;
                var compiledArgument = Expression.Lambda(argument).Compile();
                var specifiedValue = compiledArgument.DynamicInvoke();
                if (recordedMatches == MatchIt.LastMatch.Count)
                {
                    // Nothing has been added in MatchIt by It.IsAny or It.Is : It was not a predicate, but a real value : handle accordingly.
                    MatchIt.LastMatch.Add(o => o.Equals(specifiedValue));
                }

            }
            MatchIt.RecordMatches = false;

            //IEnumerable<Func<object, bool>> evaluatedArgs = arguments.ForEach()
            //    arg =>
            //        {
            //            var c = Expression.Lambda(arg).Compile();
            //            var specifiedValue = c.DynamicInvoke();
            //            return new Func<object, bool>(o => o.Equals(specifiedValue));
            //            //return specifiedValue;
            //        });

            var oldSetups = this.Setups.ToArray();

            var c = expression.Compile();
            c.DynamicInvoke(new object[] { this });

            var addedSetups = this.Setups.Except(oldSetups);

            var addedSetup = addedSetups.Single();

            // addedSetup.Item3 = returnValue;

            // FIXME : replace this dodgy implementation with a proper class which is not immutable.
            var setupWithReturnValue = new Tuple<string, Func<object, bool>[], object>(addedSetup.Item1, addedSetup.Item2, returnValue);
            Setups.Remove(addedSetup);
            Setups.Add(setupWithReturnValue);

            //var evargs = evaluatedArgs.ToArray();

            // FIXME : use IEnumerable instead of arrays if arrays are not needed.
            //var configEntry = new Tuple<string, Func<object, bool>[], object>(methodName,/* evaluatedArgs.ToArray()*/ evargs, returnValue);
            //this.Setups.Add(configEntry);
            MatchIt.LastMatch.Clear();
            this.IsApplySetupReturnsInProgess = false;
        }

        protected bool IsApplySetupReturnsInProgess { get; set; }

        protected bool IsApplySetupSetInProgess { get; set; }

        protected bool AllArgumentsSatisfyPredicates(IEnumerable<object> arguments, IEnumerable<Func<object, bool>> predicates)
        {
            var allPredicatesSatisfied = true;
            var predicatesArray = predicates as Func<object, bool>[] ?? predicates.ToArray();
            var itemsCount = predicatesArray.Count();
            var enumerable = arguments as object[] ?? arguments.ToArray();
            for (var i = 0; i < itemsCount && allPredicatesSatisfied == true; i++)
            {
                if (!predicatesArray.ElementAt(i)(enumerable.ElementAt(i)))
                {
                    allPredicatesSatisfied = false;   
                }
            }
            return allPredicatesSatisfied;
        }
    }
}