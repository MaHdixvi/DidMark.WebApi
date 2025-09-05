using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DidMark.Core.DTO.Slider
{
    public class SliderDTO
    {
        public long Id { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }
        public string ProductName { get; set; }
        public bool IsActive { get; set; }
    }
}

