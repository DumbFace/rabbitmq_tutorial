using RabbitMQ.Client;
using newtask;
using System.Text;
namespace EmitLog;

class Program
{
    static async Task Main(string[] args)
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        var connect = await factory.CreateConnectionAsync();
        var channel = await connect.CreateChannelAsync();

        // await channel.QueueDeclareAsync(String.Empty, true, false, false, null);
        await channel.ExchangeDeclareAsync(exchange: "logs", type: ExchangeType.Fanout);
        // var message = new NewTask();
        var message = GetMessage(args);
        var body = Encoding.UTF8.GetBytes(message);
        var properties = new BasicProperties
        {
            Persistent = true
        };
        channel.BasicReturnAsync += (sender, ea) =>
        {
            Console.WriteLine($"Message bị trả về! ReplyCode: {ea.ReplyCode}, ReplyText: {ea.ReplyText}");
            return Task.CompletedTask;
        };

        await channel.BasicPublishAsync(exchange: "logs", String.Empty, mandatory: true, basicProperties: properties, body);
        Console.WriteLine($" [x] Sent {message}");

        Console.WriteLine(" Press [enter] to exit.");
        Console.ReadLine();
    }

    static string GetMessage(string[] args)
    {
        return ((args.Length > 0) ? string.Join(" ", args) : "info: Hello World!");
    }
}
