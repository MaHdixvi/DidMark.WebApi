using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace DidMark.Core.DTO.Slider
{
    public class EditSliderDTO
    {
        [Required]
        public long Id { get; set; }

        [MaxLength(100)]
        [Display(Name = "اسم محصول")]
        public string? ProductName { get; set; }

        [Display(Name = "توضیحات")]
        public string? Description { get; set; }

        [Display(Name = "تصویر")]
        public IFormFile? Image { get; set; }

        public bool? IsActive { get; set; }

    }
}
