/*============================================================================
* Copyright (C) Microsoft Corporation, All rights reserved.
*=============================================================================
*/

using Microsoft.Management.Infrastructure;
using Microsoft.Management.Infrastructure.Options;
using Microsoft.Management.Infrastructure.Generic;
using System;
using System.IO;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Globalization;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Xunit;

namespace MMI.Tests.UnitTests
{
    public class CimSessionTest
    {
        [Fact]
        public void Create_ComputerName_Null()
        {
            using (CimSession cimSession = CimSession.Create(null))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");
                Assert.Null(cimSession.ComputerName, "cimSession.ComputerName should null");
            }
        }

        [Fact]
        public void Create_ComputerName_Localhost()
        {
            using (CimSession cimSession = CimSession.Create("localhost"))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");
                Assert.Equal("localhost", cimSession.ComputerName, "cimSession.ComputerName should not be the same as the value passed to Create method");
                Assert.True(cimSession.ToString().Contains("localhost"), "cimSession.ToString should contain computer name");
            }
        }

        [Fact]
        public void Create_ComputerName_Nonexistant_DCOM()
        {
            using (CimSession cimSession = CimSession.Create("nonexistantcomputer", new DComSessionOptions()))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");
                Helpers.AssertException<CimException>(
                    () => cimSession.EnumerateInstances(@"root\cimv2", "Win32_Process"),
                    delegate (CimException exception)
                        {
                            Assert.Equal(NativeErrorCode.Failed, exception.NativeErrorCode, "Got the right NativeErrorCode");
                            Assert.Equal("MSFT_WmiError", exception.ErrorData.CimSystemProperties.ClassName, "Got the right kind of ErrorData");
                            Assert.Equal((uint)1, exception.StatusCode, "Got the right StatusCode");
                        });
            }
        }

        [Fact]
        public void Create_ComputerName_Nonexistant_WSMan()
        {
            using (CimSession cimSession = CimSession.Create("nonexistantcomputer", new WSManSessionOptions()))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");
                Helpers.AssertException<CimException>(
                        () => cimSession.EnumerateInstances(@"root\cimv2", "Win32_Process"),
                        delegate (CimException exception)
                        {
                            Assert.Equal(NativeErrorCode.Failed, exception.NativeErrorCode, "Got the right NativeErrorCode");
                            Assert.Equal("MSFT_WmiError", exception.ErrorData.CimSystemProperties.ClassName, "Got the right kind of ErrorData");
                            Assert.Equal((uint)1, exception.StatusCode, "Got the right StatusCode");
                            if (Thread.CurrentThread.CurrentUICulture.DisplayName.Equals("en-US", StringComparison.OrdinalIgnoreCase))
                            {
                                Assert.True(exception.Message.Contains("winrm"), "Got the right Message (1)");
                                Assert.True(exception.Message.Contains("cannot find"), "Got the right Message (2)");
                                Assert.True(exception.Message.Contains("computer"), "Got the right Message (3)");
                                Assert.True(exception.Message.Contains("nonexistantcomputer"), "Got the right Message (4)");
                            }
                        });
            }
        }

        [Fact]
        public void Create_ComputerName_EnvironmentMachineName()
        {
            string machineName = Environment.MachineName;
            using (CimSession cimSession = CimSession.Create(machineName))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");
                Assert.Equal(machineName, cimSession.ComputerName, "cimSession.ComputerName should not be the same as the value passed to Create method");
            }
        }

        [Fact]
        public void Close()
        {
            using (CimSession cimSession = CimSession.Create(null))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");
                cimSession.Close();
            }
        }

        [Fact]
        public void AlreadyDisposed_Close_Then_Close()
        {
            CimSession cimSession = CimSession.Create(null);
            Assert.NotNull(cimSession, "cimSession should not be null");
            cimSession.Close();
            cimSession.Close(); // expecting no exception - Close/CloseAsync/Dispose should be idempotent
        }

        [Fact]
        public void AlreadyDisposed_Dispose_Then_Close()
        {
            CimSession cimSession = CimSession.Create(null);
            Assert.NotNull(cimSession, "cimSession should not be null");
            cimSession.Dispose();
            cimSession.Close(); // expecting no exception - Close/CloseAsync/Dispose should be idempotent
        }
 
        [Fact]
        public void EnumerateInstances_ClassName_NotFound()
        {
            using (CimSession cimSession = CimSession.Create(null))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");
                Helpers.AssertException<CimException>(
                        () => cimSession.EnumerateInstances(@"root\cimv2", "nonExistantClassHere").Count(),                     
                        delegate (CimException exception)
                        {
                            Assert.Equal(NativeErrorCode.InvalidClass, exception.NativeErrorCode, "Got the right NativeErrorCode");
                            Assert.Equal("Invalid_Class", exception.Message, "Got the right message (1)");
                            Assert.Equal("InvalidClass", exception.Message, "Got the right message (2)");
                            Assert.True(exception.Message.IndexOf(' ') != (-1), "Got the right message (3)");
                        });
            }
        }

        [Fact]
        public void EnumerateInstances_ClassName_NotFound_EnumerationIsLazy()
        {
            using (CimSession cimSession = CimSession.Create(null))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");
                cimSession.EnumerateInstances(@"root\cimv2", "NotExistantClass");
            }
        }

        [Fact]
        public void EnumerateInstances()
        {
            using (CimSession cimSession = CimSession.Create(null))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");
                IEnumerable<CimInstance> enumeratedInstances = cimSession.EnumerateInstances(@"root\cimv2", "Win32_Process");
                Assert.NotNull(enumeratedInstances, "cimSession.EnumerateInstances returned something other than null");
                Assert.True(enumeratedInstances.Count() > 0, "Got some results back from CimSession.EnumerateInstances");
            }
        }

        [Fact]
        public void EnumerateInstances_SessionComputerName()
        {
            using (CimSession cimSession = CimSession.Create(Environment.MachineName, new WSManSessionOptions()))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");
                CimInstance topInstance = cimSession.EnumerateInstances(@"root\cimv2", "CIM_ProcessExecutable").FirstOrDefault();
                Assert.NotNull(topInstance, "topInstance should not be null");
                Assert.Equal(Environment.MachineName, topInstance.GetCimSessionComputerName(), "Verifying topInstance.GetCimSessionComputerName");

                CimInstance embeddedInstance = (CimInstance)topInstance.CimInstanceProperties["Antecedent"].Value;
                Assert.Equal(Environment.MachineName, embeddedInstance.GetCimSessionComputerName(), "Verifying embeddedInstance.GetCimSessionComputerName");

                Assert.Equal(topInstance.GetCimSessionInstanceId(), embeddedInstance.GetCimSessionInstanceId(), "Verifying GetCimSessionInstanceId");
            }
        }

        public CimSession EnumerateInstances_AbandonedEnumerator_Helper()
        {
            CimSession cimSession = CimSession.Create(null, new WSManSessionOptions());
            Assert.NotNull(cimSession, "cimSession should not be null");
            IEnumerable<CimInstance> enumeratedInstances = cimSession.EnumerateInstances(@"root\cimv2", "Win32_Process");
            Assert.NotNull(enumeratedInstances, "cimSession.EnumerateInstances returned something other than null");

            IEnumerator<CimInstance> enumerator = enumeratedInstances.GetEnumerator();
            enumerator.MoveNext();

            return cimSession;
        }

        [Fact]
        public void EnumerateInstances_AbandonedEnumerator()
        {
            CimSession cimSession = EnumerateInstances_AbandonedEnumerator_Helper();

            Thread.Sleep(1000);
            GC.Collect();
            GC.WaitForPendingFinalizers();
            Thread.Sleep(1000);
            GC.Collect();
            GC.WaitForPendingFinalizers();
            Thread.Sleep(1000);
            GC.Collect();
            GC.WaitForPendingFinalizers();

            // this will hang if garbage collection has not drained the sync operation
            cimSession.Close();
        }

        [Fact]
        public void EnumerateInstances_IsValueModified()
        {
            using (CimSession cimSession = CimSession.Create(null))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");
                IEnumerable<CimInstance> enumeratedInstances = cimSession.EnumerateInstances(@"root\cimv2", "Win32_Process");
                Assert.NotNull(enumeratedInstances, "cimSession.EnumerateInstances returned something other than null");
                CimInstance cimInstance = enumeratedInstances.FirstOrDefault();
                Assert.NotNull(cimInstance, "cimSession.EnumerateInstances returned some instances");

                foreach (CimProperty property in cimInstance.CimInstanceProperties)
                {
                    Assert.True(!property.IsValueModified, "No property should be modified at this point / " + property.Name);
                }

                CimProperty testProperty = cimInstance.CimInstanceProperties["Name"];
                testProperty.Value = "foo";
                Assert.True(testProperty.IsValueModified, "Test property should be marked as modified (test point #10)");
                testProperty.IsValueModified = false;
                Assert.True(!testProperty.IsValueModified, "Test property should be marked as not modified (test point #12)");
                testProperty.IsValueModified = true;
                Assert.True(testProperty.IsValueModified, "Test property should be marked as modified (test point #14)");
            }
        }

        [Fact]
        public void EnumerateInstances_IsValueModified_ValueEquality()
        {
            using (CimSession cimSession = CimSession.Create(null))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");
                IEnumerable<CimInstance> enumeratedInstances = cimSession.EnumerateInstances(@"root\cimv2", "Win32_Process");
                Assert.NotNull(enumeratedInstances, "cimSession.EnumerateInstances returned something other than null");
                CimInstance cimInstance = enumeratedInstances.FirstOrDefault();
                Assert.NotNull(cimInstance, "cimSession.EnumerateInstances returned some instances");

                foreach (CimProperty property in cimInstance.CimInstanceProperties)
                {
                    Assert.True(!property.IsValueModified, "No property should be modified at this point / " + property.Name);
                }

                CimProperty testProperty = cimInstance.CimInstanceProperties["InstallDate"];
                object origValue = testProperty.Value;
                Assert.Null(origValue, "Sanity check - test assumes that InstallDate of first/idle/system process is null");

                testProperty.IsValueModified = false;
                Assert.True(!testProperty.IsValueModified, "Test property should be marked as not modified (test point #12)");
                Assert.Null(testProperty.Value, "Orig null value should be retained (test point #12)");

                testProperty.IsValueModified = true;
                Assert.True(testProperty.IsValueModified, "Test property should be marked as modified (test point #14)");
                Assert.Null(testProperty.Value, "Orig null value should be retained (test point #14)");
            }
        }

        [Fact]
        public void GetClass_Property_Null()
        {       
            using (CimSession cimSession = CimSession.Create(null))
            {
                CimClass enumeratedClass = cimSession.GetClass(@"root\cimv2", "Win32_Process");
                Assert.NotNull(enumeratedClass, "cimSession.GetClass returneded null");
                Assert.Equal((string)enumeratedClass.CimSystemProperties.ClassName, "Win32_Process", "CimClass.CimSystemProperties.ClassName returneded null");
                Assert.Throws<ArgumentNullException>(() => {
                    CimPropertyDeclaration prop = enumeratedClass.CimClassProperties[null];
                    return prop; });                
            }
        }

        [Fact]
        public void GetClass_Method_Null()
        {
            using (CimSession cimSession = CimSession.Create(null))
            {
                CimClass enumeratedClass = cimSession.GetClass(@"root\cimv2", "Win32_Process");
                Assert.NotNull(enumeratedClass, "cimSession.GetClass returneded null");
                Assert.Equal("Win32_Process", (string)enumeratedClass.CimSystemProperties.ClassName, "CimClass.CimSystemProperties.ClassName returneded null");            
                Assert.Throws<ArgumentNullException>(() => {
                    CimMethodDeclaration method = enumeratedClass.CimClassMethods[null];
                    return method;
                });
            }
        }

        [Fact]
        public void GetClass_MethodParameter_Null()
        {
            using (CimSession cimSession = CimSession.Create(null))
            {
                CimClass enumeratedClass = cimSession.GetClass(@"root\cimv2", "Win32_Process");
                Assert.NotNull(enumeratedClass, "cimSession.GetClass returneded null");
                Assert.Equal("Win32_Process", (string)enumeratedClass.CimSystemProperties.ClassName, "CimClass.CimSystemProperties.ClassName returneded null");
                CimMethodDeclaration method = enumeratedClass.CimClassMethods["Create"];
                Assert.Throws<ArgumentNullException>(() =>
                {
                    CimMethodParameterDeclaration param = method.Parameters[null];
                    return param;
                });
            }
        }

        [Fact]
        public void GetClass_MethodParameterQualifier_Null()
        {
            using (CimSession cimSession = CimSession.Create(null))
            {
                CimClass enumeratedClass = cimSession.GetClass(@"root\cimv2", "Win32_Process");
                Assert.NotNull(enumeratedClass, "cimSession.GetClass returneded null");
                Assert.Equal("Win32_Process", (string)enumeratedClass.CimSystemProperties.ClassName, "CimClass.CimSystemProperties.ClassName returneded null");
                Assert.Throws<ArgumentNullException>(() =>
                {
                    CimMethodDeclaration method = enumeratedClass.CimClassMethods["Create"];
                    CimMethodParameterDeclaration param = method.Parameters["CommandLine"];
                    CimQualifier qualifier = param.Qualifiers[null];
                    return qualifier;
                });
            }
        }

        [Fact]
        public void GetClass_PropertyQualifier_Null()
        {
            using (CimSession cimSession = CimSession.Create(null))
            {
                CimClass enumeratedClass = cimSession.GetClass(@"root\cimv2", "Win32_Process");
                Assert.NotNull(enumeratedClass, "cimSession.GetClass returneded null");
                Assert.Equal("Win32_Process", (string)enumeratedClass.CimSystemProperties.ClassName, "CimClass.CimSystemProperties.ClassName returneded null");
                Assert.Throws<ArgumentNullException>(() =>
                {
                    CimPropertyDeclaration prop = enumeratedClass.CimClassProperties["CommandLine"];
                    CimQualifier qualifier = prop.Qualifiers[null];
                    return qualifier;
                });
            }
        }

        [Fact]
        public void GetClassMethod()
        {
            using (CimSession cimSession = CimSession.Create(null))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");
                CimClass enumeratedClass = cimSession.GetClass(@"root\cimv2", "Win32_Process");
                Assert.NotNull(enumeratedClass, "cimSession.GetClass returneded null");
                Assert.Equal("Win32_Process", (string)enumeratedClass.CimSystemProperties.ClassName, "CimClass.CimSystemProperties.ClassName returneded null");
                Assert.NotNull(enumeratedClass.CimClassMethods, "CimClass.CimClassMethods returneded null");
                Assert.NotNull(enumeratedClass.CimClassMethods["Create"], @"CimClass.CimClassMethods[Create] returneded null");
                //method qualifier
                Assert.NotNull(enumeratedClass.CimClassMethods["Create"].Qualifiers, @"CimClass.CimClassMethods[Create].CimClassQualifiers returneded null");
                Assert.NotNull(enumeratedClass.CimClassMethods["Create"].Qualifiers["static"], @"CimClass.CimClassMethods[Create].CimClassQualifiers[static] returneded null");
                Assert.Equal(true, (Boolean)enumeratedClass.CimClassMethods["Create"].Qualifiers["static"].Value, @"CimClass.CimClassMethods[Create].CimClassQualifiers[static].Value is not true");
                //method parameter
                Assert.NotNull(enumeratedClass.CimClassMethods["Create"].Parameters, @"CimClass.CimClassMethods[Create].Parameters returneded null");
                Assert.NotNull(enumeratedClass.CimClassMethods["Create"].Parameters["CommandLine"], @"CimClass.CimClassMethods[Create].Parameters[CommandLine] returneded null");
                Assert.Equal("CommandLine", enumeratedClass.CimClassMethods["Create"].Parameters["CommandLine"].Name, @"CimClass.CimClassMethods[Create].Parameters[CommandLine].Name returneded null");
                //Method parameter qualifier
                Assert.NotNull(enumeratedClass.CimClassMethods["Create"].Parameters["CommandLine"].Qualifiers, @"CimClass.CimClassMethods[Create].Parameters[CommandLine].CimClassQualifiers returneded null");
                Assert.NotNull(enumeratedClass.CimClassMethods["Create"].Parameters["CommandLine"].Qualifiers["ID"], @"CimClass.CimClassMethods[Create].Parameters[CommandLine].CimClassQualifiers[ID returneded null");
                Assert.Equal(CimType.SInt32, (CimType)enumeratedClass.CimClassMethods["Create"].Parameters["CommandLine"].Qualifiers["ID"].CimType, @"CimClass.CimClassMethods[Create].Parameters[CommandLine].CimClassQualifiers[ID returneded null");
            }
        }

        [Fact]
        public void GetClassSync()
        {
            using (CimSession cimSession = CimSession.Create(null))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");
                CimClass enumeratedClass = cimSession.GetClass(@"root\cimv2", "CIM_Process");
                Assert.NotNull(enumeratedClass, "cimSession.GetClass returneded null");
                Assert.Equal("CIM_Process", (string)enumeratedClass.CimSystemProperties.ClassName, "CimClass.CimSystemProperties.ClassName returneded null");
                Assert.Equal("CIM_LogicalElement", (string)enumeratedClass.CimSuperClassName, "CimClass.CimSuperClassName returneded null");
                //Assert.NotNull(enumeratedClass.CimSuperClass, "CimClass.CimSuperClassName returneded null");
                Assert.Equal(@"root/cimv2", (string)enumeratedClass.CimSystemProperties.Namespace, "CimClass.CimSystemProperties.Namespace returneded null");
                Assert.NotNull(enumeratedClass.CimClassProperties, "CimClass.CimClassProperties returneded null");
                Assert.NotNull(enumeratedClass.CimClassProperties["Handle"], @"CimClass.CimClassProperties[Handle] returneded null");

                Assert.Equal("Handle", (string)enumeratedClass.CimClassProperties["Handle"].Name, @"CimClass.CimClassProperties[Handle].Name returneded null");
                Assert.Equal(CimType.String, (CimType)enumeratedClass.CimClassProperties["Handle"].CimType, @"CimClass.CimClassProperties[Handle].type returneded null");
                // Assert.Equal(enumeratedClass.CimClassProperties["Handle"].Flags & CimFlags.Key, CimFlags.Key, @"CimClass.CimClassProperties[Handle].Flags returneded null");

                Assert.NotNull(enumeratedClass.CimClassProperties["Handle"].Qualifiers, @"CimClass.CimClassProperties[Handle].Qualifiers returneded null");
                Assert.Equal("read", (string)enumeratedClass.CimClassProperties["Handle"].Qualifiers["read"].Name, @"CimClass.CimClassProperties[Handle].Qualifiers[Cim_Key].Name returneded null");
                Assert.Equal(CimType.Boolean, (CimType)enumeratedClass.CimClassProperties["Handle"].Qualifiers["read"].CimType, @"CimClass.CimClassProperties[Handle].Qualifiers[read].Type returneded null");
                //Assert.Equal(enumeratedClass.CimClassProperties["Handle"].CimClassQualifiers["read"].Flags & CimFlags.DisableOverride, CimFlags.DisableOverride, @"CimClass.CimClassProperties[Handle].CimClassQualifiers[read].Flags returneded null");
                //Assert.Equal((bool)enumeratedClass.CimClassProperties["Handle"].CimClassQualifiers["read"].Value, true, @"CimClass.CimClassProperties[Handle].CimClassQualifiers[Cim_Key].Value returneded null");

                //Check class qualifiers
                Assert.NotNull(enumeratedClass.CimClassQualifiers, "CimClass.CimClassQualifiers returneded null");
                Assert.NotNull(enumeratedClass.CimClassQualifiers["Abstract"], "CimClass.CimClassQualifiers[Abstract] returneded null");
                Assert.Equal("Abstract", enumeratedClass.CimClassQualifiers["Abstract"].Name, "CimClass.CimClassQualifiers[Abstract] returneded null");
                Assert.Equal(CimType.Boolean, (CimType)enumeratedClass.CimClassQualifiers["Abstract"].CimType, @"enumeratedClass.CimClassQualifiers[Abstract].Type returneded null");
                //Assert.Equal(enumeratedClass.CimClassQualifiers["Abstract"].Flags & CimFlags.DisableOverride, CimFlags.DisableOverride, @"enumeratedClass.CimClassQualifiers[Abstract].Flags returneded null");

                //Check System properties
                //System properties Environment.MachineName
                Assert.Equal("root/cimv2", enumeratedClass.CimSystemProperties.Namespace, "cimInstance.CimSystemProperties.Namespace is not correct");
                Assert.Equal("CIM_Process", enumeratedClass.CimSystemProperties.ClassName, "cimInstance.CimSystemProperties.ClassName is not correct");
                Assert.Equal(Environment.MachineName, enumeratedClass.CimSystemProperties.ServerName, "cimInstance.CimSystemProperties.ServerName is not correct");
                Assert.Null(enumeratedClass.CimSystemProperties.Path);
                /*Assert.Equal(enumeratedClass.CimSystemProperties.Path,
                        "//" + Environment.MachineName + "/root/cimv2:CIM_Process",
                        "cimInstance.CimSystemProperties.Path is not correct " + enumeratedClass.CimSystemProperties.Path);
                */
            }
        }

        [Fact]
        public void GetClassASync()
        {
            using (CimSession cimSession = CimSession.Create(null))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");
                IObservable<CimClass> asyncInstance = cimSession.GetClassAsync(@"root\cimv2", "CIM_Process");
                List<AsyncItem<CimClass>> asyncList = Helpers.ObservableToList(asyncInstance);

                Assert.Equal(2, asyncList.Count, "Got 2 async callbacks");
                Assert.Equal(AsyncItemKind.Item, asyncList[0].Kind, "First callback for item");
                Assert.Equal(AsyncItemKind.Completion, asyncList[1].Kind, "Second callback for completion");
                CimClass enumeratedClass = asyncList[0].Item;


                Assert.NotNull(enumeratedClass, "cimSession.GetClass returneded null");
                Assert.NotNull(enumeratedClass.CimSuperClass, "cimSession.CimSuperClass is null");
                Assert.Equal("CIM_Process", (string)enumeratedClass.CimSystemProperties.ClassName, "CimClass.CimSystemProperties.ClassName returneded null");
                Assert.Equal("CIM_LogicalElement", (string)enumeratedClass.CimSuperClassName, "CimClass.CimSuperClassName returneded null");
                Assert.Equal("CIM_LogicalElement", (string)enumeratedClass.CimSuperClassName, "CimClass.CimSuperClassName returneded null");
                //Assert.NotNull(enumeratedClass.CimSuperClass, "CimClass.CimSuperClassName returneded null");
                Assert.Equal(@"root/cimv2", (string)enumeratedClass.CimSystemProperties.Namespace, "CimClass.CimSystemProperties.Namespace returneded null");
                Assert.NotNull(enumeratedClass.CimClassProperties, "CimClass.CimClassProperties returneded null");
                Assert.NotNull(enumeratedClass.CimClassProperties["Handle"], @"CimClass.CimClassProperties[Handle] returneded null");
                Assert.Equal("Handle", (string)enumeratedClass.CimClassProperties["Handle"].Name, @"CimClass.CimClassProperties[Handle].Name returneded null");
                Assert.Equal(CimType.String, (CimType)enumeratedClass.CimClassProperties["Handle"].CimType, @"CimClass.CimClassProperties[Handle].type returneded null");
                Assert.NotNull(enumeratedClass.CimClassProperties["Handle"].Qualifiers, @"CimClass.CimClassProperties[Handle].Qualifiers returneded null");
                Assert.Equal("read", (string)enumeratedClass.CimClassProperties["Handle"].Qualifiers["read"].Name, @"CimClass.CimClassProperties[Handle].Qualifiers[Cim_Key].Name returneded null");
                Assert.Equal(CimType.Boolean, (CimType)enumeratedClass.CimClassProperties["Handle"].Qualifiers["read"].CimType, @"CimClass.CimClassProperties[Handle].Qualifiers[read].Type returneded null");


                Assert.NotNull(enumeratedClass.CimClassProperties, "CimClass.CimClassProperties returneded null");
                Assert.NotNull(enumeratedClass.CimClassProperties["Handle"], @"CimClass.CimClassProperties[Handle] returneded null");
                Assert.Equal("Handle", (string)enumeratedClass.CimClassProperties["Handle"].Name, @"CimClass.CimClassProperties[Handle].Name returneded null");
                Assert.Equal(CimType.String, (CimType)enumeratedClass.CimClassProperties["Handle"].CimType, @"CimClass.CimClassProperties[Handle].type returneded null");
                Assert.NotNull(enumeratedClass.CimClassProperties["Handle"].Qualifiers, @"CimClass.CimClassProperties[Handle].Qualifiers returneded null");
                Assert.Equal("read", (string)enumeratedClass.CimClassProperties["Handle"].Qualifiers["read"].Name, @"CimClass.CimClassProperties[Handle].Qualifiers[Cim_Key].Name returneded null");
                Assert.Equal(CimType.Boolean, (CimType)enumeratedClass.CimClassProperties["Handle"].Qualifiers["read"].CimType, @"CimClass.CimClassProperties[Handle].Qualifiers[read].Type returneded null");



                // Assert.Equal(enumeratedClass.CimClassProperties["Handle"].Flags & CimFlags.Key, CimFlags.Key, @"CimClass.CimClassProperties[Handle].Flags returneded null");

                //Assert.Equal(enumeratedClass.CimClassProperties["Handle"].CimClassQualifiers["read"].Flags & CimFlags.DisableOverride, CimFlags.DisableOverride, @"CimClass.CimClassProperties[Handle].CimClassQualifiers[read].Flags returneded null");
                //Assert.Equal((bool)enumeratedClass.CimClassProperties["Handle"].CimClassQualifiers["read"].Value, true, @"CimClass.CimClassProperties[Handle].CimClassQualifiers[Cim_Key].Value returneded null");

                //Check class qualifiers
                Assert.NotNull(enumeratedClass.CimClassQualifiers, "CimClass.CimClassQualifiers returneded null");
                Assert.NotNull(enumeratedClass.CimClassQualifiers["Abstract"], "CimClass.CimClassQualifiers[Abstract] returneded null");
                Assert.Equal("Abstract", enumeratedClass.CimClassQualifiers["Abstract"].Name, "CimClass.CimClassQualifiers[Abstract] returneded null");
                Assert.Equal(CimType.Boolean, (CimType)enumeratedClass.CimClassQualifiers["Abstract"].CimType, @"enumeratedClass.CimClassQualifiers[Abstract].Type returneded null");

                //Check class qualifiers
                Assert.NotNull(enumeratedClass.CimClassQualifiers, "CimClass.CimClassQualifiers returneded null");
                Assert.NotNull(enumeratedClass.CimClassQualifiers["Abstract"], "CimClass.CimClassQualifiers[Abstract] returneded null");
                Assert.Equal("Abstract", enumeratedClass.CimClassQualifiers["Abstract"].Name, "CimClass.CimClassQualifiers[Abstract] returneded null");
                Assert.Equal(CimType.Boolean, (CimType)enumeratedClass.CimClassQualifiers["Abstract"].CimType, @"enumeratedClass.CimClassQualifiers[Abstract].Type returneded null");

                //Assert.Equal(enumeratedClass.CimClassQualifiers["Abstract"].Flags & CimFlags.DisableOverride, CimFlags.DisableOverride, @"enumeratedClass.CimClassQualifiers[Abstract].Flags returneded null");

                Assert.Equal(asyncList.Last().Kind, AsyncItemKind.Completion, "Got OnCompleted callback");

            }
        }

        [Fact]
        public void EnumerateClassesSync()
        {
            using (CimSession cimSession = CimSession.Create(null))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");
                IEnumerable<CimClass> enumeratedClassList = cimSession.EnumerateClasses(@"root\cimv2", "CIM_LogicalElement");
                Assert.NotNull(enumeratedClassList, "cimSession.EnumerateInstances returned something other than null");
                Assert.True(enumeratedClassList.Count() > 0, "Got some results back from CimSession.EnumerateInstances");

                IEnumerator enumerator = enumeratedClassList.GetEnumerator();
                CimClass enumeratedClass = null;
                while (enumerator.MoveNext())
                {
                    if (string.Compare(((CimClass)enumerator.Current).CimSystemProperties.ClassName, "Win32_Process", StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        enumeratedClass = (CimClass)enumerator.Current;
                    }
                }
                Assert.NotNull(enumeratedClass, "cimSession.GetClass returneded null");
                Assert.Equal("Win32_Process", (string)enumeratedClass.CimSystemProperties.ClassName, "CimClass.CimSystemProperties.ClassName returneded null");
                //Assert.Equal((string)enumeratedClass.CimSuperClassName, "Win32_Process", "CimClass.CimSuperClassName returneded null");
                Assert.Equal(@"root/cimv2", (string)enumeratedClass.CimSystemProperties.Namespace, "CimClass.CimSystemProperties.Namespace returneded null");
                Assert.NotNull(enumeratedClass.CimClassProperties, "CimClass.CimClassProperties returneded null");
                Assert.NotNull(enumeratedClass.CimClassProperties["HandleCount"], @"CimClass.CimClassProperties[Handle] returneded null");

                Assert.Equal("HandleCount", (string)enumeratedClass.CimClassProperties["HandleCount"].Name, @"CimClass.CimClassProperties[Handle].Name returneded null");
                Assert.Equal(CimType.UInt32, (CimType)enumeratedClass.CimClassProperties["HandleCount"].CimType, @"CimClass.CimClassProperties[HandleCount].type returneded null");
                // Assert.Equal(enumeratedClass.CimClassProperties["Handle"].Flags & CimFlags.Key, CimFlags.Key, @"CimClass.CimClassProperties[Handle].Flags returneded null");

                Assert.NotNull(enumeratedClass.CimClassProperties["HandleCount"].Qualifiers, @"CimClass.CimClassProperties[HandleCount].Qualifiers returneded null");
                Assert.Equal("read", (string)enumeratedClass.CimClassProperties["HandleCount"].Qualifiers["read"].Name, @"CimClass.CimClassProperties[HandleCount].Qualifiers[Cim_Key].Name returneded null");
                Assert.Equal(CimType.Boolean, (CimType)enumeratedClass.CimClassProperties["HandleCount"].Qualifiers["read"].CimType, @"CimClass.CimClassProperties[HandleCount].Qualifiers[read].Type returneded null");
                // Assert.Equal(enumeratedClass.CimClassProperties["Handle"].CimClassQualifiers["read"].Flags & CimFlags.DisableOverride, CimFlags.DisableOverride, @"CimClass.CimClassProperties[Handle].CimClassQualifiers[read].Flags returneded null");
                //Assert.Equal((bool)enumeratedClass.CimClassProperties["Handle"].CimClassQualifiers["read"].Value, true, @"CimClass.CimClassProperties[Handle].CimClassQualifiers[Cim_Key].Value returneded null");

                //Check class qualifiers
                Assert.NotNull(enumeratedClass.CimClassQualifiers, "CimClass.CimClassQualifiers returneded null");
                Assert.NotNull(enumeratedClass.CimClassQualifiers["dynamic"], "CimClass.CimClassQualifiers[dynamic] returneded null");
                Assert.Equal("dynamic", enumeratedClass.CimClassQualifiers["dynamic"].Name, "CimClass.CimClassQualifiers[dynamic] returneded null");
                Assert.Equal(CimType.Boolean, (CimType)enumeratedClass.CimClassQualifiers["dynamic"].CimType, @"enumeratedClass.CimClassQualifiers[dynamic].Type returneded null");
                //Assert.Equal(enumeratedClass.CimClassQualifiers["Abstract"].Flags & CimFlags.DisableOverride, CimFlags.DisableOverride, @"enumeratedClass.CimClassQualifiers[Abstract].Flags returneded null");

            }
        }

        [Fact]
        public void EnumerateClassesASync()
        {
            using (CimSession cimSession = CimSession.Create(null))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");
                IObservable<CimClass> asyncInstance = cimSession.EnumerateClassesAsync(@"root\cimv2", "CIM_LogicalElement");
                List<AsyncItem<CimClass>> asyncList = Helpers.ObservableToList(asyncInstance);

                Assert.True(asyncList.Count > 50, "Got less than 50 async callbacks");
                Assert.Equal(AsyncItemKind.Item, asyncList[0].Kind, "First callback for item");
                CimClass enumeratedClass = null;
                foreach (AsyncItem<CimClass> item in asyncList)
                {
                    if (item.Kind == AsyncItemKind.Item)
                    {
                        if (string.Compare(item.Item.CimSystemProperties.ClassName, "Win32_Process", StringComparison.OrdinalIgnoreCase) == 0)
                        {
                            enumeratedClass = item.Item;
                            break;
                        }
                    }
                }

                Assert.NotNull(enumeratedClass, "cimSession.GetClass returneded null");
                Assert.Equal("Win32_Process", (string)enumeratedClass.CimSystemProperties.ClassName, "CimClass.CimSystemProperties.ClassName returneded null");
                //Assert.Equal((string)enumeratedClass.CimSuperClassName, "Win32_Process", "CimClass.CimSuperClassName returneded null");
                Assert.Equal(@"root/cimv2", (string)enumeratedClass.CimSystemProperties.Namespace, "CimClass.CimSystemProperties.Namespace returneded null");
                Assert.NotNull(enumeratedClass.CimClassProperties, "CimClass.CimClassProperties returneded null");
                Assert.NotNull(enumeratedClass.CimClassProperties["HandleCount"], @"CimClass.CimClassProperties[Handle] returneded null");

                Assert.Equal("HandleCount", (string)enumeratedClass.CimClassProperties["HandleCount"].Name, @"CimClass.CimClassProperties[Handle].Name returneded null");
                Assert.Equal(CimType.UInt32, (CimType)enumeratedClass.CimClassProperties["HandleCount"].CimType, @"CimClass.CimClassProperties[HandleCount].type returneded null");
                // Assert.Equal(enumeratedClass.CimClassProperties["Handle"].Flags & CimFlags.Key, CimFlags.Key, @"CimClass.CimClassProperties[Handle].Flags returneded null");

                Assert.NotNull(enumeratedClass.CimClassProperties["HandleCount"].Qualifiers, @"CimClass.CimClassProperties[HandleCount].Qualifiers returneded null");
                Assert.Equal("read", (string)enumeratedClass.CimClassProperties["HandleCount"].Qualifiers["read"].Name, @"CimClass.CimClassProperties[HandleCount].Qualifiers[Cim_Key].Name returneded null");
                Assert.Equal(CimType.Boolean, (CimType)enumeratedClass.CimClassProperties["HandleCount"].Qualifiers["read"].CimType, @"CimClass.CimClassProperties[HandleCount].Qualifiers[read].Type returneded null");
                // Assert.Equal(enumeratedClass.CimClassProperties["Handle"].CimClassQualifiers["read"].Flags & CimFlags.DisableOverride, CimFlags.DisableOverride, @"CimClass.CimClassProperties[Handle].CimClassQualifiers[read].Flags returneded null");
                //Assert.Equal((bool)enumeratedClass.CimClassProperties["Handle"].CimClassQualifiers["read"].Value, true, @"CimClass.CimClassProperties[Handle].CimClassQualifiers[Cim_Key].Value returneded null");

                //Check class qualifiers
                Assert.NotNull(enumeratedClass.CimClassQualifiers, "CimClass.CimClassQualifiers returneded null");
                Assert.NotNull(enumeratedClass.CimClassQualifiers["dynamic"], "CimClass.CimClassQualifiers[dynamic] returneded null");
                Assert.Equal("dynamic", enumeratedClass.CimClassQualifiers["dynamic"].Name, "CimClass.CimClassQualifiers[dynamic] returneded null");
                Assert.Equal(CimType.Boolean, (CimType)enumeratedClass.CimClassQualifiers["dynamic"].CimType, @"enumeratedClass.CimClassQualifiers[dynamic].Type returneded null");
                //Assert.Equal(enumeratedClass.CimClassQualifiers["Abstract"].Flags & CimFlags.DisableOverride, CimFlags.DisableOverride, @"enumeratedClass.CimClassQualifiers[Abstract].Flags returneded null");
                Assert.Equal(AsyncItemKind.Completion, asyncList.Last().Kind, "Got OnCompleted callback");

            }
        }

        [Fact]
        public void TestConnectionSync()
        {
            using (CimSession cimSession = CimSession.Create(null))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");
                CimInstance instance;
                CimException exception;
                bool bResult = cimSession.TestConnection(out instance, out exception);
                Assert.Equal(bResult, true, "TestConnection Failed");
                Assert.Null(exception, "Unexpectidly for CIMException");
            }
        }

        [Fact]
        public void TestConnectionASync()
        {
            using (CimSession cimSession = CimSession.Create(null))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");
                IObservable<CimInstance> asyncInstance = cimSession.TestConnectionAsync();
                List<AsyncItem<CimInstance>> asyncList = Helpers.ObservableToList(asyncInstance);

                //COM doesn't return an instance
                Assert.Equal(1, asyncList.Count, "Got 2 async callbacks");
                Assert.Equal(AsyncItemKind.Completion, asyncList[0].Kind, "First callback for completion");
            }
        }

        [Fact]
        public void TestConnectionSyncRemote()
        {
            using (CimSession cimSession = CimSession.Create(@"ABCD"))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");
                CimInstance instance;
                CimException exception;
                bool bResult = cimSession.TestConnection(out instance, out exception);
                Assert.Equal(false, bResult, "TestConnection Failed");
                Assert.NotNull(exception, "Unexpectidly for CIMException");
            }
        }

        [Fact]
        public void TestConnectionASyncRemote()
        {
            using (CimSession cimSession = CimSession.Create(@"ABCD"))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");
                IObservable<CimInstance> asyncInstance = cimSession.TestConnectionAsync();
                List<AsyncItem<CimInstance>> asyncList = Helpers.ObservableToList(asyncInstance);

                //COM doesn't return an instance
                Assert.Equal(1, asyncList.Count, "Got 2 async callbacks");
                Assert.Equal(AsyncItemKind.Exception, asyncList[0].Kind, "Got OnError callback");
                Assert.True(asyncList[0].Exception.GetType().Equals(typeof(CimException)), "Got right type of exception back");
            }
        }

        [Fact]
        public void EnumerateInstances_ReturnedProperties()
        {
            using (CimSession cimSession = CimSession.Create(null))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");
                List<CimInstance> enumeratedInstances = cimSession.EnumerateInstances(@"root\cimv2", "Win32_Process").ToList();
                Assert.NotNull(enumeratedInstances, "cimSession.EnumerateInstances returned something other than null");

                CimInstance currentProcess = enumeratedInstances.Single(x => (UInt32)(x.CimInstanceProperties["ProcessId"].Value) == Process.GetCurrentProcess().Id);
                Assert.NotNull(currentProcess, "cimSession.EnumerateInstances(..., Win32_Process) found the current process");
                Assert.Equal(Environment.MachineName, currentProcess.CimSystemProperties.ServerName, "Got correct CimInstance.CimSystemProperties.ServerName");
                Assert.Equal("Win32_Process", currentProcess.CimSystemProperties.ClassName, "Got correct CimInstance.CimSystemProperties.ClassName");
                Assert.True(Regex.IsMatch(currentProcess.CimSystemProperties.Namespace, "root.cimv2"), "Got correct CimInstance.CimSystemProperties.Namespace");

                Assert.Equal("Name", currentProcess.CimInstanceProperties["Name"].Name, "currentProcess.CimInstanceProperties['Name'].Name is correct");
                Assert.Equal(CimType.String, currentProcess.CimInstanceProperties["Name"].CimType, "currentProcess.CimInstanceProperties['Name'].CimType is correct");
                Assert.Equal("ttest.exe", (string)(currentProcess.CimInstanceProperties["Name"].Value), "currentProcess.CimInstanceProperties['Name'].Value is correct");
                Assert.Equal(CimFlags.Property | CimFlags.ReadOnly | CimFlags.NotModified, currentProcess.CimInstanceProperties["Name"].Flags, "currentProcess.CimInstanceProperties['Name'].Flags is correct");

                Assert.Equal(CimFlags.Key, currentProcess.CimInstanceProperties["Handle"].Flags & CimFlags.Key, "currentProcess.CimInstanceProperties['Handle'].Flags includes CimFlags.Key");

                Assert.True(
                    Regex.IsMatch(currentProcess.ToString(), "Win32_Process: ttest\\.exe \\(Handle = \"[0-9]*\"\\)", RegexOptions.IgnoreCase),
                    "currentProcess.ToString() returns expected value");
            }
        }

        [Fact]
        public void EnumerateInstances_CimClass()
        {
            using (CimSession cimSession = CimSession.Create(null))
            using (CimOperationOptions operationOptions = new CimOperationOptions())
            {
                operationOptions.Flags = CimOperationFlags.FullTypeInformation;
                Assert.NotNull(cimSession, "cimSession should not be null");
                List<CimInstance> enumeratedInstances = cimSession.EnumerateInstances(@"root\cimv2", "Win32_Process", operationOptions).ToList();
                Assert.NotNull(enumeratedInstances, "cimSession.EnumerateInstances returned something other than null");

                CimInstance currentProcess = enumeratedInstances.Single(x => (UInt32)(x.CimInstanceProperties["ProcessId"].Value) == Process.GetCurrentProcess().Id);
                Assert.NotNull(currentProcess, "cimSession.EnumerateInstances(..., Win32_Process) found the current process");
                Assert.Equal("Win32_Process", currentProcess.CimSystemProperties.ClassName, "Got correct CimInstance.CimSystemProperties.ClassName");

                using (CimClass cimClass = currentProcess.CimClass)
                {
                    Assert.NotNull(cimClass, "Got non-null cimClass");
                    Assert.Equal("Win32_Process", cimClass.CimSystemProperties.ClassName, "Got correct CimClass.CimSystemProperties.ClassName");
                    Assert.NotNull(cimClass.CimSuperClass, "Got non-null parentClass");
                    Assert.Equal("CIM_Process", cimClass.CimSuperClass.CimSystemProperties.ClassName, "Got correct CimClass.CimSuperClass.CimSystemProperties.ClassName");
                    Assert.Null(cimClass.CimClassQualifiers["nonExistantQualifier"], "Nonexistant qualifier returns null");
                    Assert.Null(cimClass.CimClassProperties["nonExistantProperty"], "Nonexistant property returns null");
                    Assert.Null(cimClass.CimClassMethods["nonExistantMethod"], "Nonexistant method returns null");
                }
            }
        }

        [Fact]
        public void EnumerateInstances_SettingReceivedProperty()
        {
            using (CimSession cimSession = CimSession.Create(null))
            using (CimOperationOptions operationOptions = new CimOperationOptions())
            {
                operationOptions.Flags = CimOperationFlags.FullTypeInformation;
                Assert.NotNull(cimSession, "cimSession should not be null");
                List<CimInstance> enumeratedInstances = cimSession.EnumerateInstances(@"root\cimv2", "Win32_DiskQuota", operationOptions).ToList();
                Assert.NotNull(enumeratedInstances, "cimSession.EnumerateInstances returned something other than null");

                CimInstance diskQuota = enumeratedInstances.FirstOrDefault();
                Assert.NotNull(diskQuota, "cimSession.EnumerateInstances(..., Win32_DiskQuota) found a disk quota");
                Assert.Equal("Win32_DiskQuota", diskQuota.CimSystemProperties.ClassName, "Got correct CimInstance.CimSystemProperties.ClassName");

                // this used to fail because of bug #308968
                diskQuota.CimInstanceProperties["Limit"].Value = 123;

                Assert.Equal((UInt64)(diskQuota.CimInstanceProperties["Limit"].Value), (UInt64)123, "Win32_DiskQuota.Limit got changed locally");
            }
        }

        [Fact]
        public void EnumerateInstances_ResultsAreNotNull()
        {
            using (CimSession cimSession = CimSession.Create(null))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");
                IEnumerable<CimInstance> enumeratedInstances = cimSession.EnumerateInstances(@"root\cimv2", "Win32_Process");
                Assert.NotNull(enumeratedInstances, "cimSession.EnumerateInstances returned something other than null");

                bool atLeastOneInstance = false;
                foreach (CimInstance cimInstance in enumeratedInstances)
                {
                    Assert.NotNull(cimInstance, "cimSession.EnumerateInstances returned something other than null");
                    atLeastOneInstance = true;
                }
                Assert.True(atLeastOneInstance, "cimSession.EnumerateInstances returned some instances");
            }
        }


        private class EnumerateInstances_Cancellation_FromTwoThreads_Helper : IObserver<CimInstance>
        {
            private IDisposable observableSubscription;
            private ManualResetEventSlim onNextEvent = new ManualResetEventSlim(initialState: false);
            private CancellationTokenSource cts;

            public EnumerateInstances_Cancellation_FromTwoThreads_Helper(CancellationTokenSource cts)
            {
                this.cts = cts;
            }

            public void WaitForOnNext()
            {
                this.onNextEvent.Wait();
            }

            public void DisposeSubscription()
            {
                this.observableSubscription.Dispose();
            }

            public void Subscribe(IObservable<CimInstance> observable)
            {
                observableSubscription = observable.Subscribe(this);
            }

            public void OnNext(CimInstance value)
            {
                Thread.Sleep(TimeSpan.FromMilliseconds(500));
                this.onNextEvent.Set();
                Thread.Sleep(TimeSpan.FromMilliseconds(500));
                //this.DisposeSubscription();
                cts.Cancel();
            }

            public void OnError(Exception error)
            {
            }

            public void OnCompleted()
            {
            }
        }

        [Fact]
        public void EnumerateInstances_Cancellation_FromTwoThreads()
        {
            Console.WriteLine();
            using (CimSession cimSession = CimSession.Create(null))
            {
                CancellationTokenSource cts = new CancellationTokenSource();
                CimOperationOptions oo = new CimOperationOptions() { CancellationToken = cts.Token };
                IObservable<CimInstance> observable = cimSession.EnumerateInstancesAsync("root/cimv2", "Win32_Process", oo);
                EnumerateInstances_Cancellation_FromTwoThreads_Helper observer = new EnumerateInstances_Cancellation_FromTwoThreads_Helper(cts);
                observer.Subscribe(observable);
                observer.WaitForOnNext();
                observer.DisposeSubscription();
                //cts.Cancel();
            }
        }

        [Fact]
        public void EnumerateInstances_SecondEnumeration()
        {
            using (CimSession cimSession = CimSession.Create(null))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");
                IEnumerable<CimInstance> enumeratedInstances = cimSession.EnumerateInstances(@"root\cimv2", "Win32_Process");

                int count1 = enumeratedInstances.Count();
                Assert.True(count1 > 0, "count1 > 0");

                int count2 = enumeratedInstances.Count();
                Assert.True(count2 > 0, "count2 > 0");

                int floor = Math.Max(count1, count2) / 2;
                Assert.True(count1 > floor, "count1 is reasonably close to count2");
                Assert.True(count2 > floor, "count2 is reasonably close to count1");
            }
        }

        [Fact]
        public void EnumerateInstancesAsync()
        {
            using (CimSession cimSession = CimSession.Create(null))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");
                IObservable<CimInstance> enumeratedInstances = cimSession.EnumerateInstancesAsync(@"root\cimv2", "Win32_Process");
                Assert.NotNull(enumeratedInstances, "cimSession.EnumerateInstancesAsync returned something other than null");

                List<AsyncItem<CimInstance>> listOfInstances = Helpers.ObservableToList(enumeratedInstances);
                Assert.True(listOfInstances.Count > 0, "Got some results back from CimSession.EnumerateInstancesAsync");
                Assert.Equal(AsyncItemKind.Completion, listOfInstances.Last().Kind, "Got OnCompleted callback");
            }
        }

        private static void EnumerateInstancesAsync_AnotherAppDomain_RunTest()
        {
            CimSession cimSession = CimSession.Create(null);
            IObservable<CimInstance> enumeratedInstances = cimSession.EnumerateInstancesAsync(@"root\cimv2", "Win32_Process");
            Helpers.ObservableToList(enumeratedInstances);
        }

        [Fact]
        public void EnumerateInstancesAsync_AnotherAppDomain()
        {
            AppDomainSetup domainSetup = new AppDomainSetup
            {
                ApplicationBase = AppDomain.CurrentDomain.SetupInformation.ApplicationBase,
                ConfigurationFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile,
                ApplicationName = AppDomain.CurrentDomain.SetupInformation.ApplicationName,
                LoaderOptimization = LoaderOptimization.MultiDomainHost
            };

            // To workaround problems with loading test code into the other appdomain
            // I am just copying the DRTs assembly to the base directory (the directory where TTest.exe is)
            try
            {
                File.Copy(
                    typeof(CimSessionTest).Assembly.Location,
                    Path.Combine(
                        domainSetup.ApplicationBase, Path.GetFileName(typeof(CimSessionTest).Assembly.Location)),
                    overwrite: true);
            }
            catch (IOException)
            {
            }

            AppDomain anotherAppDomain = AppDomain.CreateDomain("MyOtherAppDomain " + Guid.NewGuid(), null, domainSetup);
            try
            {
                anotherAppDomain.DoCallBack(EnumerateInstancesAsync_AnotherAppDomain_RunTest);
            }
            finally
            {
                AppDomain.Unload(anotherAppDomain);
            }
        }

        [Fact]
        public void EnumerateInstancesAsync_ClassName_NotFound()
        {
            using (CimSession cimSession = CimSession.Create(null))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");
                IObservable<CimInstance> enumeratedInstances = cimSession.EnumerateInstancesAsync(@"root\cimv2", "NotExistantClass");
                var serializedResult = Helpers.ObservableToList(enumeratedInstances);
                Assert.True(serializedResult.Count == 1, "Got some results back from CimSession.EnumerateInstancesAsync");
                Assert.Equal(AsyncItemKind.Exception, serializedResult[0].Kind, "Got OnError callback");
                Assert.True(serializedResult[0].Exception.GetType().Equals(typeof(CimException)), "Got right type of exception back");
                Assert.True(!string.IsNullOrWhiteSpace(serializedResult[0].Exception.StackTrace), "Exception has the stack trace filled in");
            }
        }

        [Fact]
        public void EnumerateInstancesAsync_ClassName_NotFound_SubscriptionIsLazy()
        {
            using (CimSession cimSession = CimSession.Create(null))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");
                cimSession.EnumerateInstancesAsync(@"root\cimv2", "NotExistantClass");
            }
        }

        public void EnumerateInstancesAsync_Cancellation_ViaCancellationToken_AfterOperationEnds()
        {
            using (CimSession cimSession = CimSession.Create(null))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");

                CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
                CimOperationOptions operationOptions = new CimOperationOptions { CancellationToken = cancellationTokenSource.Token };
                IObservable<CimInstance> enumeratedInstances = cimSession.EnumerateInstancesAsync("this.TestNamespace", "TestClass_AllDMTFTypes", operationOptions);

                var serializedResult = Helpers.ObservableToList(enumeratedInstances);
                Assert.True(serializedResult.Count > 0, "Got some results back from CimSession.EnumerateInstancesAsync");
                Assert.Equal(AsyncItemKind.Completion, serializedResult.Last().Kind, "Got OnCompleted callback");

                cancellationTokenSource.Cancel();
            }
        }

        [Fact]
        public void EnumerateInstancesAsync_ResultsAreNotNull()
        {
            using (CimSession cimSession = CimSession.Create(null))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");
                IObservable<CimInstance> enumeratedInstances = cimSession.EnumerateInstancesAsync(@"root\cimv2", "Win32_Process");
                Assert.NotNull(enumeratedInstances, "cimSession.EnumerateInstancesAsync returned something other than null");

                List<AsyncItem<CimInstance>> listOfInstances = Helpers.ObservableToList(enumeratedInstances);
                Assert.True(listOfInstances.Count > 0, "Got some results back from CimSession.EnumerateInstancesAsync");
                Assert.Equal(AsyncItemKind.Completion, listOfInstances.Last().Kind, "Got OnCompleted callback");

                bool atLeastOneInstance = false;
                foreach (CimInstance cimInstance in listOfInstances.Where(i => i.Kind == AsyncItemKind.Item).Select(i => i.Item))
                {
                    Assert.NotNull(cimInstance, "cimSession.EnumerateInstancesAsync returned something other than null");
                    atLeastOneInstance = true;
                }
                Assert.True(atLeastOneInstance, "cimSession.EnumerateInstancesAsync returned some instances");
            }
        }

        [Fact]
        public void EnumerateInstancesAsync_ReturnedProperties()
        {
            using (CimSession cimSession = CimSession.Create(null))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");
                IObservable<CimInstance> enumeratedInstances = cimSession.EnumerateInstancesAsync(@"root\cimv2", "Win32_Process");
                Assert.NotNull(enumeratedInstances, "cimSession.EnumerateInstancesAsync returned something other than null");

                List<AsyncItem<CimInstance>> listOfInstances = Helpers.ObservableToList(enumeratedInstances);
                Assert.True(listOfInstances.Count > 0, "Got some results back from CimSession.EnumerateInstancesAsync");
                Assert.Equal(AsyncItemKind.Completion, listOfInstances.Last().Kind, "Got OnCompleted callback");

                CimInstance currentProcess = listOfInstances
                    .Where(i => i.Kind == AsyncItemKind.Item)
                    .Select(i => i.Item)
                    .Single(x => (UInt32)(x.CimInstanceProperties["ProcessId"].Value) == Process.GetCurrentProcess().Id);

                Assert.NotNull(currentProcess, "cimSession.EnumerateInstances(..., Win32_Process) found the current process");

                Assert.Equal("Name", currentProcess.CimInstanceProperties["Name"].Name, "currentProcess.CimInstanceProperties['Name'].Name is correct");
                Assert.Equal(CimType.String, currentProcess.CimInstanceProperties["Name"].CimType, "currentProcess.CimInstanceProperties['Name'].CimType is correct");
                Assert.Equal("ttest.exe", (string)(currentProcess.CimInstanceProperties["Name"].Value), "currentProcess.CimInstanceProperties['Name'].Value is correct");
                Assert.Equal(CimFlags.ReadOnly | CimFlags.Property | CimFlags.NotModified, currentProcess.CimInstanceProperties["Name"].Flags, "currentProcess.CimInstanceProperties['Name'].Flags is correct");

                Assert.Equal(CimFlags.Key, currentProcess.CimInstanceProperties["Handle"].Flags & CimFlags.Key, "currentProcess.CimInstanceProperties['Handle'].Flags includes CimFlags.Key");
            }
        }

        [Fact]
        public void EnumerateInstancesAsync_SecondSubscription()
        {
            using (CimSession cimSession = CimSession.Create(null))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");

                IObservable<CimInstance> enumeratedInstances = cimSession.EnumerateInstancesAsync(@"root\cimv2", "Win32_Process");
                Assert.NotNull(enumeratedInstances, "cimSession.EnumerateInstancesAsync returned something other than null");

                int count1 = Helpers.ObservableToList(enumeratedInstances).Count;
                Assert.True(count1 > 0, "count1 > 0");

                int count2 = Helpers.ObservableToList(enumeratedInstances).Count;
                Assert.True(count2 > 0, "count2 > 0");

                int floor = Math.Max(count1, count2) / 2;
                Assert.True(count1 > floor, "count1 is reasonably close to count2");
                Assert.True(count2 > floor, "count2 is reasonably close to count1");
            }
        }

        [Fact]
        public void GetInstance()
        {
            using (CimSession cimSession = CimSession.Create(null))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");

                CimInstance instanceId = new CimInstance("Win32_Process");
                instanceId.CimInstanceProperties.Add(CimProperty.Create("Handle", Process.GetCurrentProcess().Id.ToString(), CimType.String, CimFlags.Key));

                CimInstance currentProcess = cimSession.GetInstance(@"root\cimv2", instanceId);
                Assert.NotNull(currentProcess, "cimSession.GetInstance returned something other than null");
                Assert.NotNull(currentProcess.CimClass, "cimSession.CimClass is null");

                Assert.True(currentProcess.CimInstanceProperties.Count > 1, "Got more than 1 property back from CimSession.GetInstance");

                Assert.Equal("Name", currentProcess.CimInstanceProperties["Name"].Name, "currentProcess.CimInstanceProperties['Name'].Name is correct");
                Assert.Equal(CimType.String, currentProcess.CimInstanceProperties["Name"].CimType, "currentProcess.CimInstanceProperties['Name'].CimType is correct");
                Assert.Equal("ttest.exe", (string)(currentProcess.CimInstanceProperties["Name"].Value), "currentProcess.CimInstanceProperties['Name'].Value is correct");
                Assert.Equal(CimFlags.Property | CimFlags.ReadOnly | CimFlags.NotModified, currentProcess.CimInstanceProperties["Name"].Flags, "currentProcess.CimInstanceProperties['Name'].Flags is correct");

                Assert.Equal(currentProcess.CimInstanceProperties["Handle"].Flags & CimFlags.Key, CimFlags.Key, "currentProcess.CimInstanceProperties['Handle'].Flags includes CimFlags.Key");

                Assert.True(currentProcess.CimInstanceProperties.Count > 1, "Got more than 1 property back from CimSession.GetInstance");

                Assert.Equal(currentProcess.CimInstanceProperties["Name"].Name, "Name", "currentProcess.CimInstanceProperties['Name'].Name is correct");
                Assert.Equal(currentProcess.CimInstanceProperties["Name"].CimType, CimType.String, "currentProcess.CimInstanceProperties['Name'].CimType is correct");
                Assert.Equal((string)(currentProcess.CimInstanceProperties["Name"].Value), "ttest.exe", "currentProcess.CimInstanceProperties['Name'].Value is correct");
                Assert.Equal(currentProcess.CimInstanceProperties["Name"].Flags, CimFlags.Property | CimFlags.ReadOnly | CimFlags.NotModified, "currentProcess.CimInstanceProperties['Name'].Flags is correct");

                Assert.Equal(currentProcess.CimInstanceProperties["Handle"].Flags & CimFlags.Key, CimFlags.Key, "currentProcess.CimInstanceProperties['Handle'].Flags includes CimFlags.Key");

                //System properties Environment.MachineName
                Assert.Equal("root/cimv2", currentProcess.CimSystemProperties.Namespace, "cimInstance.CimSystemProperties.Namespace is not correct");
                Assert.Equal("Win32_Process", currentProcess.CimSystemProperties.ClassName, "cimInstance.CimSystemProperties.ClassName is not correct");
                Assert.Equal(Environment.MachineName, currentProcess.CimSystemProperties.ServerName, "cimInstance.CimSystemProperties.ServerName is not correct");
                Assert.Null(currentProcess.CimSystemProperties.Path);
                /*
                Assert.Equal(currentProcess.CimSystemProperties.Path, 
                        "//" + Environment.MachineName + "/root/cimv2:Win32_Process.Handle=\"" + (currentProcess.CimInstanceProperties["Handle"].Value).ToString() + "\"",
                        "cimInstance.CimSystemProperties.Path is not correct " + currentProcess.CimSystemProperties.Path);
                 */
            }
        }

        [Fact]
        public void GetInstanceAsync()
        {
            using (CimSession cimSession = CimSession.Create(null))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");

                CimInstance instanceId = new CimInstance("Win32_Process");
                instanceId.CimInstanceProperties.Add(CimProperty.Create("Handle", Process.GetCurrentProcess().Id.ToString(), CimType.String, CimFlags.Key));

                IObservable<CimInstance> asyncInstance = cimSession.GetInstanceAsync(@"root\cimv2", instanceId);
                List<AsyncItem<CimInstance>> asyncList = Helpers.ObservableToList(asyncInstance);
                Assert.Equal(2, asyncList.Count, "Got 2 async callbacks");
                Assert.Equal(AsyncItemKind.Item, asyncList[0].Kind, "First callback for item");
                Assert.Equal(AsyncItemKind.Completion, asyncList[1].Kind, "Second callback for completion");

                CimInstance currentProcess = asyncList[0].Item;
                Assert.NotNull(currentProcess, "cimSession.GetInstance returned something other than null");

                Assert.True(currentProcess.CimInstanceProperties.Count > 1, "Got more than 1 property back from CimSession.GetInstance");

                Assert.Equal("Name", currentProcess.CimInstanceProperties["Name"].Name, "currentProcess.CimInstanceProperties['Name'].Name is correct");
                Assert.Equal(CimType.String, currentProcess.CimInstanceProperties["Name"].CimType, "currentProcess.CimInstanceProperties['Name'].CimType is correct");
                Assert.Equal("ttest.exe", (string)(currentProcess.CimInstanceProperties["Name"].Value), "currentProcess.CimInstanceProperties['Name'].Value is correct");
                Assert.Equal(CimFlags.Property | CimFlags.ReadOnly | CimFlags.NotModified, currentProcess.CimInstanceProperties["Name"].Flags, "currentProcess.CimInstanceProperties['Name'].Flags is correct");

                Assert.Equal(CimFlags.Key, currentProcess.CimInstanceProperties["Handle"].Flags & CimFlags.Key, "currentProcess.CimInstanceProperties['Handle'].Flags includes CimFlags.Key");
            }
        }

        [Fact]
        public void QueryInstances()
        {
            using (CimSession cimSession = CimSession.Create(null))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");

                string queryExpression = string.Format(
                    CultureInfo.InvariantCulture,
                    "SELECT * FROM Win32_Process WHERE ProcessId = {0}",
                    Process.GetCurrentProcess().Id);

                IEnumerable<CimInstance> enumeratedInstances = cimSession.QueryInstances(@"root\cimv2", "WQL", queryExpression);
                Assert.NotNull(enumeratedInstances, "cimSession.QueryInstances returned something other than null");

                CimInstance currentProcess = enumeratedInstances.Single();

                Assert.Equal("Name", currentProcess.CimInstanceProperties["Name"].Name, "currentProcess.CimInstanceProperties['Name'].Name is correct");
                Assert.Equal(CimType.String, currentProcess.CimInstanceProperties["Name"].CimType, "currentProcess.CimInstanceProperties['Name'].CimType is correct");
                Assert.Equal("ttest.exe", (string)(currentProcess.CimInstanceProperties["Name"].Value), "currentProcess.CimInstanceProperties['Name'].Value is correct");
                Assert.Equal(CimFlags.Property | CimFlags.ReadOnly | CimFlags.NotModified, currentProcess.CimInstanceProperties["Name"].Flags, "currentProcess.CimInstanceProperties['Name'].Flags is correct");

                Assert.Equal(CimFlags.Key, currentProcess.CimInstanceProperties["Handle"].Flags & CimFlags.Key, "currentProcess.CimInstanceProperties['Handle'].Flags includes CimFlags.Key");
            }
        }



        [Fact]
        public void QueryInstances_QueryDialect_Null()
        {
            using (CimSession cimSession = CimSession.Create(null))
            {
                string queryExpression = string.Format(
                    CultureInfo.InvariantCulture,
                    "SELECT * FROM Win32_Process WHERE ProcessId = {0}",
                    Process.GetCurrentProcess().Id);
                Assert.NotNull(cimSession, "cimSession should not be null");
                Assert.Throws<ArgumentNullException>(()=> {
                    return cimSession.QueryInstances(@"root\cimv2", null, queryExpression); });
                
            }
        }

        [Fact]
        public void QueryInstances_QueryExpression_Null()
        {
            using (CimSession cimSession = CimSession.Create(null))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");
                Assert.Throws<ArgumentNullException>(() =>
                {
                    return cimSession.QueryInstances(@"root\cimv2", "WQL", null);
                });
            }
        }

        [Fact]
        public void QueryInstances_CimClass()
        {
            using (CimSession cimSession = CimSession.Create(null))
            using (CimOperationOptions operationOptions = new CimOperationOptions())
            {
                operationOptions.Flags = CimOperationFlags.FullTypeInformation;
                string queryExpression = string.Format(
                    CultureInfo.InvariantCulture,
                    "SELECT * FROM Win32_Process WHERE ProcessId = {0}",
                    Process.GetCurrentProcess().Id);

                IEnumerable<CimInstance> enumeratedInstances = cimSession.QueryInstances(@"root\cimv2", "WQL", queryExpression, operationOptions);
                Assert.NotNull(enumeratedInstances, "cimSession.QueryInstances returned something other than null");

                using (CimInstance currentProcess = enumeratedInstances.Single())
                {
                    using (CimClass cimClass = currentProcess.CimClass)
                    {
                        Assert.NotNull(cimClass, "Got non-null cimClass");
                        Assert.Equal(cimClass.CimSystemProperties.ClassName, "Win32_Process", "Got correct CimClass.CimSystemProperties.ClassName");
                        using (CimClass superClass = cimClass.CimSuperClass)
                        {
                            Assert.NotNull(superClass, "Got non-null parentClass");
                            Assert.Equal("CIM_Process", superClass.CimSystemProperties.ClassName, "Got correct CimClass.CimSuperClass.CimSystemProperties.ClassName");
                        }
                    }

                    Assert.Equal("Name", currentProcess.CimInstanceProperties["Name"].Name, "currentProcess.CimInstanceProperties['Name'].Name is correct");
                    Assert.Equal(CimType.String, currentProcess.CimInstanceProperties["Name"].CimType, "currentProcess.CimInstanceProperties['Name'].CimType is correct");
                    Assert.Equal("ttest.exe", (string)(currentProcess.CimInstanceProperties["Name"].Value), "currentProcess.CimInstanceProperties['Name'].Value is correct");
                    Assert.Equal(CimFlags.Property | CimFlags.ReadOnly | CimFlags.NotModified, currentProcess.CimInstanceProperties["Name"].Flags, "currentProcess.CimInstanceProperties['Name'].Flags is correct");
                }
            }
        }

        private class ImpersonationObserver : IObserver<CimInstance>
        {
            public ImpersonationObserver(CimSession cimSession, string queryExpression, bool useThreadPool)
            {
                this.cimSession = cimSession;
                this.queryExpression = queryExpression;
                this.useThreadPool = useThreadPool;
            }

            private readonly ManualResetEventSlim testCompleted = new ManualResetEventSlim(initialState: false);
            private readonly CimSession cimSession;
            private readonly string queryExpression;
            private readonly bool useThreadPool;
            private int countOfCompletedOperations = 0;

            public void OnNext(CimInstance value)
            {
                Helpers.AssertRunningAsTestUser("in IObserver.OnNext callback");
            }

            public void OnError(Exception error)
            {
            }

            public void OnCompleted()
            {
                Helpers.AssertRunningAsTestUser("in IObserver.OnCompleted callback");

                countOfCompletedOperations++;
                if (countOfCompletedOperations >= 3)
                {
                    this.testCompleted.Set();
                }
                else
                {
                    if (useThreadPool)
                    {
                        ThreadPool.QueueUserWorkItem(
                            delegate
                            {
                                Helpers.AssertRunningAsTestUser("in ThreadPool thread spawned from IObserver.OnCompleted callback");
                                this.cimSession.QueryInstancesAsync("root/cimv2", "WQL", this.queryExpression).
                                    Subscribe(this);
                            });
                    }
                    else
                    {
                        this.cimSession.QueryInstancesAsync("root/cimv2", "WQL", this.queryExpression).Subscribe(this);
                    }
                }
            }

            public void WaitForEndOfTest()
            {
                this.testCompleted.Wait();
            }
        }

        [Fact]
        public void QueryInstancesAsync()
        {
            using (CimSession cimSession = CimSession.Create(null))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");

                string queryExpression = string.Format(
                    CultureInfo.InvariantCulture,
                    "SELECT * FROM Win32_Process WHERE ProcessId = {0}",
                    Process.GetCurrentProcess().Id);

                IObservable<CimInstance> enumeratedInstances = cimSession.QueryInstancesAsync(@"root\cimv2", "WQL", queryExpression);
                Assert.NotNull(enumeratedInstances, "cimSession.QueryInstancesAsync returned something other than null");

                List<AsyncItem<CimInstance>> asyncList = Helpers.ObservableToList(enumeratedInstances);
                Assert.Equal(2, asyncList.Count, "Got 2 async callbacks");
                Assert.Equal(AsyncItemKind.Item, asyncList[0].Kind, "First callback for item");
                Assert.Equal(AsyncItemKind.Completion, asyncList[1].Kind, "Second callback for completion");

                CimInstance currentProcess = asyncList[0].Item;
                Assert.NotNull(currentProcess, "cimSession.GetInstance returned something other than null");

                Assert.True(currentProcess.CimInstanceProperties.Count > 1, "Got more than 1 property back from CimSession.GetInstance");

                Assert.Equal("Name", currentProcess.CimInstanceProperties["Name"].Name, "currentProcess.CimInstanceProperties['Name'].Name is correct");
                Assert.Equal(CimType.String, currentProcess.CimInstanceProperties["Name"].CimType, "currentProcess.CimInstanceProperties['Name'].CimType is correct");
                Assert.Equal("ttest.exe", (string)(currentProcess.CimInstanceProperties["Name"].Value), "currentProcess.CimInstanceProperties['Name'].Value is correct");
                Assert.Equal(CimFlags.Property | CimFlags.ReadOnly | CimFlags.NotModified, currentProcess.CimInstanceProperties["Name"].Flags, "currentProcess.CimInstanceProperties['Name'].Flags is correct");

                Assert.Equal(CimFlags.Key, currentProcess.CimInstanceProperties["Handle"].Flags & CimFlags.Key, "currentProcess.CimInstanceProperties['Handle'].Flags includes CimFlags.Key");
            }
        }

        [Fact]
        public void QueryInstancesAsync_QueryDialect_Null()
        {
            using (CimSession cimSession = CimSession.Create(null))
            {
                string queryExpression = string.Format(
                    CultureInfo.InvariantCulture,
                    "SELECT * FROM Win32_Process WHERE ProcessId = {0}",
                    Process.GetCurrentProcess().Id);
                Assert.NotNull(cimSession, "cimSession should not be null");
                Assert.Throws<ArgumentNullException>(() =>
                {
                    return cimSession.QueryInstancesAsync(@"root\cimv2", null, queryExpression);
                });
            }
        }

        [Fact]
        public void QueryInstancesAsync_QueryExpression_Null()
        {
            using (CimSession cimSession = CimSession.Create(null))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");
                Assert.Throws<ArgumentNullException>(() =>
                {
                   return cimSession.QueryInstancesAsync(@"root\cimv2", "WQL", null);
                });
            }
        }

        [Fact]
        public void EnumerateAssociatedInstances()
        {
            using (CimSession cimSession = CimSession.Create(null))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");

                CimInstance instanceId = new CimInstance("Win32_Process");
                instanceId.CimInstanceProperties.Add(CimProperty.Create("Handle", Process.GetCurrentProcess().Id.ToString(), CimType.String, CimFlags.Key));

                IEnumerable<CimInstance> enumeratedFiles = cimSession.EnumerateAssociatedInstances(
                    @"root\cimv2",
                    instanceId,
                    "CIM_ProcessExecutable",
                    "CIM_DataFile",
                    "Dependent",
                    "Antecedent");
                Assert.NotNull(enumeratedFiles, "cimSession.EnumerateInstances returned something other than null");

                List<CimInstance> associatedFiles = enumeratedFiles.ToList();
                Assert.True(associatedFiles.Count > 0, "Got some results back from CimSession.EnumerateAssociatedInstances");
                Assert.True(associatedFiles.All(f => f.CimInstanceProperties["Name"].CimType == CimType.String), "Got correct type of 'Name' property");
                Assert.True(
                    null != associatedFiles.Single(f => ((string)(f.CimInstanceProperties["Name"].Value)).EndsWith("mi.dll", StringComparison.OrdinalIgnoreCase)),
                    "Associated files includes the DRTs dll");
            }
        }

        [Fact]
        public void EnumerateAssociatedInstancesAsync()
        {
            using (CimSession cimSession = CimSession.Create(null))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");

                CimInstance instanceId = new CimInstance("Win32_Process");
                instanceId.CimInstanceProperties.Add(CimProperty.Create("Handle", Process.GetCurrentProcess().Id.ToString(), CimType.String, CimFlags.Key));

                IObservable<CimInstance> observableFiles = cimSession.EnumerateAssociatedInstancesAsync(
                    @"root\cimv2",
                    instanceId,
                    "CIM_ProcessExecutable",
                    "CIM_DataFile",
                    "Dependent",
                    "Antecedent");
                Assert.NotNull(observableFiles, "cimSession.EnumerateInstances returned something other than null");

                List<AsyncItem<CimInstance>> unraveledObservable = Helpers.ObservableToList(observableFiles);
                Assert.True(unraveledObservable.Count > 0, "Got some results back from CimSession.EnumerateInstancesAsync");
                Assert.Equal(AsyncItemKind.Completion, unraveledObservable.Last().Kind, "Got OnCompleted callback");

                List<CimInstance> associatedFiles = unraveledObservable.Where(a => a.Kind == AsyncItemKind.Item).Select(a => a.Item).ToList();

                Assert.True(associatedFiles.Count > 0, "Got some results back from CimSession.EnumerateAssociatedInstances");
                Assert.True(associatedFiles.All(f => f.CimInstanceProperties["Name"].CimType == CimType.String), "Got correct type of 'Name' property");

                Assert.True(
                    null != associatedFiles.Single(f => ((string)(f.CimInstanceProperties["Name"].Value)).EndsWith("mi.dll", StringComparison.OrdinalIgnoreCase)),
                    "Associated files includes the DRTs dll");
            }
        }

        [Fact]
        public void EnumerateAssociatedInstances_Instance_Null()
        {
            using (CimSession cimSession = CimSession.Create(null))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");
                Assert.Throws<ArgumentNullException>(() =>
                {
                    return cimSession.EnumerateAssociatedInstances(
                        @"root\cimv2",
                        null,
                        "CIM_ProcessExecutable",
                        "CIM_DataFile",
                        "Dependent",
                        "Antecedent");
                });
            }
        }

        [Fact]
        public void EnumerateAssociatedInstancesAsync_Instance_Null()
        {
            using (CimSession cimSession = CimSession.Create(null))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");
                Assert.Throws<ArgumentNullException>(() =>
                {
                    return cimSession.EnumerateAssociatedInstancesAsync(
                        @"root\cimv2",
                        null,
                        "CIM_ProcessExecutable",
                        "CIM_DataFile",
                        "Dependent",
                        "Antecedent");
                });
            }
        }

        [Fact]
        public void EnumerateReferencingInstances()
        {
            using (CimSession cimSession = CimSession.Create(null))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");

                CimInstance instanceId = new CimInstance("Win32_Process");
                instanceId.CimInstanceProperties.Add(CimProperty.Create("Handle", Process.GetCurrentProcess().Id.ToString(), CimType.String, CimFlags.Key));

                IEnumerable<CimInstance> enumeratedAssociations = cimSession.EnumerateReferencingInstances(
                    @"root\cimv2",
                    instanceId,
                    "CIM_ProcessExecutable",
                    "Dependent");
                Assert.NotNull(enumeratedAssociations, "cimSession.EnumerateInstances returned something other than null");

                List<CimInstance> associations = enumeratedAssociations.ToList();
                Assert.True(associations.Count > 0, "Got some results back from CimSession.EnumerateReferencingInstances");

                Assert.True(
                    associations.All(a => a.CimInstanceProperties["Antecedent"].CimType == CimType.Reference),
                    "association.Antecedent is of a correct CimType");

                List<CimInstance> associatedFiles = associations
                    .Select(a => a.CimInstanceProperties["Antecedent"].Value)
                    .OfType<CimInstance>()
                    .ToList();

                Assert.True(associatedFiles.Count > 0, "Got some results back from CimSession.EnumerateAssociatedInstances");
                Assert.True(associatedFiles.All(f => f.CimInstanceProperties["Name"].CimType == CimType.String), "Got correct type of 'Name' property");

                Assert.True(
                    null != associatedFiles.Single(f => ((string)(f.CimInstanceProperties["Name"].Value)).EndsWith("mi.dll", StringComparison.OrdinalIgnoreCase)),
                    "Associated files includes the DRTs dll");

                /*
                foreach (var x in associatedFiles)
                {
                    x.Dispose();
                }
                 */
                foreach (var x in associations)
                {
                    x.Dispose();
                }
            }
        }

        [Fact]
        public void EnumerateReferencingInstancesAsync()
        {
            using (CimSession cimSession = CimSession.Create(null))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");

                CimInstance instanceId = new CimInstance("Win32_Process");
                instanceId.CimInstanceProperties.Add(CimProperty.Create("Handle", Process.GetCurrentProcess().Id.ToString(), CimType.String, CimFlags.Key));

                IObservable<CimInstance> observableAssociations = cimSession.EnumerateReferencingInstancesAsync(
                    @"root\cimv2",
                    instanceId,
                    "CIM_ProcessExecutable",
                    "Dependent");
                Assert.NotNull(observableAssociations, "cimSession.EnumerateInstancesAsync returned something other than null");

                List<AsyncItem<CimInstance>> unraveledObservable = Helpers.ObservableToList(observableAssociations);
                Assert.True(unraveledObservable.Count > 0, "Got some results back from CimSession.EnumerateInstancesAsync");
                Assert.Equal(unraveledObservable.Last().Kind, AsyncItemKind.Completion, "Got OnCompleted callback");

                List<CimInstance> associations = unraveledObservable.Where(a => a.Kind == AsyncItemKind.Item).Select(a => a.Item).ToList();
                Assert.True(associations.Count > 0, "Got some results back from CimSession.EnumerateReferencingInstances");

                Assert.True(
                    associations.All(a => a.CimInstanceProperties["Antecedent"].CimType == CimType.Reference),
                    "association.Antecedent is of a correct CimType");

                List<CimInstance> associatedFiles = associations
                    .Select(a => a.CimInstanceProperties["Antecedent"].Value)
                    .OfType<CimInstance>()
                    .ToList();

                Assert.True(associatedFiles.Count > 0, "Got some results back from CimSession.EnumerateAssociatedInstances");
                Assert.True(associatedFiles.All(f => f.CimInstanceProperties["Name"].CimType == CimType.String), "Got correct type of 'Name' property");

                Assert.True(
                    null != associatedFiles.Single(f => ((string)(f.CimInstanceProperties["Name"].Value)).EndsWith("mi.dll", StringComparison.OrdinalIgnoreCase)),
                    "Associated files includes the DRTs dll");

            }
        }

        [Fact]
        public void EnumerateReferencingInstances_Instance_Null()
        {
            using (CimSession cimSession = CimSession.Create(null))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");
                Assert.Throws<ArgumentNullException>(() =>
                {
                    return cimSession.EnumerateReferencingInstances(
                        @"root\cimv2",
                        null,
                        "CIM_ProcessExecutable",
                        "Dependent");
                });
            }
        }

        [Fact]
        public void EnumerateReferencingInstancesAsync_Instance_Null()
        {
            using (CimSession cimSession = CimSession.Create(null))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");
                Assert.Throws<ArgumentNullException>(() =>
                {
                    return cimSession.EnumerateReferencingInstancesAsync(
                        @"root\cimv2",
                        null,
                        "CIM_ProcessExecutable",
                        "Dependent");
                });
            }
        }

        [Fact]
        public void CreateInstance_Instance_Null()
        {
            using (CimSession cimSession = CimSession.Create(null))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");
                Assert.Throws<ArgumentNullException>(() =>
                {
                    return cimSession.CreateInstance(@"root\cimv2", null);
                });
            }
        }

        [Fact]
        public void CreateInstanceAsync_Instance_Null()
        {
            using (CimSession cimSession = CimSession.Create(null))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");
                Assert.Throws<ArgumentNullException>(() =>
                {
                    return cimSession.CreateInstanceAsync(@"root\cimv2", null);
                });
            }
        }

        [Fact]
        public void DeleteInstance_Instance_Null()
        {
            using (CimSession cimSession = CimSession.Create(null))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");
                Assert.Throws<ArgumentNullException>(() =>
                {
                    cimSession.DeleteInstance(@"root\cimv2", null);
                    return null;
                });
            }
        }

        [Fact]
        public void DeleteInstance_Instance_Null_NoNamespace()
        {
            using (CimSession cimSession = CimSession.Create(null))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");

                Helpers.AssertException<ArgumentNullException>(
                    () => cimSession.DeleteInstance(null),
                    e => Assert.Equal(e.ParamName, "instance", "Got correct ArgumentNullException.ParamName"));
            }
        }

        [Fact]
        public void DeleteInstanceAsync_Namespace_Null_NoNamespace()
        {
            using (CimSession cimSession = CimSession.Create(null))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");

                CimInstance instanceId = new CimInstance("Win32_Process");
                instanceId.CimInstanceProperties.Add(CimProperty.Create("Handle", Process.GetCurrentProcess().Id.ToString(), CimType.String, CimFlags.Key));
                Assert.Null(instanceId.CimSystemProperties.Namespace, "Sanity check: instanceId.CimSystemProperties.Namespace == null");

                Helpers.AssertException<ArgumentNullException>(
                    () => cimSession.DeleteInstanceAsync(instanceId),
                    e => Assert.Equal(e.ParamName, "instance", "Got correct ArgumentNullException.ParamName"));
            }
        }

        [Fact]
        public void DeleteInstance_Namespace_Null_NoNamespace()
        {
            using (CimSession cimSession = CimSession.Create(null))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");

                CimInstance instanceId = new CimInstance("Win32_Process");
                instanceId.CimInstanceProperties.Add(CimProperty.Create("Handle", Process.GetCurrentProcess().Id.ToString(), CimType.String, CimFlags.Key));
                Assert.Null(instanceId.CimSystemProperties.Namespace, "Sanity check: instanceId.CimSystemProperties.Namespace should be null");

                Helpers.AssertException<ArgumentNullException>(
                    () => cimSession.DeleteInstance(instanceId),
                    e => Assert.Equal(e.ParamName, "instance", "Got correct ArgumentNullException.ParamName"));
            }
        }

        [Fact]
        public void DeleteInstanceAsync_Instance_Null()
        {
            using (CimSession cimSession = CimSession.Create(null))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");
                Assert.Throws<ArgumentNullException>(() =>
                {
                   return cimSession.DeleteInstanceAsync(@"root\cimv2", null);
                });
            }
        }

        [Fact]
        public void DeleteInstanceAsync_Instance_Null_NoNamespace()
        {
            using (CimSession cimSession = CimSession.Create(null))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");

                Helpers.AssertException<ArgumentNullException>(
                    () => cimSession.DeleteInstanceAsync(null),
                    e => Assert.Equal(e.ParamName, "instance", "Got correct ArgumentNullException.ParamName"));
            }
        }

        [Fact]
        public void ModifyInstance_Namespace_Null_NoNamespace()
        {
            using (CimSession cimSession = CimSession.Create(null))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");

                CimInstance instanceId = new CimInstance("Win32_Process");
                instanceId.CimInstanceProperties.Add(CimProperty.Create("Handle", Process.GetCurrentProcess().Id.ToString(), CimType.String, CimFlags.Key));
                Assert.Null(instanceId.CimSystemProperties.Namespace, "Sanity check: instanceId.CimSystemProperties.Namespace == null");

                Helpers.AssertException<ArgumentNullException>(
                    () => cimSession.ModifyInstance(instanceId),
                    e => Assert.Equal(e.ParamName, "instance", "Got correct ArgumentNullException.ParamName"));

            }
        }

        [Fact]
        public void ModifyInstance_Instance_Null()
        {
            using (CimSession cimSession = CimSession.Create(null))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");
                Assert.Throws<ArgumentNullException>(() =>
                {
                    return cimSession.ModifyInstance(@"root\cimv2", null);
                });
            }
        }

        [Fact]
        public void ModifyInstance_Instance_Null_NoNamespace()
        {
            using (CimSession cimSession = CimSession.Create(null))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");

                Helpers.AssertException<ArgumentNullException>(
                    () => cimSession.ModifyInstance(null),
                    e => Assert.Equal(e.ParamName, "instance", "Got correct ArgumentNullException.ParamName"));
            }
        }

        [Fact]
        public void ModifyInstanceAsync_Namespace_Null_NoNamespace()
        {
            using (CimSession cimSession = CimSession.Create(null))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");

                CimInstance instanceId = new CimInstance("Win32_Process");
                instanceId.CimInstanceProperties.Add(CimProperty.Create("Handle", Process.GetCurrentProcess().Id.ToString(), CimType.String, CimFlags.Key));
                Assert.Null(instanceId.CimSystemProperties.Namespace, "Sanity check: instanceId.CimSystemProperties.Namespace == null");

                Helpers.AssertException<ArgumentNullException>(
                    () => cimSession.ModifyInstanceAsync(instanceId),
                    e => Assert.Equal("instance", e.ParamName, "Got correct ArgumentNullException.ParamName"));
            }
        }

        [Fact]
        public void ModifyInstanceAsync_Instance_Null()
        {
            using (CimSession cimSession = CimSession.Create(null))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");
                Assert.Throws<ArgumentNullException>(() =>
                {
                   return cimSession.ModifyInstanceAsync(@"root\cimv2", null);
                });
            }
        }

        [Fact]
        public void ModifyInstanceAsync_Instance_Null_NoNamespace()
        {
            using (CimSession cimSession = CimSession.Create(null))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");

                Helpers.AssertException<ArgumentNullException>(
                    () => cimSession.ModifyInstanceAsync(null),
                    e => Assert.Equal("instance", e.ParamName, "Got correct ArgumentNullException.ParamName"));
            }
        }

        [Fact]
        public void InvokeInstanceMethod_Namespace_Null_NoNamespace()
        {
            using (CimSession cimSession = CimSession.Create(null))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");

                CimInstance instanceId = new CimInstance("Win32_Process");
                instanceId.CimInstanceProperties.Add(CimProperty.Create("Handle", Process.GetCurrentProcess().Id.ToString(), CimType.String, CimFlags.Key));
                Assert.Null(instanceId.CimSystemProperties.Namespace, "Sanity check: instanceId.CimSystemProperties.Namespace == null");

                CimMethodParametersCollection methodParameters = new CimMethodParametersCollection();
                Helpers.AssertException<ArgumentNullException>(
                    () => cimSession.InvokeMethod(instanceId, "MethodName", methodParameters),
                    e => Assert.Equal("instance", e.ParamName, "Got correct ArgumentNullException.ParamName"));
            }
        }

        [Fact]
        public void InvokeInstanceMethod_SetSint32Value()
        {
            using (var cimSession = CimSession.Create(null))
            {
                var cimInstance = new CimInstance("TestClass_AllDMTFTypes");
                cimInstance.CimInstanceProperties.Add(CimProperty.Create("v_Key", 123, CimType.UInt64, CimFlags.Key));
                cimInstance.CimInstanceProperties.Add(CimProperty.Create("v_sint32", 456, CimType.SInt32, CimFlags.None));

                cimSession.CreateInstance("this.TestNamespace", cimInstance);
                //this.AssertPresenceOfTestInstance(123, 456);

                CimMethodResult result;
                using (var methodParameters = new CimMethodParametersCollection())
                {
                    Assert.Equal(0, methodParameters.Count, "methodParameters.Count is correct");
                    Assert.Equal(0, methodParameters.Count(), "methodParameters.Count is correct");
                    methodParameters.Add(CimMethodParameter.Create("Sint32Val", 789, CimType.SInt32, CimFlags.None));
                    Assert.Equal(1, methodParameters.Count, "methodParameters.Count is correct");
                    Assert.Equal(1, methodParameters.Count(), "methodParameters.Count is correct");
                    result = cimSession.InvokeMethod("this.TestNamespace", cimInstance, "SetSint32Value", methodParameters);
                }
                Assert.NotNull(result, "CimSession.InvokeMethod returned non-null");
                Assert.Equal(CimType.UInt32, result.ReturnValue.CimType, "Got the right type of return value");
                object returnValue = result.ReturnValue.Value;
                Assert.Equal((uint)0, (UInt32)returnValue, "Got the right return value");
                //this.AssertPresenceOfTestInstance(123, 789);
            }
        }

        [Fact]
        public void InvokeStaticMethod_Add()
        {
            using (var cimSession = CimSession.Create(null))
            {
                var methodParameters = new CimMethodParametersCollection();
                methodParameters.Add(CimMethodParameter.Create("Left", 123, CimType.SInt64, CimFlags.None));
                methodParameters.Add(CimMethodParameter.Create("Right", 456, CimType.SInt64, CimFlags.None));
                CimMethodResult result = cimSession.InvokeMethod("this.TestNamespace", "TestClass_MethodProvider_Calc", "Add", methodParameters);

                Assert.NotNull(result, "CimSession.InvokeMethod returned non-null");
                Assert.Equal(CimType.UInt64, result.ReturnValue.CimType, "Got the right type of return value");
                object returnValue = result.ReturnValue.Value;
                Assert.Equal((uint)0, (UInt64)returnValue, "Got the right return value");

                Assert.NotNull(result.OutParameters["sum"], "CimSession.InvokeMethod returned out parameter");
                Assert.Equal(CimType.SInt64, result.OutParameters["sum"].CimType, "CimSession.InvokeMethod returned right type of out parameter");
                object outParameterValue = result.OutParameters["sum"].Value;
                Assert.Equal(123 + 456, (Int64)outParameterValue, "Got the right out parameter value");
            }
        }

        [Fact]
        public void InvokeStaticMethod_Add_InvalidParameterType()
        {
            using (var cimSession = CimSession.Create(null))
            {
                var methodParameters = new CimMethodParametersCollection();
                methodParameters.Add(CimMethodParameter.Create("Left", 123, CimType.SInt64, CimFlags.None));
                methodParameters.Add(CimMethodParameter.Create("Right", "456", CimType.String, CimFlags.None));

                Helpers.AssertException(
                    () => cimSession.InvokeMethod("this.TestNamespace", "TestClass_MethodProvider_Calc", "Add", methodParameters),
                    delegate (CimException cimException)
                    {
                        Assert.Equal(cimException.Message, "InvalidParameter", "Unlocalized enum value is not used as exception message");
                        if (Thread.CurrentThread.CurrentCulture.DisplayName.Equals("en-US", StringComparison.OrdinalIgnoreCase))
                        {
                            string lowerCaseMessage = cimException.Message.ToLowerInvariant();
                            Assert.True(lowerCaseMessage.Contains("invalid"), "Message contains 'invalid' in English locale");
                            Assert.True(lowerCaseMessage.Contains("parameter"), "Message contains 'parameter' in English locale");
                        }
                    });
            }
        }

        [Fact]
        public void InvokeStreamingMethod_Sync()
        {
            using (var cimSession = CimSession.Create(null))
            {
                var methodParameters = new CimMethodParametersCollection();
                methodParameters.Add(CimMethodParameter.Create("count", 3, CimType.UInt32, CimFlags.None));
                CimMethodResult result = cimSession.InvokeMethod("this.TestNamespace", "TestClass_Streaming", "StreamNumbers", methodParameters);

                Assert.NotNull(result, "CimSession.InvokeMethod returned non-null");
                Assert.Equal(CimType.UInt32, result.ReturnValue.CimType, "Got the right type of return value");
                object returnValue = result.ReturnValue.Value;
                Assert.Equal((uint)0, (UInt32)returnValue, "Got the right return value");

                Assert.NotNull(result.OutParameters["firstTen"], "CimSession.InvokeMethod returned out parameter");
                Assert.Equal(CimType.InstanceArray, result.OutParameters["firstTen"].CimType, "CimSession.InvokeMethod returned right type of out parameter");
                CimInstance[] outParameterValue = result.OutParameters["firstTen"].Value as CimInstance[];
                Assert.True(outParameterValue.All(v => v.CimSystemProperties.ClassName.Equals("Numbers", StringComparison.OrdinalIgnoreCase)), "Results have the right class name");
                Assert.True(outParameterValue.All(v => v.CimInstanceProperties["Numbers"] != null), "Results have 'Numbers' property");
                Assert.True(outParameterValue.All(v => v.CimInstanceProperties["Numbers"].CimType == CimType.SInt64Array), "Results have 'Numbers' property with correct type");

                Assert.Equal(10, ((long[])(outParameterValue[0].CimInstanceProperties["Numbers"].Value))[9], "1st result is correct");
                Assert.Equal(20, ((long[])(outParameterValue[1].CimInstanceProperties["Numbers"].Value))[9], "2nd result is correct");
                Assert.Equal(30, ((long[])(outParameterValue[2].CimInstanceProperties["Numbers"].Value))[9], "3rd result is correct");
            }
        }

        [Fact]
        public void InvokeStreamingMethod_Async()
        {
            using (var cimSession = CimSession.Create(null))
            {
                var operationOptions = new CimOperationOptions { EnableMethodResultStreaming = true };

                var methodParameters = new CimMethodParametersCollection();
                methodParameters.Add(CimMethodParameter.Create("count", 3, CimType.UInt32, CimFlags.None));
                IObservable<CimMethodResultBase> observable = cimSession.InvokeMethodAsync("this.TestNamespace", "TestClass_Streaming", "StreamNumbers", methodParameters, operationOptions);
                Assert.NotNull(observable, "CimSession.InvokeMethod returned non-null");

                List<AsyncItem<CimMethodResultBase>> result = Helpers.ObservableToList(observable);
                Assert.True(result.Count > 0, "Got some callbacks");
                Assert.Equal(AsyncItemKind.Completion, result[result.Count - 1].Kind, "Got completion callback");

                Assert.True(result.Count > 1, "Got more than 1 callback");
                Assert.Equal(AsyncItemKind.Item, result[result.Count - 2].Kind, "Got non-streamed result (presence)");
                Assert.True(result[result.Count - 2].Item.GetType().Equals(typeof(CimMethodResult)), "Got non-streamed result (type)");

                Assert.True(result[0].Item.GetType().Equals(typeof(CimMethodStreamedResult)), "Got streamed result");
                Assert.Equal("firstTen", ((CimMethodStreamedResult)(result[0].Item)).ParameterName, "Streamed result has correct parameter name");
                Assert.Equal(CimType.Instance, ((CimMethodStreamedResult)(result[0].Item)).ItemType, "Streamed result has correct type of item");

                CimInstance item = (CimInstance)((CimMethodStreamedResult)(result[0].Item)).ItemValue;
                Assert.NotNull(item.CimInstanceProperties["Numbers"], "Streamed result has 'Numbers' property (1)");
                Assert.Equal(CimType.SInt64Array, item.CimInstanceProperties["Numbers"].CimType, "Streamed result has 'Numbers' property of the correct type (1)");
                Assert.Equal(10, ((long[])(item.CimInstanceProperties["Numbers"].Value))[9], "Streamed result has 'Numbers' property with right value (1)");

                item = (CimInstance)((CimMethodStreamedResult)(result[1].Item)).ItemValue;
                Assert.NotNull(item.CimInstanceProperties["Numbers"], "Streamed result has 'Numbers' property (2)");
                Assert.Equal(CimType.SInt64Array, item.CimInstanceProperties["Numbers"].CimType, "Streamed result has 'Numbers' property of the correct type (2)");
                Assert.Equal(20, ((long[])(item.CimInstanceProperties["Numbers"].Value))[9], "Streamed result has 'Numbers' property with right value (2)");
            }
        }

        [Fact]
        public void InvokeStreamingMethod_Async_ServerSideStreamingOnly()
        {
            using (var cimSession = CimSession.Create(null))
            {
                var methodParameters = new CimMethodParametersCollection();
                methodParameters.Add(CimMethodParameter.Create("count", 3, CimType.UInt32, CimFlags.None));
                IObservable<CimMethodResult> observable = cimSession.InvokeMethodAsync("this.TestNamespace", "TestClass_Streaming", "StreamNumbers", methodParameters);
                Assert.NotNull(observable, "CimSession.InvokeMethod returned non-null");

                List<AsyncItem<CimMethodResult>> asyncResult = Helpers.ObservableToList(observable);
                Assert.True(asyncResult.Count > 0, "Got some callbacks");
                Assert.Equal(AsyncItemKind.Completion, asyncResult[asyncResult.Count - 1].Kind, "Got completion callback");

                Assert.True(asyncResult.Count == 2, "Got exactly one asyncResult");
                Assert.Equal(AsyncItemKind.Item, asyncResult[asyncResult.Count - 2].Kind, "Got non-streamed asyncResult (presence)");
                Assert.True(asyncResult[asyncResult.Count - 2].Item.GetType().Equals(typeof(CimMethodResult)), "Got non-streamed asyncResult (type)");

                CimMethodResult result = asyncResult[asyncResult.Count - 2].Item;

                Assert.NotNull(result, "CimSession.InvokeMethod returned non-null");
                Assert.Equal(CimType.UInt32, result.ReturnValue.CimType, "Got the right type of return value");
                object returnValue = result.ReturnValue.Value;
                Assert.Equal((uint)0, (UInt32)returnValue, "Got the right return value");

                Assert.NotNull(result.OutParameters["firstTen"], "CimSession.InvokeMethod returned out parameter");
                Assert.Equal(CimType.InstanceArray, result.OutParameters["firstTen"].CimType, "CimSession.InvokeMethod returned right type of out parameter");
                CimInstance[] outParameterValue = result.OutParameters["firstTen"].Value as CimInstance[];
                Assert.True(outParameterValue.All(v => v.CimSystemProperties.ClassName.Equals("Numbers", StringComparison.OrdinalIgnoreCase)), "Results have the right class name");
                Assert.True(outParameterValue.All(v => v.CimInstanceProperties["Numbers"] != null), "Results have 'Numbers' property");
                Assert.True(outParameterValue.All(v => v.CimInstanceProperties["Numbers"].CimType == CimType.SInt64Array), "Results have 'Numbers' property with correct type");

                Assert.Equal(10, ((long[])(outParameterValue[0].CimInstanceProperties["Numbers"].Value))[9], "1st result is correct");
                Assert.Equal(20, ((long[])(outParameterValue[1].CimInstanceProperties["Numbers"].Value))[9], "2nd result is correct");
                Assert.Equal(30, ((long[])(outParameterValue[2].CimInstanceProperties["Numbers"].Value))[9], "3rd result is correct");
            }
        }

        [Fact]
        public void InvokeInstanceMethod_Instance_Null()
        {
            using (CimSession cimSession = CimSession.Create(null))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");
                CimMethodParametersCollection methodParameters = new CimMethodParametersCollection();
                Assert.Throws<ArgumentNullException>(() =>
                {
                   return cimSession.InvokeMethod(@"root\cimv2", (CimInstance)null, "MethodName", methodParameters);
                });
            }
        }

        [Fact]
        public void InvokeInstanceMethod_Instance_Null_NoNamespace()
        {
            using (CimSession cimSession = CimSession.Create(null))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");

                CimMethodParametersCollection methodParameters = new CimMethodParametersCollection();
                Helpers.AssertException<ArgumentNullException>(
                    () => cimSession.InvokeMethod((CimInstance)null, "MethodName", methodParameters),
                    e => Assert.Equal("instance", e.ParamName, "Got correct ArgumentNullException.ParamName"));
            }
        }

        [Fact]
        public void InvokeInstanceMethod_MethodName_Null()
        {
            using (CimSession cimSession = CimSession.Create(null))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");

                CimInstance instanceId = new CimInstance("Win32_Process");
                instanceId.CimInstanceProperties.Add(CimProperty.Create("Handle", Process.GetCurrentProcess().Id.ToString(), CimType.String, CimFlags.Key));

                CimMethodParametersCollection methodParameters = new CimMethodParametersCollection();
                Assert.Throws<ArgumentNullException>(() =>
                {
                    return cimSession.InvokeMethod(@"root\cimv2", instanceId, null, methodParameters);
                });
            }
        }

        [Fact]
        public void InvokeInstanceMethodAsync_Namespace_Null_NoNamespace()
        {
            using (CimSession cimSession = CimSession.Create(null))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");

                CimInstance instanceId = new CimInstance("Win32_Process");
                instanceId.CimInstanceProperties.Add(CimProperty.Create("Handle", Process.GetCurrentProcess().Id.ToString(), CimType.String, CimFlags.Key));
                Assert.Null(instanceId.CimSystemProperties.Namespace, "Sanity check: instanceId.CimSystemProperties.Namespace = null");

                CimMethodParametersCollection methodParameters = new CimMethodParametersCollection();
                Helpers.AssertException<ArgumentNullException>(
                    () => cimSession.InvokeMethodAsync(instanceId, "MethodAsyncName", methodParameters),
                    e => Assert.Equal("instance", e.ParamName, "Got correct ArgumentNullException.ParamName"));
            }
        }

        [Fact]
        public void InvokeInstanceMethodAsync_Instance_Null()
        {
            using (CimSession cimSession = CimSession.Create(null))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");

                CimMethodParametersCollection methodParameters = new CimMethodParametersCollection();
                cimSession.InvokeMethodAsync(@"root\cimv2", (CimInstance)null, "MethodAsyncName", methodParameters);
            }
        }

        [Fact]
        public void InvokeInstanceMethodAsync_Instance_Null_NoNamespace()
        {
            using (CimSession cimSession = CimSession.Create(null))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");

                CimMethodParametersCollection methodParameters = new CimMethodParametersCollection();
                Helpers.AssertException<ArgumentNullException>(
                    () => cimSession.InvokeMethodAsync((CimInstance)null, "MethodAsyncName", methodParameters),
                    e => Assert.Equal("instance", e.ParamName, "Got correct A4rgumentNullException.ParamName"));
            }
        }

        [Fact]
        public void InvokeInstanceMethodAsync_MethodAsyncName_Null()
        {
            using (CimSession cimSession = CimSession.Create(null))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");

                CimInstance instanceId = new CimInstance("Win32_Process");
                instanceId.CimInstanceProperties.Add(CimProperty.Create("Handle", Process.GetCurrentProcess().Id.ToString(), CimType.String, CimFlags.Key));

                CimMethodParametersCollection methodParameters = new CimMethodParametersCollection();
                Assert.Throws<ArgumentNullException>(() =>
                {
                    return cimSession.InvokeMethodAsync(@"root\cimv2", instanceId, null, methodParameters);
                });
            }
        }

        [Fact]
        public void InvokeStaticMethod_MethodName_Null()
        {
            using (CimSession cimSession = CimSession.Create(null))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");

                CimMethodParametersCollection methodParameters = new CimMethodParametersCollection();
                Assert.Throws<ArgumentNullException>(() =>
                {
                   return cimSession.InvokeMethod(@"root\cimv2", "ClassName", null, methodParameters);
                });
            }
        }

        [Fact]
        public void InvokeStaticMethodAsync_MethodName_Null()
        {
            using (CimSession cimSession = CimSession.Create(null))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");

                CimMethodParametersCollection methodParameters = new CimMethodParametersCollection();
                Assert.Throws<ArgumentNullException>(() =>
                {
                    return cimSession.InvokeMethodAsync(@"root\cimv2", "ClassName", null, methodParameters);
                });
            }
        }

        [Fact]
        public void Subscribe()
        {
            using (CimSession cimSession = CimSession.Create(null))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");
                CimOperationOptions options = new CimOperationOptions();
                //options.Timeout = TimeSpan.FromMilliseconds(20000);
                IEnumerable<CimSubscriptionResult> enumeratedFiles = cimSession.Subscribe(
                    @"root\cimv2",
                    "WQL",
                    @"Select * from Win32_ProcessStartTrace",
                    options
                    );
                Assert.NotNull(enumeratedFiles, "cimSession.Subscribe returned something other than null");
                //Start a process here
                IEnumerator<CimSubscriptionResult> iterator = enumeratedFiles.GetEnumerator();
                bool bGotInstance = false;
                Thread.Sleep(3000);
                Helpers.StartDummyProcess();

                while (iterator.MoveNext())
                {
                    CimSubscriptionResult instance = iterator.Current;
                    Assert.True((instance.Instance.CimInstanceProperties["ProcessName"].CimType == CimType.String), "Got correct type of 'Name' property");
                    bGotInstance = true;
                    iterator.Dispose();
                    break;
                }
                Assert.True(bGotInstance, "Got some results back from CimSession.Subscribe");
            }
        }

        [Fact]
        public void Subscribe_DeliveryOptionsInterval()
        {
            using (CimSession cimSession = CimSession.Create(Environment.MachineName, new WSManSessionOptions()))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");
                CimSubscriptionDeliveryOptions options = new CimSubscriptionDeliveryOptions(CimSubscriptionDeliveryType.Pull);
                options.SetInterval("__MI_SUBSCRIPTIONDELIVERYOPTIONS_SET_HEARTBEAT_INTERVAL", TimeSpan.FromSeconds(30), 0);

                //options.Timeout = TimeSpan.FromMilliseconds(20000);
                IEnumerable<CimSubscriptionResult> enumeratedFiles = cimSession.Subscribe(
                    @"root\cimv2",
                    "WQL",
                    @"Select * from Win32_ProcessStartTrace",
                    options
                    );
                Assert.NotNull(enumeratedFiles, "cimSession.Subscribe returned something other than null");
                //Start a process here
                using (IEnumerator<CimSubscriptionResult> iterator = enumeratedFiles.GetEnumerator())
                {
                    bool bGotInstance = false;
                    Thread.Sleep(3000);
                    Helpers.StartDummyProcess();

                    while (iterator.MoveNext())
                    {
                        CimSubscriptionResult instance = iterator.Current;
                        Assert.True(
                            (instance.Instance.CimInstanceProperties["ProcessName"].CimType == CimType.String),
                            "Got correct type of 'Name' property");
                        bGotInstance = true;
                        break;
                    }
                    Assert.True(bGotInstance, "Got some results back from CimSession.Subscribe");
                }
            }
        }

        [Fact]
        private void Subscribe_DeliveryOptionsDateTime_Core(CimSubscriptionDeliveryOptions options)
        {
            DateTime startTime = DateTime.UtcNow;

            using (CimSession cimSession = CimSession.Create(Environment.MachineName, new WSManSessionOptions()))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");

                //options.Timeout = TimeSpan.FromMilliseconds(20000);
                IEnumerable<CimSubscriptionResult> enumeratedFiles = cimSession.Subscribe(
                    @"root\cimv2",
                    "WQL",
                    @"Select * from Win32_ProcessStartTrace",
                    options
                    );
                Assert.NotNull(enumeratedFiles, "cimSession.Subscribe returned something other than null");
                //Start a process here
                Helpers.AssertException<CimException>(
                    delegate
                    {
                        using (IEnumerator<CimSubscriptionResult> iterator = enumeratedFiles.GetEnumerator())
                        {
                            bool bGotInstance = false;
                            Thread.Sleep(3000);
                            Helpers.StartDummyProcess();

                            while (iterator.MoveNext())
                            {
                                CimSubscriptionResult instance = iterator.Current;
                                Assert.True(
                                    (instance.Instance.CimInstanceProperties["ProcessName"].CimType ==
                                     CimType.String),
                                    "Got correct type of 'Name' property");
                                bGotInstance = true;
                            }
                            Assert.True(bGotInstance, "Got some results back from CimSession.Subscribe");
                        }
                    },
                    delegate (CimException e)
                    {
                        Assert.True(e.NativeErrorCode == NativeErrorCode.Failed, "Got the expected NativeErrorCode");
                    });
            }

            TimeSpan testDuration = DateTime.UtcNow - startTime;
            Assert.True(testDuration > TimeSpan.FromSeconds(8), "Test should last more than 5 seconds");
            Assert.True(testDuration < TimeSpan.FromSeconds(60), "Test should last less than 60 seconds");
        }

        [Fact]
        public void Subscribe_DeliveryOptionsDateTime_TimeSpan()
        {
            CimSubscriptionDeliveryOptions options = new CimSubscriptionDeliveryOptions(CimSubscriptionDeliveryType.Pull);
            options.SetDateTime("__MI_SUBSCRIPTIONDELIVERYOPTIONS_SET_EXPIRATION_TIME", TimeSpan.FromSeconds(10), 0);
            Subscribe_DeliveryOptionsDateTime_Core(options);
        }

        [Fact]
        public void SubscribeAsync()
        {
            using (CimSession cimSession = CimSession.Create(null))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");
                CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
                CimOperationOptions options = new CimOperationOptions();
                //options.Timeout = TimeSpan.FromMilliseconds(20000);
                options.CancellationToken = cancellationTokenSource.Token;

                IObservable<CimSubscriptionResult> observableFiles = cimSession.SubscribeAsync(
                    @"root\cimv2",
                    "WQL",
                    @"Select * from Win32_ProcessStartTrace",
                    options
                    );
                Assert.NotNull(observableFiles, "cimSession.Subscribe returned something other than null");

                AsyncItem<CimSubscriptionResult> unraveledObservable = Helpers.ObservableToSingleItem(observableFiles);
                cancellationTokenSource.Cancel();
                Assert.True(unraveledObservable != null, "Did not get any instance back from CimSession.Subscribe");
                Assert.True((unraveledObservable.Item.Instance.CimInstanceProperties["ProcessName"].CimType == CimType.String), "Got correct type of 'Name' property");
            }
        }

        [Fact]
        public void Subscribe_Dialect_Null()
        {
            using (CimSession cimSession = CimSession.Create(null))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");

                Assert.Throws<ArgumentNullException>(() =>
                {
                    return cimSession.Subscribe(
                        @"root\cimv2",
                        null,
                        @"Select * from __InstanceCreationEvent WITHIN 2 where TargetInstance isa 'Win32_Process'");
                });
            }
        }

        [Fact]
        public void Subscribe_Query_Null()
        {
            using (CimSession cimSession = CimSession.Create(null))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");
                Assert.Throws<ArgumentNullException>(() =>
                {
                    return cimSession.Subscribe(
                        @"root\cimv2",
                        "WQL",
                        null);
                });
            }
        }

        [Fact]
        public void SubscribeAsync_Dialect_Null()
        {
            using (CimSession cimSession = CimSession.Create(null))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");
                Assert.Throws<ArgumentNullException>(() =>
                {
                   return cimSession.SubscribeAsync(
                       @"root\cimv2",
                       null,
                       @"Select * from __InstanceCreationEvent WITHIN 2 where TargetInstance isa 'Win32_Process'");
                });
            }
        }

        [Fact]
        public void SubscribeAsync_Query_Null()
        {
            using (CimSession cimSession = CimSession.Create(null))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");

                Assert.Throws<ArgumentNullException>(() =>
                {
                    return cimSession.SubscribeAsync(
                        @"root\cimv2",
                        "WQL",
                        null);
                });
            }
        }

        [Fact]
        public void GetNonexistQualifer()
        {
            using (CimSession cimSession = CimSession.Create(null))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");
                CimClass enumeratedClass = cimSession.GetClass(@"root\cimv2", "Win32_Process");
                Assert.NotNull(enumeratedClass, "cimSession.GetClass returneded null");

                //Check non-exist class qualifiers
                Assert.Null(enumeratedClass.CimClassQualifiers["nonexists"], "CimClass.CimClassQualifiers returneded not null");

                //Check non-exist class property qualifiers
                Assert.NotNull(enumeratedClass.CimClassProperties["Caption"], "enumeratedClass.CimClassProperties returneded null");
                Assert.NotNull(enumeratedClass.CimClassProperties["Caption"].Qualifiers, "enumeratedClass.CimClassProperties[Caption].Qualifiers returneded null");
                Assert.Null(enumeratedClass.CimClassProperties["Caption"].Qualifiers["nonexist"], "enumeratedClass.CimClassProperties[Caption].Qualifiers[nonexist] returneded not null");

                //Check non-exist method qualifier
                Assert.NotNull(enumeratedClass.CimClassMethods["Create"], @"CimClass.CimClassMethods[Create] returneded null");
                Assert.Null(enumeratedClass.CimClassMethods["Create"].Qualifiers["nonexist"], @"CimClass.CimClassMethods[Create].Qualifiers[nonexist] returneded not null");

                //Check non-exist method parameter qualifier
                Assert.NotNull(enumeratedClass.CimClassMethods["Create"].Parameters["CommandLine"].Qualifiers, @"CimClass.CimClassMethods[Create].Parameters[CommandLine].Qualifiers returneded null");
                Assert.Null(enumeratedClass.CimClassMethods["Create"].Parameters["CommandLine"].Qualifiers["nonexist"], @"CimClass.CimClassMethods[Create].Parameters[CommandLine].Qualifiers[nonexist] returneded not null");
            }
        }
    }
}