using RabbitMQAPI.RequestResponseLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RabbitMQAPI.RabbitMQ
{
    public interface IRabbitMqCRED
    {

        /// <summary>
        /// fetch and read from queue
        /// </summary>
        T Read<T>( string queueName) where T : class;

        /// <summary>
        /// publish or insert into queue
        /// </summary>
        void Write<T>(T message, string queueName)
           where T : class;

        /// <summary>
        /// update in the queue
        /// </summary>
        void Update<T>(T message, string queueName)
           where T : class;

        /// <summary>
        /// delete from queue
        /// </summary>
        void Delete<T>(T message, string queueName)
           where T : class;

       
    }
}
