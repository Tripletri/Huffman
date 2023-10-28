using Huffman.Cli;
using Spectre.Console.Cli;

var app = new CommandApp();
app.Configure(x =>
{
    x.AddCommand<EncodeCommand>("encode");
    x.AddCommand<DecodeCommand>("decode");
});

return app.Run(args);