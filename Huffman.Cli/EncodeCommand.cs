using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Spectre.Console.Cli;

namespace Huffman.Cli;

internal sealed class EncodeCommand : Command<EncodeCommand.Settings>
{
    public override int Execute([NotNull] CommandContext context, [NotNull] Settings settings)
    {
        if (!File.Exists(settings.FilePath))
        {
            throw new Exception($"File '{settings.FilePath}' does not exist");
        }

        var file = new FileInfo(settings.FilePath);
        var text = File.ReadAllText(settings.FilePath);

        var tree = HuffmanCoding.BuildTree(text);
        var codes = HuffmanCoding.BuildCodes(tree);
        var encoded = HuffmanCoding.Encode(text, codes);

        var outputFile = new FileInfo(file.Name + ".bin");
        using (var encodedFile = new FileStream(outputFile.Name, FileMode.Create))
        using (var bitStream = new BitStreamWriter(encodedFile))
        {
            bitStream.Write(encoded);
        }

        var meta = CreateMeta(codes, text);
        var metaJson = JsonSerializer.Serialize(meta);
        File.WriteAllText($"{outputFile}.meta.json", metaJson);

        return 0;
    }

    private static Meta CreateMeta(IReadOnlyDictionary<char, BitArray> codes, string text)
    {
        var metaCodes = codes.ToDictionary(x => x.Key, x =>
        {
            var bools = new List<bool>();
            for (var i = 0; i < x.Value.Count; i++)
            {
                bools.Add(x.Value[i]);
            }

            return bools.ToArray();
        });

        var meta = new Meta(metaCodes, text.Length);
        return meta;
    }

    internal sealed class Settings : CommandSettings
    {
        [CommandArgument(0, "<FILE_PATH>")]
        public required string FilePath { get; init; }
    }
}