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

        // �и� ������� ��ȣ�� ���� �ø�
        if (den < 0)
        {
            num = -num;
            den = -den;
        }

        // ���
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

    // 1) ���ڿ� �Ҽ��� ��Ȯ�ϰ� �м��� ��ȯ
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

    // 2) decimal �� �м� ��ȯ (��Ȯ)
    public static Fraction FromDecimal(decimal d)
    {
        int[] bits = decimal.GetBits(d);
        int scale = (bits[3] >> 16) & 0xFF; // �Ҽ� �ڸ���
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

    // 3) double �� �ٻ� �м� ��ȯ (���м�)
    public static Fraction FromDouble(double value, int maxDenominator = 100000, double epsilon = 1e-12)
    {
        if (double.IsNaN(value) || double.IsInfinity(value))
            throw new ArgumentException("Value must be finite.");

        int sign = Math.Sign(value);
        double x = Math.Abs(value);

        long a = (long)Math.Floor(x);
        if (Math.Abs(x - a) < epsilon)
            return new Fraction(sign * a, 1);

        // p(-1)=1,q(-1)=0 ; p(0)=a0,q(0)=1
        BigInteger p0 = 1, q0 = 0;
        BigInteger p1 = a, q1 = 1;

        double r = x;
        while (true)
        {
            double frac = r - Math.Floor(r);
            if (frac < epsilon) // �������� �� ������
                return new Fraction(sign * p1, q1);

            r = 1.0 / frac;
            long ai = (long)Math.Floor(r);

            BigInteger p2 = ai * p1 + p0;
            BigInteger q2 = ai * q1 + q0;

            if (q2 > maxDenominator)
            {
                // q1�� q2 �� �� ����� �� ����
                Fraction f1 = new Fraction(sign * p1, q1);
                Fraction f2 = new Fraction(sign * p2, q2);
                return (Math.Abs(f1.ToDouble() - x) <= Math.Abs(f2.ToDouble() - x)) ? f1 : f2;
            }

            double approx = (double)p2 / (double)q2;
            if (Math.Abs(approx - x) < epsilon)
                return new Fraction(sign * p2, q2);

            p0 = p1; q0 = q1;
            p1 = p2; q1 = q2;
        }
    }

}
