using System.Collections;

namespace Huffman;

public sealed class BitStreamWriter : IDisposable, IAsyncDisposable
{
    private readonly Stream stream;
    private int bitPosition;
    private byte currentByte;

    public BitStreamWriter(Stream stream)
    {
        this.stream = stream;
        currentByte = 0;
        bitPosition = 7;
    }

    public async ValueTask DisposeAsync()
    {
        Close();
        await stream.DisposeAsync();
    }

    public void Dispose()
    {
        Close();
        stream.Dispose();
    }

    public void Write(BitArray bits)
    {
        var bytes = new byte[(bits.Length + 7) / 8];
        bits.CopyTo(bytes, 0);
        stream.Write(bytes, 0, bytes.Length);
    }

    public void WriteBit(bool bit)
    {
        if (bitPosition < 0)
        {
            stream.WriteByte(currentByte);
            currentByte = 0;
            bitPosition = 7;
        }

        if (bit)
            currentByte |= (byte)(1 << bitPosition);

        bitPosition--;
    }

    public void Close()
    {
        if (bitPosition != 7)
            stream.WriteByte(currentByte);

        stream.Close();
    }
}