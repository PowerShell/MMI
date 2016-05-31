using Xunit;
// 0 means use one thread per processor
[assembly: CollectionBehavior(CollectionBehavior.CollectionPerAssembly, MaxParallelThreads = 0)]