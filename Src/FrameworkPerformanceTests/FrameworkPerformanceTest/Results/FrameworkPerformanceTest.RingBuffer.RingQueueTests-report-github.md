``` ini

BenchmarkDotNet=v0.10.12, OS=Windows 10 Redstone 3 [1709, Fall Creators Update] (10.0.16299.248)
Intel Core i7 CPU 930 2.80GHz (Nehalem), 1 CPU, 8 logical cores and 4 physical cores
Frequency=2740589 Hz, Resolution=364.8851 ns, Timer=TSC
  [Host]     : .NET Framework 4.7 (CLR 4.0.30319.42000), 32bit LegacyJIT-v4.7.2633.0
  Job-TVVWEI : .NET Framework 4.7 (CLR 4.0.30319.42000), 64bit LegacyJIT/clrjit-v4.7.2633.0;compatjit-v4.7.2633.0

Jit=LegacyJit  Platform=X64  Runtime=Clr  

```
|                                   Method |     Mean |    Error |    StdDev |   Median |     Gen 0 |     Gen 1 |     Gen 2 | Allocated |
|----------------------------------------- |---------:|---------:|----------:|---------:|----------:|----------:|----------:|----------:|
|                &#39;Ring queue encode speed&#39; | 155.7 ms | 3.100 ms |  4.346 ms | 155.5 ms | 4562.5000 | 2500.0000 |  875.0000 |  22.89 MB |
|         &#39;Ring queue double encode speed&#39; | 318.8 ms | 6.243 ms | 10.769 ms | 325.1 ms | 9125.0000 | 4687.5000 | 1500.0000 |  45.78 MB |
|        &#39;Ring queue encode/dequeue speed&#39; | 181.2 ms | 1.247 ms |  1.167 ms | 181.2 ms | 4500.0000 | 2250.0000 |  687.5000 |  22.89 MB |
| &#39;Ring queue double encode/dequeue speed&#39; | 351.6 ms | 2.747 ms |  2.435 ms | 350.7 ms | 9125.0000 | 4625.0000 | 1500.0000 |  45.78 MB |
|        &#39;Parallel enqueue / dequeue task&#39; | 204.9 ms | 2.561 ms |  2.396 ms | 204.5 ms | 4500.0000 | 2250.0000 |  687.5000 |   22.9 MB |
