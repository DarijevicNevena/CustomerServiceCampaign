using Azure.Core;
using CustomerService.Models;
using CustomerService.Services.Contracts;
using CustomerService.Validators.EntityValidators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Runtime.Intrinsics.X86;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using static Azure.Core.HttpHeader;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CustomerService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PurchaseController : ControllerBase
    {
        private readonly IPurchaseService _purchaseService;
        private readonly ICustomerService _customerService;
        private readonly PurchaseValidator _purchaseValidator;
        private readonly HttpClient _httpClient;

        public PurchaseController(IPurchaseService purchaseService, ICustomerService customerService, PurchaseValidator purchaseValidator, HttpClient httpClient)
        {
            _purchaseService = purchaseService;
            _customerService = customerService;
            _purchaseValidator = purchaseValidator;
            _httpClient = httpClient;
        }

        // GET: api/<PurchaseController>
        [HttpGet]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<Purchase>>> GetPurchases()
        {
            var purchases = await _purchaseService.GetAllPurchasesAsync();
            return Ok(purchases);
        }

        // GET api/<PurchaseController>/5
        [HttpGet("{id:int}", Name = "GetPurchaseById")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<Purchase>> GetPurchaseById(int id)
        {
            try
            {
                var purchase = await _purchaseService.GetPurchaseByIdAsync(id);
                return Ok(purchase);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        // POST api/<PurchaseController>
        [HttpPost("create", Name = "CreatePurchase")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<Purchase>> CreatePurchase([FromBody] Purchase purchase)
        {
            try
            {
                // Validate other data in request
                var validationResult = await _purchaseValidator.ValidateAsync(purchase);
                if (!validationResult.IsValid)
                {
                    var errorResponse = validationResult.Errors.Select(e => new { Field = e.PropertyName, Error = e.ErrorMessage }).ToList();
                    return BadRequest(new { errors = errorResponse });
                }

                // Check if customer with passed Id exists
                var customerExists = await _customerService.DoesCustomerExist(purchase.CustomerId);
                if (!customerExists)
                {
                    ModelState.AddModelError("CustomError", "Customer does not exist!");
                    return BadRequest(ModelState);
                }

                var createdPurchase = await _purchaseService.CreateNewPurchaseAsync(purchase);
                return CreatedAtAction(nameof(GetPurchaseById), new { id = createdPurchase.Id }, createdPurchase);
            }
            catch (ArgumentNullException)
            {
                return BadRequest("Purchase cannot be null.");
            }
            catch (KeyNotFoundException)
            {
                return BadRequest("Invalid data sent.");
            }
        }

        // PUT api/<PurchaseController>/5
        [HttpPut("{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public void Put(int id, [FromBody] string value)
        {
            //Can purchase be changed???
        }

        // DELETE api/<PurchaseController>/5]
        [HttpDelete("{id:int}", Name = "DeletePurchase")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeletePurchase(int id)
        {
            try
            {
                await _purchaseService.DeletePurchaseAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpGet("customer/{customerId:int}", Name = "GetCustomerInfoById")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<Customer>> GetCustomerInfoById(int customerId)
        {
            if (customerId <= 0)
                return BadRequest("Invalid customer ID");

            string soapApiUrl = $"https://www.crcind.com/csp/samples/SOAP.Demo.cls?soap_method=FindPerson&id={customerId}";

            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(soapApiUrl);

                if (!response.IsSuccessStatusCode)
                    return StatusCode((int)response.StatusCode, "SOAP service returned an error");

                string xmlResponse = await response.Content.ReadAsStringAsync();
                XDocument xmlDoc = XDocument.Parse(xmlResponse);

                XNamespace ns = "http://tempuri.org";
                XElement customerElement = xmlDoc.Descendants(ns + "FindPersonResult").FirstOrDefault();

                if (customerElement == null)
                    return NotFound();

                Customer customer = new Customer
                {
                    Id = customerId,
                    Name = customerElement.Element(ns + "Name")?.Value,
                    SSN = customerElement.Element(ns + "SSN")?.Value,
                    DOB = DateTime.TryParse(customerElement.Element(ns + "DOB")?.Value, out DateTime dob) ? dob : default,
                    HomeAddress = new Address
                    {
                        Street = customerElement.Element(ns + "Home")?.Element(ns + "Street")?.Value,
                        City = customerElement.Element(ns + "Home")?.Element(ns + "City")?.Value,
                        State = customerElement.Element(ns + "Home")?.Element(ns + "State")?.Value,
                        Zip = customerElement.Element(ns + "Home")?.Element(ns + "Zip")?.Value
                    },
                    OfficeAddress = new Address
                    {
                        Street = customerElement.Element(ns + "Office")?.Element(ns + "Street")?.Value,
                        City = customerElement.Element(ns + "Office")?.Element(ns + "City")?.Value,
                        State = customerElement.Element(ns + "Office")?.Element(ns + "State")?.Value,
                        Zip = customerElement.Element(ns + "Office")?.Element(ns + "Zip")?.Value
                    },
                    Age = int.TryParse(customerElement.Element(ns + "Age")?.Value, out int age) ? age : default
                };

                return Ok(customer);
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, "Error accessing SOAP service");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Server error");
            }
        }

        [HttpGet("customer-exists/{customerId:int}", Name = "CustomerExistCheck")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CustomerExistCheck(int customerId)
        {
            if (customerId <= 0)
                return BadRequest("Invalid customer ID");
            string soapApiUrl = $"https://www.crcind.com/csp/samples/SOAP.Demo.cls?soap_method=FindPerson&id={customerId}";

            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(soapApiUrl);

                if (!response.IsSuccessStatusCode)
                    return StatusCode((int)response.StatusCode, "SOAP service returned an error");

                string xmlResponse = await response.Content.ReadAsStringAsync();
                XDocument xmlDoc = XDocument.Parse(xmlResponse);

                XNamespace ns = "http://tempuri.org";
                XElement customerElement = xmlDoc.Descendants(ns + "FindPersonResult").FirstOrDefault();

                if (customerElement == null)
                    return NotFound();

                return Ok(true);
            }
            catch (HttpRequestException ex)
            {
                return StatusCode(500, "Error accessing SOAP service");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Server error");
            }
        }
    }
}
