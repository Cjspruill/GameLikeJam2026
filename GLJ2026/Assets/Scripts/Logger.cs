/*public static class Logger
{
    private static Level _level = Level.Info;

    public static void SetLevel(Level level) => _level = level;

    private static readonly enum Level
    {
        Verbose,
        Info,
        Error
    };

    public static void LogVerbose(string message)
    {
        if (_level > Level.Verbose)
        {
            return;
        }

        Console.WriteLine(message);
    }

    public static void LogInfo(string message)
    {
        if (_level < Level.Info)
        {
            return;
        }
    }
}
*/
