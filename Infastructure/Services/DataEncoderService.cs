namespace WebProject.Infastructure.Services;

public static class DataEncoderService
{
    public static string Encode(this int value) => Encode(BitConverter.GetBytes(value));
    public static string Encode(this ReadOnlySpan<byte> data)
    {
        Span<byte> splitData = stackalloc byte[data.Length * 2];
        for (int i = 0; i < data.Length; i++)
        {
            splitData[i * 2] = (byte)((data[i] & 0xF0) >> 4); // Augstākie 4 biti
            splitData[i * 2 + 1] = (byte)(data[i] & 0xF); // Zemākie 4 biti
        }

        return Convert.ToBase64String(splitData);
    }

    public static byte[] Decode(this byte[] data)
    {
        byte[] output = new byte[data.Length / 2];
        for (int i = 0; i < output.Length; i++)
        {
            //                  Augstākie 4 biti +  Zemākie 4 biti
            output[i] = (byte)((data[i * 2] << 4) + data[i * 2 + 1]);
        }

        return output;
    }

    public static int Decode(this string value) => BitConverter.ToInt32(Decode(Convert.FromBase64String(value)));
}