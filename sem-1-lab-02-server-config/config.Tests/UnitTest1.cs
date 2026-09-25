namespace config.Tests;

public class Tests
{
    [Test]
    public void CheckConfiguration_ValidConfiguration_isReady()
    {
        bool result = Program.ValidateServer(new ServerConfig(20, 10, 10));
        
        Assert.That(
            result,
            "Сервер готов к запуску.");
    }

    [Test]
    public void CheckConfiguration_ValidConfiguration_MaxConnections()
    {
        bool result = Program.ValidateConnection(new ServerConfig(20, -2, 10));
        
        Assert.That(
            result,
            "Запуск невозможен: неверное поле Max Connections");
    }
    
    [Test]
    public void CheckConfiguration_ValidConfiguration_RateLimit()
    {
        bool result = Program.ValidateRateLimit(new ServerConfig(1, 20, 10));
        
        Assert.That(
            result,
            "Запуск невозможен: неверное поле Rate Limit");
    }
    
    [Test]
    public void CheckConfiguration_ValidConfiguration_Timeout()
    {
        bool result = Program.ValidateTimeout(new ServerConfig(10000, 20, 100));
        
        Assert.That(
            result,
            "Запуск невозможен: неверное поле Timeout");
    }
    
    [Test]
    public void CheckConfiguration_TooManyPlayersForAvailableMemory_LaunchWithWarning()
    {
        bool result = Program.CheckCPUUsage(new ServerConfig(10000, 400, 1000));

        Assert.That(result, "Запуск возможен с предупреждением: для таких настроек рекомендуется больше ресурсов процессора");
    }
    
}