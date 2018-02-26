// Copyright (c) 2018 Kelvin Hoover.  All rights reserved
// Licensed under the MIT License, See License.txt in the project root for license information.

using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using FrameworkPerformanceTest.BaseTypes;
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
            BenchmarkRunner.Run<TypeofTests>(new MainConfig());
        }

        public class MainConfig : ManualConfig
        {
            public MainConfig()
            {
                Add(DefaultConfig.Instance);
                Add(Job.Default.With(Runtime.Clr).With(Jit.LegacyJit).With(Platform.X64));
            }
        }
    }
}
