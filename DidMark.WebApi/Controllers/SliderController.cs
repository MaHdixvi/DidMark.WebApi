using DidMark.Core.DTO.Slider;
using DidMark.Core.Services.Interfaces;
using DidMark.Core.Utilities.Common;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DidMark.WebApi.Controllers
{
    public class SliderController : SiteBaseController
    {
        private readonly ISliderService _sliderService;

        public SliderController(ISliderService sliderService)
        {
            _sliderService = sliderService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllSliders()
        {
            var sliders = await _sliderService.GetAllSliders();
            return JsonResponseStatus.Success(sliders);
        }

        [HttpGet("active")]
        public async Task<IActionResult> GetActiveSliders()
        {
            var sliders = await _sliderService.GetActiveSliders();
            return JsonResponseStatus.Success(sliders);
        }

        [HttpGet("{id:long}")]
        public async Task<IActionResult> GetSlider(long id)
        {
            var slider = await _sliderService.GetSliderById(id);
            if (slider == null)
                return JsonResponseStatus.NotFound(new { message = "اسلایدر یافت نشد" });

            return JsonResponseStatus.Success(slider);
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddSlider([FromForm] AddSliderDTO dto)
        {
            var (success, sliderId) = await _sliderService.AddSlider(dto);
            if (!success)
                return JsonResponseStatus.Error(new { message = "خطا در افزودن اسلایدر" });

            return JsonResponseStatus.Success(new { message = "اسلایدر با موفقیت اضافه شد", id = sliderId });
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateSlider([FromForm] EditSliderDTO dto)
        {
            var (success, sliderId) = await _sliderService.UpdateSlider(dto);
            if (!success)
                return JsonResponseStatus.NotFound(new { message = "اسلایدر یافت نشد" });

            return JsonResponseStatus.Success(new { message = "اسلایدر با موفقیت ویرایش شد", id = sliderId });
        }

        [HttpDelete("{id:long}")]
        public async Task<IActionResult> DeleteSlider(long id)
        {
            var success = await _sliderService.DeleteSlider(id);
            if (!success)
                return JsonResponseStatus.NotFound(new { message = "اسلایدر یافت نشد" });

            return JsonResponseStatus.Success(new { message = "اسلایدر با موفقیت حذف شد" });
        }
    }
}
