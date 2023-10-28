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

        AnsiConsole.WriteLine("Building tree...");
        var tree = HuffmanCoding.BuildTree(text);

        AnsiConsole.WriteLine("Building codes...");
        var codes = HuffmanCoding.BuildCodes(tree);

        AnsiConsole.WriteLine("Encoding...");
        var encoded = HuffmanCoding.Encode(text, codes);

        var output = new FileInfo(input.Name + ".bin");
        using (var encodedFile = new FileStream(output.Name, FileMode.Create))
        using (var bitStream = new BitStreamWriter(encodedFile))
        {
            bitStream.Write(encoded);
        }

        var meta = new Meta(codes, text.Length);
        var metaJson = JsonSerializer.Serialize(meta);
        File.WriteAllText($"{output}.meta.json", metaJson);

        WriteInfo(input, output);

        return 0;
    }

    private static void WriteInfo(FileInfo input, FileInfo output)
    {
        var chart = new BarChart()
            .Label("[underline]Size in bytes[/]")
            .AddItem("Input", input.Length)
            .AddItem("Output", output.Length);

        AnsiConsole.Write(chart);

        var ratio = (float)input.Length / output.Length;
        AnsiConsole.Markup($"Compression ratio: {ratio:F4} ({ratio * 100:0.##\\%})");
    }

    internal sealed class Settings : CommandSettings
    {
        [CommandArgument(0, "<FILE_PATH>")]
        public required string FilePath { get; init; }
    }
}