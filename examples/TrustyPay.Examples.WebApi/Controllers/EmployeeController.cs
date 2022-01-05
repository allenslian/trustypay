
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TrustyPay.Examples.WebApi.Models;

namespace TrustyPay.Examples.WebApi.Controllers
{
    [Route("api/v1/employees")]
    public class EmployeeController : ControllerBase
    {
        private readonly ILogger<EmployeeController> _logger;

        private List<Employee> _employees;

        public EmployeeController(
            ILogger<EmployeeController> logger
        )
        {
            _logger = logger;
            _employees = new List<Employee>{
                new Employee{
                    Name = "allen lian",
                    No = "001",
                    Age = 40
                },
                new Employee{
                    Name = "griffin li",
                    No = "002",
                    Age = 50
                }
            };
        }

        [HttpGet("")]
        public ActionResult<IEnumerable<Employee>> GetEmployees([FromQuery] string name)
        {
            _logger.LogInformation("name=" + name);
            return Ok(_employees.Where(m => m.Name.Contains(name)).ToArray());
        }

        [HttpGet("v1")]
        public ActionResult<IEnumerable<Employee>> GetEmployeesV1([FromQuery] int age)
        {
            _logger.LogInformation("age=" + age);
            return Ok(_employees.Where(m => m.Age > age).ToArray());
        }

        [HttpGet("v2")]
        public ActionResult<IEnumerable<Employee>> GetEmployeesV2([FromQuery] string[] names)
        {
            _logger.LogInformation("names=" + string.Join(',', names));
            return Ok(_employees.Where(m => names.Contains(m.Name)).ToArray());
        }

        [HttpGet("v3")]
        public ActionResult<IEnumerable<Employee>> GetEmployeesV3([FromQuery] EmployeeCriteria criteria)
        {
            _logger.LogInformation("names=" + criteria.Name + ";age=" + criteria.Age);
            return Ok(_employees.Where(m => m.Name.Contains(criteria.Name) || m.Age > criteria.Age).ToArray());
        }

    }

}