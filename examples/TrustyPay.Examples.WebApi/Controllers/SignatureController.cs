
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

        public SignatureController(
            ILogger<SignatureController> logger
        )
        {
            _logger = logger;
        }

        [HttpGet(""), SignatureVerification]
        public ActionResult<IEnumerable<Employee>> GetEmployees(string filter)
        {
            _logger.LogInformation("filter=" + filter);
            return new Employee[] {
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
    }
}