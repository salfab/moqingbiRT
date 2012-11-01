using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace Specs
{
    using System.Diagnostics;
    using System.IO;
    using System.Reflection;

    using seesharp.moqingbirt;
    using seesharp.moqingbirt.TestBench;
    using System.Composition.Hosting;

    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestProperties()
        {
            var t = typeof(List<string>);
            var mock = new Mock<IMyInjectedService>();
            mock.Setup(o => o.IsAvailable).Returns(true);
            Assert.IsTrue(mock.Object.IsAvailable);

            mock.SetupSet(o => o.IsAvailable = It.IsAny<bool>());
            mock.Setup(o => o.IsAvailable).Returns(false);

            mock.Setup(o => o.ReturnAnInteger()).Returns(1);
            mock.Setup(o => o.SetAnInteger(1.0));

            mock.Object.SetAnInteger(1.0);

            mock.Verify(o => o.SetAnInteger(1.0), 1);
            Assert.IsFalse(mock.Object.IsAvailable);

            mock.VerifyGet(o => o.IsAvailable, 2);
            mock.VerifySet(o => o.IsAvailable = false, Times.Never());
            mock.Object.IsAvailable = false;
            mock.Object.IsAvailable = true;
            mock.VerifySet(o => o.IsAvailable = false, Times.Once());
            mock.VerifySet(o => o.IsAvailable = false, Times.AtLeast(0));
            mock.VerifySet(o => o.IsAvailable = true, Times.Exactly(1));
            mock.VerifySet(o => o.IsAvailable = It.IsAny<bool>(), Times.Exactly(2));
            mock.VerifySet(o => o.IsAvailable = It.IsAny<bool>(), Times.Exactly(2));
        }

        [TestMethod]
        public void TestSettingPropertyToValueNotConsideredInSetupMustFail()
        {
            var t = typeof(List<string>);
            var mock = new Mock<IMyInjectedService>();
            mock.SetupSet(o => o.IsAvailable = false);
           
            mock.Object.IsAvailable = false;
            try
            {
                mock.Object.IsAvailable = true;
            }
            catch (Exception)
            {
                // ok !   
                return;
            }
            Assert.Fail("We must not ignore the fact that SetupSet is only configured for value 'false'.");
            //mock.VerifySet(o => o.IsAvailable = true, Times.Exactly(1));
        }

        [TestMethod]
        public void TestSettingPropertyToAnyValueDoesNotThrowAnExceptionForAnyOfItsCall()
        {
            var t = typeof(List<string>);
            var mock = new Mock<IMyInjectedService>();
            mock.SetupSet(o => o.IsAvailable = It.IsAny<bool>());

            mock.Object.IsAvailable = false;
            mock.Object.IsAvailable = true;
          
            // Assert.Fail("We must not ignore the fact that SetupSet is only configured for value 'false'.");
            //mock.VerifySet(o => o.IsAvailable = true, Times.Exactly(1));
        }

        [TestMethod]
        public void TestMethodsWithIsAny()
        {
            var mock = new Mock<IMyInjectedService>();
            Guid returnValue = Guid.NewGuid();
            mock
                .Setup(o => o.SetAnOtherInteger(It.IsAny<double>()))
                .Returns(returnValue);
            
            Assert.AreSame(returnValue, mock.Object.SetAnOtherInteger(5.0));
        }

        [TestMethod]
        public void TestMethodsWithDouble()
        {
            var mock = new Mock<IMyInjectedService>();
            Guid returnValue = Guid.NewGuid();
            mock
                .Setup(o => o.SetAnOtherInteger(5.0))
                .Returns(returnValue);

            Guid anOtherInteger = mock.Object.SetAnOtherInteger(5.0);
            Assert.AreEqual(returnValue, anOtherInteger);
        }


        //[TestMethod]
        //public void Test()
        //{
        //    var absoluteDllPaths = new List<string>();
        //    var serviceProvider = Host as IServiceProvider;
        //    var dte = serviceProvider.GetService(typeof(DTE)) as DTE;
        //    var projects = dte.Solution.Projects;//this.Host.ResolveParameterValue("-", "-", "projects").Split('|');
        //    Debug.WriteLine("// Mocks generated for :");

        //    foreach (var project in projects)
        //    {
        //        Debug.WriteLine(string.Empty);
        //        string projectFullPath = string.Empty;
        //        string outputFileName = string.Empty;
        //        string outputPath = string.Empty;
        //        var configManager = ((EnvDTE.Project)project)
        //                .ConfigurationManager;
        //        foreach (ProjectItem pi in ((EnvDTE.Project)project).ProjectItems)
        //        {
        //            Debug.WriteLine("// project item name : " + pi.Name);
        //            if (pi.FileCodeModel != null)
        //            {
        //                foreach (CodeElement item in pi.FileCodeModel.CodeElements)
        //                {
        //                    if (item.Kind.ToString() == "vsCMElementNamespace")
        //                    {
        //                        foreach (CodeElement ce in item.Children)
        //                        {
        //                            Debug.WriteLine("//     item children : " + item.FullName);
        //                        }
        //                        // Pay attention to the interface ISpecifiedServiceInOtherNamespace : it is in a sub-namespace : do we resolve it ?
        //                        foreach (CodeElement ce in ((CodeNamespace)item).Members)
        //                        {
        //                            var ce2 = ce as CodeInterface;
        //                            if (ce2 != null)
        //                            {
        //                                Debug.WriteLine("//     Interface name : " + ce2.Name);
        //                                Debug.WriteLine("namespace " + ((CodeNamespace)item).Name + ".Mocks\r\n{");
        //                                Debug.WriteLine("    using System;");
        //                                Debug.WriteLine("    using System.Linq;");
        //                                Debug.WriteLine(string.Empty);
        //                                Debug.WriteLine(string.Format("    public class {0}Mock : MockBase, {1}", ce2.Name.TrimStart('I'), ce2.Name));
        //                                Debug.WriteLine("    {");

        //                                foreach (CodeElement interfaceMember in ce2.Members)
        //                                {
        //                                    // Debug.WriteLine("//         member of interface: " + interfaceMember.Kind);			
        //                                    // Debug.WriteLine("//         member of interface: " + interfaceMember.Name);										
        //                                    if (interfaceMember.Kind.ToString() == "vsCMElementProperty")
        //                                    {
        //                                        var property = (CodeProperty)interfaceMember;
        //                                        Debug.WriteLine("//         Property : " + property.Name);
        //                                        Debug.WriteLine("//         Property Type : " + ((CodeTypeRef)property.Type).AsString);
        //                                        Debug.WriteLine("//         Property : " + property.Access);
        //                                        // Debug.WriteLine("//         Property : " + prop.Comment);										
        //                                        // Debug.WriteLine("//         Property : " + prop.Attributes);										
        //                                        // Debug.WriteLine("//         Property : " + prop.Getter);										
        //                                        // Debug.WriteLine("//         Property : " + prop.Setter);
        //                                        var propType = ((CodeTypeRef)property.Type).AsString;
        //                                        Debug.WriteLine("        public " + propType + " " + property.Name);
        //                                        Debug.WriteLine("        { ");

        //                                        // TODO : Record the calls
        //                                        Debug.WriteLine("            get");
        //                                        Debug.WriteLine("            {");
        //                                        Debug.WriteLine("                this.PropertyGetCalls.Add(\"" + property.Name + "\");");
        //                                        Debug.WriteLine("                return (" + propType + ")this.Setups.Single(o => o.Item1 == \"" + property.Name + "\").Item3;");
        //                                        Debug.WriteLine("            }");


        //                                        // TODO : Record the calls
        //                                        Debug.WriteLine("            set");
        //                                        Debug.WriteLine("            {");
        //                                        Debug.WriteLine("                if (!this.IsVerifySetInProgess)");
        //                                        Debug.WriteLine("                {");
        //                                        Debug.WriteLine("                    this.PropertySetCalls.Add(new Tuple<string, object>(\"" + property.Name + "\", value));");
        //                                        Debug.WriteLine("                }");
        //                                        Debug.WriteLine("                else");
        //                                        Debug.WriteLine("                {");
        //                                        Debug.WriteLine("                    // todo : make a single object containing the count AND the name of the method last verified.");
        //                                        Debug.WriteLine("                    this.LastVerifySetMatchesCount = this.PropertySetCalls.Count(o => o.Item1 == \"" + property.Name + "\" && MatchIt<" + propType + ">.LastMatch != null ? MatchIt<" + propType + ">.LastMatch(value) : (" + propType + ")o.Item2 ==  value);");
        //                                        Debug.WriteLine("                    this.LastVerifySetPropertyName = \"" + property.Name + "\";");
        //                                        Debug.WriteLine("                    MatchIt<" + propType + ">.LastMatch = null;");
        //                                        Debug.WriteLine("                }");
        //                                        Debug.WriteLine("            }");

        //                                        Debug.WriteLine("        }");

        //                                    }
        //                                    if (interfaceMember.Kind.ToString() == "vsCMElementFunction")
        //                                    {
        //                                        var function = (CodeFunction2)interfaceMember;
        //                                        Debug.WriteLine("//         Function : " + function.FullName);
        //                                        Debug.WriteLine("//         Function : " + function.Name);
        //                                        var returnType = ((CodeTypeRef)function.Type).AsString;
        //                                        Debug.WriteLine("//         Function Type: " + ((CodeTypeRef)function.Type).AsString);

        //                                        // http://msdn.microsoft.com/en-US/library/envdte.vscmaccess(v=vs.80).aspx
        //                                        Debug.WriteLine("//         Function : " + function.Access);
        //                                        Debug.WriteLine("//         Function IsGeneric : " + function.IsGeneric);

        //                                        // is it needed ?
        //                                        Debug.WriteLine("//         Function OL : " + function.Overloads);

        //                                        //Debug.WriteLine("//         Function args : " + function.Parameters);	

        //                                        string parms = string.Empty;

        //                                        string typeName = returnType;
        //                                        // if(returnType == typeof(void))
        //                                        // {
        //                                        // 	typeName = "void";
        //                                        // }
        //                                        // else
        //                                        // {
        //                                        // 	typeName = returnType;					
        //                                        // }
        //                                        Write(string.Format("        public {0} {1}(", typeName, function.FullName));
        //                                        //var parameters = method.GetParameters();
        //                                        bool isFirstIteration = true;
        //                                        foreach (CodeParameter param in function.Parameters)
        //                                        {
        //                                            var paramType = ((CodeTypeRef)param.Type).AsString;
        //                                            //Debug.WriteLine("//             Function arg Type: " + ((CodeTypeRef)param.Type).AsString);	
        //                                            //Debug.WriteLine("//             Function arg Name: " + param.Name);	
        //                                            if (!isFirstIteration)
        //                                            {
        //                                                Write(", ");
        //                                            }
        //                                            Write(string.Format("{0} {1}", paramType, param.Name));
        //                                            isFirstIteration = false;
        //                                        }

        //                                        Debug.WriteLine(")");

        //                                        Debug.WriteLine("        {");
        //                                        Write("            var callRecord = new Tuple<string, object[]>( \"" + function.FullName + "\", new object[] { ");

        //                                        var paramsCount = ((CodeElements)function.Parameters).Count;
        //                                        if (paramsCount == 0)
        //                                        {
        //                                            Write("null");
        //                                        }
        //                                        else
        //                                        {
        //                                            isFirstIteration = true;
        //                                            foreach (CodeParameter param in function.Parameters)
        //                                            {
        //                                                if (isFirstIteration == false)
        //                                                {
        //                                                    Write(", ");
        //                                                }
        //                                                //var paramType = ((CodeTypeRef)param.Type).AsString;																									
        //                                                Write(param.Name);
        //                                                isFirstIteration = false;
        //                                            }
        //                                            // Write("/* "+paramsCount+"  */");
        //                                            // Write(function.Parameters.Item(0).ToString());	
        //                                            // for (int i = 1; i < paramsCount; i++)
        //                                            // {
        //                                            // 	Write(string.Format(", {0}", function.Parameters.Item(i).Name));	              
        //                                            // }									
        //                                        }
        //                                        Debug.WriteLine(" });");
        //                                        Debug.WriteLine("            this.Calls.Add(callRecord);");
        //                                        // within the body of the class.



        //                                        // return value of the method 
        //                                        if (typeName != "void")
        //                                        {
        //                                            Debug.WriteLine("            return default(" + typeName + ");");
        //                                        }
        //                                        Debug.WriteLine("        }");
        //                                        Debug.WriteLine(string.Empty);


        //                                    }
        //                                }
        //                                Debug.WriteLine("    }");
        //                                Debug.WriteLine("}");
        //                                Debug.WriteLine(string.Empty);
        //                            }
        //                            // var ce3 = ce as CodeClass;
        //                            // if(ce3 != null)
        //                            // {
        //                            // 	Debug.WriteLine("// member Class : " + ce3.Name);							
        //                            // }
        //                            // else
        //                            // {
        //                            // 	Debug.WriteLine("// member Class : " + ce.Kind);
        //                            // }
        //                        }
        //                    }
        //                    // Debug.WriteLine("// NameSpace : " + ((CodeNamespace)item).Name);

        //                }
        //            }
        //        }
        //        if (configManager != null)
        //        {
        //            var activeConfig = configManager.ActiveConfiguration;
        //            var msg = string.Empty;

        //            foreach (Property prop in ((EnvDTE.Project)project).Properties)
        //            {
        //                if (prop.Name == "FullPath")
        //                {
        //                    projectFullPath = prop.Value.ToString();
        //                    Debug.WriteLine("// " + prop.Name + " = " + prop.Value);
        //                }
        //                if (prop.Name == "OutputFileName")
        //                {
        //                    outputFileName = prop.Value.ToString();
        //                    Debug.WriteLine("// " + prop.Name + " = " + prop.Value);
        //                }
        //                var val = "null";
        //                try
        //                {
        //                    if (prop.Value != null)
        //                        val = prop.Value.ToString();
        //                }
        //                catch
        //                {
        //                }
        //            }

        //            foreach (Property prop in activeConfig.Properties)
        //            {
        //                if (prop.Name == "OutputPath")
        //                {
        //                    outputPath = prop.Value.ToString();
        //                    Debug.WriteLine("// " + prop.Name + " = " + prop.Value);
        //                }
        //                //msg += " \r\n//   " + prop.Name + " = " + prop.Value;				
        //            }
        //            //Debug.WriteLine(msg);
        //            var absoluteDllPath = Path.Combine(Path.Combine(projectFullPath, outputPath), outputFileName);
        //            absoluteDllPaths.Add(absoluteDllPath);
        //            Debug.WriteLine("// --> " + absoluteDllPath);
        //        }
        //    }
        //}
    }
}
