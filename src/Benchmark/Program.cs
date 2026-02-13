using BenchmarkDotNet.Running;

#if DEBUG
Console.Error.WriteLine("You are in debug mode!");
#endif

BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
