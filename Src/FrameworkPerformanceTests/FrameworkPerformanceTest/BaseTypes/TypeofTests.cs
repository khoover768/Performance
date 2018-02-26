// Copyright (c) 2018 Kelvin Hoover.  All rights reserved
// Licensed under the MIT License, See License.txt in the project root for license information.

using BenchmarkDotNet.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrameworkPerformanceTest.BaseTypes
{
    public class TypeofTests
    {
        private Type _type = null;
        private RuntimeTypeHandle _typeHnd = default(RuntimeTypeHandle);
        private object _anObj = "aString";
        private bool _result = false;
        private First _first = new First();
        private Second _second = new Second();
        private object[] _testType = new object[] { new First(), new Second(), new Third(), new Fourth() };
        private Random _rnd = new Random();
        private int _randomObjectIndex;

        public TypeofTests()
        {
            _randomObjectIndex = _rnd.Next(0, _testType.Length - 1);
        }

        [Benchmark(Description = "_type = typeof(string)")]
        public void TypeofStringTest()
        {
            _type = typeof(string);
            _type = typeof(string);
            _type = typeof(string);
            _type = typeof(string);
            _type = typeof(string);
            _type = typeof(string);
            _type = typeof(string);
            _type = typeof(string);
            _type = typeof(string);
        }

        [Benchmark(Description = "_typeHnd = typeof(string).TypeHandle")]
        public void TypeofTypeHandleTest()
        {
            _typeHnd = typeof(string).TypeHandle;
            _typeHnd = typeof(string).TypeHandle;
            _typeHnd = typeof(string).TypeHandle;
            _typeHnd = typeof(string).TypeHandle;
            _typeHnd = typeof(string).TypeHandle;
            _typeHnd = typeof(string).TypeHandle;
            _typeHnd = typeof(string).TypeHandle;
            _typeHnd = typeof(string).TypeHandle;
            _typeHnd = typeof(string).TypeHandle;
            _typeHnd = typeof(string).TypeHandle;
        }

        [Benchmark(Description = "_result = _anObj.GetType() == _type")]
        public void GetTypeTest()
        {
            _result = _anObj.GetType() == _type;
            _result = _anObj.GetType() == _type;
            _result = _anObj.GetType() == _type;
            _result = _anObj.GetType() == _type;
            _result = _anObj.GetType() == _type;
            _result = _anObj.GetType() == _type;
            _result = _anObj.GetType() == _type;
            _result = _anObj.GetType() == _type;
            _result = _anObj.GetType() == _type;
            _result = _anObj.GetType() == _type;
        }

        [Benchmark(Description = "_result = Type.GetTypeHandle(_anObj).Equals(_typeHnd)")]
        public void GetTypeHandleEqualsTest()
        {
            _result = Type.GetTypeHandle(_anObj).Equals(_typeHnd);
            _result = Type.GetTypeHandle(_anObj).Equals(_typeHnd);
            _result = Type.GetTypeHandle(_anObj).Equals(_typeHnd);
            _result = Type.GetTypeHandle(_anObj).Equals(_typeHnd);
            _result = Type.GetTypeHandle(_anObj).Equals(_typeHnd);
            _result = Type.GetTypeHandle(_anObj).Equals(_typeHnd);
            _result = Type.GetTypeHandle(_anObj).Equals(_typeHnd);
            _result = Type.GetTypeHandle(_anObj).Equals(_typeHnd);
            _result = Type.GetTypeHandle(_anObj).Equals(_typeHnd);
            _result = Type.GetTypeHandle(_anObj).Equals(_typeHnd);
        }

        [Benchmark(Description = "_result = _anObj.GetType() == typeof(string)")]
        public void GetTypeEqualTypeofTest()
        {
            _result = _anObj.GetType() == typeof(string);
            _result = _anObj.GetType() == typeof(string);
            _result = _anObj.GetType() == typeof(string);
            _result = _anObj.GetType() == typeof(string);
            _result = _anObj.GetType() == typeof(string);
            _result = _anObj.GetType() == typeof(string);
            _result = _anObj.GetType() == typeof(string);
            _result = _anObj.GetType() == typeof(string);
            _result = _anObj.GetType() == typeof(string);
            _result = _anObj.GetType() == typeof(string);
        }

        [Benchmark(Description = "_result = _anObj is string")]
        public void AnObjectIsTypeTest()
        {
            _result = _anObj is string;
            _result = _anObj is string;
            _result = _anObj is string;
            _result = _anObj is string;
            _result = _anObj is string;
            _result = _anObj is string;
            _result = _anObj is string;
            _result = _anObj is string;
            _result = _anObj is string;
            _result = _anObj is string;
        }

        [Benchmark(Description = "_result = _first.GetType() == typeof(First)")]
        public void CustomClassGetTypeEqualTypeOf()
        {
            _result = _first.GetType() == typeof(First);
            _result = _first.GetType() == typeof(First);
            _result = _first.GetType() == typeof(First);
            _result = _first.GetType() == typeof(First);
            _result = _first.GetType() == typeof(First);
            _result = _first.GetType() == typeof(First);
            _result = _first.GetType() == typeof(First);
            _result = _first.GetType() == typeof(First);
            _result = _first.GetType() == typeof(First);
            _result = _first.GetType() == typeof(First);
        }

        [Benchmark(Description = "_result = _first is First")]
        public void CustomClassIsType()
        {
            _result = _first is First;
            _result = _first is First;
            _result = _first is First;
            _result = _first is First;
            _result = _first is First;
            _result = _first is First;
            _result = _first is First;
            _result = _first is First;
            _result = _first is First;
            _result = _first is First;
        }

        [Benchmark(Description = "Test type random index with if 'is' test")]
        public void CustomClassIfTest()
        {
            if (_testType[_randomObjectIndex] is First)
            {
                _result = true;
            }
            else if (_testType[_randomObjectIndex] is Second)
            {
                _result = true;
            }
            else if (_testType[_randomObjectIndex] is Third)
            {
                _result = true;
            }
            else if (_testType[_randomObjectIndex] is Fourth)
            {
                _result = true;
            }

            _result = false;
        }

        [Benchmark(Description = "Pattern switch statement")]
        public void CustomClassSwitchTest()
        {
            switch (_testType[_randomObjectIndex])
            {
                case First f:
                    _result = true;
                    break;

                case Second s:
                    _result = true;
                    break;

                case Third t:
                    _result = true;
                    break;

                case Fourth f:
                    _result = true;
                    break;

                default:
                    _result = false;
                    break;
            }
        }

        private class First
        {
            public int Value { get; set; }
        }

        private class Second
        {
            public int Value { get; set; }
        }

        private class Third
        {
            public int Value { get; set; }
        }

        private class Fourth
        {
            public int Value { get; set; }
        }
    }
}
