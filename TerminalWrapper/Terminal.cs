namespace TerminalWrapper;

public abstract class Terminal
{
    protected Dictionary<int, MainTask> Tasks { get; private set; }
    protected int Padding { get; set; }
    protected int ExitCommand { get; set; }

    public event Action? OnExit;

    protected Terminal()
    {
        Tasks = new();
    }

    public virtual async Task RunAsync()
    {
        try
        {
            await TaskFlow();
        }
        catch(Exception ex)
        {
            await WriteAsync($"Exception \"{ex.GetType().FullName}\" found.", TerminalColor.Red);
            await WriteAsync(ex.Message, TerminalColor.Red);
            await WriteAsync("Shutting Terminal down", TerminalColor.Yellow);
            await PauseAsync();
        }
        finally
        {
            OnExit?.Invoke();
        }
    }

    private async Task TaskFlow()
    {
        int currentInstruction;

        do
        {
            await SeparatorAsync();
            await WriteAsync("Choose your test");
            string key;

            foreach (KeyValuePair<int, MainTask> kv in Tasks)
            {
                key = kv.Key.ToString().PadLeft(Padding);
                await WriteAsync($"  {key} -> {kv.Value.TaskName}");
            }

            key = ExitCommand.ToString().PadLeft(Padding);

            await WriteAsync($"  {key} -> Exit");
            await SeparatorAsync();

            string? input = await ReadAsync();

            currentInstruction = await ValidateInstruction(input);
            await SeparatorAsync();

            if (currentInstruction > -1)
                await Tasks[currentInstruction].ExecuteAsync();

        } while (currentInstruction >= -1);
    }

    private async Task<int> ValidateInstruction(string? input)
    {
        if(int.TryParse(input, out int result))
        {
            if (result == ExitCommand)
                return -2;

            if (Tasks.ContainsKey(result))
                return result;
        }

        await SeparatorAsync();
        await WriteAsync("Invalid Options");

        return -1;
    }

    public abstract Task SeparatorAsync();

    public abstract Task WriteAsync(string text);
    public abstract Task WriteAsync(string text, TerminalColor color);

    public abstract Task<string?> ReadAsync();

    public abstract Task PauseAsync();

}