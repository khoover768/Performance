``` ini

BenchmarkDotNet=v0.10.12, OS=Windows 10 Redstone 3 [1709, Fall Creators Update] (10.0.16299.248)
Intel Core i7 CPU 930 2.80GHz (Nehalem), 1 CPU, 8 logical cores and 4 physical cores
Frequency=2740589 Hz, Resolution=364.8851 ns, Timer=TSC
  [Host]     : .NET Framework 4.7 (CLR 4.0.30319.42000), 32bit LegacyJIT-v4.7.2633.0
  Job-TVVWEI : .NET Framework 4.7 (CLR 4.0.30319.42000), 64bit LegacyJIT/clrjit-v4.7.2633.0;compatjit-v4.7.2633.0

Jit=LegacyJit  Platform=X64  Runtime=Clr  

```
|                                                  Method |      Mean |     Error |    StdDev |    Median | Allocated |
|-------------------------------------------------------- |----------:|----------:|----------:|----------:|----------:|
|                                &#39;_type = typeof(string)&#39; | 22.105 ns | 0.4715 ns | 0.4842 ns | 22.031 ns |       0 B |
|                  &#39;_typeHnd = typeof(string).TypeHandle&#39; | 38.971 ns | 0.8213 ns | 1.0086 ns | 38.341 ns |       0 B |
|                   &#39;_result = _anObj.GetType() == _type&#39; |  8.914 ns | 0.2202 ns | 0.3556 ns |  8.762 ns |       0 B |
| &#39;_result = Type.GetTypeHandle(_anObj).Equals(_typeHnd)&#39; | 79.044 ns | 1.5983 ns | 2.1337 ns | 78.058 ns |       0 B |
|          &#39;_result = _anObj.GetType() == typeof(string)&#39; |  6.479 ns | 0.1736 ns | 0.3468 ns |  6.470 ns |       0 B |
|                            &#39;_result = _anObj is string&#39; |  6.930 ns | 0.1830 ns | 0.3392 ns |  6.910 ns |       0 B |
|           &#39;_result = _first.GetType() == typeof(First)&#39; |  6.165 ns | 0.2177 ns | 0.2330 ns |  6.046 ns |       0 B |
|                             &#39;_result = _first is First&#39; |  6.028 ns | 0.1623 ns | 0.2275 ns |  6.029 ns |       0 B |
|              &#39;Test type random index with if &#39;is&#39; test&#39; |  7.739 ns | 0.2912 ns | 0.2991 ns |  7.587 ns |       0 B |
|                              &#39;Pattern switch statement&#39; |  7.394 ns | 0.2071 ns | 0.3681 ns |  7.339 ns |       0 B |
