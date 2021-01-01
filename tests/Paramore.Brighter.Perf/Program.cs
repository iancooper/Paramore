using System;
using BenchmarkDotNet.Running;

namespace Paramore.Brighter.Perf
{
    class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<Benchmark>();
            Console.WriteLine(summary);
        }
    }


    public class TestCommand : Command
    {
        public TestCommand() : base(Guid.NewGuid())
        {
        }

        public string Message { get; set; }
        public int Number { get; set; }
        public DateTime DateNow { get; set; }
    }
}
