namespace TerminalWrapper.Console;

public sealed class ConsoleTerminal : Terminal
{
    private ConsoleTerminal() : base() { }

    public static ConsoleTerminal CreateTerminal(MainTask[] tasks)
    {
        ConsoleTerminal result = new();

        int len = tasks.Length;
        result.ExitCommand = len + 1;
        result.Padding = result.ExitCommand.ToString().Length + 1;

        for(int i = 0; i < len; i++)
        {
            MainTask t = tasks[i];

            if (t != null)
            {
                t.Terminal = result;
                result.Tasks.Add(i + 1, t);
            }
        }

        result.OnExit += async () =>
        {
            await result.WriteAsync("Terminal closed...");
        };

        return result;
    }

    public override Task PauseAsync()
    {
        System.Console.ReadKey();
        return Task.CompletedTask;
    }

    public override async Task<string?> ReadAsync()
    {
        string? input = await System.Console.In.ReadLineAsync();
        return input;
    }

    public override async Task SeparatorAsync()
    {
        await WriteAsync(string.Empty.PadLeft(50, '='));
    }

    public override async Task WriteAsync(string text)
    {
        await System.Console.Out.WriteLineAsync(text);
    }

    public override async Task WriteAsync(string text, TerminalColor color)
    {
        ConsoleColor consoleColor = TranslateColor(color);

        System.Console.ForegroundColor = consoleColor;
        await WriteAsync(text);
        System.Console.ForegroundColor = ConsoleColor.White;
    }

    private ConsoleColor TranslateColor(TerminalColor color)
    {
        if(color == TerminalColor.Red)
            return ConsoleColor.Red;
        if(color == TerminalColor.Green)
            return ConsoleColor.Green;
        if(color == TerminalColor.Blue)
            return ConsoleColor.Blue;
        if(color == TerminalColor.Yellow)
            return ConsoleColor.Yellow;
        if(color == TerminalColor.Magenta)
            return ConsoleColor.Magenta;
        if(color == TerminalColor.Cyan)
            return ConsoleColor.Cyan;
            
        return ConsoleColor.White;
    }
}
