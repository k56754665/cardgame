using System;
using System.Globalization;
using System.Numerics;

public readonly struct Fraction : IEquatable<Fraction>
{
    public BigInteger Num { get; }
    public BigInteger Den { get; }

    public Fraction(BigInteger num, BigInteger den)
    {
        if (den == 0)
            throw new DivideByZeroException("Denominator cannot be zero.");

        // 분모가 음수라면 부호를 위로 올림
        if (den < 0)
        {
            num = -num;
            den = -den;
        }

        // 약분
        BigInteger g = BigInteger.GreatestCommonDivisor(BigInteger.Abs(num), den);
        Num = num / g;
        Den = den / g;
    }

    public override string ToString()
    {
        return $"{Num}/{Den}";
    }

    public double ToDouble()
    {
        return (double)Num / (double)Den;
    }

    public bool Equals(Fraction other)
    {
        return Num == other.Num && Den == other.Den;
    }

    // 1) 문자열 소수를 정확하게 분수로 변환
    public static Fraction FromDecimalString(string s)
    {
        s = s.Trim().Replace(',', '.');

        if (!s.Contains("."))
        {
            BigInteger n = BigInteger.Parse(s, CultureInfo.InvariantCulture);
            return new Fraction(n, 1);
        }

        string[] parts = s.Split('.');
        string intPart = parts[0];
        string fracPart = parts[1];

        bool neg = intPart.StartsWith("-");
        if (neg)
            intPart = intPart.Substring(1);

        BigInteger scale = BigInteger.Pow(10, fracPart.Length);
        BigInteger nInt = string.IsNullOrEmpty(intPart) ? BigInteger.Zero : BigInteger.Parse(intPart, CultureInfo.InvariantCulture);
        BigInteger nFrac = BigInteger.Parse(fracPart, CultureInfo.InvariantCulture);

        BigInteger num = nInt * scale + nFrac;
        if (neg)
            num = -num;

        return new Fraction(num, scale);
    }

    // 2) decimal → 분수 변환 (정확)
    public static Fraction FromDecimal(decimal d)
    {
        int[] bits = decimal.GetBits(d);
        int scale = (bits[3] >> 16) & 0xFF; // 소수 자릿수
        long lo = (uint)bits[0];
        long mid = (uint)bits[1];
        long hi = (uint)bits[2];
        bool neg = (bits[3] & (1 << 31)) != 0;

        BigInteger int96 = (new BigInteger(hi) << 64) | ((new BigInteger(mid) << 32) | lo);
        if (neg)
            int96 = -int96;

        BigInteger den = BigInteger.Pow(10, scale);
        return new Fraction(int96, den);
    }

    // 3) double → 근사 분수 변환 (연분수)
    public static Fraction FromDouble(double value, int maxDenominator = 1_000_000, double epsilon = 1e-12)
    {
        if (double.IsNaN(value) || double.IsInfinity(value))
            throw new ArgumentException("Value must be a finite number.");

        double rounded = Math.Round(value);
        if (Math.Abs(value - rounded) < epsilon)
            return new Fraction((BigInteger)rounded, 1);

        long a0 = (long)Math.Floor(value);
        double frac = value - a0;

        if (Math.Abs(frac) < epsilon)
            return new Fraction(a0, 1);

        BigInteger numPrev = 1;
        BigInteger num = a0;
        BigInteger denPrev = 0;
        BigInteger den = 1;

        double x = value;
        while (true)
        {
            long a = (long)Math.Floor(x);
            BigInteger numNext = a * num + numPrev;
            BigInteger denNext = a * den + denPrev;

            if (denNext > maxDenominator)
            {
                Fraction f1 = new Fraction(num, den);
                Fraction f2 = new Fraction(numNext, denNext);
                return (Math.Abs(value - f1.ToDouble()) <= Math.Abs(value - f2.ToDouble())) ? f1 : f2;
            }

            double approx = (double)numNext / (double)denNext;
            if (Math.Abs(approx - value) < epsilon)
                return new Fraction(numNext, denNext);

            numPrev = num;
            denPrev = den;
            num = numNext;
            den = denNext;

            double remainder = x - Math.Floor(x);
            if (remainder < epsilon)
                return new Fraction(num, den);

            x = 1.0 / remainder;
        }
    }
}
