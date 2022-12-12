using CommandLine;

namespace Chat.ConsoleApp.Configurations;

public class ArgumentOptions
{
    [Option('a', "aggregation", Required = false, HelpText = "Set aggregation type")]
    public string? AggregationType { get; set; }
}
