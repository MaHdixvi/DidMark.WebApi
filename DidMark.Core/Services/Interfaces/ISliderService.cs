using DidMark.Core.DTO.Slider;
using DidMark.DataLayer.Entities.Site;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DidMark.Core.Services.Interfaces
{
    public interface ISliderService : IDisposable
    {
        Task<List<SliderDTO>> GetAllSliders();
        Task<List<SliderDTO>> GetActiveSliders();
        Task<(bool Success, long Id)> AddSlider(AddSliderDTO dto);
        Task<(bool Success, long Id)> UpdateSlider(EditSliderDTO dto);
        Task<SliderDTO?> GetSliderById(long sliderId);
        Task<bool> DeleteSlider(long sliderId);
    }
}
