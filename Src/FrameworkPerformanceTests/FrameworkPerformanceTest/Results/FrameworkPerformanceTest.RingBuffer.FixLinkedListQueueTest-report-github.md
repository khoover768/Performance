``` ini

BenchmarkDotNet=v0.10.12, OS=Windows 10 Redstone 3 [1709, Fall Creators Update] (10.0.16299.248)
Intel Core i7 CPU 930 2.80GHz (Nehalem), 1 CPU, 8 logical cores and 4 physical cores
Frequency=2740588 Hz, Resolution=364.8852 ns, Timer=TSC
  [Host]     : .NET Framework 4.7 (CLR 4.0.30319.42000), 32bit LegacyJIT-v4.7.2633.0
  Job-TVVWEI : .NET Framework 4.7 (CLR 4.0.30319.42000), 64bit LegacyJIT/clrjit-v4.7.2633.0;compatjit-v4.7.2633.0

Jit=LegacyJit  Platform=X64  Runtime=Clr  

```
|                                         Method |     Mean |     Error |    StdDev |      Gen 0 |      Gen 1 |     Gen 2 | Allocated |
|----------------------------------------------- |---------:|----------:|----------:|-----------:|-----------:|----------:|----------:|
|                    &#39;Linked Queue encode speed&#39; | 442.1 ms |  6.560 ms |  6.137 ms | 13062.5000 |  7312.5000 | 1687.5000 |  68.67 MB |
|             &#39;Linked Queue double encode speed&#39; | 916.8 ms |  4.692 ms |  3.918 ms | 26125.0000 | 14625.0000 | 3500.0000 | 137.33 MB |
|            &#39;Linked Queue encode/dequeue speed&#39; | 478.8 ms |  9.396 ms | 13.773 ms | 25687.5000 |  7062.5000 | 1812.5000 | 122.07 MB |
|     &#39;Linked Queue double encode/dequeue speed&#39; | 876.9 ms | 16.840 ms | 14.928 ms | 38437.5000 | 13875.0000 | 3250.0000 | 190.74 MB |
| &#39;Linked Queue parallel enqueue / dequeue task&#39; | 556.7 ms | 11.515 ms | 20.764 ms | 22312.5000 |  8062.5000 | 2062.5000 | 122.15 MB |
