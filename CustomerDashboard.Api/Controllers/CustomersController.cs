using CustomerDashboard.Models.Dtos;
using CustomerDashboard.Models.Entities;
using CustomerDashboard.Services.Implementation;
using CustomerDashboard.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CustomerDashboard.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomersController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpPost("search")]
        public async Task<IActionResult> SearchCustomers([FromBody] FilterModelDto filterModel)
        {
            var res = await _customerService.SearchCustomers(filterModel);
            if (res.IsSuccess)
                return Ok(res.Data);
            else
                return BadRequest(res.Message);

        }

    }
}
