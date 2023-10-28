using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Spectre.Console;
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

        var input = new FileInfo(settings.FilePath);
        var text = File.ReadAllText(settings.FilePath);

        var tree = HuffmanCoding.BuildTree(text);
        var codes = HuffmanCoding.BuildCodes(tree);
        var encoded = HuffmanCoding.Encode(text, codes);

        var output = new FileInfo(input.Name + ".bin");
        using (var encodedFile = new FileStream(output.Name, FileMode.Create))
        using (var bitStream = new BitStreamWriter(encodedFile))
        {
            bitStream.Write(encoded);
        }

        var meta = CreateMeta(codes, text);
        var metaJson = JsonSerializer.Serialize(meta);
        File.WriteAllText($"{output}.meta.json", metaJson);

        WriteInfo(input, output);

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

    private static void WriteInfo(FileInfo input, FileInfo output)
    {
        var table = new Table
        {
            Expand = false
        };

        table.AddColumns("", "Input", "Output");
        table.AddRow(new Text("File"), new Text(input.Name), new Text(output.Name));
        table.AddRow("Size (bytes)", input.Length.ToString(), output.Length.ToString());
        AnsiConsole.Write(table);

        var ratio = (float)input.Length / output.Length;
        AnsiConsole.Markup($"[bold]Compression ratio: {ratio:F4} ({ratio * 100:0.##\\%})[/]");
    }

    internal sealed class Settings : CommandSettings
    {
        [CommandArgument(0, "<FILE_PATH>")]
        public required string FilePath { get; init; }
    }
}