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

        public bool Verify(Expression<Action<T>> expression, int timesCalled)
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
            return callsToMethod.Count() == timesCalled;
        }

        public Setup<T, TReturn> Setup<TReturn>(Expression<Func<T, TReturn>> expression)
        {
            return new Setup<T, TReturn>((IMock)this.Object, expression);
        }
    }
}