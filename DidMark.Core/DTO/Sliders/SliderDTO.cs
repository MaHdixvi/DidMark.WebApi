using System;

namespace DidMark.Core.DTO.Slider
{
    public class SliderDTO
    {
        public long Id { get; set; }
        public string Title { get; set; }             // جایگزین ProductName
        public string? Description { get; set; }     // اختیاری
        public string ImageUrl { get; set; }         // جایگزین Image
        public string? Link { get; set; }            // اختیاری
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }   // اختیاری
        public string ProductName { get; set; }

    }
}
