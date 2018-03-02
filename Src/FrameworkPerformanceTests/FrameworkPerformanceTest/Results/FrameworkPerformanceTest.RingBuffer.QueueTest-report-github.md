``` ini

BenchmarkDotNet=v0.10.12, OS=Windows 10 Redstone 3 [1709, Fall Creators Update] (10.0.16299.248)
Intel Core i7 CPU 930 2.80GHz (Nehalem), 1 CPU, 8 logical cores and 4 physical cores
Frequency=2740589 Hz, Resolution=364.8851 ns, Timer=TSC
  [Host]     : .NET Framework 4.7 (CLR 4.0.30319.42000), 32bit LegacyJIT-v4.7.2633.0
  Job-TVVWEI : .NET Framework 4.7 (CLR 4.0.30319.42000), 64bit LegacyJIT/clrjit-v4.7.2633.0;compatjit-v4.7.2633.0

Jit=LegacyJit  Platform=X64  Runtime=Clr  

```
|                                  Method |     Mean |     Error |    StdDev |     Gen 0 |     Gen 1 |     Gen 2 | Allocated |
|---------------------------------------- |---------:|----------:|----------:|----------:|----------:|----------:|----------:|
|                    &#39;Queue encode speed&#39; | 148.3 ms | 1.3346 ms | 1.2484 ms | 4562.5000 | 2375.0000 |  750.0000 |  22.89 MB |
|             &#39;Queue double encode speed&#39; | 306.0 ms | 0.9928 ms | 0.8290 ms | 9250.0000 | 5000.0000 | 1625.0000 |  45.78 MB |
|            &#39;Queue encode/dequeue speed&#39; | 124.4 ms | 1.8943 ms | 1.6793 ms | 4500.0000 | 2125.0000 |  625.0000 |  22.89 MB |
|     &#39;Queue double encode/dequeue speed&#39; | 275.7 ms | 5.0508 ms | 4.7245 ms | 9187.5000 | 4875.0000 | 1500.0000 |  45.78 MB |
| &#39;Queue parallel enqueue / dequeue task&#39; | 148.6 ms | 1.4131 ms | 1.1032 ms | 4437.5000 | 2125.0000 |  687.5000 |   22.9 MB |
