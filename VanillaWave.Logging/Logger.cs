namespace VanillaWave.Logging;

public class Logger
{
    public static Logger Global = new();

    public enum LogLevel
    {
        Trace,
        Info,
        Warn,
        Error,
        Fatal
    }

    public LogLevel Threshold = LogLevel.Trace;

    public void Trace(object message) => Log(LogLevel.Trace, message, ConsoleColor.Gray);

    public void Info(object message) => Log(LogLevel.Info, message, ConsoleColor.Green);

    public void Warn(object message) => Log(LogLevel.Warn, message, ConsoleColor.Yellow);

    public void Error(object message) => Log(LogLevel.Error, message, ConsoleColor.Red);

    public void Fatal(object message) => Log(LogLevel.Fatal, message, ConsoleColor.Magenta);

    private void Log(LogLevel level, object message, ConsoleColor color)
    {
        if (level < Threshold)
            return;

        Console.ForegroundColor = color;

        Console.WriteLine($"[{level}] {message}");

        Console.ResetColor();
    }
}
