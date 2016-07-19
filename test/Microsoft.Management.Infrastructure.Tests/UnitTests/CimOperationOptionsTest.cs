/*============================================================================
* Copyright (C) Microsoft Corporation, All rights reserved.
*=============================================================================
*/

using System;
using System.Globalization;
using System.Security;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Management.Infrastructure;
using Microsoft.Management.Infrastructure.Options;
using Xunit;

namespace MMI.Tests.UnitTests
{

    public class CimOperationOptionsTest
    {
        [TDDFact]
        public void BaseOptions_Empty()
        {
            using (var operationOptions = new CimOperationOptions(mustUnderstand: true))
            using (CimSession cimSession = CimSession.Create(null))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");
                IEnumerable<CimInstance> enumeratedInstances = cimSession.EnumerateInstances(@"root\cimv2", "Win32_Process", operationOptions);
                Assert.NotNull(enumeratedInstances, "cimSession.EnumerateInstances returned something other than null");
                Assert.True(enumeratedInstances.Count() > 0, "Got some results back from CimSession.EnumerateInstances");
            }
        }

        [TDDFact]
        public void BaseOptions_Timeout_Medium()
        {
            var operationOptions = new CimOperationOptions(mustUnderstand: true);
            operationOptions.Timeout = TimeSpan.FromSeconds(123);
            using (CimSession cimSession = CimSession.Create(null))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");
                IEnumerable<CimInstance> enumeratedInstances = cimSession.EnumerateInstances(@"root\cimv2", "Win32_Process", operationOptions);
                Assert.NotNull(enumeratedInstances, "cimSession.EnumerateInstances returned something other than null");
                Assert.True(enumeratedInstances.Count() > 0, "Got some results back from CimSession.EnumerateInstances");
            }
        }

        [TDDFact]
        public void BaseOptions_Timeout_Small()
        {
            var operationOptions = new CimOperationOptions(mustUnderstand: true);
            operationOptions.Timeout = TimeSpan.FromMilliseconds(1);
            Assert.Throws<CimException>(() =>
            {
                using (CimSession cimSession = CimSession.Create(null))
                {
                    Assert.NotNull(cimSession, "cimSession should not be null");
                    IEnumerable<CimInstance> enumeratedInstances = cimSession.EnumerateInstances(@"root\cimv2", "Win32_Process", operationOptions);
                    Assert.NotNull(enumeratedInstances, "cimSession.EnumerateInstances returned something other than null");
                    enumeratedInstances.Count();
                }
                return null;
            });
        }

        [TDDFact]
        public void BaseOptions_Timeout_Infinity()
        {
            var operationOptions = new CimOperationOptions(mustUnderstand: true);
            operationOptions.Timeout = TimeSpan.MaxValue;
            using (CimSession cimSession = CimSession.Create(null))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");
                IEnumerable<CimInstance> enumeratedInstances = cimSession.EnumerateInstances(@"root\cimv2", "Win32_Process", operationOptions);
                Assert.NotNull(enumeratedInstances, "cimSession.EnumerateInstances returned something other than null");
                Assert.True(enumeratedInstances.Count() > 0, "Got some results back from CimSession.EnumerateInstances");
            }
        }

        [TDDFact]
        public void BaseOptions_UseMachineId_True()
        {
            var operationOptions = new CimOperationOptions(mustUnderstand: true);
            operationOptions.UseMachineId = true;
            using (CimSession cimSession = CimSession.Create(null))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");
                IEnumerable<CimInstance> enumeratedInstances = cimSession.EnumerateInstances(@"root\cimv2", "Win32_Process", operationOptions);
                Assert.NotNull(enumeratedInstances, "cimSession.EnumerateInstances returned something other than null");
                Assert.True(enumeratedInstances.Count() > 0, "Got some results back from CimSession.EnumerateInstances");
            }
        }

        [TDDFact]
        public void BaseOptions_UseMachineId_False()
        {
            var operationOptions = new CimOperationOptions(mustUnderstand: true);
            operationOptions.UseMachineId = false;
            using (CimSession cimSession = CimSession.Create(null))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");
                IEnumerable<CimInstance> enumeratedInstances = cimSession.EnumerateInstances(@"root\cimv2", "Win32_Process", operationOptions);
                Assert.NotNull(enumeratedInstances, "cimSession.EnumerateInstances returned something other than null");
                Assert.True(enumeratedInstances.Count() > 0, "Got some results back from CimSession.EnumerateInstances");
            }
        }

        [TDDFact]
        public void BaseOptions_ResourceUriPrefix()
        {
            var operationOptions = new CimOperationOptions(mustUnderstand: true);
            operationOptions.ResourceUriPrefix = new Uri("http://schemas.microsoft.com/wbem/wsman/1/wmi/root/cimv2");

            var sessionOptions = new WSManSessionOptions();
            using (CimSession cimSession = CimSession.Create(null, sessionOptions))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");
                IEnumerable<CimInstance> enumeratedInstances = cimSession.EnumerateInstances(@"root\cimv2", "Win32_Process", operationOptions);
                Assert.NotNull(enumeratedInstances, "cimSession.EnumerateInstances returned something other than null");
                Assert.True(enumeratedInstances.Count() > 0, "Got some results back from CimSession.EnumerateInstances");
            }
        }

        [Fact]
        public void BaseOptions_ResourceUriPrefix_Null()
        {
            var operationOptions = new CimOperationOptions(mustUnderstand: true);
            Assert.Throws<ArgumentNullException>(() =>
            {
                return operationOptions.ResourceUriPrefix = null;
            });

        }

        [TDDFact]
        public void BaseOptions_CustomOptionString_MustUnderstandFalse()
        {
            var operationOptions = new CimOperationOptions(mustUnderstand: false);
            operationOptions.SetCustomOption("MyOptionName", "MyOptionValue", false);

            var sessionOptions = new WSManSessionOptions();
            using (CimSession cimSession = CimSession.Create(null, sessionOptions))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");
                IEnumerable<CimInstance> enumeratedInstances = cimSession.EnumerateInstances(@"root\cimv2", "Win32_Process", operationOptions);
                Assert.NotNull(enumeratedInstances, "cimSession.EnumerateInstances returned something other than null");
                Assert.True(enumeratedInstances.Count() > 0, "Got some results back from CimSession.EnumerateInstances");
            }
        }

        [TDDFact]
        public void BaseOptions_CustomOptionString_MustUnderstandTrue()
        {
            var operationOptions = new CimOperationOptions(mustUnderstand: true);
            operationOptions.SetCustomOption("MyOptionName", "MyOptionValue", false);

            var sessionOptions = new WSManSessionOptions();
            using (CimSession cimSession = CimSession.Create(null, sessionOptions))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");
                IEnumerable<CimInstance> enumeratedInstances = cimSession.EnumerateInstances(@"root\cimv2", "Win32_Process", operationOptions);
                Assert.NotNull(enumeratedInstances, "cimSession.EnumerateInstances returned something other than null");
                Assert.True(enumeratedInstances.Count() > 0, "Got some results back from CimSession.EnumerateInstances");
            }
        }

        [Fact]
        public void BaseOptions_CustomOptionString_OptionName_Null()
        {
            var operationOptions = new CimOperationOptions(mustUnderstand: false);
            Assert.Throws<ArgumentNullException>(() =>
            {
                operationOptions.SetCustomOption(null, "MyOptionValue", false);
                return null;
            });
        }

        [TDDFact]
        public void BaseOptions_CustomOption_MismatchedValueAndType()
        {
            var operationOptions = new CimOperationOptions(mustUnderstand: false);
            Assert.Throws<ArgumentException>(() =>
            {
                operationOptions.SetCustomOption("MyOptionName", 123, CimType.String, false);
                return null;
            });
        }

        [TDDFact]
        public void BaseOptions_CustomOption_UnsupportedType()
        {
            var operationOptions = new CimOperationOptions(mustUnderstand: false);
            Assert.Throws<CimException>(() =>
            {
                var cimInstance = new CimInstance("MyClassName");
                operationOptions.SetCustomOption("MyOptionName", cimInstance, CimType.Instance, false);
                return null;
            });
        }

        [TDDFact]
        public void BaseOptions_CustomOption_RoundtripString()
        {
            using (var cimSession = CimSession.Create(null))
            {
                var cimInstance = cimSession.EnumerateInstances("this.TestNamespace", "TestClass_AllDMTFTypes").First();
                Assert.NotNull(cimInstance, "Sanity check - got an instance to act on");

                var operationOptions = new CimOperationOptions(mustUnderstand: false);
                operationOptions.SetCustomOption("MyOptionName", "MyOptionValue", CimType.String, mustComply: false);

                CimMethodParametersCollection methodParameters = new CimMethodParametersCollection();
                methodParameters.Add(CimMethodParameter.Create("optionName", "MyOptionName", CimType.String, CimFlags.None));
                CimMethodResult result = cimSession.InvokeMethod("this.TestNamespace", cimInstance, "GetStringCustomOption", methodParameters, operationOptions);
                Assert.NotNull(result, "CimSession.InvokeMethod returned non-null");

                Assert.Equal(result.ReturnValue.CimType, CimType.UInt32, "Got the right type of return value");
                object returnValue = result.ReturnValue.Value;
                Assert.Equal((UInt32)returnValue, (uint)0, "Got the right return value");

                CimMethodParameter optionValueParameter = result.OutParameters["optionValue"];
                Assert.NotNull(optionValueParameter, "Got the out parameter");
                Assert.Equal(optionValueParameter.CimType, CimType.String, "Got the right CIM type of out parameter");
                Assert.True(optionValueParameter.Value is string, "Got the right .NET type of out parameter");
                Assert.Equal((string)(optionValueParameter.Value), "MyOptionValue", "Got the right value of out parameter");
            }
        }

        [Fact]
        public void BaseOptions_CustomOption_OptionName_Null()
        {
            var operationOptions = new CimOperationOptions(mustUnderstand: false);
            Assert.Throws<ArgumentNullException>(() =>
            {
                operationOptions.SetCustomOption(null, 123, CimType.UInt8, false);
                return null;
            });
        }

        [Fact]
        public void BaseOptions_RawOptionString()
        {
            var operationOptions = new CimOperationOptions(mustUnderstand: false);
            operationOptions.SetOption("MyOptionName", "MyOptionValue");
        }

        [Fact]
        public void BaseOptions_RawOptionUInt32()
        {
            var operationOptions = new CimOperationOptions(mustUnderstand: false);
            operationOptions.SetOption("MyOptionName", 123);
        }

        [Fact]
        public void BaseOptions_RawOptionString_OptionName_Null()
        {
            var operationOptions = new CimOperationOptions(mustUnderstand: false);
            Assert.Throws<ArgumentNullException>(() =>
            {
                operationOptions.SetOption(null, "MyOptionValue");
                return null;
            });
        }

        [Fact]
        public void BaseOptions_RawOptionUInt32_OptionName_Null()
        {
            var operationOptions = new CimOperationOptions(mustUnderstand: false);
            Assert.Throws<ArgumentNullException>(() =>
            {
                operationOptions.SetOption(null, 123);
                return null;
            });
        }

        [TDDFact]
        public void BaseOptions_KeysOnly_True()
        {
            var operationOptions = new CimOperationOptions { KeysOnly = true };
            Assert.Equal(operationOptions.KeysOnly, true, "KeysOnly round-trips properly");
            using (CimSession cimSession = CimSession.Create(null))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");
                IEnumerable<CimInstance> enumeratedInstances = cimSession.EnumerateInstances(@"root\cimv2", "Win32_Process", operationOptions);
                Assert.NotNull(enumeratedInstances, "cimSession.EnumerateInstances returned something other than null");

                CimInstance cimInstance = enumeratedInstances.ToList().First();
                Assert.NotNull(cimInstance, "cimSession.EnumerateInstances returned results other than null");
                List<CimProperty> allProperties = cimInstance.CimInstanceProperties.ToList();

                Assert.Equal(allProperties.Where(p => p.Value != null).Count(), 1, "Only key properties should get values");
                Assert.True(allProperties.Where(p => p.Value != null).All(p => CimFlags.Key == (p.Flags & CimFlags.Key)), "keyProperty.Flags includes CimFlags.Key");
                Assert.True(allProperties.Where(p => p.Value != null).All(p => p.Name.Equals("Handle", StringComparison.OrdinalIgnoreCase)), "keyProperty.Name equals 'Handle' for Win32_Process");
            }
        }

        [TDDFact]
        public void BaseOptions_KeysOnly_False()
        {
            var operationOptions = new CimOperationOptions { KeysOnly = false };
            Assert.Equal(operationOptions.KeysOnly, false, "KeysOnly round-trips properly");
            using (CimSession cimSession = CimSession.Create(null))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");
                IEnumerable<CimInstance> enumeratedInstances = cimSession.EnumerateInstances(@"root\cimv2", "Win32_Process", operationOptions);
                Assert.NotNull(enumeratedInstances, "cimSession.EnumerateInstances returned something other than null");

                CimInstance cimInstance = enumeratedInstances.ToList().First();
                Assert.NotNull(cimInstance, "cimSession.EnumerateInstances returned results other than null");
                List<CimProperty> allProperties = cimInstance.CimInstanceProperties.ToList();

                Assert.True(allProperties.Count > 1, "More than 1 property of Win32_Process is returned");
                Assert.True(allProperties.Any(property => CimFlags.Key != (property.Flags & CimFlags.Key)), "Some of the returned properties are non-key");
                Assert.True(allProperties.Any(property => CimFlags.Key == (property.Flags & CimFlags.Key)), "Some of the returned properties are key");
            }
        }

        [TDDFact]
        public void BaseOptions_ShortenLifetimeOfResults_Sync()
        {
            var operationOptions = new CimOperationOptions { ShortenLifetimeOfResults = true };

            using (CimSession cimSession = CimSession.Create(null))
            {
                Assert.NotNull(cimSession, "cimSession should not be null");
                IEnumerable<CimInstance> enumeratedInstances = cimSession.EnumerateInstances(@"root\cimv2", "Win32_Process", operationOptions);
                Assert.NotNull(enumeratedInstances, "cimSession.EnumerateInstances returned something other than null");

                CimInstance cimInstance = enumeratedInstances.ToList().First();
                Assert.NotNull(cimInstance, "cimSession.EnumerateInstances returned results other than null");

                Assert.Throws<ObjectDisposedException>(() => { return cimInstance.CimSystemProperties.ClassName; });
            }
        }
        /*
                [Fact]
                public void BaseOptions_ShortenLifetimeOfResults_Async_EnumInstances()
                {
                    var operationOptions = new CimOperationOptions { ShortenLifetimeOfResults = true };

                    using (CimSession cimSession = CimSession.Create(null))
                    {
                        Assert.NotNull(cimSession, "cimSession should not be null");
                        IObservable<CimInstance> enumeratedInstances = cimSession.EnumerateInstancesAsync(@"root\cimv2", "Win32_Process", operationOptions);
                        Assert.NotNull(enumeratedInstances, "cimSession.EnumerateInstances returned something other than null");

                        List<AsyncItem<CimInstance>> serializedResults = Helpers.ObservableToList(enumeratedInstances);
                        Assert.NotNull(serializedResults, "cimSession.EnumerateInstances returned results other than null");
                        Assert.True(serializedResults.Count >= 2, "cimSession.EnumerateInstances returned some results");
                        Assert.Equal(serializedResults[0].Kind, AsyncItemKind.Item, "cimSession.EnumerateInstances returned an actual CimInstance");
                        Assert.Throws<ObjectDisposedException>(() => 
                        {
                            for (int i = 0; i < 10; i++)
                            {
                                var className = serializedResults[0].Item.CimSystemProperties.ClassName;
                                Thread.Sleep(200);
                            }
                            return null;
                        });
                    }
                }

                [Fact]
                public void BaseOptions_ShortenLifetimeOfResults_Async_MethodRegular()
                {
                    using (var cimSession = CimSession.Create(null))
                    {
                        var operationOptions = new CimOperationOptions { ShortenLifetimeOfResults = true, EnableMethodResultStreaming = false };

                        var methodParameters = new CimMethodParametersCollection();
                        methodParameters.Add(CimMethodParameter.Create("count", 3, CimType.UInt32, CimFlags.None));
                        IObservable<CimMethodResultBase> observable = cimSession.InvokeMethodAsync("this.TestNamespace", "TestClass_Streaming", "StreamNumbers", methodParameters, operationOptions);
                        Assert.NotNull(observable, "CimSession.InvokeMethod returned non-null");

                        List<AsyncItem<CimMethodResultBase>> result = Helpers.ObservableToList(observable);
                        Assert.True(result.Count > 0, "Got some callbacks");
                        Assert.Equal(result[result.Count - 1].Kind, AsyncItemKind.Completion, "Got completion callback");

                        Assert.True(result.Count > 1, "Got more than 1 callback");
                        Assert.Equal(result[result.Count - 2].Kind, AsyncItemKind.Item, "Got non-streamed result (presence)");
                        Assert.True(result[result.Count - 2].Item.GetType().Equals(typeof(CimMethodResult)), "Got non-streamed result (type)");

                        CimMethodResult methodResult = (CimMethodResult)result[result.Count - 2].Item;
                        Assert.Throws<ObjectDisposedException>(() =>
                        {
                            for (int i = 0; i < 10; i++)
                            {
                                var tmp = methodResult.OutParameters.Count; // expecting ObjectDisposedException here
                                Thread.Sleep(200);
                            }
                            return null;
                        });
                    }
                }

                [Fact]
                public void BaseOptions_ShortenLifetimeOfResults_Async_MethodStreaming()
                {
                    using (var cimSession = CimSession.Create(null))
                    {
                        var operationOptions = new CimOperationOptions { ShortenLifetimeOfResults = true, EnableMethodResultStreaming = true };

                        var methodParameters = new CimMethodParametersCollection();
                        methodParameters.Add(CimMethodParameter.Create("count", 3, CimType.UInt32, CimFlags.None));
                        IObservable<CimMethodResultBase> observable = cimSession.InvokeMethodAsync("this.TestNamespace", "TestClass_Streaming", "StreamNumbers", methodParameters, operationOptions);
                        Assert.NotNull(observable, "CimSession.InvokeMethod returned non-null");

                        List<AsyncItem<CimMethodResultBase>> result = Helpers.ObservableToList(observable);
                        Assert.True(result.Count > 0, "Got some callbacks");
                        Assert.Equal(result[result.Count - 1].Kind, AsyncItemKind.Completion, "Got completion callback");

                        Assert.True(result.Count > 1, "Got more than 1 callback");

                        Assert.True(result[0].Item.GetType().Equals(typeof(CimMethodStreamedResult)), "Got streamed result");
                        CimMethodStreamedResult streamedResult = ((CimMethodStreamedResult)(result[0].Item));

                        Assert.True(streamedResult.ItemValue is CimInstance, "Got streamed instance back");
                        CimInstance cimInstance = (CimInstance)streamedResult.ItemValue;
                        Assert.Throws<ObjectDisposedException>(() =>
                        {
                            for (int i = 0; i < 10; i++)
                            {
                                var tmp = cimInstance.CimSystemProperties.ClassName; // expecting ObjectDisposedException here
                                Thread.Sleep(200);
                            }
                            return null;
                        });
                    }
                }
        */
    }
}
