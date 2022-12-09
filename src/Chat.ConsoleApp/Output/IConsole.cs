namespace Chat.ConsoleApp.Output;

public interface IConsole
{
    void WriteLine(string str);
    void WriteTable<T>(IEnumerable<T> data);
    string? ReadLine();
}
