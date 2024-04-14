using CustomerService.Models;
using CustomerService.Models.ModelDto;
using CustomerService.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CustomerService.Controllers
{

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

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<CampaignReadDto>>> GetCampaigns()
        {
            var campaigns = await _campaignService.GetAllCampaignsAsync();
            return Ok(campaigns);
        }

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

        //TODO: Check if this is okay to have
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
