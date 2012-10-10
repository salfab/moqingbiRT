		
namespace seesharp.moqingbirt.TestBench.Mocks
{
    using System;
    using System.Linq;

    public class MyInjectedServiceMock : MockBase, IMyInjectedService
    {
        public System.Int32 ReturnAnInteger()
        {
            return default(System.Int32);
        }

        public void SetAnInteger(System.Double myDouble)
        {
        }

        public System.Boolean IsAvailable
        { 
            get
            {
                this.PropertyGetCalls.Add("IsAvailable");
                return (System.Boolean)this.Setups.Single(o => o.Item1 == "IsAvailable").Item3;
            }
            set
            {
                this.PropertySetCalls.Add(new Tuple<string, object>("IsAvailable", value));
            }
        }
    }
}

namespace seesharp.moqingbirt.TestBench.NewNameSpace.Mocks
{
    using System;
    using System.Linq;

    public class SpecifiedServiceInOtherNamespaceMock : MockBase, ISpecifiedServiceInOtherNamespace
    {
        public System.String SayMyName(System.Globalization.CultureInfo language)
        {
            return default(System.String);
        }

        public System.String MyName
        { 
            get
            {
                this.PropertyGetCalls.Add("MyName");
                return (System.String)this.Setups.Single(o => o.Item1 == "MyName").Item3;
            }
            set
            {
                this.PropertySetCalls.Add(new Tuple<string, object>("MyName", value));
            }
        }
    }
}

