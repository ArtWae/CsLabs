namespace config;

public class Program
{
    private static ServerConfig serverConfig;

    static void Main(string[] args)
    {
        Log("Загрузка конфига...");
        serverConfig = ServerConfig.getRandomConfig();

        try
        {
            ValidateServer(serverConfig);
            Log("Конфиг загружен!");
            Log("Host:  " + serverConfig.getHost() + ":" + serverConfig.getPort());
            Log("Timeout:  " + serverConfig.getTimeout() + " ms");
            Log("Rate Limit:  " + serverConfig.getRateLimit());
            Log("Max Connections:  " + serverConfig.getMaxConnections());
        }
        catch (Exception e)
        {
            Error("Сервер остановлен, ошибка конфигурации: "  + e.Message);
        }
}

    public static bool ValidateServer(ServerConfig config)
    {
        if (ValidateRateLimit(config) && ValidateConnection(config) && ValidateTimeout(config))
        {
            return true;
        }

        return false;
    }
    
    public static bool ValidateConnection(ServerConfig config)
    {
        if (config.getMaxConnections() < 1 || config.getMaxConnections() > 5000)
        {
            throw new Exception("поле Max Connection может содержать только значения от 1 до 5000");
        }

        return true;
    }
    
    public static bool ValidateTimeout(ServerConfig config)
    {
        if (config.getTimeout() > 1000)
        {
            throw new Exception("Поле Timeout лучше не ставить выше 1000");
        }
        else if (config.getTimeout() < 20)
        {
            throw new Exception("Поле Timeout лучше не ставить ниже 20");
        }

        return true;
    }
    
    public static bool ValidateRateLimit(ServerConfig config)
    {
        if (config.getRateLimit() < 3 || config.getRateLimit() > 10000)
        {
            throw new Exception("поле Rate Limit может содержать только значения от 3 до 10000");
        }

        return true;
    }

    public static bool CheckCPUUsage(ServerConfig config)
    {
        int usage = 0;

        if (config.getMaxConnections() > 1000) { usage = 1; }
        if (config.getTimeout() < 60) { usage += 1; }
        if (config.getRateLimit() > 3000) { usage += 1; }
        
        return usage > 1;
    }
    
    public static void Log(String log)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(log);
        Console.ResetColor();
    }
    
    public static void Error(String err)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Ошибка: " + err);
        Console.ResetColor();
    }
}