namespace Huffman;

public sealed record Meta(IReadOnlyDictionary<char, bool[]> Codes, int Length);