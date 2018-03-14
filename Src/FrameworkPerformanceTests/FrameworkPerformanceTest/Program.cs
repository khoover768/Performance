// Copyright (c) 2018 Kelvin Hoover.  All rights reserved
// Licensed under the MIT License, See License.txt in the project root for license information.

using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using FrameworkPerformanceTest.BaseTypes;
using FrameworkPerformanceTest.LruList;
using FrameworkPerformanceTest.LruList.Tests;
using FrameworkPerformanceTest.RingBuffer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrameworkPerformanceTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new MainConfig();

            //BenchmarkRunner.Run<TypeofTests>(config);
            //BenchmarkRunner.Run<LoopTests>(config);
            //BenchmarkRunner.Run<TryPerformanceTests>(config);

            //BenchmarkRunner.Run<FixedListStoragePerformanceTests>();

            //BenchmarkRunner.Run<RingQueueTests>(config);
            //BenchmarkRunner.Run<QueueTest>(config);
            //BenchmarkRunner.Run<QueueStructTests>(config);
            //BenchmarkRunner.Run<FixLinkedListQueueTest>(config);

            BenchmarkRunner.Run<LruCacheTests>(config);
            BenchmarkRunner.Run<LruDictionaryTests>(config);
            BenchmarkRunner.Run<LruDictionary2PerformanceTests>(config);
            BenchmarkRunner.Run<DictionaryOnlyPerformanceTests>(config);

            //FixedListStorageTests ft = new FixedListStorageTests();
            //ft.FixedListStorageVariationTests();

            //LruListUnitTests t = new LruListUnitTests();
            //t.SizeTests();
            //t.FuzzyTest();

            //LruUnitDictionaryTests t = new LruUnitDictionaryTests();
            //t.SimpleTests();

            //LruCacheTests t1 = new LruCacheTests();
            //t1.SetSpeedTest();
            //t1.SetDoubleSpeedTest();
            //t1.SetGetSpeedTest();
            //t1.SetGetDoubleSpeedTest();
            //t1.ParallelEnqueueGetTest();

            //LruDictionaryTests t = new LruDictionaryTests();
            //t.SetSpeedTest();
            //t.SetDoubleSpeedTest();
            //t.SetGetSpeedTest();
            //t.SetGetDoubleSpeedTest();
            //t.ParallelEnqueueGetTest();

            //new FixedListStoragePerformanceTests().SetSpeedTest();
            //new FixedListStoragePerformanceTests().SetGetRemoveSpeedTest();
            //new FixedListStoragePerformanceTests().ParallelEnqueueGetTest();

            //new DictionaryOnlyPerformanceTests().SetSpeedTest();
            //new DictionaryOnlyPerformanceTests().SetDoubleSpeedTest();
            //new DictionaryOnlyPerformanceTests().SetGetSpeedTest();
            //new DictionaryOnlyPerformanceTests().SetGetDoubleSpeedTest();
            //new DictionaryOnlyPerformanceTests().ParallelEnqueueGetTest();
        }

        public class MainConfig : ManualConfig
        {
            public MainConfig()
            {
                Add(DefaultConfig.Instance);
                Add(Job.Default.With(Runtime.Clr).With(Jit.LegacyJit).With(Platform.X64));
                Add(MemoryDiagnoser.Default);
            }
        }
    }
}
