using DidMark.Core.Services.Interfaces;
using DidMark.Core.Utilities.Common;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DidMark.WebApi.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class SliderController : SiteBaseController
    {
        #region Fields
        private readonly ISliderService _sliderService;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="SliderController"/> class.
        /// </summary>
        /// <param name="sliderService">The slider service for handling slider-related operations.</param>
        public SliderController(ISliderService sliderService)
        {
            _sliderService = sliderService ?? throw new ArgumentNullException(nameof(sliderService));
        }
        #endregion

        #region Get Active Sliders
        /// <summary>
        /// Retrieves all active sliders.
        /// </summary>
        /// <returns>A JSON response containing the list of active sliders.</returns>
        [HttpGet("projects")]
        public async Task<IActionResult> GetActiveSliders()
        {
            var sliders = await _sliderService.GetActiveSliders();
            return JsonResponseStatus.Success(sliders);
        }
        #endregion

        #region Get Single Slider
        /// <summary>
        /// Retrieves a single slider by its ID.
        /// </summary>
        /// <param name="id">The ID of the slider.</param>
        /// <returns>A JSON response containing the slider details, or a not found status if the slider does not exist.</returns>
        [HttpGet("projects/{id}")]
        public async Task<IActionResult> GetSlider(long id)
        {
            if (id <= 0)
            {
                return JsonResponseStatus.BadRequest(new { message = "شناسه اسلایدر نامعتبر است" });
            }

            var slider = await _sliderService.GetSliderById(id);
            if (slider == null)
            {
                return JsonResponseStatus.NotFound(new { message = "اسلایدر یافت نشد" });
            }

            return JsonResponseStatus.Success(slider);
        }
        #endregion
    }
}
