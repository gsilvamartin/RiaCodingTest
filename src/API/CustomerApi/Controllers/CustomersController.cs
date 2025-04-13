using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using CustomerApi.Models;
using CustomerApi.Services;

namespace CustomerApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CustomersController(CustomerService customerService) : ControllerBase
{
    [HttpGet]
    public ActionResult<IEnumerable<Customer>> GetCustomers()
    {
        return Ok(customerService.GetAllCustomers());
    }

    [HttpPost]
    public ActionResult AddCustomers([FromBody] List<Customer> customers)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (customers.Count < 1)
        {
            return BadRequest("At least one customer must be provided");
        }

        var result = customerService.AddCustomers(customers);
        if (!result)
        {
            return BadRequest("Failed to add customers. Ensure all ages are at least 18 and IDs are unique.");
        }

        return Ok();
    }
}