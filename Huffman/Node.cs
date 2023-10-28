namespace Huffman;

internal sealed record Node(string Name)
{
    public Node? Left { get; init; }
    public Node? Right { get; init; }
}