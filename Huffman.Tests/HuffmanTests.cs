using System.Collections;
using FluentAssertions;

namespace Huffman.Tests;

internal sealed class HuffmanTests
{
    private static IEnumerable<(string Input, Node Tree, Dictionary<char, BitArray> Codes)> GetTestCases()
    {
        var input1 = "AAB";
        var tree1 = new Node("BA")
        {
            Name = "BA",
            Left = new Node("B"),
            Right = new Node("A")
        };

        var codes1 = new Dictionary<char, BitArray>
        {
            ['B'] = new(new[] { false }),
            ['A'] = new(new[] { true })
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

        var codes2 = new Dictionary<char, BitArray>
        {
            ['_'] = new(new[] { false, false }),
            ['D'] = new(new[] { false, true }),
            ['A'] = new(new[] { true, false }),
            ['E'] = new(new[] { true, true, false }),
            ['C'] = new(new[] { true, true, true, false }),
            ['B'] = new(new[] { true, true, true, true }),
        };

        yield return (input2, tree2, codes2);
    }

    [TestCaseSource(nameof(BuildTreeTestCases))]
    public void Should_build_tree(string input, Node root)
    {
        var actual = HuffmanCoding.BuildTree(input);

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
    public void Should_build_codes(Node tree, Dictionary<char, BitArray> codes)
    {
        var actual = HuffmanCoding.BuildCodes(tree);

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