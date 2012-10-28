using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace seesharp.moqingbirt.TestBench
{
    public class MyInjectedService : IMyInjectedService
    {
        public bool IsAvailable { get; set; }

        public int ReturnAnInteger()
        {
            return 3;
        }

        public void SetAnInteger(double myDouble)
        {            
        }

        public Guid SetAnOtherInteger(double myDouble)
        {
            throw new NotImplementedException();
        }
    }
}
