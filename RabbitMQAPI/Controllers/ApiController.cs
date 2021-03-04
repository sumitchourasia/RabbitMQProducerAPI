using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQAPI.RabbitMQ;
using RabbitMQAPI.RequestResponseLog;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace RabbitMQAPI.Controllers
{
    [ApiController]
    [Route("api")]
    [Produces("application/json")]
    public class ApiController : ControllerBase
    {
        private readonly ILogger<ApiController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IRabbitMqCRED _rabbitMqCRED;
        // private readonly 
        public ApiController(IConfiguration configuration , ILogger<ApiController> logger, IRabbitMqCRED rabbitMqCRED)
        {
            _configuration = configuration;
            _logger = logger;
            _rabbitMqCRED = rabbitMqCRED;
        }

        // GET: api/<ApiController>
        [HttpGet("get")]
        public RequestResponse Get()
        {
            RequestResponse requestResponse = null;
            try
            {
                 requestResponse = _rabbitMqCRED.Read<RequestResponse>(_configuration["RabbitMQ:QueueName"]);
            }
            catch(Exception ex)
            {
                return null;
            }
            return requestResponse;
        }

        //// GET api/<ApiController>/5
        [HttpGet("/get/{queueName}")]
        public RequestResponse Get(string queueName)
        {
            RequestResponse requestResponse = null;
            try
            {
                if (queueName == null)
                    return null;
                requestResponse = _rabbitMqCRED.Read<RequestResponse>(queueName);
            }
            catch (Exception ex)
            {
                return null;
            }
            return requestResponse;
        }

        // POST api/<ApiController>
        [HttpPost("publish")]
        public bool Post(RequestResponse requestResponse)
        {
            _rabbitMqCRED.Write<RequestResponse>(requestResponse, _configuration["RabbitMQ:QueueName"]);
            return true;

        }

    //    // PUT api/<ApiController>/5
    //    [HttpPut("{id}")]
    //    public void Put(int id, [FromBody] string value)
    //    {
    //    }

    //    // DELETE api/<ApiController>/5
    //    [HttpDelete("{id}")]
    //    public void Delete(int id)
    //    {
    //    }
    }
}
