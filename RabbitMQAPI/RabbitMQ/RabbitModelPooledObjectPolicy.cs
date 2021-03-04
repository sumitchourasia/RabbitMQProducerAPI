using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RabbitMQAPI.RabbitMQ
{
    public class RabbitModelPooledObjectPolicy : IPooledObjectPolicy<IModel>
    {
        private readonly IConnection _connection;
        private readonly ILogger<RabbitModelPooledObjectPolicy> _logger;
        private readonly IConfiguration _config;

        public RabbitModelPooledObjectPolicy(ILogger<RabbitModelPooledObjectPolicy> logger, IConfiguration configuration)
        {
            _logger = logger;
            _config = configuration;
            _connection = GetConnection();
        }

        private IConnection GetConnection()
        {
            var factory = new ConnectionFactory()
            {
                HostName = _config["RabbitMQ:HostName"],
                UserName = _config["RabbitMQ:UserName"],
                Password = _config["RabbitMQ:Password"],
                //Port = Convert.ToInt32(_config["RabbitMQ:Port"]),
                //VirtualHost = _config["RabbitMQ:VHost"],
            };
            factory.AutomaticRecoveryEnabled = true;
            //return factory.CreateConnection(_config["RabbitMQ:QueueName"]);
            return factory.CreateConnection();
        }

        public IModel Create()
        {
            return _connection.CreateModel();
        }

        public bool Return(IModel obj)
        {
            if (obj.IsOpen)
            {
                return true;
            }
            else
            {
                obj?.Dispose();
                return false;
            }
        }

        private bool ConnectionExists()
        {
            if (_connection != null)
            {
                return true;
            }

            return _connection != null;
        }
    }
}
