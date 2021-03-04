using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQAPI.RequestResponseLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RabbitMQAPI.RabbitMQ
{
    public class RabbitMqCRED : IRabbitMqCRED
    {
        private readonly ILogger<RabbitMqCRED> _logger;
        private readonly IConfiguration _configuration;
        private readonly DefaultObjectPool<IModel> _objectPool;

        public RabbitMqCRED (IPooledObjectPolicy<IModel> objectPolicy, IConfiguration configuration , ILogger<RabbitMqCRED> logger)
        {
            _objectPool = new DefaultObjectPool<IModel>(objectPolicy, Environment.ProcessorCount* 2);
            _configuration = configuration;
            _logger = logger;
        }

        public T Read<T>(string queueName) where T : class
        {
            RequestResponse requestResponse = null;
            string msg = null;
            IModel channel = null;
            try
            {
                channel = _objectPool.Get();
                var response = channel.BasicGet(queueName, true);
                if (response == null)
                {
                    return null;
                }
                if (response != null)
                {
                    var body = response.Body.ToArray();
                    msg = Encoding.ASCII.GetString(body);
                    requestResponse = JsonConvert.DeserializeObject<RequestResponse>(msg);
                }
                channel.BasicAck(response.DeliveryTag , false);
                return (T)Convert.ChangeType(requestResponse, typeof(T)); ; 
            }
            catch(Exception ex)
            {
                throw ex;
            }
            finally
            {
                ////channel.close();
                ////connection.close();
                _objectPool.Return(channel);
            }
        } 


        public void Write<T>(T message, string queueName) where T : class
        {
            IModel channel = null;
            if (message == null)
                return;
            try
            {
                channel = _objectPool.Get();
                var MessageSerialize = JsonConvert.SerializeObject(message);
                _logger.LogInformation($"Sending to RabbitMQ: {MessageSerialize}");
                var sendBytes = Encoding.UTF8.GetBytes(MessageSerialize);
                channel.QueueDeclare(queue: queueName,
                             durable: true,
                             exclusive: false,
                             autoDelete: false,
                             arguments: null);
                IBasicProperties basicProperties = channel.CreateBasicProperties();
                basicProperties.Persistent = true;
                channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: basicProperties, body: sendBytes);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                ////channel.close();
                ////connection.close();
                _objectPool.Return(channel);
            }
        }


        public void Delete<T>(T message, string queueName) where T : class
        {
            throw new NotImplementedException();
        }

     
        public void Update<T>(T message, string queueName) where T : class
        {
            throw new NotImplementedException();
        }
    }
}
