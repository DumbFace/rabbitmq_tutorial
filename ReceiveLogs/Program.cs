
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
namespace ReceiveLogs;

class Program
{
    static async Task Main(string[] args)
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        var connection = await factory.CreateConnectionAsync();
        var channel = await connection.CreateChannelAsync();

        await channel.ExchangeDeclareAsync(exchange: "logs", type: ExchangeType.Fanout);

        QueueDeclareOk queueDeclareResult = await channel.QueueDeclareAsync(durable: true);
        string queueName = queueDeclareResult.QueueName;

        await channel.QueueBindAsync(queue: queueName, exchange: "logs", routingKey: string.Empty);
        Console.WriteLine(" [*] Waiting for logs.");

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += (model, ea) =>
        {
            byte[] body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine($" [x] {message}");

            return Task.Run(() => { });
        };

        await channel.BasicConsumeAsync(queueName, autoAck: true, consumer: consumer);

        Console.WriteLine(" Press [enter] to exit.");
        Console.ReadLine();
    }
}
