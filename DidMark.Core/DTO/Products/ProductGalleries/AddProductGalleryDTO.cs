using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DidMark.Core.DTO.Products.NewFolder
{
    public class AddProductGalleryDTO
    {

        [Display(Name = "شناسه محصول")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public long ProductId { get; set; }


        [Display(Name = "تصویر")]
        [Required(ErrorMessage = "لطفا {0} را آپلود کنید")]
        public IFormFile Image { get; set; }
    }

}
