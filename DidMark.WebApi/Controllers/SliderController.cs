using DidMark.Core.Services.Interfaces;
using DidMark.Core.Utilities.Common;
using Microsoft.AspNetCore.Mvc;

namespace DidMark.WebApi.Controllers
{
    public class SliderController : SiteBaseController
    {
        #region constructor

        private ISliderService sliderService;

        public SliderController(ISliderService sliderService)
        {
            this.sliderService = sliderService;
        }

        #endregion

        #region all active sliders

        [HttpGet("Projects")]
        public async Task<IActionResult> GetActiveSliders()
        {
            Thread.Sleep(4000);
            var sliders = await sliderService.GetActiveSliders();

            return JsonResponseStatus.Success(sliders);
        }

        #endregion

        #region get single product

        [HttpGet("Projects/{id}")]
        public async Task<IActionResult> GetProduct(long id)
        {
            var slider = await sliderService.GetSliderById(id);

            if (slider != null)
                return JsonResponseStatus.Success(slider);

            return JsonResponseStatus.NotFound();
        }

        #endregion
    }
}

