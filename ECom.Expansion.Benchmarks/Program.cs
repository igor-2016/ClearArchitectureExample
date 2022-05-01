// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Hello, World!");
using BenchmarkDotNet.Running;

BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);

