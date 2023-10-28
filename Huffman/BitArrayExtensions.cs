using System.Collections;

namespace Huffman;

public static class BitArrayExtensions
{
    public static BitArray Append(this BitArray current, BitArray after)
    {
        var bools = new bool[current.Count + after.Count];
        current.CopyTo(bools, 0);
        after.CopyTo(bools, current.Count);
        return new BitArray(bools);
    }
}