using ConsoleTables;
using Microsoft.Extensions.Hosting;

namespace Chat.ConsoleApp.Output;

public class DefaultConsole : IConsole
{
    public DefaultConsole()
    {
        // Hosted service keeps waiting ReadLine, so better to terminate it this way
        Console.CancelKeyPress += (s, e) => e.Cancel = false;
    }

    public void WriteLine(string str) =>
        Console.WriteLine(str);

    public string? ReadLine() =>
        Console.ReadLine();

    public void WriteTable<T>(IEnumerable<T> data) =>
        ConsoleTable
            .From<T>(data)
            .Configure(o => o.NumberAlignment = Alignment.Right)
            .Write(Format.Alternative);
}
