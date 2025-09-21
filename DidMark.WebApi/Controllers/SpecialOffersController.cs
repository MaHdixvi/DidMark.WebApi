using DidMark.Core.DTO.Products.SpecialOffers;
using DidMark.Core.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DidMark.WebApi.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]

    public class SpecialOffersController : SiteBaseController
    {
        private readonly ISpecialOfferService _specialOfferService;

        public SpecialOffersController(ISpecialOfferService specialOfferService)
        {
            _specialOfferService = specialOfferService;
        }

        // GET: api/SpecialOffers/{id}
        [HttpGet("{id}")]
        public IActionResult GetOfferById(long id)
        {
            var offer = _specialOfferService.GetByIdAsync(id);
            if (offer == null) return NotFound();
            return Ok(offer);
        }

        // GET: api/SpecialOffers
        [HttpGet]
        public IActionResult GetAllOffers()
        {
            return Ok(_specialOfferService.GetAllAsync());
        }

        // POST: api/SpecialOffers
        [HttpPost]
        public IActionResult CreateOffer([FromBody] CreateSpecialOfferDto dto)
        {
            _specialOfferService.CreateOfferAsync(dto);
            return Ok(new { message = "Offer created successfully" });
        }

        // PUT: api/SpecialOffers/{id}
        [HttpPut("{id}")]
        public IActionResult UpdateOffer(long id, [FromBody] UpdateSpecialOfferDto dto)
        {
            if (id != dto.Id) return BadRequest();
            _specialOfferService.UpdateOfferAsync(dto);
            return Ok(new { message = "Offer updated successfully" });
        }

        // DELETE: api/SpecialOffers/{id}
        [HttpDelete("{id}")]
        public IActionResult DeleteOfferById(long id)
        {
            _specialOfferService.DeleteOfferAsync(id);
            return Ok(new { message = "Offer deleted successfully" });
        }

        // GET: api/SpecialOffers/active
        [HttpGet("active")]
        public IActionResult GetActiveOffer()
        {
            var offer = _specialOfferService.GetActiveOfferAsync();
            if (offer == null) return NotFound(new { message = "No active offer" });
            return Ok(offer);
        }

        // POST: api/SpecialOffers/assign
        [HttpPost("assign")]
        public async Task<IActionResult> AssignOfferToProduct([FromBody] AssignOfferToProductDto dto)
        {
            await _specialOfferService.AssignOfferToProductAsync(dto);
            return Ok(new { message = "Offer assigned to product successfully" });
        }

        // PUT: api/SpecialOffers/assign
        [HttpPut("assign")]
        public async Task<IActionResult> UpdateOfferAssignment([FromBody] EditOfferProductDto dto)
        {
            var result = await _specialOfferService.UpdateOfferForProductAsync(dto);
            if (!result) return NotFound(new { message = "Offer assignment not found" });
            return Ok(new { message = "Offer assignment updated successfully" });
        }

        // DELETE: api/SpecialOffers/assign/{id}
        [HttpDelete("assign/{id}")]
        public async Task<IActionResult> RemoveOfferAssignment(long id)
        {
            var result = await _specialOfferService.RemoveOfferFromProductAsync(id);
            if (!result) return NotFound(new { message = "Offer assignment not found" });
            return Ok(new { message = "Offer assignment removed successfully" });
        }
    }
}
