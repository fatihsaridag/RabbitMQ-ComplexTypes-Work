using RabbitMQ.Client;
using Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace RabbitmqFirstProject.publisher
{
    class Program
    {


        public enum LogNames
        {
            Critical = 1,
            Error = 2,
            Warning = 3,
            Info = 4
        }


        static void Main(string[] args)
        {
            //RabbitMq ya bağlanmamız için connection factory isminde bir nesne örneğği alalım
            var factory = new ConnectionFactory();
            //Uri yımızı belirticez. CloudAmqp deki instancedan urli alıyoruz. Gerçek seneryoda bunları appsetting.json içeçrisinde tutuyoruz.
            factory.Uri = new Uri("amqps://eznbdupx:Qf4h0Avxf0yEipy5VaR1D7UHRfIL0Gfn@gerbil.rmq.cloudamqp.com/eznbdupx");

            //factory üzerinden bir bağlantı açıyoruz.
            using var connection = factory.CreateConnection();
            //Artık bir bağlantımız var ve bu bağlantı  üzerinden kanal oluşturuyoruz onun üzerinden bağlanıcaz.
            var channel = connection.CreateModel(); //Bu kanal üzerinden rabbitMq ile haberleşebiliriz.
            //1.param : Exchange ismi , 2.param : restart atınca uygulama fiziksel olarak kaydedilsin , 3.param : Exchange tipi ? 
            channel.ExchangeDeclare("header-exchange",durable:true, type:ExchangeType.Headers);


            Dictionary<string,object> headers = new Dictionary<string, object>();
            headers.Add("format", "pdf");
            headers.Add("shape", "a4");

            var properties = channel.CreateBasicProperties();
            properties.Headers = headers;
            properties.Persistent = true;



            var product = new Product
            {
                Id = 1,
                Name = "Kalem",
                Price = 100,
                Stock = 10
            };

            var productJsonString = JsonSerializer.Serialize(product);




            channel.BasicPublish("header-exchange", string.Empty, properties, Encoding.UTF8.GetBytes(productJsonString));

            Console.WriteLine("mesaj gönderilmiştir.");

            Console.ReadLine();
               
        }
    }
}
