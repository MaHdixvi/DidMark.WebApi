using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace DidMark.Core.DTO.Products.ProductGalleries
{
        public class EditProductGalleryDTO
    {
        [Display(Name = "تصویر")]

        public IFormFile? Image { get; set; }
    }

}
