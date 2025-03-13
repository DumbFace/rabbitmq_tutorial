using System.Text;
using RabbitMQ.Client;

namespace newtask;

public class NewTask
{
    static async Task Main(string[] args)
    {
        var factory = new ConnectionFactory { HostName = "localhost" };
        using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(queue: "task_queue", durable: true, exclusive: false, autoDelete: false,
      arguments: null);

        var message = GetMessage(args);

        var body = Encoding.UTF8.GetBytes(message);
        BasicProperties properties = new BasicProperties
        {
            Persistent = true
        };
        await channel.BasicPublishAsync(exchange: string.Empty, routingKey: "task_queue", mandatory: true, basicProperties: properties, body: body);

        Console.WriteLine($" [x] Sent {message}");

        Console.WriteLine(" Press [enter] to exit.");
        Console.ReadLine();


    }

    public static string GetMessage(string[] args)
    {
        return ((args.Length > 0) ? string.Join(" ", args) : "Hello World!");
    }
}
