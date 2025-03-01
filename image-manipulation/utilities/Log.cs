using System.Text;

namespace Maynard.Imaging.Utilities;

public static class Log
{
    private const int PADDING_TIMESTAMP = 12;
    private const int MAX_MESSAGE_WIDTH = 80;
    private const int PADDING_EVENT_ID = 10;
    private static readonly int PADDING_SEVERITY = Enum.GetNames<Severity>().Max(s => s.Length);
    private static readonly int PADDING_TO_MESSAGE = PADDING_TIMESTAMP + PADDING_SEVERITY + 7;
    private static readonly int END_OF_LINE = PADDING_TO_MESSAGE + MAX_MESSAGE_WIDTH + PADDING_EVENT_ID;
    private static readonly bool INITIALIZED = Initialize();
    private enum Severity { Verbose, Info, Warn, Error, Critical }

    private static Dictionary<int, long> _timestamps = new();
    private static int _counter;

    private class FilteringTextWriter(TextWriter original) : TextWriter
    {
        public override void WriteLine(string value) => Log.Error(value);

        public override Encoding Encoding => original.Encoding;
    }

    private static bool Initialize()
    {
        if (INITIALIZED)
            return true;
        
        Console.SetError(new FilteringTextWriter(Console.Error));
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"{"Timestamp",-PADDING_TIMESTAMP} | Event ID   | {"Severity".PadRight(PADDING_SEVERITY)} | Message");
        Console.WriteLine("-----------------------------------------------------------------------------------------------------------------------");
        return true;
    }

    private static Stack<Action> _stack;
    // End goal:
    // HH:MM:SS.sss | TYPE | Filename:Line | Message
    private static int Write(Severity severity, string message)
    {
        if (!INITIALIZED)
        {
            _stack ??= new();
            _stack.Push(() => Write(severity, message));
            return -1;
        }
        while (_stack?.Any() ?? false)
            _stack.Pop()();

        int eventId = _counter++;
        StringBuilder sb = new();
        
        sb.Append($"{DateTime.Now:HH:mm:ss.fff} | {eventId.ToString().PadLeft(PADDING_EVENT_ID)} | {severity.ToString().ToUpper().PadRight(PADDING_SEVERITY)} | ");
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
            Severity.Verbose => ConsoleColor.Gray,
            Severity.Info => ConsoleColor.White,
            Severity.Warn => ConsoleColor.Yellow,
            Severity.Error => ConsoleColor.Red,
            Severity.Critical => ConsoleColor.DarkRed,
            _ => ConsoleColor.Gray,
        };
        Console.ForegroundColor = severity switch
        {
            Severity.Critical => ConsoleColor.Gray,
            _ => ConsoleColor.Black
        };
        Console.WriteLine(sb.ToString());
        _timestamps[eventId] = TimestampMs.Now;
        return eventId;
    }

    public static int Verbose(string message) => Write(Severity.Verbose, message);
    public static int Info(string message) => Write(Severity.Info, message);
    public static int Warn(string message) => Write(Severity.Warn, message);
    public static int Error(string message) => Write(Severity.Error, message);
    public static int Critical(string message) => Write(Severity.Critical, message);
    
    public static int Verbose(string message, out int eventId) => eventId = Write(Severity.Verbose, message);
    public static int Info(string message, out int eventId) => eventId = Write(Severity.Info, message);
    public static int Warn(string message, out int eventId) => eventId = Write(Severity.Warn, message);
    public static int Error(string message, out int eventId) => eventId = Write(Severity.Error, message);
    public static int Critical(string message, out int eventId) => eventId = Write(Severity.Critical, message);

    public static long TimeSince(int eventId)
    {
        if (_timestamps.TryGetValue(eventId, out long timestamp))
            return TimestampMs.Now - timestamp;
        Warn($"Unable to find timestamp for one or more event IDs ({eventId})");
        return -1;
    }

    public static long TimeBetween(int firstEventId, int secondEventId)
    {
        bool success = _timestamps.TryGetValue(firstEventId, out long first)
            & _timestamps.TryGetValue(secondEventId, out long second);
        if (success)
            return Math.Abs(first - second);
        Warn($"Unable to find timestamp for one or more event IDs ({firstEventId}, {secondEventId})");
        return -1;
    }

    public static void PrintTimeBetween(string message, int firstEventId, int secondEventId)
    {
        long time = TimeBetween(firstEventId, secondEventId);
        if (time == -1)
            return;
        Info(message == null
            ? $"Time between {firstEventId} and {secondEventId}: {time:N0}ms"
            : $"{message} {time:N0}ms ({firstEventId}, {secondEventId})"
        );
    }
    public static void PrintTimeBetween(int firstEventId, int secondEventId) => PrintTimeBetween(null, firstEventId, secondEventId);
}