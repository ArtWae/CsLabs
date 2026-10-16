namespace server_report_generator;
using System;
using System.IO;

class Program
{
    static void Main(string[] args)
    {
        string folder = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../"));
        string filePath = Path.Combine(folder, "event_server.log");
       generateReport(filePath);
        
        
    }

    public static void generateReport(string path)
    {
        if (!File.Exists(path))
        {
            Error("Файл не найден, по пути: " + path);
            return;
        }

        generate(path);
    }
    
    public static void generate(string path)
    {
        string date = "---";
        string eventName = "Восстание Ледяного Пламени";
        string[] lines = File.ReadAllLines(path);
        string winnerName = getWinnerName(lines, eventName);
        string item = "---";
        string uteshTeamName = "---";
        string uteshItem = "---";
        int points = 0;
        int warnings = 0;
        int errors = 0;
        bool isEventRunning = false;
        Log("Чтение лога...");

        

        foreach (string line in lines) {
            if (line.Contains("Событие началось: " + eventName))
            {
                isEventRunning = true;
                date = DateTime.Parse(line.Substring(0, line.IndexOf(' '))).ToString("dd.MM.yyyy");
            }

            if (!isEventRunning)
            {
                continue;
            }

            if (line.Contains("[Warning]"))
            {
                warnings++;
            }

            if (line.Contains("[Error]"))
            {
                errors++;
            }

            if (line.Contains("[Reward]") && line.Contains(" получили ") && line.Contains(" очков события"))
            {
                int messageStart = line.IndexOf("] ") + 2;
                int pnStart = line.IndexOf(" получили ", messageStart);
                int vlStart = pnStart + " получили ".Length;
                int pnEnd = line.IndexOf(" очков события", vlStart);

                string rewardTeamName = line.Substring(messageStart, pnStart - messageStart);
                string pointsStr = line.Substring(vlStart, pnEnd - vlStart);

                if (rewardTeamName.Equals(winnerName))
                {
                    points = int.Parse(pointsStr);
                }
            }

            string indexItemStr = "получили ивентовый предмет:";
            if (line.Contains("[Loot]") && line.Contains(indexItemStr))
            {
                int itemStart = line.IndexOf(indexItemStr) + indexItemStr.Length;
                item = line.Substring(itemStart).Trim();
            }
            
            string uteshIndexStr = "утешительную награду:";
            if (line.Contains("[Reward]") && line.Contains(uteshIndexStr))
            {
                int rewardItemStart = line.IndexOf(uteshIndexStr) + uteshIndexStr.Length;
                int rewardStart = line.IndexOf("] ") + 2;
                int giveStart = line.IndexOf(" получили ");
                uteshTeamName = line.Substring(rewardStart , giveStart - rewardStart);
                uteshItem = line.Substring(rewardItemStart).Trim();
            }
            
            if (line.Contains("Событие") && line.Contains("закрыто") && line.Contains(eventName)) {
                Log("Результат чтения:");
                break;
            }
        }
            
        Log("Итоги события \"" + eventName + "\"");
        Log(" ");
        Log("Дата: " + date); ;
        Log("Очки победителя: " + points);
        Log("Ивентовый предмет: " + item);
        Log($"Утешительная награда {uteshTeamName}: " + uteshItem);
        Log("Предупреждений во время события: " + warnings);
        Log("Ошибок во время события: " + errors);
    }

    public static string getWinnerName(string[] lines, string eventName) {
        string winnerName = "---";
        bool isEventRunning = false;
        foreach (string line in lines)
        {
            if (line.Contains("Событие началось: Восстание Ледяного Пламени"))
            {
                isEventRunning = true;
            }
            
            if (line.Contains("[Reward]") && line.Contains(" объявлены победителями события"))
            {
                int messageStart = line.IndexOf("] ") + 2;
                int winnerEnd = line.IndexOf(" объявлены победителями");
                winnerName = line.Substring(messageStart, winnerEnd - messageStart);
                
                return winnerName;
            }

            
            if (line.Contains("Событие") && line.Contains("Закончено") && line.Contains(eventName))
            {
                break;
            }
        }

        return winnerName;
    }
    

    public static void Log(String message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
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