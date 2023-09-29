using System.Data.Common;
using System.Diagnostics.CodeAnalysis;

namespace TerminalWrapper;

public struct TerminalColor
{
    private float m_red;
    public float RedChannel
    {
        get { return m_red; }
        set { m_red = Math.Clamp(value, 0f, 1f); }
    }

    private float m_green;
    public float GreenChannel
    {
        get { return m_green; }
        set { m_green = Math.Clamp(value, 0f, 1f); }
    }

    private float m_blue;
    public float BlueChannel
    {
        get { return m_blue; }
        set { m_blue = Math.Clamp(value, 0f, 1f); }
    }

    private float m_alpha;
    public float AlphaChannel
    {
        get { return m_alpha; }
        set { m_alpha = Math.Clamp(value, 0f, 1f); }
    }

    public TerminalColor(float r, float g, float b, float a)
    {
        m_red = 1f;
        m_green = 1f;
        m_blue = 1f;
        m_alpha = 1f;

        RedChannel = r;
        GreenChannel = g;
        BlueChannel = b;
        AlphaChannel = a;
    }

    public TerminalColor(float r, float g, float b) : this(r, g, b, 1f) { }

    public TerminalColor() : this(1f, 1f, 1f, 1f) { }

    public float[] Channels()
    {
        return new float[] { RedChannel, GreenChannel, BlueChannel };
    }

    public static TerminalColor FromHexCode(string hexcode)
    {
        if (hexcode.Length != 6)
            throw new FormatException($"{nameof(hexcode)} must be in FFFFFF format");

        float[] channels = new float[3];
        for (int i = 0; i < 3; i++)
        {
            string channelHex = hexcode.ToLower().Substring(i * 2, 2);
            channels[i] = ((float)Convert.ToByte(channelHex, 16)) / 255f;
        }

        return new TerminalColor(channels[0], channels[1], channels[2]);
    }

    public static TerminalColor Red => new TerminalColor(1f, 0f, 0f);
    public static TerminalColor Green => new TerminalColor(0f, 1f, 0f);
    public static TerminalColor Blue => new TerminalColor(0f, 0f, 1f);
    public static TerminalColor Black => new TerminalColor(0f, 0f, 0f);
    public static TerminalColor White => new TerminalColor(1f, 1f, 1f);
    public static TerminalColor Yellow => new TerminalColor(1f, 1f, 0f);
    public static TerminalColor Magenta => new TerminalColor(1f, 0f, 1f);
    public static TerminalColor Cyan => new TerminalColor(0f, 1f, 1f);

    public override readonly bool Equals([NotNullWhen(true)] object? obj)
    {
        if(obj is TerminalColor color)
            if(color == this)
                return true;
        return false;
    }

    public override int GetHashCode()
    {
        return RedChannel.GetHashCode() + GreenChannel.GetHashCode() + BlueChannel.GetHashCode() + AlphaChannel.GetHashCode();
    }

    public static bool operator == (TerminalColor a, TerminalColor b)
    {
        bool rb = a.RedChannel == b.RedChannel;
        bool gb = a.GreenChannel == b.GreenChannel;
        bool bb = a.BlueChannel == b.BlueChannel;
        bool ab = a.AlphaChannel == b.AlphaChannel;
        return rb && gb && bb && ab;
    }

    public static bool operator != (TerminalColor a, TerminalColor b)
    {
        bool rb = a.RedChannel != b.RedChannel;
        bool gb = a.GreenChannel != b.GreenChannel;
        bool bb = a.BlueChannel != b.BlueChannel;
        bool ab = a.AlphaChannel != b.AlphaChannel;
        return rb || gb || bb || ab;
    }
}
