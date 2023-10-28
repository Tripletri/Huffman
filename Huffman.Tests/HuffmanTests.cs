using FluentAssertions;

namespace Huffman.Tests;

internal sealed class HuffmanTests
{
    private static IEnumerable<(string Input, Node Tree, Dictionary<char, string> Codes)> GetTestCases()
    {
        var input1 = "AAB";
        var tree1 = new Node("BA")
        {
            Name = "BA",
            Left = new Node("B"),
            Right = new Node("A")
        };

        var codes1 = new Dictionary<char, string>
        {
            ['B'] = "0",
            ['A'] = "1"
        };

        yield return (input1, tree1, codes1);

        var input2 = "A_DEAD_DAD_CEDED_A_BAD_BABE_A_BEADED_ABACA_BED";
        var tree2 = new Node("_DAECB")
        {
            Left = new Node("_D")
            {
                Left = new Node("_"),
                Right = new Node("D")
            },
            Right = new Node("AECB")
            {
                Left = new Node("A"),
                Right = new Node("ECB")
                {
                    Left = new Node("E"),
                    Right = new Node("CB")
                    {
                        Left = new Node("C"),
                        Right = new Node("B")
                    },
                }
            }
        };

        var codes2 = new Dictionary<char, string>
        {
            ['_'] = "00",
            ['D'] = "01",
            ['A'] = "10",
            ['E'] = "110",
            ['C'] = "1110",
            ['B'] = "1111",
        };

        yield return (input2, tree2, codes2);
    }

    [TestCaseSource(nameof(BuildTreeTestCases))]
    public void Should_build_tree(string input, Node root)
    {
        var actual = Huffman.BuildTree(input);

        actual.Should().BeEquivalentTo(root);
    }

    public static IEnumerable<TestCaseData> BuildTreeTestCases()
    {
        foreach (var testCase in GetTestCases())
        {
            yield return new TestCaseData(testCase.Input, testCase.Tree);
        }
    }

    [TestCaseSource(nameof(BuildCodesTestCases))]
    public void Should_build_codes(Node tree, Dictionary<char, string> codes)
    {
        var actual = Huffman.BuildCodes(tree);

        actual.Should().BeEquivalentTo(codes);
    }

    public static IEnumerable<TestCaseData> BuildCodesTestCases()
    {
        foreach (var testCase in GetTestCases())
        {
            yield return new TestCaseData(testCase.Tree, testCase.Codes);
        }
    }
}