using System.Collections;
using System.Collections.Immutable;
using System.Text;

namespace Huffman;

public static class HuffmanCoding
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

    public static IReadOnlyDictionary<char, bool[]> BuildCodes(Node root)
    {
        var codes = new Dictionary<char, bool[]>();
        BuildCodesInternal(root, ImmutableList<bool>.Empty, codes);
        return codes;
    }

    private static void BuildCodesInternal(Node? node, ImmutableList<bool> code, IDictionary<char, bool[]> codes)
    {
        if (node == null)
            return;

        if (node.Left == null && node.Right == null)
        {
            codes.Add(node.Name.ToCharArray()[0], code.ToArray());
        }

        BuildCodesInternal(node.Left, code.Add(false), codes);
        BuildCodesInternal(node.Right, code.Add(true), codes);
    }

    public static BitArray Encode(string input, IReadOnlyDictionary<char, bool[]> codes)
    {
        var output = new List<bool>();

        foreach (var symbol in input)
        {
            output.AddRange(codes[symbol]);
        }

        return new BitArray(output.ToArray());
    }

    public static string Decode(BitArray input, Meta meta)
    {
        var codes = meta.Codes.ToDictionary(x => x.Key, x => new BitArray(x.Value));
        var output = new StringBuilder();

        var currentBits = new List<bool>();
        foreach (bool bit in input)
        {
            currentBits.Add(bit);
            var symbol = Find(codes, new BitArray(currentBits.ToArray()));

            if (symbol != null)
            {
                output.Append(symbol);
                currentBits.Clear();

                if (output.Length == meta.Length)
                    return output.ToString();
            }
        }

        return output.ToString();
    }

    private static char? Find(IReadOnlyDictionary<char, BitArray> codes, BitArray bits)
    {
        foreach (var (s, b) in codes)
        {
            if (Equals(bits, b))
                return s;
        }

        return null;
    }

    private static bool Equals(BitArray a, BitArray b)
    {
        if (a.Length != b.Length)
            return false;

        var xor = new BitArray(a).Xor(b);

        foreach (bool bit in xor)
        {
            if (bit) return false;
        }

        return true;
    }
}