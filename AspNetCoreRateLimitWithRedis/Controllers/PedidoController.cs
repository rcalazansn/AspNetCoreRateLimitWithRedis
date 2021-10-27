using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace AspNetCoreRateLimitWithRedis.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PedidoController : ControllerBase
    {
        private readonly ILogger<PedidoController> _logger;

        public PedidoController(ILogger<PedidoController> logger)
        {
            _logger = logger;
        }

        [HttpGet("/pedido")]
        public string GetOrder() => $"OrderId { Guid.NewGuid()} - {DateTime.Now.ToString("hh:mm:ss")}";
    }
}
