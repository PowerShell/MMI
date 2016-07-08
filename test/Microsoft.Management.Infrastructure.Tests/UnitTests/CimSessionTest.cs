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
        #region Test create
        [Fact]
        public void Create_ComputerName_Null()
        {
            using (CimSession cimSession = CimSession.Create(null))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");
                Assert.Null(cimSession.ComputerName, "cimSession.ComputerName should not be the same as the value passed to Create method");
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


        //==============
        [Fact]
        public void Impersonated_GarbageCollection()
        {
            Helpers.AssertRunningAsNonTestUser("Start of test");
            using (Helpers.ImpersonateTestUser())
            {
                Helpers.AssertRunningAsTestUser("Before CImSession.Create()");
                CimSession cimSession = CimSession.Create(null, new WSManSessionOptions());
                cimSession = null;
            }
            Helpers.AssertRunningAsNonTestUser("Before waiting for garbage collection");
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        [Fact]
        public void Impersonated_Close()
        {
            Helpers.AssertRunningAsNonTestUser("Start of test");
            CimSession cimSession = CimSession.Create(null);
            using (Helpers.ImpersonateTestUser())
            {
                Helpers.AssertRunningAsTestUser("Before CImSession.Dispose()");
                cimSession.Close();
            }
            Helpers.AssertRunningAsNonTestUser("End of test");
        }

        //[Fact]
        //public void Impersonated_CloseAsync()
        //{
        //    Helpers.AssertRunningAsNonTestUser("Start of test");
        //    CimSession cimSession = CimSession.Create(null);
        //    using (Helpers.ImpersonateTestUser())
        //    {
        //        Helpers.AssertRunningAsTestUser("Before CImSession.Dispose()");
        //        ManualResetEventSlim waiter = new ManualResetEventSlim(false);
        //        cimSession.CloseAsync().Subscribe(onErrorAction: e => { throw e; }, onCompletedAction: waiter.Set);
        //        waiter.Wait();
        //    }
        //    Helpers.AssertRunningAsNonTestUser("End of test");
        //}

        //[Fact]
        //public void Impersonated_CloseAsyncCallback_NoOperations()
        //{
        //    Helpers.AssertRunningAsNonTestUser("Start of test");
        //    ManualResetEventSlim waiter = new ManualResetEventSlim(false);

        //    using (Helpers.ImpersonateTestUser())
        //    {
        //        Helpers.AssertRunningAsTestUser("Before CimSession.Create()");
        //        CimSession cimSession = CimSession.Create(null);
        //        cimSession.CloseAsync().Subscribe(
        //            onErrorAction: delegate (Exception e)
        //            {
        //                CimImpersonationHelpers.AssertRunningAsTestUser("In CimSession.CloseAsync.OnError");
        //                waiter.Set();
        //                throw e;
        //            },
        //            onCompletedAction: delegate
        //            {
        //                CimImpersonationHelpers.AssertRunningAsTestUser("In CimSession.CloseAsync.OnCompleted");
        //                waiter.Set();
        //            });
        //    }
        //    Helpers.AssertRunningAsNonTestUser("End of test");
        //    waiter.Wait();
        //}

        [Fact]
        public void CloseAsync()
        {
            using (CimSession cimSession = CimSession.Create(null))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");
                CimAsyncStatus asyncResult = cimSession.CloseAsync();

                List<AsyncItem<object>> serializedResults = Helpers.ObservableToList(asyncResult);
                Assert.Equal(serializedResults.Count, 1, "Only expecting OnCompleted callback (1)");
                Assert.Equal(serializedResults[0].Kind, AsyncItemKind.Completion, "Only expecting OnCompleted callback (2)");
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
        public void AlreadyDisposed_CloseAsync_Then_Close()
        {
            CimSession cimSession = CimSession.Create(null);
            Assert.NotNull(cimSession, "cimSession should not be null");
            Helpers.ObservableToList(cimSession.CloseAsync());
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
                        //() => cimSession.EnumerateInstances(@"root\cimv2", "nonExistantClassHere").Count(),
                        () => cimSession.EnumerateInstances(@"root\cimv2", "nonExistantClassHere"),
                        delegate (CimException exception)
                        {
                            Assert.Equal(exception.NativeErrorCode, NativeErrorCode.InvalidClass, "Got the right NativeErrorCode");
                            Assert.Equal(exception.Message, "Invalid_Class", "Got the right message (1)");
                            Assert.Equal(exception.Message, "InvalidClass", "Got the right message (2)");
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
                //Assert.True(enumeratedInstances.Count() > 0, "Got some results back from CimSession.EnumerateInstances");
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
                Assert.Equal(topInstance.GetCimSessionComputerName(), Environment.MachineName, "Verifying topInstance.GetCimSessionComputerName");

                CimInstance embeddedInstance = (CimInstance)topInstance.CimInstanceProperties["Antecedent"].Value;
                Assert.Equal(embeddedInstance.GetCimSessionComputerName(), Environment.MachineName, "Verifying embeddedInstance.GetCimSessionComputerName");

                Assert.Equal(embeddedInstance.GetCimSessionInstanceId(), topInstance.GetCimSessionInstanceId(), "Verifying GetCimSessionInstanceId");
            }
        }

        [Fact]
        public void EnumerateInstances_StartedFromOnError()
        {
            CimSession cimSession = null;
            using (cimSession = CimSession.Create("localhost"))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");
                CimOperationOptions operationOptions = new CimOperationOptions() { Flags = (CimOperationFlags)0xffffffff };
                IObservable<CimInstance> observedInstances = cimSession.EnumerateInstancesAsync(@"!@#$%^&*(", "!@#$%^&*(", operationOptions);
                Assert.NotNull(observedInstances, "cimSession.EnumerateInstancesAsync returned something other than null");

                bool onErrorGotRun = false;
                observedInstances.Subscribe(
                        onErrorAction:
                        delegate
                        {
                            onErrorGotRun = true;
                            Thread.Sleep(TimeSpan.FromSeconds(3));

                            Assert.True(cimSession != null, "SANITY CHECK / TEST SETUP VALIDATION: Test assumes that protocol handler will call OnError on the same thread as MI_Session_Enumerat");

                            IEnumerable<CimInstance> enumeratedInstances = cimSession.EnumerateInstances(@"root\cimv2", "Win32_Process");
                            Assert.NotNull(enumeratedInstances, "cimSession.EnumerateInstances returned something other than null");
                            Assert.True(enumeratedInstances.Count() > 0, "Got some results back from CimSession.EnumerateInstances");
                        });

                Assert.True(onErrorGotRun, "onErrorGotRun");
            }
            cimSession = null;
        }

        [Fact]
        public void EnumerateInstancesAsync_StartedFromOnError()
        {
            CimSession cimSession;
            using (cimSession = CimSession.Create(null))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");
                CimOperationOptions operationOptions = new CimOperationOptions() { Flags = (CimOperationFlags)0xffffffff };
                IObservable<CimInstance> observedInstances = cimSession.EnumerateInstancesAsync(@"!@#$%^&*(", "!@#$%^&*(", operationOptions);
                Assert.NotNull(observedInstances, "cimSession.EnumerateInstancesAsync returned something other than null");

                bool onErrorGotRun = false;
                observedInstances.Subscribe(
                        onErrorAction:
                        delegate
                        {
                            onErrorGotRun = true;
                            Thread.Sleep(TimeSpan.FromSeconds(3));

                            Assert.True(cimSession != null, "SANITY CHECK / TEST SETUP VALIDATION: Test assumes that protocol handler will call OnError on the same thread as MI_Session_Enumerat");

                            IObservable<CimInstance> enumeratedInstances = cimSession.EnumerateInstancesAsync(@"root\cimv2", "Win32_Process");
                            Assert.NotNull(enumeratedInstances, "cimSession.EnumerateInstancesAsync returned something other than null");

                            List<AsyncItem<CimInstance>> listOfInstances = HelpersObservableToList(enumeratedInstances);
                            Assert.True(listOfInstances.Count > 0, "Got some results back from CimSession.EnumerateInstancesAsync");
                            Assert.Equal(listOfInstances.Last().Kind, AsyncItemKind.Completion, "Got OnCompleted callback");
                        });

                Assert.True(onErrorGotRun, "onErrorGotRun");
            }
            cimSession = null;
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
        public void Impersonated_AbandonedEnumerator()
        {
            CimSession cimSession;

            using (Helpers.ImpersonateTestUser())
            {
                Helpers.AssertRunningAsTestUser("Before calling EnumerateInstances_AbandonedEnumerator_Helper");
                cimSession = EnumerateInstances_AbandonedEnumerator_Helper();
            }

            Helpers.AssertRunningAsNonTestUser("Before calling GC.Collect/WaitForPendingFinalizers");

            Thread.Sleep(1000);
            GC.Collect();
            GC.WaitForPendingFinalizers();
            Thread.Sleep(1000);
            GC.Collect();
            GC.WaitForPendingFinalizers();
            Thread.Sleep(1000);
            GC.Collect();
            GC.WaitForPendingFinalizers();

            using (CimImpersonationHelpers.ImpersonateTestUser())
            {
                // this will hang if garbage collection has not drained the sync operation
                cimSession.Close();
            }
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
                Assert.Equal((string)enumeratedClass.CimSystemProperties.ClassName, "Win32_Process", "CimClass.CimSystemProperties.ClassName returneded null");            
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
                Assert.Equal((string)enumeratedClass.CimSystemProperties.ClassName, "Win32_Process", "CimClass.CimSystemProperties.ClassName returneded null");
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
                Assert.Equal((string)enumeratedClass.CimSystemProperties.ClassName, "Win32_Process", "CimClass.CimSystemProperties.ClassName returneded null");
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
                Assert.Equal((string)enumeratedClass.CimSystemProperties.ClassName, "Win32_Process", "CimClass.CimSystemProperties.ClassName returneded null");
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
                Assert.Equal((string)enumeratedClass.CimSystemProperties.ClassName, "Win32_Process", "CimClass.CimSystemProperties.ClassName returneded null");
                Assert.NotNull(enumeratedClass.CimClassMethods, "CimClass.CimClassMethods returneded null");
                Assert.NotNull(enumeratedClass.CimClassMethods["Create"], @"CimClass.CimClassMethods[Create] returneded null");
                //method qualifier
                Assert.NotNull(enumeratedClass.CimClassMethods["Create"].Qualifiers, @"CimClass.CimClassMethods[Create].CimClassQualifiers returneded null");
                Assert.NotNull(enumeratedClass.CimClassMethods["Create"].Qualifiers["static"], @"CimClass.CimClassMethods[Create].CimClassQualifiers[static] returneded null");
                Assert.Equal((Boolean)enumeratedClass.CimClassMethods["Create"].Qualifiers["static"].Value, true, @"CimClass.CimClassMethods[Create].CimClassQualifiers[static].Value is not true");
                //method parameter
                Assert.NotNull(enumeratedClass.CimClassMethods["Create"].Parameters, @"CimClass.CimClassMethods[Create].Parameters returneded null");
                Assert.NotNull(enumeratedClass.CimClassMethods["Create"].Parameters["CommandLine"], @"CimClass.CimClassMethods[Create].Parameters[CommandLine] returneded null");
                Assert.Equal(enumeratedClass.CimClassMethods["Create"].Parameters["CommandLine"].Name, "CommandLine", @"CimClass.CimClassMethods[Create].Parameters[CommandLine].Name returneded null");
                //Method parameter qualifier
                Assert.NotNull(enumeratedClass.CimClassMethods["Create"].Parameters["CommandLine"].Qualifiers, @"CimClass.CimClassMethods[Create].Parameters[CommandLine].CimClassQualifiers returneded null");
                Assert.NotNull(enumeratedClass.CimClassMethods["Create"].Parameters["CommandLine"].Qualifiers["ID"], @"CimClass.CimClassMethods[Create].Parameters[CommandLine].CimClassQualifiers[ID returneded null");
                Assert.Equal((CimType)enumeratedClass.CimClassMethods["Create"].Parameters["CommandLine"].Qualifiers["ID"].CimType, CimType.SInt32, @"CimClass.CimClassMethods[Create].Parameters[CommandLine].CimClassQualifiers[ID returneded null");
            }
        }

        [Fact]
        public void GetClass_Bug380707()
        {
            using (CimSession cimSession = CimSession.Create(null))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");
                CimClass enumeratedClass = cimSession.GetClass(@"root\MyTest4", "D");
                Assert.NotNull(enumeratedClass, "cimSession.GetClass returneded null");
                Assert.Equal((string)enumeratedClass.CimSystemProperties.ClassName, "D", "CimClass.CimSystemProperties.ClassName returneded null");
                //D Qualifiers
                Assert.NotNull(enumeratedClass.CimClassQualifiers, "enumeratedClass.CimClassQualifiers returneded null");
                //Qualifier Description
                Assert.NotNull(enumeratedClass.CimClassQualifiers["Description"], "enumeratedClass.CimClassQualifiers returneded null");
                Assert.NotNull(enumeratedClass.CimClassQualifiers["Description"].Value, "enumeratedClass.CimClassQualifiers[Description].Value returneded null");
                Assert.Equal(enumeratedClass.CimClassQualifiers["Description"].Value.ToString(), @"afdsasdsaf", @"enumeratedClass.CimClassQualifiers[Description].Value.ToString() is not equal to AFSDSDD");
                Assert.Equal(enumeratedClass.CimClassQualifiers["Description"].Flags & CimFlags.EnableOverride, CimFlags.EnableOverride, @"enumeratedClass.CimClassQualifiers[Description].Flags returneded null");
                Assert.Equal(enumeratedClass.CimClassQualifiers["Description"].Flags & CimFlags.ToSubclass, CimFlags.ToSubclass, @"enumeratedClass.CimClassQualifiers[Description].Flags returneded null");

                //Now check if qualifier MyQual2 and Abstract are not propogated.
                Assert.Null(enumeratedClass.CimClassQualifiers["MyQual2"], "Nonexistant MyQual2 qualifier returns null");
                Assert.Null(enumeratedClass.CimClassQualifiers["Abstract"], "Nonexistant Abstract qualifier returns null");

                //B Qualifiers.
                Assert.NotNull(enumeratedClass.CimSuperClass.CimClassQualifiers, "enumeratedClass.CimClassQualifiers returneded null");
                //Qualifier Description
                Assert.NotNull(enumeratedClass.CimSuperClass.CimClassQualifiers["Description"], "enumeratedClass.CimSuperClass.CimClassQualifiers returneded null");
                Assert.NotNull(enumeratedClass.CimSuperClass.CimClassQualifiers["Description"].Value, "enumeratedClass.CimSuperClass.CimClassQualifiers[Description].Value returneded null");
                Assert.Equal(enumeratedClass.CimSuperClass.CimClassQualifiers["Description"].Value.ToString(), @"afdsasdsaf", @"enumeratedClass.CimSuperClass.CimClassQualifiers[Description].Value.ToString() is not equal to AFSDSDD");
                Assert.Equal(enumeratedClass.CimSuperClass.CimClassQualifiers["Description"].Flags & CimFlags.EnableOverride, CimFlags.EnableOverride, @"enumeratedClass.CimSuperClass.CimClassQualifiers[Description].Flags returneded null");
                Assert.Equal(enumeratedClass.CimSuperClass.CimClassQualifiers["Description"].Flags & CimFlags.ToSubclass, CimFlags.ToSubclass, @"enumeratedClass.CimSuperClass.CimClassQualifiers[Description].Flags returneded null");

                //Now check if qualifier MyQual2 and Abstract are not propogated.
                Assert.Null(enumeratedClass.CimSuperClass.CimClassQualifiers["MyQual2"], "Nonexistant MyQual2 qualifier returns null");
                Assert.Null(enumeratedClass.CimSuperClass.CimClassQualifiers["Abstract"], "Nonexistant Abstract qualifier returns null");

                //A Qualifiers.
                Assert.NotNull(enumeratedClass.CimSuperClass.CimSuperClass.CimClassQualifiers, "enumeratedClass.CimClassQualifiers returneded null");
                //Qualifier Description
                Assert.NotNull(enumeratedClass.CimSuperClass.CimSuperClass.CimClassQualifiers["Description"], "enumeratedClass.CimSuperClass.CimSuperClass.CimClassQualifiers returneded null");
                Assert.NotNull(enumeratedClass.CimSuperClass.CimSuperClass.CimClassQualifiers["Description"].Value, "enumeratedClass.CimSuperClass.CimSuperClass.CimClassQualifiers[Description].Value returneded null");
                Assert.Equal(enumeratedClass.CimSuperClass.CimSuperClass.CimClassQualifiers["Description"].Value.ToString(), @"afdsasdsaf", @"enumeratedClass.CimSuperClass.CimSuperClass.CimClassQualifiers[Description].Value.ToString() is not equal to AFSDSDD");
                Assert.Equal(enumeratedClass.CimSuperClass.CimSuperClass.CimClassQualifiers["Description"].Flags & CimFlags.EnableOverride, CimFlags.EnableOverride, @"enumeratedClass.CimSuperClass.CimSuperClass.CimClassQualifiers[Description].Flags returneded null");
                Assert.Equal(enumeratedClass.CimSuperClass.CimSuperClass.CimClassQualifiers["Description"].Flags & CimFlags.ToSubclass, CimFlags.ToSubclass, @"enumeratedClass.CimSuperClass.CimSuperClass.CimClassQualifiers[Description].Flags returneded null");
                //Qualifier Abstract
                Assert.NotNull(enumeratedClass.CimSuperClass.CimSuperClass.CimClassQualifiers["Abstract"], "enumeratedClass.CimSuperClass.CimSuperClass.CimClassQualifiers returneded null");
                Assert.NotNull(enumeratedClass.CimSuperClass.CimSuperClass.CimClassQualifiers["Abstract"].Value, "enumeratedClass.CimSuperClass.CimSuperClass.CimClassQualifiers[Description].Value returneded null");
                Assert.Equal(enumeratedClass.CimSuperClass.CimSuperClass.CimClassQualifiers["Abstract"].Flags & CimFlags.EnableOverride, CimFlags.EnableOverride, @"enumeratedClass.CimSuperClass.CimSuperClass.CimClassQualifiers[Description].Flags returneded null");
                Assert.Equal(enumeratedClass.CimSuperClass.CimSuperClass.CimClassQualifiers["Abstract"].Flags & CimFlags.Restricted, CimFlags.Restricted, @"enumeratedClass.CimSuperClass.CimSuperClass.CimClassQualifiers[Description].Flags returneded null");

                //Check properties presence
                Assert.NotNull(enumeratedClass.CimClassProperties["Key"], "enumeratedClass.CimClassProperties returneded null");
                Assert.NotNull(enumeratedClass.CimClassProperties["Key"].Qualifiers, "enumeratedClass.CimClassProperties returneded null");
                Assert.NotNull(enumeratedClass.CimClassProperties["normal"], "enumeratedClass.CimClassProperties returneded null");
                Assert.NotNull(enumeratedClass.CimClassProperties["normal"].Qualifiers, "enumeratedClass.CimClassProperties returneded null");

                Assert.NotNull(enumeratedClass.CimSuperClass.CimClassProperties["Key"], "enumeratedClass.CimClassProperties returneded null");
                Assert.NotNull(enumeratedClass.CimSuperClass.CimClassProperties["Key"].Qualifiers, "enumeratedClass.CimClassProperties returneded null");
                Assert.NotNull(enumeratedClass.CimSuperClass.CimClassProperties["normal"], "enumeratedClass.CimClassProperties returneded null");
                Assert.NotNull(enumeratedClass.CimSuperClass.CimClassProperties["normal"].Qualifiers, "enumeratedClass.CimClassProperties returneded null");

                Assert.NotNull(enumeratedClass.CimSuperClass.CimSuperClass.CimClassProperties["Key"].Qualifiers, "enumeratedClass.CimClassProperties returneded null");
                Assert.NotNull(enumeratedClass.CimSuperClass.CimSuperClass.CimClassProperties["Key"], "enumeratedClass.CimClassProperties returneded null");
                Assert.NotNull(enumeratedClass.CimSuperClass.CimSuperClass.CimClassProperties["normal"], "enumeratedClass.CimClassProperties returneded null");
                Assert.NotNull(enumeratedClass.CimSuperClass.CimSuperClass.CimClassProperties["normal"].Qualifiers, "enumeratedClass.CimClassProperties returneded null");

                //Check properties qualifiers
                //D "key" properties qualifiers
                Assert.NotNull(enumeratedClass.CimClassProperties["Key"].Qualifiers["key"], "enumeratedClass.CimClassProperties[key].CimClassQualifiers[key] returneded null");
                Assert.Equal(enumeratedClass.CimClassProperties["Key"].Qualifiers["key"].Flags & CimFlags.ToSubclass, CimFlags.ToSubclass, "enumeratedClass.CimClassProperties[key].CimClassQualifiers[key].Flags returneded null");
                Assert.Equal(enumeratedClass.CimClassProperties["Key"].Qualifiers["key"].Flags & CimFlags.DisableOverride, CimFlags.DisableOverride, "enumeratedClass.CimClassProperties[key].CimClassQualifiers[key].Flags returneded null");
                Assert.NotNull(enumeratedClass.CimClassProperties["Key"].Qualifiers["MyQual"], "enumeratedClass.CimClassProperties[key].CimClassQualifiers[MyQual] returneded null");
                Assert.Equal(enumeratedClass.CimClassProperties["Key"].Qualifiers["MyQual"].Flags & CimFlags.ToSubclass, CimFlags.ToSubclass, "enumeratedClass.CimClassProperties[key].CimClassQualifiers[MyQual].Flags returneded null");
                Assert.Equal(enumeratedClass.CimClassProperties["Key"].Qualifiers["MyQual"].Flags & CimFlags.EnableOverride, CimFlags.EnableOverride, "enumeratedClass.CimClassProperties[key].CimClassQualifiers[MyQual].Flags returneded null");
                Assert.NotNull(enumeratedClass.CimClassProperties["Key"].Qualifiers["Description"], "enumeratedClass.CimClassProperties[key].CimClassQualifiers[Description] returneded null");
                Assert.Equal(enumeratedClass.CimClassProperties["Key"].Qualifiers["Description"].Flags & CimFlags.ToSubclass, CimFlags.ToSubclass, "enumeratedClass.CimClassProperties[key].CimClassQualifiers[Description].Flags returneded null");
                Assert.Equal(enumeratedClass.CimClassProperties["Key"].Qualifiers["Description"].Flags & CimFlags.EnableOverride, CimFlags.EnableOverride, "enumeratedClass.CimClassProperties[key].CimClassQualifiers[Description].Flags returneded null");
                Assert.Equal(enumeratedClass.CimClassProperties["Key"].Qualifiers["Description"].Flags & CimFlags.Translatable, CimFlags.Translatable, "enumeratedClass.CimClassProperties[key].CimClassQualifiers[Description].Flags returneded null");
                //D "normal" properties qualifiers
                Assert.NotNull(enumeratedClass.CimClassProperties["normal"].Qualifiers["Description"], "enumeratedClass.CimClassProperties[normal].CimClassQualifiers[Description] returneded null");
                Assert.Equal(enumeratedClass.CimClassProperties["normal"].Qualifiers["Description"].Flags & CimFlags.ToSubclass, CimFlags.ToSubclass, "enumeratedClass.CimClassProperties[normal].CimClassQualifiers[Description].Flags returneded null");
                Assert.Equal(enumeratedClass.CimClassProperties["normal"].Qualifiers["Description"].Flags & CimFlags.EnableOverride, CimFlags.EnableOverride, "enumeratedClass.CimClassProperties[normal].CimClassQualifiers[Description].Flags returneded null");
                Assert.NotNull(enumeratedClass.CimClassProperties["normal"].Qualifiers["MyQual"], "enumeratedClass.CimClassProperties[normal].CimClassQualifiers[key] returneded null");
                Assert.Equal(enumeratedClass.CimClassProperties["normal"].Qualifiers["MyQual"].Flags & CimFlags.ToSubclass, CimFlags.ToSubclass, "enumeratedClass.CimClassProperties[normal].CimClassQualifiers[Description].Flags returneded null");
                Assert.Equal(enumeratedClass.CimClassProperties["normal"].Qualifiers["MyQual"].Flags & CimFlags.EnableOverride, CimFlags.EnableOverride, "enumeratedClass.CimClassProperties[normal].CimClassQualifiers[Description].Flags returneded null");

                //B "key" properties qualifiers
                Assert.NotNull(enumeratedClass.CimSuperClass.CimClassProperties["Key"].Qualifiers["key"], "enumeratedClass.CimSuperClass.CimInstanceProperties[key].CimClassQualifiers[key] returneded null");
                Assert.Equal(enumeratedClass.CimSuperClass.CimClassProperties["Key"].Qualifiers["key"].Flags & CimFlags.ToSubclass, CimFlags.ToSubclass, "enumeratedClass.CimSuperClass.CimInstanceProperties[key].CimClassQualifiers[key].Flags returneded null");
                Assert.Equal(enumeratedClass.CimSuperClass.CimClassProperties["Key"].Qualifiers["key"].Flags & CimFlags.DisableOverride, CimFlags.DisableOverride, "enumeratedClass.CimSuperClass.CimInstanceProperties[key].CimClassQualifiers[key].Flags returneded null");
                Assert.NotNull(enumeratedClass.CimSuperClass.CimClassProperties["Key"].Qualifiers["MyQual"], "enumeratedClass.CimSuperClass.CimInstanceProperties[key].CimClassQualifiers[MyQual] returneded null");
                Assert.Equal(enumeratedClass.CimSuperClass.CimClassProperties["Key"].Qualifiers["MyQual"].Flags & CimFlags.ToSubclass, CimFlags.ToSubclass, "enumeratedClass.CimSuperClass.CimInstanceProperties[key].CimClassQualifiers[MyQual].Flags returneded null");
                Assert.Equal(enumeratedClass.CimSuperClass.CimClassProperties["Key"].Qualifiers["MyQual"].Flags & CimFlags.EnableOverride, CimFlags.EnableOverride, "enumeratedClass.CimSuperClass.CimInstanceProperties[key].CimClassQualifiers[MyQual].Flags returneded null");
                Assert.NotNull(enumeratedClass.CimSuperClass.CimClassProperties["Key"].Qualifiers["Description"], "enumeratedClass.CimClassProperties[key].CimClassQualifiers[Description] returneded null");
                Assert.Equal(enumeratedClass.CimSuperClass.CimClassProperties["Key"].Qualifiers["Description"].Flags & CimFlags.ToSubclass, CimFlags.ToSubclass, "enumeratedClass.CimSuperClass.CimInstanceProperties[key].CimClassQualifiers[Description].Flags returneded null");
                Assert.Equal(enumeratedClass.CimSuperClass.CimClassProperties["Key"].Qualifiers["Description"].Flags & CimFlags.EnableOverride, CimFlags.EnableOverride, "enumeratedClass.CimSuperClass.CimInstanceProperties[key].CimClassQualifiers[Description].Flags returneded null");
                Assert.Equal(enumeratedClass.CimSuperClass.CimClassProperties["Key"].Qualifiers["Description"].Flags & CimFlags.Translatable, CimFlags.Translatable, "enumeratedClass.CimSuperClass.CimInstanceProperties[key].CimClassQualifiers[Description].Flags returneded null");
                Assert.NotNull(enumeratedClass.CimSuperClass.CimClassProperties["Key"].Qualifiers["Override"], "enumeratedClass.CimClassProperties[key].CimClassQualifiers[Override] returneded null");
                Assert.Equal(enumeratedClass.CimSuperClass.CimClassProperties["Key"].Qualifiers["Override"].Flags & CimFlags.EnableOverride, CimFlags.EnableOverride, "enumeratedClass.CimSuperClass.CimInstanceProperties[key].CimClassQualifiers[Override].Flags returneded null");
                Assert.Equal(enumeratedClass.CimSuperClass.CimClassProperties["Key"].Qualifiers["Override"].Flags & CimFlags.Restricted, CimFlags.Restricted, "enumeratedClass.CimSuperClass.CimInstanceProperties[key].CimClassQualifiers[Override].Flags returneded null");
                //B "normal" properties qualifiers
                Assert.NotNull(enumeratedClass.CimSuperClass.CimClassProperties["normal"].Qualifiers["Description"], "enumeratedClass.CimSuperClass.CimInstanceProperties[normal].CimClassQualifiers[Description] returneded null");
                Assert.Equal(enumeratedClass.CimSuperClass.CimClassProperties["normal"].Qualifiers["Description"].Flags & CimFlags.ToSubclass, CimFlags.ToSubclass, "enumeratedClass.CimSuperClass.CimInstanceProperties[normal].CimClassQualifiers[Description].Flags returneded null");
                Assert.Equal(enumeratedClass.CimSuperClass.CimClassProperties["normal"].Qualifiers["Description"].Flags & CimFlags.EnableOverride, CimFlags.EnableOverride, "enumeratedClass.CimSuperClass.CimInstanceProperties[normal].CimClassQualifiers[Description].Flags returneded null");
                Assert.NotNull(enumeratedClass.CimSuperClass.CimClassProperties["normal"].Qualifiers["MyQual"], "enumeratedClass.CimSuperClass.CimInstanceProperties[normal].CimClassQualifiers[MyQual] returneded null");
                Assert.Equal(enumeratedClass.CimSuperClass.CimClassProperties["normal"].Qualifiers["MyQual"].Flags & CimFlags.ToSubclass, CimFlags.ToSubclass, "enumeratedClass.CimSuperClass.CimInstanceProperties[normal].CimClassQualifiers[MyQual].Flags returneded null");
                Assert.Equal(enumeratedClass.CimSuperClass.CimClassProperties["normal"].Qualifiers["MyQual"].Flags & CimFlags.EnableOverride, CimFlags.EnableOverride, "enumeratedClass.CimSuperClass.CimInstanceProperties[normal].CimClassQualifiers[MyQual].Flags returneded null");


                //A "key" properties qualifiers
                Assert.NotNull(enumeratedClass.CimSuperClass.CimSuperClass.CimClassProperties["Key"].Qualifiers["key"], "enumeratedClass.CimSuperClass.CimSuperClass.CimInstanceProperties[key].CimClassQualifiers[key] returneded null");
                Assert.Equal(enumeratedClass.CimSuperClass.CimSuperClass.CimClassProperties["Key"].Qualifiers["key"].Flags & CimFlags.ToSubclass, CimFlags.ToSubclass, "enumeratedClass.CimSuperClass.CimSuperClass.CimInstanceProperties[key].CimClassQualifiers[key].Flags returneded null");
                Assert.Equal(enumeratedClass.CimSuperClass.CimSuperClass.CimClassProperties["Key"].Qualifiers["key"].Flags & CimFlags.DisableOverride, CimFlags.DisableOverride, "enumeratedClass.CimSuperClass.CimSuperClass.CimInstanceProperties[key].CimClassQualifiers[key].Flags returneded null");
                Assert.NotNull(enumeratedClass.CimSuperClass.CimSuperClass.CimClassProperties["Key"].Qualifiers["MyQual"], "enumeratedClass.CimSuperClass.CimSuperClass.CimInstanceProperties[key].CimClassQualifiers[MyQual] returneded null");
                Assert.Equal(enumeratedClass.CimSuperClass.CimSuperClass.CimClassProperties["Key"].Qualifiers["MyQual"].Flags & CimFlags.ToSubclass, CimFlags.ToSubclass, "enumeratedClass.CimSuperClass.CimSuperClass.CimInstanceProperties[key].CimClassQualifiers[MyQual].Flags returneded null");
                Assert.Equal(enumeratedClass.CimSuperClass.CimSuperClass.CimClassProperties["Key"].Qualifiers["MyQual"].Flags & CimFlags.EnableOverride, CimFlags.EnableOverride, "enumeratedClass.CimSuperClass.CimSuperClass.CimInstanceProperties[key].CimClassQualifiers[MyQual].Flags returneded null");
                Assert.NotNull(enumeratedClass.CimSuperClass.CimSuperClass.CimClassProperties["Key"].Qualifiers["Description"], "enumeratedClass.CimSuperClass.CimSuperClass.CimInstanceProperties[key].CimClassQualifiers[Description] returneded null");
                Assert.Equal(enumeratedClass.CimSuperClass.CimSuperClass.CimClassProperties["Key"].Qualifiers["Description"].Flags & CimFlags.ToSubclass, CimFlags.ToSubclass, "enumeratedClass.CimSuperClass.CimSuperClass.CimInstanceProperties[key].CimClassQualifiers[Description].Flags returneded null");
                Assert.Equal(enumeratedClass.CimSuperClass.CimSuperClass.CimClassProperties["Key"].Qualifiers["Description"].Flags & CimFlags.EnableOverride, CimFlags.EnableOverride, "enumeratedClass.CimSuperClass.CimSuperClass.CimInstanceProperties[key].CimClassQualifiers[Description].Flags returneded null");
                //A "normal" properties qualifiers
                Assert.NotNull(enumeratedClass.CimSuperClass.CimSuperClass.CimClassProperties["normal"].Qualifiers["Description"], "enumeratedClass.CimSuperClass.CimSuperClass.CimInstanceProperties[normal].CimClassQualifiers[Description] returneded null");
                Assert.Equal(enumeratedClass.CimSuperClass.CimSuperClass.CimClassProperties["normal"].Qualifiers["Description"].Flags & CimFlags.ToSubclass, CimFlags.ToSubclass, "enumeratedClass.CimSuperClass.CimSuperClass.CimInstanceProperties[normal].CimClassQualifiers[Description].Flags returneded null");
                Assert.Equal(enumeratedClass.CimSuperClass.CimSuperClass.CimClassProperties["normal"].Qualifiers["Description"].Flags & CimFlags.EnableOverride, CimFlags.EnableOverride, "enumeratedClass.CimSuperClass.CimSuperClass.CimInstanceProperties[normal].CimClassQualifiers[Description].Flags returneded null");
                Assert.NotNull(enumeratedClass.CimSuperClass.CimSuperClass.CimClassProperties["normal"].Qualifiers["MyQual"], "enumeratedClass.CimSuperClass.CimSuperClass.CimInstanceProperties[key].CimClassQualifiers[MyQual] returneded null");
                Assert.Equal(enumeratedClass.CimSuperClass.CimSuperClass.CimClassProperties["normal"].Qualifiers["MyQual"].Flags & CimFlags.ToSubclass, CimFlags.ToSubclass, "enumeratedClass.CimSuperClass.CimSuperClass.CimInstanceProperties[normal].CimClassQualifiers[MyQual].Flags returneded null");
                Assert.Equal(enumeratedClass.CimSuperClass.CimSuperClass.CimClassProperties["normal"].Qualifiers["MyQual"].Flags & CimFlags.EnableOverride, CimFlags.EnableOverride, "enumeratedClass.CimSuperClass.CimSuperClass.CimInstanceProperties[normal].CimClassQualifiers[MyQual].Flags returneded null");

                //METHODS
                //D Methods
                Assert.NotNull(enumeratedClass.CimClassMethods, "enumeratedClass.CimClassMethods returneded null");
                Assert.NotNull(enumeratedClass.CimClassMethods["MyMethod"], "enumeratedClass.CimClassMethods[MyMethod] returneded null");
                Assert.NotNull(enumeratedClass.CimClassMethods["MyMethod2"], "enumeratedClass.CimClassMethods[MyMethod2] returneded null");
                //D Method MyMethod
                Assert.NotNull(enumeratedClass.CimClassMethods["MyMethod"].Qualifiers, "enumeratedClass.CimClassMethods[MyMethod].CimClassQualifiers returneded null");
                Assert.NotNull(enumeratedClass.CimClassMethods["MyMethod"].Qualifiers["Maxlen"], "enumeratedClass.CimClassMethods[MyMethod].CimClassQualifiers[Maxlen] returneded null");
                Assert.Equal(enumeratedClass.CimClassMethods["MyMethod"].Qualifiers["Maxlen"].Flags & CimFlags.EnableOverride, CimFlags.EnableOverride, "enumeratedClass.CimClassMethods[MyMethod].CimClassQualifiers[Maxlen] returneded null");
                Assert.Equal(enumeratedClass.CimClassMethods["MyMethod"].Qualifiers["Maxlen"].Flags & CimFlags.ToSubclass, CimFlags.ToSubclass, "enumeratedClass.CimClassMethods[MyMethod].CimClassQualifiers[Maxlen] returneded null");
                Assert.NotNull(enumeratedClass.CimClassMethods["MyMethod"].Qualifiers["Description"], "enumeratedClass.CimClassMethods[MyMethod].CimClassQualifiers[Maxlen] returneded null");
                Assert.Equal(enumeratedClass.CimClassMethods["MyMethod"].Qualifiers["Description"].Flags & CimFlags.EnableOverride, CimFlags.EnableOverride, "enumeratedClass.CimClassMethods[MyMethod].CimClassQualifiers[Maxlen] returneded null");
                Assert.Equal(enumeratedClass.CimClassMethods["MyMethod"].Qualifiers["Description"].Flags & CimFlags.ToSubclass, CimFlags.ToSubclass, "enumeratedClass.CimClassMethods[MyMethod].CimClassQualifiers[Maxlen] returneded null");
                Assert.Equal(enumeratedClass.CimClassMethods["MyMethod"].Qualifiers["Description"].Flags & CimFlags.Translatable, CimFlags.Translatable, "enumeratedClass.CimClassMethods[MyMethod].CimClassQualifiers[Maxlen] returneded null");
                //D Method MyMethod2
                Assert.NotNull(enumeratedClass.CimClassMethods["MyMethod2"].Qualifiers["Description"], "enumeratedClass.CimClassMethods[MyMethod].CimClassQualifiers[Maxlen] returneded null");
                Assert.Equal(enumeratedClass.CimClassMethods["MyMethod2"].Qualifiers["Description"].Flags & CimFlags.EnableOverride, CimFlags.EnableOverride, "enumeratedClass.CimClassMethods[MyMethod].CimClassQualifiers[Maxlen] returneded null");
                Assert.Equal(enumeratedClass.CimClassMethods["MyMethod2"].Qualifiers["Description"].Flags & CimFlags.ToSubclass, CimFlags.ToSubclass, "enumeratedClass.CimClassMethods[MyMethod].CimClassQualifiers[Maxlen] returneded null");
                Assert.Equal(enumeratedClass.CimClassMethods["MyMethod2"].Qualifiers["Description"].Flags & CimFlags.Translatable, CimFlags.Translatable, "enumeratedClass.CimClassMethods[MyMethod].CimClassQualifiers[Maxlen] returneded null");
                Assert.Equal(enumeratedClass.CimClassMethods["MyMethod"].Parameters.Count, 1, "enumeratedClass.CimClassMethods[MyMethod].CimClassQualifiers[Maxlen] returneded null");

                //B Methods
                Assert.NotNull(enumeratedClass.CimSuperClass.CimClassMethods, "enumeratedClass.CimSuperClass.CimClassMethods returneded null");
                Assert.NotNull(enumeratedClass.CimSuperClass.CimClassMethods["MyMethod"], "enumeratedClass.CimSuperClass.CimClassMethods[MyMethod] returneded null");
                Assert.NotNull(enumeratedClass.CimSuperClass.CimClassMethods["MyMethod2"], "enumeratedClass.CimSuperClass.CimClassMethods[MyMethod2] returneded null");
                //B Method MyMethod
                Assert.NotNull(enumeratedClass.CimSuperClass.CimClassMethods["MyMethod"].Qualifiers, "enumeratedClass.CimClassMethods[MyMethod].CimClassQualifiers returneded null");
                Assert.NotNull(enumeratedClass.CimSuperClass.CimClassMethods["MyMethod"].Qualifiers["Maxlen"], "enumeratedClass.CimClassMethods[MyMethod].CimClassQualifiers[Maxlen] returneded null");
                Assert.Equal(enumeratedClass.CimSuperClass.CimClassMethods["MyMethod"].Qualifiers["Maxlen"].Flags & CimFlags.EnableOverride, CimFlags.EnableOverride, "enumeratedClass.CimClassMethods[MyMethod].CimClassQualifiers[Maxlen] returneded null");
                Assert.Equal(enumeratedClass.CimSuperClass.CimClassMethods["MyMethod"].Qualifiers["Maxlen"].Flags & CimFlags.ToSubclass, CimFlags.ToSubclass, "enumeratedClass.CimClassMethods[MyMethod].CimClassQualifiers[Maxlen] returneded null");
                Assert.NotNull(enumeratedClass.CimSuperClass.CimClassMethods["MyMethod"].Qualifiers["Description"], "enumeratedClass.CimClassMethods[MyMethod].CimClassQualifiers[Maxlen] returneded null");
                Assert.Equal(enumeratedClass.CimSuperClass.CimClassMethods["MyMethod"].Qualifiers["Description"].Flags & CimFlags.EnableOverride, CimFlags.EnableOverride, "enumeratedClass.CimClassMethods[MyMethod].CimClassQualifiers[Maxlen] returneded null");
                Assert.Equal(enumeratedClass.CimSuperClass.CimClassMethods["MyMethod"].Qualifiers["Description"].Flags & CimFlags.ToSubclass, CimFlags.ToSubclass, "enumeratedClass.CimClassMethods[MyMethod].CimClassQualifiers[Maxlen] returneded null");
                Assert.Equal(enumeratedClass.CimSuperClass.CimClassMethods["MyMethod"].Qualifiers["Description"].Flags & CimFlags.Translatable, CimFlags.Translatable, "enumeratedClass.CimClassMethods[MyMethod].CimClassQualifiers[Maxlen] returneded null");

                //B Method MyMethod2
                Assert.NotNull(enumeratedClass.CimSuperClass.CimClassMethods["MyMethod2"].Qualifiers["Description"], "enumeratedClass.CimClassMethods[MyMethod].CimClassQualifiers[Maxlen] returneded null");
                Assert.Equal(enumeratedClass.CimSuperClass.CimClassMethods["MyMethod2"].Qualifiers["Description"].Flags & CimFlags.EnableOverride, CimFlags.EnableOverride, "enumeratedClass.CimClassMethods[MyMethod].CimClassQualifiers[Maxlen] returneded null");
                Assert.Equal(enumeratedClass.CimSuperClass.CimClassMethods["MyMethod2"].Qualifiers["Description"].Flags & CimFlags.ToSubclass, CimFlags.ToSubclass, "enumeratedClass.CimClassMethods[MyMethod].CimClassQualifiers[Maxlen] returneded null");
                Assert.Equal(enumeratedClass.CimSuperClass.CimClassMethods["MyMethod2"].Qualifiers["Description"].Flags & CimFlags.Translatable, CimFlags.Translatable, "enumeratedClass.CimClassMethods[MyMethod].CimClassQualifiers[Maxlen] returneded null");
                Assert.Equal(enumeratedClass.CimSuperClass.CimClassMethods["MyMethod"].Parameters.Count, 1, "enumeratedClass.CimClassMethods[MyMethod].CimClassQualifiers[Maxlen] returneded null");

                //A Methods
                Assert.NotNull(enumeratedClass.CimSuperClass.CimSuperClass.CimClassMethods, "enumeratedClass.CimClassMethods returneded null");
                Assert.NotNull(enumeratedClass.CimSuperClass.CimSuperClass.CimClassMethods["MyMethod"], "enumeratedClass.CimClassMethods[MyMethod] returneded null");
                Assert.Null(enumeratedClass.CimSuperClass.CimSuperClass.CimClassMethods["MyMethod2"], "enumeratedClass.CimSuperClass.CimSuperClass.CimClassMethods[MyMethod2] returneded non null");
                //A Method MyMethod
                Assert.NotNull(enumeratedClass.CimSuperClass.CimSuperClass.CimClassMethods["MyMethod"].Qualifiers, "enumeratedClass.CimClassMethods[MyMethod].CimClassQualifiers returneded null");
                Assert.NotNull(enumeratedClass.CimSuperClass.CimSuperClass.CimClassMethods["MyMethod"].Qualifiers["Maxlen"], "enumeratedClass.CimClassMethods[MyMethod].CimClassQualifiers[Maxlen] returneded null");
                Assert.Equal(enumeratedClass.CimSuperClass.CimSuperClass.CimClassMethods["MyMethod"].Qualifiers["Maxlen"].Flags & CimFlags.EnableOverride, CimFlags.EnableOverride, "enumeratedClass.CimClassMethods[MyMethod].CimClassQualifiers[Maxlen] returneded null");
                Assert.Equal(enumeratedClass.CimSuperClass.CimSuperClass.CimClassMethods["MyMethod"].Qualifiers["Maxlen"].Flags & CimFlags.ToSubclass, CimFlags.ToSubclass, "enumeratedClass.CimClassMethods[MyMethod].CimClassQualifiers[Maxlen] returneded null");

                //Now Check the default qualifier flavor behavior


            }
            using (CimSession cimSession = CimSession.Create(null))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");
                CimClass enumeratedClass = cimSession.GetClass(@"root\MyTest4", "E");
                Assert.NotNull(enumeratedClass, "cimSession.GetClass returneded null");
                Assert.Equal((string)enumeratedClass.CimSystemProperties.ClassName, "E", "CimClass.CimSystemProperties.ClassName returneded null");
                //D Qualifiers
                Assert.NotNull(enumeratedClass.CimClassQualifiers, "enumeratedClass.CimClassQualifiers returneded null");
                Assert.NotNull(enumeratedClass.CimClassQualifiers["association"], "enumeratedClass.CimClassQualifiers returneded null");
                Assert.Equal(enumeratedClass.CimClassQualifiers["association"].Flags & CimFlags.DisableOverride, CimFlags.DisableOverride, @"enumeratedClass.CimClassQualifiers[association].Flags returneded null");
                Assert.Equal(enumeratedClass.CimClassQualifiers["association"].Flags & CimFlags.ToSubclass, CimFlags.ToSubclass, @"enumeratedClass.CimClassQualifiers[association].Flags returneded null");
                Assert.NotNull(enumeratedClass.CimClassQualifiers["indication"], "enumeratedClass.CimClassQualifiers returneded null");
                Assert.Equal(enumeratedClass.CimClassQualifiers["indication"].Flags & CimFlags.DisableOverride, CimFlags.DisableOverride, @"enumeratedClass.CimClassQualifiers[indication].Flags returneded null");
                Assert.Equal(enumeratedClass.CimClassQualifiers["indication"].Flags & CimFlags.ToSubclass, CimFlags.ToSubclass, @"enumeratedClass.CimClassQualifiers[indication].Flags returneded null");
                Assert.NotNull(enumeratedClass.CimClassQualifiers["ClassConstraint"], "enumeratedClass.CimClassQualifiers returneded null");
                Assert.Equal(enumeratedClass.CimClassQualifiers["ClassConstraint"].Flags & CimFlags.EnableOverride, CimFlags.EnableOverride, @"enumeratedClass.CimClassQualifiers[ClassConstraint].Flags returneded null");
                Assert.Equal(enumeratedClass.CimClassQualifiers["ClassConstraint"].Flags & CimFlags.ToSubclass, CimFlags.ToSubclass, @"enumeratedClass.CimClassQualifiers[ClassConstraint].Flags returneded null");
                Assert.NotNull(enumeratedClass.CimClassQualifiers["Deprecated"], "enumeratedClass.CimClassQualifiers returneded null");
                Assert.Equal(enumeratedClass.CimClassQualifiers["Deprecated"].Flags & CimFlags.EnableOverride, CimFlags.EnableOverride, @"enumeratedClass.CimClassQualifiers[Deprecated].Flags returneded null");
                Assert.Equal(enumeratedClass.CimClassQualifiers["Deprecated"].Flags & CimFlags.Restricted, CimFlags.Restricted, @"enumeratedClass.CimClassQualifiers[Deprecated].Flags returneded null");
                Assert.NotNull(enumeratedClass.CimClassQualifiers["DisplayName"], "enumeratedClass.CimClassQualifiers returneded null");
                Assert.Equal(enumeratedClass.CimClassQualifiers["DisplayName"].Flags & CimFlags.EnableOverride, CimFlags.EnableOverride, @"enumeratedClass.CimClassQualifiers[DisplayName].Flags returneded null");
                Assert.Equal(enumeratedClass.CimClassQualifiers["DisplayName"].Flags & CimFlags.ToSubclass, CimFlags.ToSubclass, @"enumeratedClass.CimClassQualifiers[DisplayName].Flags returneded null");
                Assert.NotNull(enumeratedClass.CimClassQualifiers["Exception"], "enumeratedClass.CimClassQualifiers returneded null");
                Assert.Equal(enumeratedClass.CimClassQualifiers["Exception"].Flags & CimFlags.DisableOverride, CimFlags.DisableOverride, @"enumeratedClass.CimClassQualifiers[Exception].Flags returneded null");
                Assert.Equal(enumeratedClass.CimClassQualifiers["Exception"].Flags & CimFlags.ToSubclass, CimFlags.ToSubclass, @"enumeratedClass.CimClassQualifiers[Exception].Flags returneded null");
                Assert.NotNull(enumeratedClass.CimClassQualifiers["Experimental"], "enumeratedClass.CimClassQualifiers returneded null");
                Assert.Equal(enumeratedClass.CimClassQualifiers["Experimental"].Flags & CimFlags.EnableOverride, CimFlags.EnableOverride, @"enumeratedClass.CimClassQualifiers[Experimental].Flags returneded null");
                Assert.Equal(enumeratedClass.CimClassQualifiers["Experimental"].Flags & CimFlags.Restricted, CimFlags.Restricted, @"enumeratedClass.CimClassQualifiers[Experimental].Flags returneded null");

                //Properties qualifiers
                //p1 property qualifier
                Assert.NotNull(enumeratedClass.CimClassProperties, "enumeratedClass.CimClassProperties returneded null");
                Assert.NotNull(enumeratedClass.CimClassProperties["p1"], "enumeratedClass.CimClassProperties[p1] returneded null");
                Assert.NotNull(enumeratedClass.CimClassProperties["p1"].Qualifiers, "enumeratedClass.CimClassProperties[p1].CimClassQualifiers returneded null");
                Assert.NotNull(enumeratedClass.CimClassProperties["p1"].Qualifiers["ArrayType"], "enumeratedClass.CimClassProperties[p1].CimClassQualifiers[ArrayType] returneded null");
                Assert.Equal(enumeratedClass.CimClassProperties["p1"].Qualifiers["ArrayType"].Flags & CimFlags.DisableOverride, CimFlags.DisableOverride, "enumeratedClass.CimClassProperties[p1].CimClassQualifiers[ArrayType] returneded null");
                Assert.Equal(enumeratedClass.CimClassProperties["p1"].Qualifiers["ArrayType"].Flags & CimFlags.ToSubclass, CimFlags.ToSubclass, "enumeratedClass.CimClassProperties[p1].CimClassQualifiers[ArrayType] returneded null");

                Assert.NotNull(enumeratedClass.CimClassProperties["p1"].Qualifiers["BitMap"], "enumeratedClass.CimClassProperties[p1].CimClassQualifiers[BitMap] returneded null");
                Assert.Equal(enumeratedClass.CimClassProperties["p1"].Qualifiers["BitMap"].Flags & CimFlags.EnableOverride, CimFlags.EnableOverride, "enumeratedClass.CimClassProperties[p1].CimClassQualifiers[BitMap] returneded null");
                Assert.Equal(enumeratedClass.CimClassProperties["p1"].Qualifiers["BitMap"].Flags & CimFlags.ToSubclass, CimFlags.ToSubclass, "enumeratedClass.CimClassProperties[p1].CimClassQualifiers[BitMap] returneded null");

                Assert.NotNull(enumeratedClass.CimClassProperties["p1"].Qualifiers["BitValues"], "enumeratedClass.CimClassProperties[p1].CimClassQualifiers[BitValues] returneded null");
                Assert.Equal(enumeratedClass.CimClassProperties["p1"].Qualifiers["BitValues"].Flags & CimFlags.EnableOverride, CimFlags.EnableOverride, "enumeratedClass.CimClassProperties[p1].CimClassQualifiers[BitValues] returneded null");
                Assert.Equal(enumeratedClass.CimClassProperties["p1"].Qualifiers["BitValues"].Flags & CimFlags.ToSubclass, CimFlags.ToSubclass, "enumeratedClass.CimClassProperties[p1].CimClassQualifiers[BitValues] returneded null");
                Assert.Equal(enumeratedClass.CimClassProperties["p1"].Qualifiers["BitValues"].Flags & CimFlags.Translatable, CimFlags.Translatable, "enumeratedClass.CimClassProperties[p1].CimClassQualifiers[BitValues] returneded null");

                Assert.NotNull(enumeratedClass.CimClassProperties["p1"].Qualifiers["Correlatable"], "enumeratedClass.CimClassProperties[p1].CimClassQualifiers[Correlatable] returneded null");
                Assert.Equal(enumeratedClass.CimClassProperties["p1"].Qualifiers["Correlatable"].Flags & CimFlags.EnableOverride, CimFlags.EnableOverride, "enumeratedClass.CimClassProperties[p1].CimClassQualifiers[Correlatable] returneded null");
                Assert.Equal(enumeratedClass.CimClassProperties["p1"].Qualifiers["Correlatable"].Flags & CimFlags.ToSubclass, CimFlags.ToSubclass, "enumeratedClass.CimClassProperties[p1].CimClassQualifiers[Correlatable] returneded null");

                Assert.NotNull(enumeratedClass.CimClassProperties["p1"].Qualifiers["DN"], "enumeratedClass.CimClassProperties[p1].CimClassQualifiers[DN] returneded null");
                Assert.Equal(enumeratedClass.CimClassProperties["p1"].Qualifiers["DN"].Flags & CimFlags.DisableOverride, CimFlags.DisableOverride, "enumeratedClass.CimClassProperties[p1].CimClassQualifiers[DN] returneded null");
                Assert.Equal(enumeratedClass.CimClassProperties["p1"].Qualifiers["DN"].Flags & CimFlags.ToSubclass, CimFlags.ToSubclass, "enumeratedClass.CimClassProperties[p1].CimClassQualifiers[DN] returneded null");

                Assert.NotNull(enumeratedClass.CimClassProperties["p1"].Qualifiers["Counter"], "enumeratedClass.CimClassProperties[p1].CimClassQualifiers[Counter] returneded null");
                Assert.Equal(enumeratedClass.CimClassProperties["p1"].Qualifiers["Counter"].Flags & CimFlags.EnableOverride, CimFlags.EnableOverride, "enumeratedClass.CimClassProperties[p1].CimClassQualifiers[Counter] returneded null");
                Assert.Equal(enumeratedClass.CimClassProperties["p1"].Qualifiers["Counter"].Flags & CimFlags.ToSubclass, CimFlags.ToSubclass, "enumeratedClass.CimClassProperties[p1].CimClassQualifiers[Counter] returneded null");

                Assert.NotNull(enumeratedClass.CimClassProperties["p1"].Qualifiers["EmbeddedInstance"], "enumeratedClass.CimClassProperties[p1].CimClassQualifiers[EmbeddedInstance] returneded null");
                Assert.Equal(enumeratedClass.CimClassProperties["p1"].Qualifiers["EmbeddedInstance"].Flags & CimFlags.EnableOverride, CimFlags.EnableOverride, "enumeratedClass.CimClassProperties[p1].CimClassQualifiers[EmbeddedInstance] returneded null");
                Assert.Equal(enumeratedClass.CimClassProperties["p1"].Qualifiers["EmbeddedInstance"].Flags & CimFlags.ToSubclass, CimFlags.ToSubclass, "enumeratedClass.CimClassProperties[p1].CimClassQualifiers[EmbeddedInstance] returneded null");

                Assert.NotNull(enumeratedClass.CimClassProperties["p1"].Qualifiers["EmbeddedObject"], "enumeratedClass.CimClassProperties[p1].CimClassQualifiers[EmbeddedObject] returneded null");
                Assert.Equal(enumeratedClass.CimClassProperties["p1"].Qualifiers["EmbeddedObject"].Flags & CimFlags.DisableOverride, CimFlags.DisableOverride, "enumeratedClass.CimClassProperties[p1].CimClassQualifiers[EmbeddedObject] returneded null");
                Assert.Equal(enumeratedClass.CimClassProperties["p1"].Qualifiers["EmbeddedObject"].Flags & CimFlags.ToSubclass, CimFlags.ToSubclass, "enumeratedClass.CimClassProperties[p1].CimClassQualifiers[EmbeddedObject] returneded null");

                //p2 property qualifier
                Assert.NotNull(enumeratedClass.CimClassProperties, "enumeratedClass.CimClassProperties returneded null");
                Assert.NotNull(enumeratedClass.CimClassProperties["p2"], "enumeratedClass.CimClassProperties[p2] returneded null");
                Assert.NotNull(enumeratedClass.CimClassProperties["p2"].Qualifiers, "enumeratedClass.CimClassProperties[p2].CimClassQualifiers returneded null");
                Assert.NotNull(enumeratedClass.CimClassProperties["p2"].Qualifiers["aggregation"], "enumeratedClass.CimClassProperties[p2].CimClassQualifiers[aggregation] returneded null");
                Assert.Equal(enumeratedClass.CimClassProperties["p2"].Qualifiers["aggregation"].Flags & CimFlags.DisableOverride, CimFlags.DisableOverride, "enumeratedClass.CimClassProperties[p2].CimClassQualifiers[aggregation] returneded null");
                Assert.Equal(enumeratedClass.CimClassProperties["p2"].Qualifiers["aggregation"].Flags & CimFlags.ToSubclass, CimFlags.ToSubclass, "enumeratedClass.CimClassProperties[p2].CimClassQualifiers[aggregation] returneded null");

                Assert.NotNull(enumeratedClass.CimClassProperties["p2"].Qualifiers["Composition"], "enumeratedClass.CimClassProperties[p1].CimClassQualifiers[Composition] returneded null");
                Assert.Equal(enumeratedClass.CimClassProperties["p2"].Qualifiers["Composition"].Flags & CimFlags.DisableOverride, CimFlags.DisableOverride, "enumeratedClass.CimClassProperties[p2].CimClassQualifiers[Composition] returneded null");
                Assert.Equal(enumeratedClass.CimClassProperties["p2"].Qualifiers["Composition"].Flags & CimFlags.ToSubclass, CimFlags.ToSubclass, "enumeratedClass.CimClassProperties[p2].CimClassQualifiers[Composition] returneded null");

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
                Assert.Equal((string)enumeratedClass.CimSystemProperties.ClassName, "CIM_Process", "CimClass.CimSystemProperties.ClassName returneded null");
                Assert.Equal((string)enumeratedClass.CimSuperClassName, "CIM_LogicalElement", "CimClass.CimSuperClassName returneded null");
                //Assert.NotNull(enumeratedClass.CimSuperClass, "CimClass.CimSuperClassName returneded null");
                Assert.Equal((string)enumeratedClass.CimSystemProperties.Namespace, @"root/cimv2", "CimClass.CimSystemProperties.Namespace returneded null");
                Assert.NotNull(enumeratedClass.CimClassProperties, "CimClass.CimClassProperties returneded null");
                Assert.NotNull(enumeratedClass.CimClassProperties["Handle"], @"CimClass.CimClassProperties[Handle] returneded null");

                Assert.Equal((string)enumeratedClass.CimClassProperties["Handle"].Name, "Handle", @"CimClass.CimClassProperties[Handle].Name returneded null");
                Assert.Equal((CimType)enumeratedClass.CimClassProperties["Handle"].CimType, CimType.String, @"CimClass.CimClassProperties[Handle].type returneded null");
                // Assert.Equal(enumeratedClass.CimClassProperties["Handle"].Flags & CimFlags.Key, CimFlags.Key, @"CimClass.CimClassProperties[Handle].Flags returneded null");

                Assert.NotNull(enumeratedClass.CimClassProperties["Handle"].Qualifiers, @"CimClass.CimClassProperties[Handle].Qualifiers returneded null");
                Assert.Equal((string)enumeratedClass.CimClassProperties["Handle"].Qualifiers["read"].Name, "read", @"CimClass.CimClassProperties[Handle].Qualifiers[Cim_Key].Name returneded null");
                Assert.Equal((CimType)enumeratedClass.CimClassProperties["Handle"].Qualifiers["read"].CimType, CimType.Boolean, @"CimClass.CimClassProperties[Handle].Qualifiers[read].Type returneded null");
                //Assert.Equal(enumeratedClass.CimClassProperties["Handle"].CimClassQualifiers["read"].Flags & CimFlags.DisableOverride, CimFlags.DisableOverride, @"CimClass.CimClassProperties[Handle].CimClassQualifiers[read].Flags returneded null");
                //Assert.Equal((bool)enumeratedClass.CimClassProperties["Handle"].CimClassQualifiers["read"].Value, true, @"CimClass.CimClassProperties[Handle].CimClassQualifiers[Cim_Key].Value returneded null");

                //Check class qualifiers
                Assert.NotNull(enumeratedClass.CimClassQualifiers, "CimClass.CimClassQualifiers returneded null");
                Assert.NotNull(enumeratedClass.CimClassQualifiers["Abstract"], "CimClass.CimClassQualifiers[Abstract] returneded null");
                Assert.Equal(enumeratedClass.CimClassQualifiers["Abstract"].Name, "Abstract", "CimClass.CimClassQualifiers[Abstract] returneded null");
                Assert.Equal((CimType)enumeratedClass.CimClassQualifiers["Abstract"].CimType, CimType.Boolean, @"enumeratedClass.CimClassQualifiers[Abstract].Type returneded null");
                //Assert.Equal(enumeratedClass.CimClassQualifiers["Abstract"].Flags & CimFlags.DisableOverride, CimFlags.DisableOverride, @"enumeratedClass.CimClassQualifiers[Abstract].Flags returneded null");

                //Check System properties
                //System properties Environment.MachineName
                Assert.Equal(enumeratedClass.CimSystemProperties.Namespace, "root/cimv2", "cimInstance.CimSystemProperties.Namespace is not correct");
                Assert.Equal(enumeratedClass.CimSystemProperties.ClassName, "CIM_Process", "cimInstance.CimSystemProperties.ClassName is not correct");
                Assert.Equal(enumeratedClass.CimSystemProperties.ServerName, Environment.MachineName, "cimInstance.CimSystemProperties.ServerName is not correct");
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

                Assert.Equal(asyncList.Count, 2, "Got 2 async callbacks");
                Assert.Equal(asyncList[0].Kind, AsyncItemKind.Item, "First callback for item");
                Assert.Equal(asyncList[1].Kind, AsyncItemKind.Completion, "Second callback for completion");
                CimClass enumeratedClass = asyncList[0].Item;


                Assert.NotNull(enumeratedClass, "cimSession.GetClass returneded null");
                Assert.NotNull(enumeratedClass.CimSuperClass, "cimSession.CimSuperClass is null");
                Assert.Equal((string)enumeratedClass.CimSystemProperties.ClassName, "CIM_Process", "CimClass.CimSystemProperties.ClassName returneded null");
                Assert.Equal((string)enumeratedClass.CimSuperClassName, "CIM_LogicalElement", "CimClass.CimSuperClassName returneded null");
                Assert.Equal((string)enumeratedClass.CimSuperClassName, "CIM_LogicalElement", "CimClass.CimSuperClassName returneded null");
                //Assert.NotNull(enumeratedClass.CimSuperClass, "CimClass.CimSuperClassName returneded null");
                Assert.Equal((string)enumeratedClass.CimSystemProperties.Namespace, @"root/cimv2", "CimClass.CimSystemProperties.Namespace returneded null");
                Assert.NotNull(enumeratedClass.CimClassProperties, "CimClass.CimClassProperties returneded null");
                Assert.NotNull(enumeratedClass.CimClassProperties["Handle"], @"CimClass.CimClassProperties[Handle] returneded null");
                Assert.Equal((string)enumeratedClass.CimClassProperties["Handle"].Name, "Handle", @"CimClass.CimClassProperties[Handle].Name returneded null");
                Assert.Equal((CimType)enumeratedClass.CimClassProperties["Handle"].CimType, CimType.String, @"CimClass.CimClassProperties[Handle].type returneded null");
                Assert.NotNull(enumeratedClass.CimClassProperties["Handle"].Qualifiers, @"CimClass.CimClassProperties[Handle].Qualifiers returneded null");
                Assert.Equal((string)enumeratedClass.CimClassProperties["Handle"].Qualifiers["read"].Name, "read", @"CimClass.CimClassProperties[Handle].Qualifiers[Cim_Key].Name returneded null");
                Assert.Equal((CimType)enumeratedClass.CimClassProperties["Handle"].Qualifiers["read"].CimType, CimType.Boolean, @"CimClass.CimClassProperties[Handle].Qualifiers[read].Type returneded null");


                Assert.NotNull(enumeratedClass.CimClassProperties, "CimClass.CimClassProperties returneded null");
                Assert.NotNull(enumeratedClass.CimClassProperties["Handle"], @"CimClass.CimClassProperties[Handle] returneded null");
                Assert.Equal((string)enumeratedClass.CimClassProperties["Handle"].Name, "Handle", @"CimClass.CimClassProperties[Handle].Name returneded null");
                Assert.Equal((CimType)enumeratedClass.CimClassProperties["Handle"].CimType, CimType.String, @"CimClass.CimClassProperties[Handle].type returneded null");
                Assert.NotNull(enumeratedClass.CimClassProperties["Handle"].Qualifiers, @"CimClass.CimClassProperties[Handle].Qualifiers returneded null");
                Assert.Equal((string)enumeratedClass.CimClassProperties["Handle"].Qualifiers["read"].Name, "read", @"CimClass.CimClassProperties[Handle].Qualifiers[Cim_Key].Name returneded null");
                Assert.Equal((CimType)enumeratedClass.CimClassProperties["Handle"].Qualifiers["read"].CimType, CimType.Boolean, @"CimClass.CimClassProperties[Handle].Qualifiers[read].Type returneded null");



                // Assert.Equal(enumeratedClass.CimClassProperties["Handle"].Flags & CimFlags.Key, CimFlags.Key, @"CimClass.CimClassProperties[Handle].Flags returneded null");

                //Assert.Equal(enumeratedClass.CimClassProperties["Handle"].CimClassQualifiers["read"].Flags & CimFlags.DisableOverride, CimFlags.DisableOverride, @"CimClass.CimClassProperties[Handle].CimClassQualifiers[read].Flags returneded null");
                //Assert.Equal((bool)enumeratedClass.CimClassProperties["Handle"].CimClassQualifiers["read"].Value, true, @"CimClass.CimClassProperties[Handle].CimClassQualifiers[Cim_Key].Value returneded null");

                //Check class qualifiers
                Assert.NotNull(enumeratedClass.CimClassQualifiers, "CimClass.CimClassQualifiers returneded null");
                Assert.NotNull(enumeratedClass.CimClassQualifiers["Abstract"], "CimClass.CimClassQualifiers[Abstract] returneded null");
                Assert.Equal(enumeratedClass.CimClassQualifiers["Abstract"].Name, "Abstract", "CimClass.CimClassQualifiers[Abstract] returneded null");
                Assert.Equal((CimType)enumeratedClass.CimClassQualifiers["Abstract"].CimType, CimType.Boolean, @"enumeratedClass.CimClassQualifiers[Abstract].Type returneded null");

                //Check class qualifiers
                Assert.NotNull(enumeratedClass.CimClassQualifiers, "CimClass.CimClassQualifiers returneded null");
                Assert.NotNull(enumeratedClass.CimClassQualifiers["Abstract"], "CimClass.CimClassQualifiers[Abstract] returneded null");
                Assert.Equal(enumeratedClass.CimClassQualifiers["Abstract"].Name, "Abstract", "CimClass.CimClassQualifiers[Abstract] returneded null");
                Assert.Equal((CimType)enumeratedClass.CimClassQualifiers["Abstract"].CimType, CimType.Boolean, @"enumeratedClass.CimClassQualifiers[Abstract].Type returneded null");

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
                Assert.Equal((string)enumeratedClass.CimSystemProperties.ClassName, "Win32_Process", "CimClass.CimSystemProperties.ClassName returneded null");
                //Assert.Equal((string)enumeratedClass.CimSuperClassName, "Win32_Process", "CimClass.CimSuperClassName returneded null");
                Assert.Equal((string)enumeratedClass.CimSystemProperties.Namespace, @"root/cimv2", "CimClass.CimSystemProperties.Namespace returneded null");
                Assert.NotNull(enumeratedClass.CimClassProperties, "CimClass.CimClassProperties returneded null");
                Assert.NotNull(enumeratedClass.CimClassProperties["HandleCount"], @"CimClass.CimClassProperties[Handle] returneded null");

                Assert.Equal((string)enumeratedClass.CimClassProperties["HandleCount"].Name, "HandleCount", @"CimClass.CimClassProperties[Handle].Name returneded null");
                Assert.Equal((CimType)enumeratedClass.CimClassProperties["HandleCount"].CimType, CimType.UInt32, @"CimClass.CimClassProperties[HandleCount].type returneded null");
                // Assert.Equal(enumeratedClass.CimClassProperties["Handle"].Flags & CimFlags.Key, CimFlags.Key, @"CimClass.CimClassProperties[Handle].Flags returneded null");

                Assert.NotNull(enumeratedClass.CimClassProperties["HandleCount"].Qualifiers, @"CimClass.CimClassProperties[HandleCount].Qualifiers returneded null");
                Assert.Equal((string)enumeratedClass.CimClassProperties["HandleCount"].Qualifiers["read"].Name, "read", @"CimClass.CimClassProperties[HandleCount].Qualifiers[Cim_Key].Name returneded null");
                Assert.Equal((CimType)enumeratedClass.CimClassProperties["HandleCount"].Qualifiers["read"].CimType, CimType.Boolean, @"CimClass.CimClassProperties[HandleCount].Qualifiers[read].Type returneded null");
                // Assert.Equal(enumeratedClass.CimClassProperties["Handle"].CimClassQualifiers["read"].Flags & CimFlags.DisableOverride, CimFlags.DisableOverride, @"CimClass.CimClassProperties[Handle].CimClassQualifiers[read].Flags returneded null");
                //Assert.Equal((bool)enumeratedClass.CimClassProperties["Handle"].CimClassQualifiers["read"].Value, true, @"CimClass.CimClassProperties[Handle].CimClassQualifiers[Cim_Key].Value returneded null");

                //Check class qualifiers
                Assert.NotNull(enumeratedClass.CimClassQualifiers, "CimClass.CimClassQualifiers returneded null");
                Assert.NotNull(enumeratedClass.CimClassQualifiers["dynamic"], "CimClass.CimClassQualifiers[dynamic] returneded null");
                Assert.Equal(enumeratedClass.CimClassQualifiers["dynamic"].Name, "dynamic", "CimClass.CimClassQualifiers[dynamic] returneded null");
                Assert.Equal((CimType)enumeratedClass.CimClassQualifiers["dynamic"].CimType, CimType.Boolean, @"enumeratedClass.CimClassQualifiers[dynamic].Type returneded null");
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
                Assert.Equal(asyncList[0].Kind, AsyncItemKind.Item, "First callback for item");
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
                Assert.Equal((string)enumeratedClass.CimSystemProperties.ClassName, "Win32_Process", "CimClass.CimSystemProperties.ClassName returneded null");
                //Assert.Equal((string)enumeratedClass.CimSuperClassName, "Win32_Process", "CimClass.CimSuperClassName returneded null");
                Assert.Equal((string)enumeratedClass.CimSystemProperties.Namespace, @"root/cimv2", "CimClass.CimSystemProperties.Namespace returneded null");
                Assert.NotNull(enumeratedClass.CimClassProperties, "CimClass.CimClassProperties returneded null");
                Assert.NotNull(enumeratedClass.CimClassProperties["HandleCount"], @"CimClass.CimClassProperties[Handle] returneded null");

                Assert.Equal((string)enumeratedClass.CimClassProperties["HandleCount"].Name, "HandleCount", @"CimClass.CimClassProperties[Handle].Name returneded null");
                Assert.Equal((CimType)enumeratedClass.CimClassProperties["HandleCount"].CimType, CimType.UInt32, @"CimClass.CimClassProperties[HandleCount].type returneded null");
                // Assert.Equal(enumeratedClass.CimClassProperties["Handle"].Flags & CimFlags.Key, CimFlags.Key, @"CimClass.CimClassProperties[Handle].Flags returneded null");

                Assert.NotNull(enumeratedClass.CimClassProperties["HandleCount"].Qualifiers, @"CimClass.CimClassProperties[HandleCount].Qualifiers returneded null");
                Assert.Equal((string)enumeratedClass.CimClassProperties["HandleCount"].Qualifiers["read"].Name, "read", @"CimClass.CimClassProperties[HandleCount].Qualifiers[Cim_Key].Name returneded null");
                Assert.Equal((CimType)enumeratedClass.CimClassProperties["HandleCount"].Qualifiers["read"].CimType, CimType.Boolean, @"CimClass.CimClassProperties[HandleCount].Qualifiers[read].Type returneded null");
                // Assert.Equal(enumeratedClass.CimClassProperties["Handle"].CimClassQualifiers["read"].Flags & CimFlags.DisableOverride, CimFlags.DisableOverride, @"CimClass.CimClassProperties[Handle].CimClassQualifiers[read].Flags returneded null");
                //Assert.Equal((bool)enumeratedClass.CimClassProperties["Handle"].CimClassQualifiers["read"].Value, true, @"CimClass.CimClassProperties[Handle].CimClassQualifiers[Cim_Key].Value returneded null");

                //Check class qualifiers
                Assert.NotNull(enumeratedClass.CimClassQualifiers, "CimClass.CimClassQualifiers returneded null");
                Assert.NotNull(enumeratedClass.CimClassQualifiers["dynamic"], "CimClass.CimClassQualifiers[dynamic] returneded null");
                Assert.Equal(enumeratedClass.CimClassQualifiers["dynamic"].Name, "dynamic", "CimClass.CimClassQualifiers[dynamic] returneded null");
                Assert.Equal((CimType)enumeratedClass.CimClassQualifiers["dynamic"].CimType, CimType.Boolean, @"enumeratedClass.CimClassQualifiers[dynamic].Type returneded null");
                //Assert.Equal(enumeratedClass.CimClassQualifiers["Abstract"].Flags & CimFlags.DisableOverride, CimFlags.DisableOverride, @"enumeratedClass.CimClassQualifiers[Abstract].Flags returneded null");
                Assert.Equal(asyncList.Last().Kind, AsyncItemKind.Completion, "Got OnCompleted callback");

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
                Assert.Equal(asyncList.Count, 1, "Got 2 async callbacks");
                Assert.Equal(asyncList[0].Kind, AsyncItemKind.Completion, "First callback for completion");
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
                Assert.Equal(bResult, false, "TestConnection Failed");
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
                Assert.Equal(asyncList.Count, 1, "Got 2 async callbacks");
                Assert.Equal(asyncList[0].Kind, AsyncItemKind.Exception, "Got OnError callback");
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
                Assert.Equal(currentProcess.CimSystemProperties.ServerName, Environment.MachineName, "Got correct CimInstance.CimSystemProperties.ServerName");
                Assert.Equal(currentProcess.CimSystemProperties.ClassName, "Win32_Process", "Got correct CimInstance.CimSystemProperties.ClassName");
                Assert.True(Regex.IsMatch(currentProcess.CimSystemProperties.Namespace, "root.cimv2"), "Got correct CimInstance.CimSystemProperties.Namespace");

                Assert.Equal(currentProcess.CimInstanceProperties["Name"].Name, "Name", "currentProcess.CimInstanceProperties['Name'].Name is correct");
                Assert.Equal(currentProcess.CimInstanceProperties["Name"].CimType, CimType.String, "currentProcess.CimInstanceProperties['Name'].CimType is correct");
                Assert.Equal((string)(currentProcess.CimInstanceProperties["Name"].Value), "ttest.exe", "currentProcess.CimInstanceProperties['Name'].Value is correct");
                Assert.Equal(currentProcess.CimInstanceProperties["Name"].Flags, CimFlags.Property | CimFlags.ReadOnly | CimFlags.NotModified, "currentProcess.CimInstanceProperties['Name'].Flags is correct");

                Assert.Equal(currentProcess.CimInstanceProperties["Handle"].Flags & CimFlags.Key, CimFlags.Key, "currentProcess.CimInstanceProperties['Handle'].Flags includes CimFlags.Key");

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
                Assert.Equal(currentProcess.CimSystemProperties.ClassName, "Win32_Process", "Got correct CimInstance.CimSystemProperties.ClassName");

                using (CimClass cimClass = currentProcess.CimClass)
                {
                    Assert.NotNull(cimClass, "Got non-null cimClass");
                    Assert.Equal(cimClass.CimSystemProperties.ClassName, "Win32_Process", "Got correct CimClass.CimSystemProperties.ClassName");
                    Assert.NotNull(cimClass.CimSuperClass, "Got non-null parentClass");
                    Assert.Equal(cimClass.CimSuperClass.CimSystemProperties.ClassName, "CIM_Process", "Got correct CimClass.CimSuperClass.CimSystemProperties.ClassName");
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
                Assert.Equal(diskQuota.CimSystemProperties.ClassName, "Win32_DiskQuota", "Got correct CimInstance.CimSystemProperties.ClassName");

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
                Assert.Equal(listOfInstances.Last().Kind, AsyncItemKind.Completion, "Got OnCompleted callback");
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
                Assert.Equal(serializedResult[0].Kind, AsyncItemKind.Exception, "Got OnError callback");
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
                IObservable<CimInstance> enumeratedInstances = cimSession.EnumerateInstancesAsync(this.TestNamespace, "TestClass_AllDMTFTypes", operationOptions);

                var serializedResult = Helpers.ObservableToList(enumeratedInstances);
                Assert.True(serializedResult.Count > 0, "Got some results back from CimSession.EnumerateInstancesAsync");
                Assert.Equal(serializedResult.Last().Kind, AsyncItemKind.Completion, "Got OnCompleted callback");

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
                Assert.Equal(listOfInstances.Last().Kind, AsyncItemKind.Completion, "Got OnCompleted callback");

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
                Assert.Equal(listOfInstances.Last().Kind, AsyncItemKind.Completion, "Got OnCompleted callback");

                CimInstance currentProcess = listOfInstances
                    .Where(i => i.Kind == AsyncItemKind.Item)
                    .Select(i => i.Item)
                    .Single(x => (UInt32)(x.CimInstanceProperties["ProcessId"].Value) == Process.GetCurrentProcess().Id);

                Assert.NotNull(currentProcess, "cimSession.EnumerateInstances(..., Win32_Process) found the current process");

                Assert.Equal(currentProcess.CimInstanceProperties["Name"].Name, "Name", "currentProcess.CimInstanceProperties['Name'].Name is correct");
                Assert.Equal(currentProcess.CimInstanceProperties["Name"].CimType, CimType.String, "currentProcess.CimInstanceProperties['Name'].CimType is correct");
                Assert.Equal((string)(currentProcess.CimInstanceProperties["Name"].Value), "ttest.exe", "currentProcess.CimInstanceProperties['Name'].Value is correct");
                Assert.Equal(currentProcess.CimInstanceProperties["Name"].Flags, CimFlags.ReadOnly | CimFlags.Property | CimFlags.NotModified, "currentProcess.CimInstanceProperties['Name'].Flags is correct");

                Assert.Equal(currentProcess.CimInstanceProperties["Handle"].Flags & CimFlags.Key, CimFlags.Key, "currentProcess.CimInstanceProperties['Handle'].Flags includes CimFlags.Key");
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

                Assert.Equal(currentProcess.CimInstanceProperties["Name"].Name, "Name", "currentProcess.CimInstanceProperties['Name'].Name is correct");
                Assert.Equal(currentProcess.CimInstanceProperties["Name"].CimType, CimType.String, "currentProcess.CimInstanceProperties['Name'].CimType is correct");
                Assert.Equal((string)(currentProcess.CimInstanceProperties["Name"].Value), "ttest.exe", "currentProcess.CimInstanceProperties['Name'].Value is correct");
                Assert.Equal(currentProcess.CimInstanceProperties["Name"].Flags, CimFlags.Property | CimFlags.ReadOnly | CimFlags.NotModified, "currentProcess.CimInstanceProperties['Name'].Flags is correct");

                Assert.Equal(currentProcess.CimInstanceProperties["Handle"].Flags & CimFlags.Key, CimFlags.Key, "currentProcess.CimInstanceProperties['Handle'].Flags includes CimFlags.Key");

                Assert.True(currentProcess.CimInstanceProperties.Count > 1, "Got more than 1 property back from CimSession.GetInstance");

                Assert.Equal(currentProcess.CimInstanceProperties["Name"].Name, "Name", "currentProcess.CimInstanceProperties['Name'].Name is correct");
                Assert.Equal(currentProcess.CimInstanceProperties["Name"].CimType, CimType.String, "currentProcess.CimInstanceProperties['Name'].CimType is correct");
                Assert.Equal((string)(currentProcess.CimInstanceProperties["Name"].Value), "ttest.exe", "currentProcess.CimInstanceProperties['Name'].Value is correct");
                Assert.Equal(currentProcess.CimInstanceProperties["Name"].Flags, CimFlags.Property | CimFlags.ReadOnly | CimFlags.NotModified, "currentProcess.CimInstanceProperties['Name'].Flags is correct");

                Assert.Equal(currentProcess.CimInstanceProperties["Handle"].Flags & CimFlags.Key, CimFlags.Key, "currentProcess.CimInstanceProperties['Handle'].Flags includes CimFlags.Key");

                //System properties Environment.MachineName
                Assert.Equal(currentProcess.CimSystemProperties.Namespace, "root/cimv2", "cimInstance.CimSystemProperties.Namespace is not correct");
                Assert.Equal(currentProcess.CimSystemProperties.ClassName, "Win32_Process", "cimInstance.CimSystemProperties.ClassName is not correct");
                Assert.Equal(currentProcess.CimSystemProperties.ServerName, Environment.MachineName, "cimInstance.CimSystemProperties.ServerName is not correct");
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
                Assert.Equal(asyncList.Count, 2, "Got 2 async callbacks");
                Assert.Equal(asyncList[0].Kind, AsyncItemKind.Item, "First callback for item");
                Assert.Equal(asyncList[1].Kind, AsyncItemKind.Completion, "Second callback for completion");

                CimInstance currentProcess = asyncList[0].Item;
                Assert.NotNull(currentProcess, "cimSession.GetInstance returned something other than null");

                Assert.True(currentProcess.CimInstanceProperties.Count > 1, "Got more than 1 property back from CimSession.GetInstance");

                Assert.Equal(currentProcess.CimInstanceProperties["Name"].Name, "Name", "currentProcess.CimInstanceProperties['Name'].Name is correct");
                Assert.Equal(currentProcess.CimInstanceProperties["Name"].CimType, CimType.String, "currentProcess.CimInstanceProperties['Name'].CimType is correct");
                Assert.Equal((string)(currentProcess.CimInstanceProperties["Name"].Value), "ttest.exe", "currentProcess.CimInstanceProperties['Name'].Value is correct");
                Assert.Equal(currentProcess.CimInstanceProperties["Name"].Flags, CimFlags.Property | CimFlags.ReadOnly | CimFlags.NotModified, "currentProcess.CimInstanceProperties['Name'].Flags is correct");

                Assert.Equal(currentProcess.CimInstanceProperties["Handle"].Flags & CimFlags.Key, CimFlags.Key, "currentProcess.CimInstanceProperties['Handle'].Flags includes CimFlags.Key");
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

                Assert.Equal(currentProcess.CimInstanceProperties["Name"].Name, "Name", "currentProcess.CimInstanceProperties['Name'].Name is correct");
                Assert.Equal(currentProcess.CimInstanceProperties["Name"].CimType, CimType.String, "currentProcess.CimInstanceProperties['Name'].CimType is correct");
                Assert.Equal((string)(currentProcess.CimInstanceProperties["Name"].Value), "ttest.exe", "currentProcess.CimInstanceProperties['Name'].Value is correct");
                Assert.Equal(currentProcess.CimInstanceProperties["Name"].Flags, CimFlags.Property | CimFlags.ReadOnly | CimFlags.NotModified, "currentProcess.CimInstanceProperties['Name'].Flags is correct");

                Assert.Equal(currentProcess.CimInstanceProperties["Handle"].Flags & CimFlags.Key, CimFlags.Key, "currentProcess.CimInstanceProperties['Handle'].Flags includes CimFlags.Key");
            }
        }

        [Fact]
        public void Impersonated_QueryInstances_EnumeratorDisposedWithDifferentIdentityToken()
        {
            using (CimSession cimSession = CimSession.Create(null))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");

                string queryExpression = string.Format(
                    CultureInfo.InvariantCulture,
                    "SELECT * FROM Win32_Process");

                Helpers.AssertRunningAsNonTestUser("Before calling CimSession.QueryInstances");
                IEnumerable<CimInstance> enumeratedInstances = cimSession.QueryInstances(@"root\cimv2", "WQL", queryExpression);
                IEnumerator<CimInstance> enumerator = enumeratedInstances.GetEnumerator();
                enumerator.MoveNext();
                using (Helpers.ImpersonateTestUser())
                {
                    Helpers.AssertRunningAsTestUser("Right before calling IEnumerator.Dispose");
                    enumerator.Dispose(); // per .NET guidelines, Dispose should not throw
                    /*
                		You should not throw exceptions from within Dispose except under critical
                		situations. If executing a Dispose(bool disposing) method, never throw an
                		exception if disposing is false; doing so will terminate the process if
                		executing inside a finalizer context.
                     */
                }
                Helpers.AssertRunningAsNonTestUser("After the test");
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
        //will be merged into MMI
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
                            Assert.Equal(superClass.CimSystemProperties.ClassName, "CIM_Process", "Got correct CimClass.CimSuperClass.CimSystemProperties.ClassName");
                        }
                    }

                    Assert.Equal(currentProcess.CimInstanceProperties["Name"].Name, "Name", "currentProcess.CimInstanceProperties['Name'].Name is correct");
                    Assert.Equal(currentProcess.CimInstanceProperties["Name"].CimType, CimType.String, "currentProcess.CimInstanceProperties['Name'].CimType is correct");
                    Assert.Equal((string)(currentProcess.CimInstanceProperties["Name"].Value), "ttest.exe", "currentProcess.CimInstanceProperties['Name'].Value is correct");
                    Assert.Equal(currentProcess.CimInstanceProperties["Name"].Flags, CimFlags.Property | CimFlags.ReadOnly | CimFlags.NotModified, "currentProcess.CimInstanceProperties['Name'].Flags is correct");
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

        public void Impersonated_QueryInstancesAsync_Impersonatation_Core(bool threadPool, CimSessionOptions sessionOptions)
        {
            string queryExpression = string.Format(
                CultureInfo.InvariantCulture,
                "SELECT * FROM Win32_Process WHERE ProcessId = {0}",
                Process.GetCurrentProcess().Id);

            CimSession cimSession = null;
            ImpersonationObserver observer = null;
            try
            {
                Helpers.AssertRunningAsNonTestUser("Before impersonation / main test thread");
                using (Helpers.ImpersonateTestUser())
                {
                    Helpers.AssertRunningAsTestUser("After successful impersonation / main test thread");
                    cimSession = CimSession.Create(null, sessionOptions);
                    {
                        observer = new ImpersonationObserver(cimSession, queryExpression, threadPool);
                        cimSession.QueryInstancesAsync("root/cimv2", "WQL", queryExpression).Subscribe(observer);
                    }
                }
                Helpers.AssertRunningAsNonTestUser("After reverting impersonation / main test thread");
                observer.WaitForEndOfTest();
            }
            finally
            {
                if (cimSession != null)
                {
                    cimSession.Dispose();
                }
            }
        }

        [Fact]
        public void Impersonated_QueryInstancesAsync_Impersonatation_NewOperationFromCallback_WSMan()
        {
            Impersonated_QueryInstancesAsync_Impersonatation_Core(threadPool: false, sessionOptions: new WSManSessionOptions());
        }

        [Fact]
        public void Impersonated_QueryInstancesAsync_Impersonatation_NewOperationFromThreadPool_WSMan()
        {
            Impersonated_QueryInstancesAsync_Impersonatation_Core(threadPool: true, sessionOptions: new WSManSessionOptions());
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
                Assert.Equal(asyncList.Count, 2, "Got 2 async callbacks");
                Assert.Equal(asyncList[0].Kind, AsyncItemKind.Item, "First callback for item");
                Assert.Equal(asyncList[1].Kind, AsyncItemKind.Completion, "Second callback for completion");

                CimInstance currentProcess = asyncList[0].Item;
                Assert.NotNull(currentProcess, "cimSession.GetInstance returned something other than null");

                Assert.True(currentProcess.CimInstanceProperties.Count > 1, "Got more than 1 property back from CimSession.GetInstance");

                Assert.Equal(currentProcess.CimInstanceProperties["Name"].Name, "Name", "currentProcess.CimInstanceProperties['Name'].Name is correct");
                Assert.Equal(currentProcess.CimInstanceProperties["Name"].CimType, CimType.String, "currentProcess.CimInstanceProperties['Name'].CimType is correct");
                Assert.Equal((string)(currentProcess.CimInstanceProperties["Name"].Value), "ttest.exe", "currentProcess.CimInstanceProperties['Name'].Value is correct");
                Assert.Equal(currentProcess.CimInstanceProperties["Name"].Flags, CimFlags.Property | CimFlags.ReadOnly | CimFlags.NotModified, "currentProcess.CimInstanceProperties['Name'].Flags is correct");

                Assert.Equal(currentProcess.CimInstanceProperties["Handle"].Flags & CimFlags.Key, CimFlags.Key, "currentProcess.CimInstanceProperties['Handle'].Flags includes CimFlags.Key");
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
                Assert.Equal(unraveledObservable.Last().Kind, AsyncItemKind.Completion, "Got OnCompleted callback");

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
                    e => Assert.Equal(e.ParamName, "instance", "Got correct ArgumentNullException.ParamName"));
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
                    e => Assert.Equal(e.ParamName, "instance", "Got correct ArgumentNullException.ParamName"));
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
                    e => Assert.Equal(e.ParamName, "instance", "Got correct ArgumentNullException.ParamName"));
            }
        }

        public void InvokeInstanceMethod_SetSint32Value()
        {
            using (var cimSession = CimSession.Create(null))
            {
                var cimInstance = new CimInstance("TestClass_AllDMTFTypes");
                cimInstance.CimInstanceProperties.Add(CimProperty.Create("v_Key", 123, CimType.UInt64, CimFlags.Key));
                cimInstance.CimInstanceProperties.Add(CimProperty.Create("v_sint32", 456, CimType.SInt32, CimFlags.None));

                cimSession.CreateInstance(this.TestNamespace, cimInstance);
                this.AssertPresenceOfTestInstance(123, 456);

                CimMethodResult result;
                using (var methodParameters = new CimMethodParametersCollection())
                {
                    Assert.Equal(methodParameters.Count, 0, "methodParameters.Count is correct");
                    Assert.Equal(methodParameters.Count(), 0, "methodParameters.Count is correct");
                    methodParameters.Add(CimMethodParameter.Create("Sint32Val", 789, CimType.SInt32, CimFlags.None));
                    Assert.Equal(methodParameters.Count, 1, "methodParameters.Count is correct");
                    Assert.Equal(methodParameters.Count(), 1, "methodParameters.Count is correct");
                    result = cimSession.InvokeMethod(this.TestNamespace, cimInstance, "SetSint32Value", methodParameters);
                }
                Assert.NotNull(result, "CimSession.InvokeMethod returned non-null");
                Assert.Equal(result.ReturnValue.CimType, CimType.UInt32, "Got the right type of return value");
                object returnValue = result.ReturnValue.Value;
                Assert.Equal((UInt32)returnValue, (uint)0, "Got the right return value");
                this.AssertPresenceOfTestInstance(123, 789);
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
                CimMethodResult result = cimSession.InvokeMethod(this.TestNamespace, "TestClass_MethodProvider_Calc", "Add", methodParameters);

                Assert.NotNull(result, "CimSession.InvokeMethod returned non-null");
                Assert.Equal(result.ReturnValue.CimType, CimType.UInt64, "Got the right type of return value");
                object returnValue = result.ReturnValue.Value;
                Assert.Equal((UInt64)returnValue, 0, "Got the right return value");

                Assert.NotNull(result.OutParameters["sum"], "CimSession.InvokeMethod returned out parameter");
                Assert.Equal(result.OutParameters["sum"].CimType, CimType.SInt64, "CimSession.InvokeMethod returned right type of out parameter");
                object outParameterValue = result.OutParameters["sum"].Value;
                Assert.Equal((Int64)outParameterValue, 123 + 456, "Got the right out parameter value");
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
                    () => cimSession.InvokeMethod(this.TestNamespace, "TestClass_MethodProvider_Calc", "Add", methodParameters),
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
                CimMethodResult result = cimSession.InvokeMethod(this.TestNamespace, "TestClass_Streaming", "StreamNumbers", methodParameters);

                Assert.NotNull(result, "CimSession.InvokeMethod returned non-null");
                Assert.Equal(result.ReturnValue.CimType, CimType.UInt32, "Got the right type of return value");
                object returnValue = result.ReturnValue.Value;
                Assert.Equal((UInt32)returnValue, (uint)0, "Got the right return value");

                Assert.NotNull(result.OutParameters["firstTen"], "CimSession.InvokeMethod returned out parameter");
                Assert.Equal(result.OutParameters["firstTen"].CimType, CimType.InstanceArray, "CimSession.InvokeMethod returned right type of out parameter");
                CimInstance[] outParameterValue = result.OutParameters["firstTen"].Value as CimInstance[];
                Assert.True(outParameterValue.All(v => v.CimSystemProperties.ClassName.Equals("Numbers", StringComparison.OrdinalIgnoreCase)), "Results have the right class name");
                Assert.True(outParameterValue.All(v => v.CimInstanceProperties["Numbers"] != null), "Results have 'Numbers' property");
                Assert.True(outParameterValue.All(v => v.CimInstanceProperties["Numbers"].CimType == CimType.SInt64Array), "Results have 'Numbers' property with correct type");

                Assert.Equal(((long[])(outParameterValue[0].CimInstanceProperties["Numbers"].Value))[9], 10, "1st result is correct");
                Assert.Equal(((long[])(outParameterValue[1].CimInstanceProperties["Numbers"].Value))[9], 20, "2nd result is correct");
                Assert.Equal(((long[])(outParameterValue[2].CimInstanceProperties["Numbers"].Value))[9], 30, "3rd result is correct");
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
                IObservable<CimMethodResultBase> observable = cimSession.InvokeMethodAsync(this.TestNamespace, "TestClass_Streaming", "StreamNumbers", methodParameters, operationOptions);
                Assert.NotNull(observable, "CimSession.InvokeMethod returned non-null");

                List<AsyncItem<CimMethodResultBase>> result = Helpers.ObservableToList(observable);
                Assert.True(result.Count > 0, "Got some callbacks");
                Assert.Equal(result[result.Count - 1].Kind, AsyncItemKind.Completion, "Got completion callback");

                Assert.True(result.Count > 1, "Got more than 1 callback");
                Assert.Equal(result[result.Count - 2].Kind, AsyncItemKind.Item, "Got non-streamed result (presence)");
                Assert.True(result[result.Count - 2].Item.GetType().Equals(typeof(CimMethodResult)), "Got non-streamed result (type)");

                Assert.True(result[0].Item.GetType().Equals(typeof(CimMethodStreamedResult)), "Got streamed result");
                Assert.Equal(((CimMethodStreamedResult)(result[0].Item)).ParameterName, "firstTen", "Streamed result has correct parameter name");
                Assert.Equal(((CimMethodStreamedResult)(result[0].Item)).ItemType, CimType.Instance, "Streamed result has correct type of item");

                CimInstance item = (CimInstance)((CimMethodStreamedResult)(result[0].Item)).ItemValue;
                Assert.NotNull(item.CimInstanceProperties["Numbers"], "Streamed result has 'Numbers' property (1)");
                Assert.Equal(item.CimInstanceProperties["Numbers"].CimType, CimType.SInt64Array, "Streamed result has 'Numbers' property of the correct type (1)");
                Assert.Equal(((long[])(item.CimInstanceProperties["Numbers"].Value))[9], 10, "Streamed result has 'Numbers' property with right value (1)");

                item = (CimInstance)((CimMethodStreamedResult)(result[1].Item)).ItemValue;
                Assert.NotNull(item.CimInstanceProperties["Numbers"], "Streamed result has 'Numbers' property (2)");
                Assert.Equal(item.CimInstanceProperties["Numbers"].CimType, CimType.SInt64Array, "Streamed result has 'Numbers' property of the correct type (2)");
                Assert.Equal(((long[])(item.CimInstanceProperties["Numbers"].Value))[9], 20, "Streamed result has 'Numbers' property with right value (2)");
            }
        }

        [Fact]
        public void InvokeStreamingMethod_Async_ServerSideStreamingOnly()
        {
            using (var cimSession = CimSession.Create(null))
            {
                var methodParameters = new CimMethodParametersCollection();
                methodParameters.Add(CimMethodParameter.Create("count", 3, CimType.UInt32, CimFlags.None));
                IObservable<CimMethodResult> observable = cimSession.InvokeMethodAsync(this.TestNamespace, "TestClass_Streaming", "StreamNumbers", methodParameters);
                Assert.NotNull(observable, "CimSession.InvokeMethod returned non-null");

                List<AsyncItem<CimMethodResult>> asyncResult = Helpers.ObservableToList(observable);
                Assert.True(asyncResult.Count > 0, "Got some callbacks");
                Assert.Equal(asyncResult[asyncResult.Count - 1].Kind, AsyncItemKind.Completion, "Got completion callback");

                Assert.True(asyncResult.Count == 2, "Got exactly one asyncResult");
                Assert.Equal(asyncResult[asyncResult.Count - 2].Kind, AsyncItemKind.Item, "Got non-streamed asyncResult (presence)");
                Assert.True(asyncResult[asyncResult.Count - 2].Item.GetType().Equals(typeof(CimMethodResult)), "Got non-streamed asyncResult (type)");

                CimMethodResult result = asyncResult[asyncResult.Count - 2].Item;

                Assert.NotNull(result, "CimSession.InvokeMethod returned non-null");
                Assert.Equal(result.ReturnValue.CimType, CimType.UInt32, "Got the right type of return value");
                object returnValue = result.ReturnValue.Value;
                Assert.Equal((UInt32)returnValue, (uint)0, "Got the right return value");

                Assert.NotNull(result.OutParameters["firstTen"], "CimSession.InvokeMethod returned out parameter");
                Assert.Equal(result.OutParameters["firstTen"].CimType, CimType.InstanceArray, "CimSession.InvokeMethod returned right type of out parameter");
                CimInstance[] outParameterValue = result.OutParameters["firstTen"].Value as CimInstance[];
                Assert.True(outParameterValue.All(v => v.CimSystemProperties.ClassName.Equals("Numbers", StringComparison.OrdinalIgnoreCase)), "Results have the right class name");
                Assert.True(outParameterValue.All(v => v.CimInstanceProperties["Numbers"] != null), "Results have 'Numbers' property");
                Assert.True(outParameterValue.All(v => v.CimInstanceProperties["Numbers"].CimType == CimType.SInt64Array), "Results have 'Numbers' property with correct type");

                Assert.Equal(((long[])(outParameterValue[0].CimInstanceProperties["Numbers"].Value))[9], 10, "1st result is correct");
                Assert.Equal(((long[])(outParameterValue[1].CimInstanceProperties["Numbers"].Value))[9], 20, "2nd result is correct");
                Assert.Equal(((long[])(outParameterValue[2].CimInstanceProperties["Numbers"].Value))[9], 30, "3rd result is correct");
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
                    e => Assert.Equal(e.ParamName, "instance", "Got correct ArgumentNullException.ParamName"));
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
                    e => Assert.Equal(e.ParamName, "instance", "Got correct ArgumentNullException.ParamName"));
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
                    e => Assert.Equal(e.ParamName, "instance", "Got correct A4rgumentNullException.ParamName"));
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
                StartDummyProcess();

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
                    StartDummyProcess();

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
                            StartDummyProcess();

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


        //=================















        // Todo: will add more test cases
        #endregion Test create

        [Fact]
        public void BaseOptions_Empty()
        {
            var sessionOptions = new CimSessionOptions();
            using (CimSession cimSession = CimSession.Create(null, sessionOptions))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");
            }
        }

        [Fact]
        public void WSManOptions_SetDestinationPort()
        {
            // TODO/FIXME - add unit test for corner cases (0, > 65535)

            var sessionOptions = new WSManSessionOptions();
            sessionOptions.DestinationPort = (uint)8080;
            using (CimSession cimSession = CimSession.Create(null, sessionOptions))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");
            }
        }

        [Fact]
        public void WSManOptions_SetProxyCredential()
        {
            var sessionOptions = new WSManSessionOptions();
            //sessionOptions.DestinationPort = 8080;
            CimCredential cred = new CimCredential(ImpersonatedAuthenticationMechanism.None); //wsman accepts only username/password
            sessionOptions.AddProxyCredentials(cred);
            //Exception is thrown after creating the session as WSMAN doesn't allow proxy without username/password.
            using (CimSession cimSession = CimSession.Create(null, sessionOptions))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");
            }
        }

    }
}