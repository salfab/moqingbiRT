using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace seesharp.moqingbirt.GenerateMockClassesTask
{
    using Microsoft.Build.Framework;

    public class GenerateMockClassesTask : ITask
    {
        public bool Execute()
        {            
            BuildEngine.LogMessageEvent(new BuildMessageEventArgs("It Worked !", "none", "me !", MessageImportance.Low));
            return false;
        }

        public IBuildEngine BuildEngine { get; set; }

        public ITaskHost HostObject { get; set; }
    }
}
