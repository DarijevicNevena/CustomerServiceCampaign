using CustomerService.Models;
using CustomerService.Models.ModelDto;
using CustomerService.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CustomerService.Controllers
{
    /// <summary>
    /// Manages campaign-related operations
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CampaignController : ControllerBase
    {
        private readonly ICampaignService _campaignService;
        private readonly ICampaignReportService _reportService;

        public CampaignController(ICampaignService campaignService, ICampaignReportService reportService)
        {
            _campaignService = campaignService;
            _reportService = reportService;
        }

        /// <summary>
        /// Retrieves all campaigns.
        /// </summary>
        /// <returns>A list of campaigns.</returns>
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<CampaignReadDto>>> GetCampaigns()
        {
            var campaigns = await _campaignService.GetAllCampaignsAsync();
            return Ok(campaigns);
        }

        /// <summary>
        /// Retrieves a specific campaign by its ID.
        /// </summary>
        /// <param name="id">The ID of the campaign to retrieve.</param>
        /// <returns>Return founded campaign or Not Found response</returns>
        [HttpGet("{id:int}", Name = "GetCampaignById")]
        [Authorize]
        public async Task<ActionResult<CampaignReadDto>> GetCampaignById(int id)
        {
            try
            {
                var campaign = await _campaignService.GetCampaignByIdAsync(id);
                return Ok(campaign);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Creates a new campaign.
        /// </summary>
        /// <param name="campaign">The campaign dto containing creation details.</param>
        /// <returns>The created campaign data.</returns>
        [HttpPost]
        [Authorize]
        [Route("/createCampaign")]
        public async Task<ActionResult<CampaignReadDto>> CreateCampaign([FromBody] CampaignWriteDto campaign)
        {
            try
            {
                var createdCampaign = await _campaignService.CreateNewCampaignAsync(campaign);
                return Ok(createdCampaign);
            }
            catch (ArgumentNullException)
            {
                return BadRequest("Campaign cannot be null.");
            }
        }

        /// <summary>
        /// Deletes a campaign by its ID.
        /// </summary>
        /// <param name="id">The ID of the campaign to delete.</param>
        /// <returns>A status indicating the outcome of the operation.</returns>
        [HttpDelete("{id:int}", Name = "DeleteCampaign")]
        [Authorize]
        public async Task<IActionResult> DeleteCampaign(int id)
        {
            try
            {
                await _campaignService.DeleteCampaignAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Exports purchases related to a specific campaign into a CSV file.
        /// </summary>
        /// <param name="campaignId">The ID of the campaign for which to generate the report.</param>
        /// <returns>A CSV file containing purchase details.</returns>
        [HttpGet("export/{campaignId}")]
        public async Task<IActionResult> ExportPurchasesToCsv(int campaignId)
        {
            try
            {
                var csvData = await _reportService.GenerateCampaignPurchasesReport(campaignId);
                return File(new System.Text.UTF8Encoding().GetBytes(csvData), "text/csv", $"campaign_{campaignId}_purchases.csv");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while generating the report: " + ex.Message);
            }
        }
    }
}
