namespace config;

public class ServerConfig
{
    private string host;
    private int port;
    private int timeout;
    private short maxConnections;
    private int rateLimit;

    public ServerConfig(string host, int port, int timeout, short maxConnections, int rateLimit)
    {
        this.host = host;
        this.port = port;
        this.timeout = timeout;
        this.maxConnections = maxConnections;
        this.rateLimit = rateLimit;
    }
    
    public ServerConfig(int timeout, short maxConnections, int rateLimit)
    {
        this.host = "localhost";
        this.port = 25565;
        this.timeout = timeout;
        this.maxConnections = maxConnections;
        this.rateLimit = rateLimit;
    }

    public static ServerConfig getRandomConfig()
    {
        Random random = new Random();
        ServerConfig config = new ServerConfig("localhost", random.Next(25000, 35000), random.Next(1, 40),
            (short) random.Next(20, 50),  random.Next(20, 100));

        return config;
    }

    public string getHost()
    {
        return host;
    }

    public int getPort()
    {
        return port;
    }

    public int getTimeout()
    {
        return timeout;
    }
    
    public int getMaxConnections()
    {
        return maxConnections;
    }

    public int getRateLimit()
    {
        return rateLimit;
    }
}