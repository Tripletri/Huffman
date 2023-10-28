using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Spectre.Console.Cli;

namespace Huffman.Cli;

internal sealed class DecodeCommand : Command<DecodeCommand.Settings>
{
    public override int Execute([NotNull] CommandContext context, [NotNull] Settings settings)
    {
        if (!File.Exists(settings.FilePath))
        {
            throw new Exception($"File '{settings.FilePath}' does not exist");
        }

        using var bytes = File.OpenRead(settings.FilePath);
        var bitReader = new BitReader(bytes);
        var bits = bitReader.Read();

        var metaJson = File.ReadAllText($"{settings.FilePath}.meta.json");
        var meta = JsonSerializer.Deserialize<Meta>(metaJson);
        if (meta == null)
        {
            throw new Exception();
        }

        var decoded = HuffmanCoding.Decode(bits, meta);
        File.WriteAllText($"{settings.FilePath}.decoded", decoded);

        return 0;
    }

    internal sealed class Settings : CommandSettings
    {
        [CommandArgument(0, "<FILE_PATH>")]
        public required string FilePath { get; init; }
    }
}