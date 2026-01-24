namespace DrawThatThing.Core.Models;

public readonly struct Color : IEquatable<Color>
{
    public byte R { get; }
    public byte G { get; }
    public byte B { get; }
    public byte A { get; }

    public bool IsEmpty => R == 0 && G == 0 && B == 0 && A == 0;

    public static Color Empty => new(0, 0, 0, 0);
    public static Color White => new(255, 255, 255, 255);
    public static Color Black => new(0, 0, 0, 255);

    public Color(byte r, byte g, byte b, byte a = 255)
    {
        R = r;
        G = g;
        B = b;
        A = a;
    }

    public static Color FromHex(string hex)
    {
        if (string.IsNullOrWhiteSpace(hex))
            return Empty;

        hex = hex.TrimStart('#');

        if (hex.Length == 6)
        {
            return new Color(
                Convert.ToByte(hex.Substring(0, 2), 16),
                Convert.ToByte(hex.Substring(2, 2), 16),
                Convert.ToByte(hex.Substring(4, 2), 16)
            );
        }

        if (hex.Length == 8)
        {
            return new Color(
                Convert.ToByte(hex.Substring(0, 2), 16),
                Convert.ToByte(hex.Substring(2, 2), 16),
                Convert.ToByte(hex.Substring(4, 2), 16),
                Convert.ToByte(hex.Substring(6, 2), 16)
            );
        }

        return Empty;
    }

    public string ToHex()
    {
        return $"#{R:X2}{G:X2}{B:X2}";
    }

    public int DifferenceTo(Color other)
    {
        return Math.Abs(R - other.R) + Math.Abs(G - other.G) + Math.Abs(B - other.B);
    }

    public override string ToString()
    {
        return ToHex();
    }

    public bool Equals(Color other)
    {
        return R == other.R && G == other.G && B == other.B && A == other.A;
    }

    public override bool Equals(object? obj)
    {
        return obj is Color other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(R, G, B, A);
    }

    public static bool operator ==(Color left, Color right) => left.Equals(right);
    public static bool operator !=(Color left, Color right) => !left.Equals(right);
}
