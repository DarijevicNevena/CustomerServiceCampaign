using CustomerService.Models;
using CustomerService.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CustomerService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CampaignController : ControllerBase
    {
        private readonly ICampaignService _campaignService;

        public CampaignController(ICampaignService campaignService)
        {
            _campaignService = campaignService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Campaign>>> GetCampaigns()
        {
            var campaigns = await _campaignService.GetAllCampaignsAsync();
            return Ok(campaigns);
        }

        [HttpGet("{id:int}", Name = "GetCampaignById")]
        public async Task<ActionResult<Campaign>> GetCampaignById(int id)
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
        [Route("/createCampaign")]
        public async Task<ActionResult<Campaign>> CreateCampaign([FromBody] Campaign campaign)
        {
            try
            {
                var createdCampaign = await _campaignService.CreateNewCampaignAsync(campaign);
                return CreatedAtAction(nameof(GetCampaignById), new { id = createdCampaign.Id }, createdCampaign);
            }
            catch (ArgumentNullException)
            {
                return BadRequest("Campaign cannot be null.");
            }
        }

        //TODO: Check if this is okay to have
        [HttpDelete("{id:int}", Name = "DeleteCampaign")]
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
