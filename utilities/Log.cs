using System.Text;

namespace Maynard.AlbumArt.Utilities;

public static class Log
{
    private const int PADDING_TIMESTAMP = 12;
    private const int MAX_MESSAGE_WIDTH = 80;
    private static readonly int PADDING_SEVERITY = Enum.GetNames<Severity>().Max(s => s.Length);
    private static readonly int PADDING_TO_MESSAGE = PADDING_TIMESTAMP + PADDING_SEVERITY + 7;
    private static readonly int END_OF_LINE = PADDING_TO_MESSAGE + MAX_MESSAGE_WIDTH;
    private static readonly bool _initialized = Initialize();
    private enum Severity { VERBOSE, INFO, WARN, ERROR, CRITICAL }

    private class FilteringTextWriter(TextWriter original) : TextWriter
    {
        public override void WriteLine(string value) => Log.Error(value);

        public override Encoding Encoding => original.Encoding;
    }

    private static bool Initialize()
    {
        if (_initialized)
            return true;
        // Console.SetOut(new FilteringTextWriter(Console.Out));
        Console.SetError(new FilteringTextWriter(Console.Error));
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"{"Timestamp",-PADDING_TIMESTAMP} | {"Severity".PadRight(PADDING_SEVERITY)} | Message");
        Console.WriteLine("-----------------------------------------------------------------------------------------------------------------------");
        return true;
    }

    private static Stack<Action> _stack;
    // End goal:
    // HH:MM:SS.sss | TYPE | Filename:Line | Message
    private static void Write(Severity severity, string message)
    {
        if (!_initialized)
        {
            _stack ??= new();
            _stack.Push(() => Write(severity, message));
            return;
        }
        while (_stack?.Any() ?? false)
            _stack.Pop()();
            
        StringBuilder sb = new();
        sb.Append($"{DateTime.Now:HH:mm:ss.fff} | {severity.ToString().PadRight(PADDING_SEVERITY)} | ");
        if (sb.Length + message.Length < END_OF_LINE)
            sb.Append(message);
        else
        {
            string[] words = message.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            int currentLength = sb.Length;
            foreach (string word in words)
                if (word.Length > MAX_MESSAGE_WIDTH)
                    foreach (char c in word)
                    {
                        if (currentLength == END_OF_LINE - 1)
                        {
                            sb.Append('-');
                            sb.Append(Environment.NewLine);
                            sb.Append(PADDING_TO_MESSAGE);
                            currentLength = PADDING_TO_MESSAGE;
                        }

                        sb.Append(c);
                        currentLength++;
                    }
                else if (currentLength + word.Length + 1 < END_OF_LINE)
                {
                    sb.Append(' ');
                    sb.Append(word);
                }
                else
                {
                    sb.Append(Environment.NewLine);
                    sb.Append(' ', PADDING_TO_MESSAGE);
                    sb.Append(word);
                }
        }
        
        Console.ForegroundColor = severity switch
        {
            Severity.VERBOSE => ConsoleColor.Gray,
            Severity.INFO => ConsoleColor.White,
            Severity.WARN => ConsoleColor.Yellow,
            Severity.ERROR => ConsoleColor.Red,
            Severity.CRITICAL => ConsoleColor.DarkRed,
            _ => ConsoleColor.Gray,
        };
        Console.ForegroundColor = severity switch
        {
            Severity.CRITICAL => ConsoleColor.Gray,
            _ => ConsoleColor.Black
        };
        Console.WriteLine(sb.ToString());
    }

    public static void Verbose(string message) => Write(Severity.VERBOSE, message);
    public static void Info(string message) => Write(Severity.INFO, message);
    public static void Warning(string message) => Write(Severity.WARN, message);
    public static void Error(string message) => Write(Severity.ERROR, message);
    public static void Critical(string message) => Write(Severity.CRITICAL, message);
}

