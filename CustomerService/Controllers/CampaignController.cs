using CustomerService.Models.ModelDto;
using CustomerService.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CustomerService.Controllers
{
    /// <summary>
    /// Manages campaign-related operations.
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
        /// Sample request: GET /api/Campaign
        /// </summary>
        /// <returns>A list of campaigns.</returns>
        /// <response code="200">Returns the list of campaigns</response>
        /// <response code="401">If the user is unauthorized</response>
        [HttpGet]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<CampaignReadDto>>> GetCampaigns()
        {
            var campaigns = await _campaignService.GetAllCampaignsAsync();
            return Ok(campaigns);
        }

        /// <summary>
        /// Retrieves a specific campaign by its ID.
        /// Sample request: GET /api/Campaign/{id}
        /// </summary>
        /// <param name="id">The ID of the campaign to retrieve.</param>
        /// <returns>The requested campaign or not found.</returns>
        /// <response code="200">Returns the requested campaign</response>
        /// <response code="404">If the campaign is not found</response>
        /// <response code="401">If the user is unauthorized</response>
        [HttpGet("{id:int}", Name = "GetCampaignById")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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
        /// Exports purchases related to a specific campaign into a CSV file.
        /// Sample request: GET /api/Campaign/export/{campaignId}
        /// </summary>
        /// <param name="campaignId">The ID of the campaign for which to generate the report.</param>
        /// <returns>A CSV file containing purchase details or an error message.</returns>
        /// <response code="200">Returns the CSV file</response>
        /// <response code="400">If an error occurs during data generation</response>
        /// <response code="404">If the campaign is not found</response>
        /// <response code="500">If an internal server error occurs</response>
        [HttpGet("export/{campaignId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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

        /// <summary>
        /// Creates a new campaign.
        /// Sample request: POST /api/Campaign
        /// </summary>
        /// <param name="campaign">The campaign DTO containing creation details.</param>
        /// <returns>The created campaign data or bad request.</returns>
        /// <response code="200">Returns the created campaign</response>
        /// <response code="400">If the campaign data is null or invalid</response>
        /// <response code="401">If the user is unauthorized</response>
        [HttpPost]
        [Authorize]
        [Route("/createCampaign")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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
        /// Sample request: DELETE /api/Campaign/{id}
        /// </summary>
        /// <param name="id">The ID of the campaign to delete.</param>
        /// <returns>A status indicating the outcome of the operation.</returns>
        /// <response code="204">If the campaign was successfully deleted</response>
        /// <response code="404">If the campaign is not found</response>
        /// <response code="401">If the user is unauthorized</response>
        [HttpDelete("{id:int}", Name = "DeleteCampaign")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
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
    }
}
