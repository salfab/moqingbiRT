namespace seesharp.moqingbirt
{
    using System;
    using System.Collections.Generic;

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
        List<Tuple<string, object[], object>> Setups { get; }
    }
}