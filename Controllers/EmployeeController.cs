using Microsoft.AspNetCore.Mvc;
using MiddlewareAndElasticSearchProject.Model;

namespace MiddlewareAndElasticSearchProject.Controllers
{
    [ApiController]
    [Route("api")]
    public class EmployeeController : ControllerBase
    {
        private static readonly List<Employee> employees =
            [
                new Employee { Id = 1, Name = "Ümit", Email="umitvuran@gmail.com" }
            ];

        private readonly ILogger<EmployeeController> _logger;

        public EmployeeController(ILogger<EmployeeController> logger)
        {
            _logger = logger;
        }

        [HttpGet("employee/{id:int}")]
        public IActionResult Get(int id)
        {
            _logger.LogTrace("LogTrace *****************");
            _logger.LogDebug("LogDebug *****************");
            _logger.LogInformation("LogInformation *****************");
            //_logger.LogWarning("LogWarning *****************");
            //_logger.LogError("LogError *****************");

            if (id == 0)
            {
                return BadRequest("bad request");
            }

            var employee = employees.FirstOrDefault(i =>  i.Id == id);

            if (employee == null)
            {
                return NotFound();
            }

            return Ok(employee);
        }

        [HttpPost("employee")]
        public IActionResult Post([FromBody] Employee employee)
        {
            if (string.IsNullOrEmpty(employee.Name))
            {
                return BadRequest("Employee name cannot be null or empty");
            }

            employee.Id = employees.Max(i => i.Id) + 1;
            employees.Add(employee);

            return Ok(employee);
        }
    }
}
