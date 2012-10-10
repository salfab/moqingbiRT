using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mocks
{
    using System.Reflection;

    class Class1
    {
        private int test1;

        public void test()
        {
            System.Reflection.Assembly.Load(new AssemblyName()).ExportedTypes.First().GetRuntimeProperties().First().PropertyType.Name.TrimStart('I');
        }     
    }
}
