namespace Engine;

public struct Color
{
    public float R, G, B, A;

    public Color(float r, float g, float b, float a = 1f)
    {
        R = r; G = g; B = b; A = a;
    }

    public static Color FromBytes(byte r, byte g, byte b, byte a = 255)
        => new(r / 255f, g / 255f, b / 255f, a / 255f);

    public (byte r, byte g, byte b, byte a) ToBytes()
        => ((byte)(R * 255f), (byte)(G * 255f), (byte)(B * 255f), (byte)(A * 255f));

    public static Color FromHex(string hex)
    {
        hex = hex.TrimStart('#');
        if (hex.Length == 6)
            hex += "FF";
        if (hex.Length != 8)
            throw new EngineException($"Color: invalid hex '{hex}'");

        byte r = Convert.ToByte(hex[..2], 16);
        byte g = Convert.ToByte(hex[2..4], 16);
        byte b = Convert.ToByte(hex[4..6], 16);
        byte a = Convert.ToByte(hex[6..8], 16);
        return FromBytes(r, g, b, a);
    }

    public string ToHex(bool includeAlpha = false)
    {
        var (r, g, b, a) = ToBytes();
        return includeAlpha
            ? $"#{r:X2}{g:X2}{b:X2}{a:X2}"
            : $"#{r:X2}{g:X2}{b:X2}";
    }

    public static Color White => new(1, 1, 1, 1);
    public static Color Black => new(0, 0, 0, 1);
    public static Color Red => new(1, 0, 0, 1);
    public static Color Green => new(0, 1, 0, 1);
    public static Color Blue => new(0, 0, 1, 1);
    public static Color Yellow => new(1, 1, 0, 1);
    public static Color Cyan => new(0, 1, 1, 1);
    public static Color Magenta => new(1, 0, 1, 1);
    public static Color Orange => new(1, 0.5f, 0, 1);
    public static Color Purple => new(0.5f, 0, 0.5f, 1);
    public static Color Pink => new(1, 0.4f, 0.7f, 1);
    public static Color Brown => new(0.6f, 0.3f, 0.1f, 1);
    public static Color Lime => new(0.5f, 1, 0, 1);
    public static Color Teal => new(0, 0.5f, 0.5f, 1);
    public static Color Navy => new(0, 0, 0.5f, 1);
    public static Color Maroon => new(0.5f, 0, 0, 1);
    public static Color Olive => new(0.5f, 0.5f, 0, 1);
    public static Color Silver => new(0.75f, 0.75f, 0.75f, 1);
    public static Color Transparent => new(0, 0, 0, 0);
    public static Color Gray => new(0.5f, 0.5f, 0.5f, 1);
    public static Color DarkGray => new(0.25f, 0.25f, 0.25f, 1);
    public static Color LightGray => new(0.75f, 0.75f, 0.75f, 1);

    public static Color FromName(string name)
    {
        return name.ToLowerInvariant() switch
        {
            "white" => White,
            "black" => Black,
            "red" => Red,
            "green" => Green,
            "blue" => Blue,
            "yellow" => Yellow,
            "cyan" => Cyan,
            "magenta" => Magenta,
            "orange" => Orange,
            "purple" => Purple,
            "pink" => Pink,
            "brown" => Brown,
            "lime" => Lime,
            "teal" => Teal,
            "navy" => Navy,
            "maroon" => Maroon,
            "olive" => Olive,
            "silver" => Silver,
            "gray" or "grey" => Gray,
            "darkgray" or "darkgrey" => DarkGray,
            "lightgray" or "lightgrey" => LightGray,
            "transparent" => Transparent,
            _ => throw new EngineException($"Color: unknown color name '{name}'")
        };
    }

    public Color WithAlpha(float a) => new(R, G, B, a);
}