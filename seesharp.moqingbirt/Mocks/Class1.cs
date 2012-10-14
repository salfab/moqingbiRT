using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mocks
{
    using System.Reflection;
using System.Composition;

    using seesharp.moqingbirt;
    using System.Composition.Hosting;

    [Export(typeof(IMock))]
    public class Class1
    {

        public Class1()
        {

        }
        private int test1;
        
        public void test()
        {
            //var catalog = new ApplicationCatalog();
            //var container = new CompositionContainer(catalog.CreateCompositionService());
            //for (int i = 0; i < 5; i++)
            //{
            //    var dude = container.GetExportedValue<IDude>();
            //    Console.WriteLine(dude.Say());
            //}

            System.Reflection.Assembly.Load(new AssemblyName()).ExportedTypes.First().GetRuntimeProperties().First().PropertyType.Name.TrimStart('I');
        }     
    }
}
