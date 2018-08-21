using System;

namespace MoveSharp.Math
{
    public interface ICalculator<T>
    {
        T Add(T a, T b);
        T Subtract(T a, T b);
        T Divide(T a, int b);
        T Zero();
        bool IsEqual(T a, T b);
    }

    public class IntCalculator : ICalculator<int>
    {
        public int Add(int a, int b)
        {
            return a + b;
        }

        public int Divide(int a, int b)
        {
            return Convert.ToInt32((double)a / b);
        }

        public bool IsEqual(int a, int b)
        {
            return a == b;
        }

        public int Subtract(int a, int b)
        {
            return a - b;
        }

        public int Zero()
        {
            return 0;
        }
    }

    public class FloatCalculator : ICalculator<float>
    {
        public float Add(float a, float b)
        {
            return a + b;
        }

        public float Divide(float a, int b)
        {
            return a / b;
        }

        public bool IsEqual(float a, float b)
        {
            return a == b;
        }

        public float Subtract(float a, float b)
        {
            return a - b;
        }

        public float Zero()
        {
            return 0.0f;
        }
    }

}
