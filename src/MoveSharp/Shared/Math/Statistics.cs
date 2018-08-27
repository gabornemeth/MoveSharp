//
// Statistics.cs
//
// Author:
//    Gabor Nemeth (gabor.nemeth.dev@gmail.com)
//
//    Copyright (C) 2017, Gabor Nemeth
//

using System;
using System.Collections.Generic;

namespace MoveSharp.Math
{
    /// <summary>
    /// Helper class for calculating average, max, summary, etc.
    /// </summary>
    public class Stat<T>
        where T : IComparable
    {
        private int _numOfSamples;
        private T _min;
        private T _max;
        private T _sum;
        private T _last, _lastMax, _lastMin;

        private readonly ICalculator<T> _calculator;

        protected Stat(ICalculator<T> calculator)
        {
            _calculator = calculator;
        }

        public T Average
        {
            get
            {
                return _numOfSamples == 0 ? _calculator.Zero() : _calculator.Divide(_sum, _numOfSamples);
            }
        }

        public T Summary
        {
            get
            {
                return _sum;
            }
        }

        public T Minimum
        {
            get { return _min; }
        }

        public T Maximum
        {
            get { return _max; }
        }

        public void Reset()
        {
            _numOfSamples = 0;
            _min = _max = _sum = _calculator.Zero();
            _last = _calculator.Zero();
            _lastMin = _lastMax = _calculator.Zero();
        }

        public void UpdateLast(T value)
        {
            _sum = _calculator.Subtract(_sum, _last);
            _numOfSamples--;
            _max = _lastMax;
            _min = _lastMin;
            Add(value);
        }

        public virtual void Add(T value)
        {
            _last = value;
            _sum = _calculator.Add(_sum, value);
            _numOfSamples++;
            if (value.CompareTo(_max) > 0)
            {
                _lastMax = _max;
                _max = value;
            }
            if (value.CompareTo(_min) < 0)
            {
                _lastMin = _min;
                _min = value;
            }
        }
    }

    public class StatWithItems<T> : Stat<T>
        where T : IComparable
    {
        private List<T> _items = new List<T>();
        public IEnumerable<T> Items => _items;

        protected StatWithItems(ICalculator<T> calculator) : base(calculator)
        {
            _items = new List<T>();
        }

        protected StatWithItems(int numberOfItems, ICalculator<T> calculator) : base(calculator)
        {
            _items = new List<T>(numberOfItems);
        }

        public override void Add(T value)
        {
            base.Add(value);
            _items.Add(value);
        }
    }

    public class IntStatistics : Stat<int>
    {
        public IntStatistics() : base(new IntCalculator())
        {
        }
    }

    public class FloatStatistics : Stat<float>
    {
        public FloatStatistics() : base(new FloatCalculator())
        {
        }
    }

    public class IntStatisticsWithItems : StatWithItems<int>
    {
        public IntStatisticsWithItems(int numberOfItems) : base(numberOfItems, new IntCalculator())
        {
        }

        public IntStatisticsWithItems() : base(new IntCalculator())
        {
        }
    }

    public class FloatStatisticsWithItems : StatWithItems<float>
    {
        public FloatStatisticsWithItems(int numberOfItems) : base(numberOfItems, new FloatCalculator())
        {
        }

        public FloatStatisticsWithItems() : base(new FloatCalculator())
        {
        }
    }

    /// <summary>
    /// Helper class for calculating average, max, summary, etc.
    /// </summary>
    public class Statistics
    {
        private int _numOfSamples;
        private float _min;
        private float _max;
        private float _sum;
        private float _last, _lastMax, _lastMin;

        public float Average
        {
            get
            {
                return _sum / _numOfSamples;
            }
        }

        public float Summary
        {
            get
            {
                return _sum;
            }
        }

        public float Minimum
        {
            get { return _min; }
        }

        public float Maximum
        {
            get { return _max; }
        }

        public void Reset()
        {
            _numOfSamples = 0;
            _min = _max = _sum = 0;
            _last = 0;
            _lastMin = _lastMax = 0;
        }

        public void UpdateLast(float value)
        {
            _sum -= _last;
            _numOfSamples--;
            _max = _lastMax;
            _min = _lastMin;
            Add(value);
        }

        public void Add(float value)
        {
            _last = value;
            _sum += value;
            _numOfSamples++;
            if (value > _max)
            {
                _lastMax = _max;
                _max = value;
            }
            if (value < _min)
            {
                _lastMin = _min;
                _min = value;
            }
        }
    }
}
