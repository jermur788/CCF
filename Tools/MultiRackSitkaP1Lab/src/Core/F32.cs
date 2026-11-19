using System.Globalization;

namespace SitkaP1.Core;

/// <summary>
/// Authoritative float32 values are stored as their IEEE-754 bit pattern only (spec §9.2 T-07):
/// <c>{"k":"f32","b":"3db851ec"}</c> — exactly 8 lowercase hex digits, most significant first, independent of byte order.
/// NaN and infinities are refused. Signed zero keeps its bits.
/// </summary>
public static class F32
{
    public static uint Bits(float value) => BitConverter.SingleToUInt32Bits(value);

    public static float FromBits(uint bits) => BitConverter.UInt32BitsToSingle(bits);

    public static string Hex(uint bits) => bits.ToString("x8", CultureInfo.InvariantCulture);

    public static uint ParseHex(string? hex)
    {
        if (hex is null || hex.Length != 8) throw new P1Refusal("FloatBitsInvalid", "f32 bits must be exactly 8 hex digits");
        foreach (char c in hex)
            if (!(c is >= '0' and <= '9' or >= 'a' and <= 'f'))
                throw new P1Refusal("FloatBitsInvalid", "f32 bits must be lowercase hex");
        uint bits = uint.Parse(hex, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture);
        if (!float.IsFinite(FromBits(bits))) throw new P1Refusal("FloatNotFinite", "NaN and infinity are not allowed");
        return bits;
    }
}
