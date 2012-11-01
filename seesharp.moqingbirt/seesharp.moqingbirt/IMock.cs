namespace seesharp.moqingbirt
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;

    public interface IMock
    {
        List<Tuple<string, object>> PropertySetCalls { get; }

        List<string> PropertyGetCalls { get; }

        List<Tuple<string, object[]>> Calls { get; }

        /// <summary>
        /// Tuple :
        /// Item 1 : method Name.
        /// Item 2 : params given to the method.
        /// Item 3 : configured returned value.
        /// </summary>
        List<Tuple<string, Func<object, bool>[], object>> Setups { get; }
        
        
        List<Tuple<string, Func<object, bool>>> SetupsPropertySet { get; }

        void StartVerifySet<T>(T mockedOject, Action<T> expression);

        // FIXME : Change this stupid Tuple type with a proper type that conveys the semantics behind those types.
        Tuple<string, int> StopVerifySet<T>(T mockedObject, Action<T> expression);

        void ApplySetupSet<T>(Action<T> expression);

        void ApplySetupReturns<T, TReturn>(Expression<Func<T, TReturn>> expression);
    }
}