using System.Collections;

namespace Huffman;

public class BitReader
{
    private const int BufferSize = 8;
    private readonly Stream stream;

    public BitReader(Stream stream)
    {
        this.stream = stream;
    }

    public BitArray Read()
    {
        var numberOfBits = (int)stream.Length * 8;
        var bits = new BitArray(numberOfBits);

        // note: skip first byte
        var bufferPosition = BufferSize;
        var buffer = 0;

        for (var i = 0; i < numberOfBits; i++)
        {
            if (bufferPosition == BufferSize)
            {
                buffer = stream.ReadByte();
                bufferPosition = 0;
            }

            var bit = (buffer & (1 << bufferPosition)) != 0;
            bits[i] = bit;
            bufferPosition++;
        }

        return bits;
    }

    public void Close()
    {
        stream.Close();
    }
}