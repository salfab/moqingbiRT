namespace seesharp.moqingbirt
{
    using System;

    public class MockException : Exception
    {
        public MockException(string message)
            : base(message)
        {            
        }
    }
}