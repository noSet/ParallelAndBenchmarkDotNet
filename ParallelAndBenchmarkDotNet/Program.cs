using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace ParallelAndBenchmarkDotNet
{
    class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<PartitionTest>();
            Console.ReadKey();
        }

        [ClrJob(baseline: true), CoreJob, CoreRtJob]
        [RPlotExporter, RankColumn]
        public class PartitionTest
        {
            private readonly List<int> _randomInt;

            public PartitionTest()
            {
                _randomInt = Enumerable.Range(1, 1000).ToList();
            }

            [Benchmark]
            public int Normal()
            {
                int sum = 0;
                for (int i = 0; i < _randomInt.Count; i++)
                {
                    // 自旋模拟计算密集
                    Thread.SpinWait(20);
                    sum += _randomInt[i];
                }

                return sum;
            }

            [Benchmark]
            public int Partition()
            {
                int allsum = 0;
                Parallel.For(0, _randomInt.Count, new ParallelOptions() { MaxDegreeOfParallelism = Environment.ProcessorCount }, () => 0, (i, state, sum) =>
                {
                    // 自旋模拟计算密集
                    Thread.SpinWait(20);
                    sum += _randomInt[i];
                    return sum;
                }, sum => Interlocked.Add(ref allsum, sum));

                return allsum;
            }

            [Benchmark]
            public int PartitionDefault()
            {
                int allsum = 0;
                Parallel.For(0, _randomInt.Count, () => 0, (i, state, sum) =>
                {
                    // 自旋模拟计算密集
                    Thread.SpinWait(20);
                    sum += _randomInt[i];
                    return sum;
                }, sum => Interlocked.Add(ref allsum, sum));

                return allsum;
            }
        }
    }
}
