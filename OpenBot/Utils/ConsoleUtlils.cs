namespace OpenBot.Utils;

public static class ConsoleUtils
{
    /// <summary>
    /// Writes the provided string to the console output
    /// </summary>
    /// <param name="unformatted">An unformatted string, which may contain colour codes such as &a</param>
    public static void WriteLine(string unformatted)
    {
        string formatted = unformatted
            // Colours
            .Replace("&0", "\u001b[30m")   // Black
            .Replace("&1", "\u001b[34m")   // Dark Blue
            .Replace("&2", "\u001b[32m")   // Dark Green
            .Replace("&3", "\u001b[36m")   // Dark Aqua
            .Replace("&4", "\u001b[31m")   // Dark Red
            .Replace("&5", "\u001b[35m")   // Dark Purple
            .Replace("&6", "\u001b[33m")   // Gold
            .Replace("&7", "\u001b[37m")   // Gray
            .Replace("&8", "\u001b[90m")   // Dark Gray (Bright Black)
            .Replace("&9", "\u001b[94m")   // Blue (Bright Blue)
            .Replace("&a", "\u001b[92m")   // Green (Bright Green)
            .Replace("&b", "\u001b[96m")   // Aqua (Bright Aqua)
            .Replace("&c", "\u001b[91m")   // Red (Bright Red)
            .Replace("&d", "\u001b[95m")   // Light Purple (Bright Magenta)
            .Replace("&e", "\u001b[93m")   // Yellow (Bright Yellow)
            .Replace("&f", "\u001b[97m")   // White (Bright White)

            // Style/format
            .Replace("&l", "\u001b[1m")    // Bold
            .Replace("&m", "\u001b[9m")    // Strikethrough
            .Replace("&n", "\u001b[4m")    // Underline
            .Replace("&o", "\u001b[3m")    // Italic
            .Replace("&r", "\u001b[0m");   // Reset

        Console.WriteLine(formatted);
    }
}