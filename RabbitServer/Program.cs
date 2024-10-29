using RabbitMQ.Client;
using System;
using System.Text;

class Program
{
    static void Main()
    {
        ConnectionFactory client = new ConnectionFactory();
        client.Uri = new Uri("amqp://guest:guest@localhost:5672");
        using (IConnection cnn = client.CreateConnection())
        using (IModel channel = cnn.CreateModel())
        {
            string exchangeName = "myName";
            string routingKey = "Route-My-Key";
            string queueName = "MyQueue";

            // Declare the exchange
            channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);

            // Declare the queue
            channel.QueueDeclare(queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

            // Bind the queue to the exchange
            channel.QueueBind(queueName, exchangeName, routingKey);

            // Create a message and publish it
            for (int i = 0; i < 10; i++) {
                Task.Delay(TimeSpan.FromSeconds(1)).Wait();
                byte[] message = Encoding.UTF8.GetBytes("HELLO WORLD");
                channel.BasicPublish(exchangeName, routingKey, null, message);
            }
        } // Channels and connections will be closed automatically here
    }
}
