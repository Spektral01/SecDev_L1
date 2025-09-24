public interface ILogger
{
    void Log(string message);
}

public class FileLogger : ILogger
{
    private readonly string _filePath;

    public FileLogger(string filePath = "app.log")
    {
        _filePath = filePath;
    }

    public void Log(string message)
    {
        Console.WriteLine($"[FileLogger -> {_filePath}] {message}");
    }
}

public class DatabaseLogger : ILogger
{
    private readonly string _connectionString;

    public DatabaseLogger(string connectionString)
    {
        _connectionString = connectionString;
    }

    public void Log(string message)
    {
        Console.WriteLine($"[DatabaseLogger -> {_connectionString}] {message}");
    }
}

// SmtpMailer получает абстракцию ILogger
public class SmtpMailer
{
    private readonly ILogger _logger;

    // внедрение зависимости через конструктор позволяет классу не зависит от конкретной реализации
    public SmtpMailer(ILogger logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public void SendMessage(string to, string subject, string body)
    {
        Console.WriteLine($"Sending mail to {to}: {subject}");

        _logger.Log($"Sent message to {to} with subject '{subject}' at {DateTime.UtcNow:o}");
    }
}

class Program
{
    static void Main()
    {
        ILogger fileLogger = new FileLogger("smtp.log");
        var mailerWithFile = new SmtpMailer(fileLogger);
        mailerWithFile.SendMessage("ivan@example.com", "Hello", "Body text");

        ILogger dbLogger = new DatabaseLogger("Server=.;Database=Logs;Trusted_Connection=True;");
        var mailerWithDb = new SmtpMailer(dbLogger);
        mailerWithDb.SendMessage("anna@example.com", "Hi", "Another body");
    }
}
