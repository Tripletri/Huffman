namespace Huffman;

public sealed record Node(string Name)
{
    public Node? Left { get; init; }
    public Node? Right { get; init; }
}