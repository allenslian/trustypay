
using System;
using System.Collections.Generic;
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
        public ActionResult<IEnumerable<Employee>> GetEmployees([FromQuery] string filter)
        {
            _logger.LogInformation("filter=" + filter);
            return Ok(_employees);
        }

        [HttpGet("v1"), SignatureVerification]
        public ActionResult<IEnumerable<Employee>> GetEmployeesV1([FromQuery] bool filter)
        {
            _logger.LogInformation("filter=" + filter);
            return Ok(_employees);
        }

        [HttpGet("v2"), SignatureVerification]
        public ActionResult<IEnumerable<Employee>> GetEmployeesV2([FromQuery] EmployeeCriteria criteria)
        {
            _logger.LogInformation("filter=" + criteria.Name);
            return Ok(_employees);
        }

        [HttpGet("v3"), SignatureVerification]
        public ActionResult<IEnumerable<Employee>> GetEmployeesV3([FromQuery] string[] names)
        {
            _logger.LogInformation("filter=" + names.Length);
            return Ok(_employees);
        }

        [HttpPost(""), SignatureVerification]
        public ActionResult<Result> CreateEmployees([FromBody] Employee employee)
        {
            _logger.LogInformation("no=" + employee.No + ";name=" + employee.Name + ";age=" + employee.Age);
            _employees.Add(employee);
            return Ok(new Result { IsOk = true, Message = null });
        }

        [HttpPost("v2")]
        public ActionResult<Result> CreateEmployeesV2([FromBody] Guid no)
        {
            _logger.LogInformation("no=" + no.ToString());
            return Ok(new Result { IsOk = true, Message = null });
        }

        [HttpPost("v3"), SignatureVerification]
        public ActionResult<Result> CreateEmployeesV3([FromBody] bool filter)
        {
            _logger.LogInformation("no=" + filter.ToString());
            return Ok(new Result { IsOk = true, Message = null });
        }

        [HttpPost("v4")]
        public ActionResult<Result> CreateEmployeesV4([FromBody] bool filter)
        {
            _logger.LogInformation("no=" + filter.ToString());
            return Ok(new Result { IsOk = true, Message = null });
        }
    }

    public class Result
    {
        public bool IsOk { get; set; }

        public string Message { get; set; }
    }
}