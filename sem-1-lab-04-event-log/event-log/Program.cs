namespace event_log;

class Program
{

    
    static void Main(string[] args)
    {
        string folder = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../"));
        string filePath = Path.Combine(folder, "event_server.log");
        
        Log("Чтение логов...");
        string[] lines = File.ReadAllLines(filePath);
        Log("Парсинг логов...");
        LogEntry[] entries = LogEntry.ParseLogs(lines);
        
        PrintStatistics(entries);
        
        LogEntry[] filtered = LogEntry.FilterByCategory(entries,"Reward");
        Log($"Сохранение всех логов с категорией Reward ({filtered.Length}):");
        if (ExportFiltered(Path.Combine(folder, "Reward_logs.log"), filtered))
        {
            Log("Логи сохранены");
        }
    }

    private static void PrintStatistics(LogEntry[] entries)
    {
        Log("Статус сервера: " + GetServerStatus(entries));
        Log("=======" + " СТАТИСТИКА" + "=======");
        Log("Количество логов по уровням:");
        PrintLevelCount(entries, "Info");
        PrintLevelCount(entries,"Warning");
        PrintLevelCount(entries,"Error");
        PrintLevelCount(entries,"Fatal");
        Log(" ");
        Log("Количество логов по категориям:");
        PrintCategoryCount(entries, "Reward");
        PrintCategoryCount(entries, "Loot");
        PrintCategoryCount(entries, "Combat");
        PrintCategoryCount(entries, "Statistics");
        PrintCategoryCount(entries, "Database");
        PrintCategoryCount(entries, "Statistics");
        PrintCategoryCount(entries, "Event");
        PrintCategoryCount(entries, "Clan");
        PrintCategoryCount(entries, "NPC");
        PrintCategoryCount(entries, "User");
        PrintCategoryCount(entries, "Server");
        Log("Всего логов - " + entries.Length);
        Log(" ");
        DateTime targetDate = new DateTime(2026, 9, 2); 
        LogEntry[] filtered = LogEntry.FilterByDate(entries, targetDate);
        Log($"Записей за {targetDate.ToString("yyyy-M-d")}: {filtered.Length}");
        
        LogEntry[] searchs = LogEntry.Search(entries, "Ночные совы");
        Log($"Найдено записей по запросу 'Ночные совы': {searchs.Length}");
    }

    public static bool ExportFiltered(string path, LogEntry[] entries)
    {
        try
        {
            File.WriteAllLines(path, entries.Select(entry => entry.getRaw()));
            return true;
        }
        catch (Exception e)
        {
            Error("Ошибка записи строк: " + e.Message);
            return false;
        }
    }

    private static void PrintLevelCount(LogEntry[] entries, string level)
    {
        Log("- " + level + " " + LogEntry.CountByLevel(entries, level), ConsoleColor.Yellow);
    }
    
    private static void PrintCategoryCount(LogEntry[] entries, string category)
    {
        Log("- " + category + " " + LogEntry.FilterByCategory(entries, category).Length, ConsoleColor.Yellow);
    }
    
    public static string GetServerStatus(LogEntry[] entries)
    {
        bool hasError = false;

        for (int i = 0; i < entries.Length; i++)
        {
            LogEntry entry = entries[i];
            if (entry.getCategory() == "Fatal" && entry.getCategory() == "Server")
            {
                return "КРИТИЧЕСКАЯ ОШИБКА! Сервер остановлен";
            }
            
            if (entry.getLevel() == "Error" || entry.getLevel() == "Fatal")
            {
                hasError = true;
            }
        }
        if (hasError) { return "Есть ошибки, требуется проверка"; }
        return "Сервер работает штатно";
    }
    
    public static void Log(String message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(message);
        Console.ResetColor();
    }
    
    public static void Log(String message, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(message);
        Console.ResetColor();
    }
    
    public static void Error(String message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(message);
        Console.ResetColor();
    }
}

public class LogEntry
{
    private DateTime timestamp;
    private string level;
    private string category;
    private string raw;
    
    public LogEntry(String raw, DateTime timestamp, string level, string category)
    {
        this.raw = raw;
        this.category = category;
        this.level = level;
        this.timestamp = timestamp;
    }

    public DateTime getDateTime() {
        return timestamp;
    }

    public string getLevel() {
        return level;
    }

    public string getCategory()
    {
        return category;
    }
    
    public string getRaw()
    {
        return raw;
    }
    
    public static LogEntry[] ParseLogs(string[] lines)
    {
        var result = new List<LogEntry>(lines.Length);

        for (int i = 0; i < lines.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i])) { continue; }
            result.Add(Parse(lines[i]));
        }

        return result.ToArray();
    }
    
    public static LogEntry Parse(string line)
    {
        string datePart = line.Substring(0, 23);
        int levelStart = line.IndexOf('[');
        int levelEnd = line.IndexOf(']', levelStart + 1);
        int categoryStart = line.IndexOf('[', levelEnd + 1);
        int categoryEnd = line.IndexOf(']', categoryStart + 1);
        LogEntry entry = new LogEntry(
            line,
            DateTime.Parse(datePart),
            line.Substring(levelStart + 1, levelEnd - levelStart - 1).Trim(),
            line.Substring(categoryStart + 1, categoryEnd - categoryStart - 1).Trim()
        );
        return entry;
    }
    
    public static LogEntry[] FilterByDate(LogEntry[] entries, DateTime date)
    {
        var result = new List<LogEntry>();
        for (int i = 0; i < entries.Length; i++)
        {
            if (entries[i].getDateTime().Date == date.Date)
            {
                result.Add(entries[i]);
            }
        }
        return result.ToArray();
    }
    public static LogEntry[] FilterByLevel(LogEntry[] entries, string level)
    {
        List<LogEntry> result = new List<LogEntry>();

        for (int i = 0; i < entries.Length; i++)
        {
            if (entries[i].getLevel().Equals(level))
            {
                result.Add(entries[i]);
            }
        }

        return result.ToArray();
    }

    public static LogEntry[] FilterByCategory(LogEntry[] entries, string category)
    {
        List<LogEntry> result = new List<LogEntry>();

        for (int i = 0; i < entries.Length; i++)
        {
            if (entries[i].getCategory().Equals(category))
            {
                result.Add(entries[i]);
            }
        }

        return result.ToArray();
    }

    public static LogEntry[] Search(LogEntry[] entries, string text)
    {
        List<LogEntry> result = new List<LogEntry>();

        for (int i = 0; i < entries.Length; i++)
        {
            if (entries[i].getRaw().Contains(text, StringComparison.OrdinalIgnoreCase))
            {
                result.Add(entries[i]);
            }
        }

        return result.ToArray();
    }

    public static int CountByLevel(LogEntry[] entries, string level)
    {
        int count = 0;

        for (int i = 0; i < entries.Length; i++)
        {
            if (entries[i].getLevel() == level)
            {
                count++;
            }
        }

        return count;
    }
}