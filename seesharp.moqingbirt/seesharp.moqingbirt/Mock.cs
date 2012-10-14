namespace seesharp.moqingbirt
{
    using System;
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

            var argumentsValues = arguments.Select(
                a =>
                    {
                        var c = Expression.Lambda(a).Compile();
                        var value = c.DynamicInvoke();
                        return value;
                    });

            var callsToMethod = this.mockImplementation.Calls.Where(o => o.Item1 == methodName && o.Item2.SequenceEqual(argumentsValues));
            int actualCallsCount = callsToMethod.Count();
            bool isMatching = actualCallsCount == expectedTimesCalled;
            if (!isMatching)
            {
                throw new Exception(methodName + " was called " + actualCallsCount + " times. Expected was " + expectedTimesCalled);
            }
            return isMatching;
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
                throw new Exception(verificationDetail.Item1 + " setter was called " + actualCallsCount + " times. Expected was " + expectedTimesCalled);
            }
        }
    }
}