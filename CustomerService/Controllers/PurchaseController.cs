using Azure.Core;
using CustomerService.Models;
using CustomerService.Models.ModelDto;
using CustomerService.Services.Contracts;
using CustomerService.Validators.EntityValidators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Runtime.Intrinsics.X86;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using static Azure.Core.HttpHeader;

namespace CustomerService.Controllers
{
    /// <summary>
    /// Manages purchase-related operations
    /// </summary>
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

        /// <summary>
        /// Returns all purchases.
        /// </summary>
        /// <returns>A list of all purchases.</returns>
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Purchase>>> GetPurchases()
        {
            var purchases = await _purchaseService.GetAllPurchasesAsync();
            return Ok(purchases);
        }

        /// <summary>
        /// Returns a purchase by ID.
        /// </summary>
        /// <param name="id">The ID of the purchase to get.</param>
        /// <returns>The purchase if found; otherwise, a 404 Not Found status.</returns>
        [HttpGet("{id:int}", Name = "GetPurchaseById")]
        [Authorize]
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

        /// <summary>
        /// Creates a new purchase.
        /// </summary>
        /// <param name="purchaseDto">The purchase dto containing the details for creation.</param>
        /// <returns>The created purchase.</returns>
        [HttpPost("create", Name = "CreatePurchase")]
        [Authorize]
        public async Task<ActionResult<PurchaseReadDto>> CreatePurchase([FromBody] PurchaseWriteDto purchaseDto)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }

            // Retrieve agent ID from token
            var agentId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(agentId))
            {
                return Unauthorized("Agent ID is missing in the token.");
            }
            purchaseDto.AgentId = int.Parse(agentId);
            var validationResult = await _purchaseValidator.ValidateAsync(purchaseDto);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.ValidationResult);
            }

            var createdPurchase = await _purchaseService.CreateNewPurchaseAsync(purchaseDto, validationResult.CampaignId.Value);
            return Ok(createdPurchase);
        }

        /// <summary>
        /// Deletes a purchase by ID.
        /// </summary>
        /// <param name="id">The ID of the purchase to delete.</param>
        /// <returns>A status indicating the outcome of the operation.</returns>
        [HttpDelete("{id:int}", Name = "DeletePurchase")]
        [Authorize]
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

        /// <summary>
        /// Returns customer information by ID, using an external SOAP service.
        /// </summary>
        /// <param name="customerId">The ID of the customer.</param>
        /// <returns>Customer information if found; otherwise, a status indicating the error.</returns>
        [HttpGet("customer/{customerId:int}", Name = "GetCustomerInfoById")]
        [Authorize]
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

        /// <summary>
        /// Checks if a customer exists using an external SOAP service.
        /// </summary>
        /// <param name="customerId">The ID of the customer to check.</param>
        /// <returns>True-customer exists, False-it doesn't</returns>
        [HttpGet("customer-exists/{customerId:int}", Name = "CustomerExistCheck")]
        [Authorize]
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
