
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TrustyPay.Core.Cryptography.Http.Service;
using TrustyPay.Examples.WebApi.Models;

namespace TrustyPay.Examples.WebApi.Controllers
{
    [Route("api/v1/signatures")]
    public class SignatureController : ControllerBase
    {
        private readonly ILogger<SignatureController> _logger;

        private List<Employee> _employees;

        public SignatureController(
            ILogger<SignatureController> logger
        )
        {
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
            _logger = logger;
        }

        [HttpGet(""), SignatureVerification]
        public ActionResult<IEnumerable<Employee>> GetEmployees([FromQuery] string name)
        {
            _logger.LogInformation("name=" + name);
            return Ok(_employees.Where(m => m.Name.Contains(name)).ToArray());
        }

        [HttpGet("v1"), SignatureVerification]
        public ActionResult<IEnumerable<Employee>> GetEmployeesV1([FromQuery] int age)
        {
            _logger.LogInformation("age=" + age);
            return Ok(_employees.Where(m => m.Age > age).ToArray());
        }

        [HttpGet("v2"), SignatureVerification]
        public ActionResult<IEnumerable<Employee>> GetEmployeesV2([FromQuery] string[] names)
        {
            _logger.LogInformation("names=" + string.Join(',', names));
            return Ok(_employees.Where(m => names.Contains(m.Name)).ToArray());
        }

        [HttpGet("v3"), SignatureVerification]
        public ActionResult<IEnumerable<Employee>> GetEmployeesV3([FromQuery] EmployeeCriteria criteria)
        {
            _logger.LogInformation("names=" + criteria.Name + ";age=" + criteria.Age);
            return Ok(_employees.Where(m => m.Name.Contains(criteria.Name) || m.Age > criteria.Age).ToArray());
        }

        [HttpGet("v4"), SignatureVerification]
        public ActionResult<IEnumerable<Employee>> GetEmployeesV4([FromQuery] string[] bizContent)
        {
            _logger.LogInformation("bizContent=" + string.Join(',', bizContent));
            return Ok(_employees.Where(m => bizContent.Contains(m.Name)).ToArray());
        }

        [HttpGet("v5"), SignatureVerification]
        public ActionResult<IEnumerable<Employee>> GetEmployeesV4([FromQuery] bool bizContent)
        {
            _logger.LogInformation("bizContent=" + bizContent.ToString());
            return Ok(_employees.ToArray());
        }

        [HttpPost(""), SignatureVerification]
        public ActionResult<Result> CreateEmployees([FromBody] Employee employee)
        {
            _logger.LogInformation("no=" + employee.No + ";name=" + employee.Name + ";age=" + employee.Age);
            _employees.Add(employee);
            return Ok(new Result { IsOk = true, Message = null });
        }

        [HttpPost("v1"), SignatureVerification]
        public ActionResult<Result> CreateEmployeesV2([FromBody] Guid no)
        {
            _logger.LogInformation("no=" + no.ToString());
            return Ok(new Result { IsOk = true, Message = null });
        }

        [HttpPost("v2"), SignatureVerification]
        public ActionResult<Result> CreateEmployeesV3([FromBody] bool filter)
        {
            _logger.LogInformation("filter=" + filter.ToString());
            return Ok(new Result { IsOk = true, Message = null });
        }

        [HttpPost("v3")]
        public ActionResult<Result> CreateEmployeesV4([FromBody] string filter)
        {
            _logger.LogInformation("filter=" + filter.ToString());
            return Ok(new Result { IsOk = true, Message = null });
        }
    }

    public class Result
    {
        public bool IsOk { get; set; }

        public string Message { get; set; }
    }
}