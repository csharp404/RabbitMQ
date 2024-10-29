using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

class Program
{
    static void Main()
    {
        ConnectionFactory client = new ConnectionFactory
        {
            Uri = new Uri("amqp://guest:guest@localhost:5672")
        };

        using (IConnection cnn = client.CreateConnection())
        using (IModel channel = cnn.CreateModel())
        {
            string exchangeName = "myName";
            string routingKey = "Route-My-Key";
            string queueName = "MyQueue";

            
            channel.ExchangeDeclare(exchangeName, ExchangeType.Direct);

            
            channel.QueueDeclare(queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

            
            channel.QueueBind(queueName, exchangeName, routingKey);

            
            channel.BasicQos(0, 1, false);

           
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, args) =>
            {
                Task.Delay(TimeSpan.FromSeconds(0.5)).Wait();
                var msg = args.Body.ToArray();
                var pureMsg = Encoding.UTF8.GetString(msg);
                Console.WriteLine($"My message is: {pureMsg}");

               
                channel.BasicAck(args.DeliveryTag, false);
            };

            
            string consTag = channel.BasicConsume(queueName, false, consumer);

            Console.WriteLine("Press [enter] to exit.");
            Console.ReadLine();

            
            channel.BasicCancel(consTag);
        } 
    }
}
