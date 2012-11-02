namespace seesharp.moqingbirt
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;

    public class Mock<T>
    {
        private readonly IMock mockImplementation;

        public T Object { get; private set; }

        public Mock()
        {
            var mockType = this.FindMockedType();
            this.mockImplementation = (IMock)Activator.CreateInstance(mockType);
            this.Object = (T)this.mockImplementation;
        }

        private Type FindMockedType()
        {
            string interfaceName = typeof(T).Name;
            if (!interfaceName.StartsWith("I", StringComparison.Ordinal))
            {
                throw new InvalidOperationException(string.Format("The interface {0} does not start with an I. we couldn't find a proper mock implementation for it.", interfaceName));
            }

            var typeName = string.Format("{0}.Mocks.{1}Mock", typeof(T).Namespace, interfaceName.Substring(1));

            var type = Type.GetType(typeName);

            if (type == null)
            {
                throw new NotImplementedException("The following mock implementation could not be found : \r\n\r\n" + typeName);
            }

            return type;
        }

        public bool Verify(Expression<Action<T>> expression, int expectedTimesCalled)
        {
            var methodName = ((MethodCallExpression)expression.Body).Method.Name;
            var arguments = ((MethodCallExpression)expression.Body).Arguments;

            MatchIt.RecordMatches = true;
            var evaluatedValues = arguments.Select(
                a =>
                    {
                        var matchesCount = MatchIt.LastMatch.Count;
                        var c = Expression.Lambda(a).Compile();
                        var value = c.DynamicInvoke();

                        // no matches has been added, i.e. it is not com,ing from It.IsAny or It.Is ! it's a scalar value for which we need to build a predicate.
                        if (matchesCount == MatchIt.LastMatch.Count)
                        {
                            MatchIt.LastMatch.Add(o => o.Equals(value));
                        }
                        return value;
                    }).ToArray();

            MatchIt.RecordMatches = false;
            var matchpredicates = MatchIt.LastMatch.ToArray();
            MatchIt.LastMatch.Clear();

            var callsToMethod = this.mockImplementation.Calls.Where(o => o.Item1 == methodName && this.PredicatesMatchCallArguments(matchpredicates, o.Item2));
            int actualCallsCount = callsToMethod.Count();
            bool isMatching = actualCallsCount == expectedTimesCalled;
            if (!isMatching)
            {
                throw new Exception(methodName + " was called " + actualCallsCount + " times. Expected was " + expectedTimesCalled);
            }
            return isMatching;
        }

        private bool PredicatesMatchCallArguments(Func<object,bool>[] argumentPredicates, object[] item2)
        {
            var allPredicatesPass = true;
            for (int i = 0; i < item2.Length; i++)
            {
                if (argumentPredicates[i](item2[i]) == false)
                {
                    allPredicatesPass = false;
                    i = item2.Length;
                }
            }
            return allPredicatesPass;
        }

        public Setup<T, TReturn> Setup<TReturn>(Expression<Func<T, TReturn>> expression)
        {
            return new Setup<T, TReturn>((IMock)this.Object, expression);
        }

        public Setup<T> Setup(Expression<Action<T>> expression)
        {
            return new Setup<T>((IMock)this.Object, expression);
        }

        public void VerifyGet<TReturn>(Expression<Func<T, TReturn>> expression, int expectedTimesCalled)
        {
            var memberExpression = expression.Body as MemberExpression;
            if (memberExpression == null)
            {
                throw new ArgumentException("passed expression is not a property", "expression");
            }

            string propertyName = memberExpression.Member.Name;
            var callsToGetter = this.mockImplementation.PropertyGetCalls.Where(o => o == propertyName);
            int actualCallsCount = callsToGetter.Count();

            bool isMatching = actualCallsCount == expectedTimesCalled;
            if (!isMatching)
            {
                throw new Exception(propertyName + " getter was called " + actualCallsCount + " times. Expected was " + expectedTimesCalled);
            }
        }

        //public void VerifySet<TResult>(Expression<Func<T, TResult>> expression, int expectedTimesCalled)
        //{
        //    var memberExpression = expression.Body as MemberExpression;
        //    if (memberExpression == null)
        //    {
        //        throw new ArgumentException("passed expression is not a property", "expression");
        //    }

        //    string propertyName = memberExpression.Member.Name;

        //    var c = Expression.Lambda(expression).Compile();
        //    object specifiedValue = c.DynamicInvoke();


        //    var callsToSetter = this.mockImplementation.PropertySetCalls.Where(o => o.Item1 == propertyName && o.Item2 == specifiedValue);
        //    int actualCallsCount = callsToSetter.Count();

        //    bool isMatching = actualCallsCount == expectedTimesCalled;
        //    if (!isMatching)
        //    {
        //        throw new Exception(propertyName + " setter was called " + actualCallsCount + " times. Expected was " + expectedTimesCalled);
        //    }
        //}

        //public void VerifySet(Action<T> expression, int expectedCallsCount)
        //{
        //    this.mockImplementation.StartVerifySet(this.Object, expression);           

        //    var verificationDetail = this.mockImplementation.StopVerifySet(this.Object, expression);

        //    int actualCallsCount = verificationDetail.Item2;
        //    bool isMatching = actualCallsCount == expectedCallsCount;
        //    if (!isMatching)
        //    {
        //        throw new Exception(verificationDetail.Item1 + " setter was called " + actualCallsCount + " times. Expected was " + expectedCallsCount);
        //    }
        //}

        public void VerifySet(Action<T> expression, Times expectedTimesCalled)
        {
            this.mockImplementation.StartVerifySet(this.Object, expression);

            var verificationDetail = this.mockImplementation.StopVerifySet(this.Object, expression);

            int actualCallsCount = verificationDetail.Item2;
            bool isMatching = expectedTimesCalled.MatchTimes(actualCallsCount);
            if (!isMatching)
            {
                var timesSpelling = actualCallsCount > 1 ? " times" : " time";
                throw new Exception(verificationDetail.Item1 + " setter was called " + actualCallsCount + timesSpelling + ". Expected was " + expectedTimesCalled);
            }
        }

        public void SetupSet(Action<T> expression)
        {
            var mockedObject = ((IMock)this.Object);
            mockedObject.ApplySetupSet(expression);
        }
    }
}