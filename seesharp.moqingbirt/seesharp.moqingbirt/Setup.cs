namespace seesharp.moqingbirt
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    public class Setup<T, TReturn>
    {
        private readonly IMock mockedObject;

        private readonly Expression<Func<T, TReturn>> expression;

        public Setup(IMock mockedObject, Expression<Func<T, TReturn>> expression)
        {
            this.mockedObject = mockedObject;
            this.expression = expression;
        }

        public Setup<T, TReturn> Returns(TReturn returnValue)
        {
            var methodCallExpression = this.expression.Body as MethodCallExpression;
            if (methodCallExpression == null)
            {
                var memberExpression = ((MemberExpression)this.expression.Body);
                var propertyName = memberExpression.Member.Name;
                var newReturnSetup = new Tuple<string, object[], object>(propertyName, null, returnValue);
                var conflictingSetups = this.mockedObject.Setups.Where(o => o.Item1 == propertyName);

                var enumeration = conflictingSetups.ToArray();

                for (var i = 0; i < enumeration.Count(); i++)
                {
                    this.mockedObject.Setups.Remove(enumeration[i]);
                }      
       
                this.mockedObject.Setups.Add(newReturnSetup);                
            }
            else
            {
                var methodName = methodCallExpression.Method.Name;
                var arguments = methodCallExpression.Arguments.ToArray();

                var evaluatedArgs = arguments.Select(
                    arg =>
                        {
                            var c = Expression.Lambda(arg).Compile();
                            var specifiedValue = c.DynamicInvoke();
                            return specifiedValue;
                        });


                // FIXME : use IEnumerable instead of arrays if arrays are not needed.
                var configEntry = new Tuple<string, object[], object>(methodName, evaluatedArgs.ToArray(), returnValue);
                this.mockedObject.Setups.Add(configEntry);
            }

            return this;
        }
    }

    public class Setup<T>
    {
        private readonly IMock mockedObject;

        private readonly Expression<Action<T>> expression;

        public Setup(IMock mockedObject, Expression<Action<T>> expression)
        {
            this.mockedObject = mockedObject;
            this.expression = expression;
        }    
    }
}
