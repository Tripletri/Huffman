namespace Huffman;

internal static class Huffman
{
    public static Node BuildTree(string input)
    {
        var nodes = GetQueue(input);

        while (nodes.TryDequeue(out var left, out var leftPriority))
        {
            if (nodes.TryDequeue(out var right, out var rightPriority))
            {
                var parent = new Node(left.Name + right.Name)
                {
                    Left = left,
                    Right = right
                };

                nodes.Enqueue(parent, leftPriority + rightPriority);
            }
            else
            {
                return left;
            }
        }

        throw new Exception("Something's wrong I can feel it...");
    }

    private static PriorityQueue<Node, int> GetQueue(string input)
    {
        var frequencyTable = new Dictionary<char, int>();

        foreach (var c in input)
        {
            if (frequencyTable.ContainsKey(c))
            {
                frequencyTable[c]++;
            }
            else
            {
                frequencyTable[c] = 1;
            }
        }

        var queue = new PriorityQueue<Node, int>();

        foreach (var (symbol, frequency) in frequencyTable)
        {
            queue.Enqueue(new Node(symbol.ToString()), frequency);
        }

        return queue;
    }

    public static Dictionary<char, string> BuildCodes(Node root)
    {
        var codes = new Dictionary<char, string>();
        BuildCodesInternal(root, "", codes);
        return codes;
    }

    private static void BuildCodesInternal(Node? node, string code, IDictionary<char, string> codes)
    {
        if (node == null)
            return;

        if (node.Left == null && node.Right == null)
        {
            codes.Add(node.Name.ToCharArray()[0], code);
        }

        BuildCodesInternal(node.Left, code + "0", codes);
        BuildCodesInternal(node.Right, code + "1", codes);
    }
}