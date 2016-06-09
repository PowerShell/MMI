using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Xunit;

// 0 means use one thread per processor
[assembly: CollectionBehavior(CollectionBehavior.CollectionPerAssembly, MaxParallelThreads = 0)]

// VS2015 xUnit test runner has a bug in which tests are not discovered in signed test assemblies
// Disable signing the test assembly if compiled under VS
#if !(VS2015 || _CORECLR)
[assembly: AssemblyKeyFileAttribute(@"..\..\signing\visualstudiopublic.snk")]
[assembly: AssemblyDelaySign(true)]
#endif
