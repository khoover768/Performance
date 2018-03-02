``` ini

BenchmarkDotNet=v0.10.12, OS=Windows 10 Redstone 3 [1709, Fall Creators Update] (10.0.16299.248)
Intel Core i7 CPU 930 2.80GHz (Nehalem), 1 CPU, 8 logical cores and 4 physical cores
Frequency=2740589 Hz, Resolution=364.8851 ns, Timer=TSC
  [Host]     : .NET Framework 4.7 (CLR 4.0.30319.42000), 32bit LegacyJIT-v4.7.2633.0
  Job-TVVWEI : .NET Framework 4.7 (CLR 4.0.30319.42000), 64bit LegacyJIT/clrjit-v4.7.2633.0;compatjit-v4.7.2633.0

Jit=LegacyJit  Platform=X64  Runtime=Clr  

```
|                                       Method |     Mean |     Error |    StdDev | Allocated |
|--------------------------------------------- |---------:|----------:|----------:|----------:|
|                           &#39;Iterate for(...)&#39; | 2.232 ms | 0.0440 ms | 0.0848 ms |       0 B |
|                       &#39;Iterate foreach(...)&#39; | 4.324 ms | 0.0854 ms | 0.0914 ms |       0 B |
| &#39;Iterate foreach(... with Enumerable.Range)&#39; | 8.047 ms | 0.0365 ms | 0.0324 ms |       0 B |
