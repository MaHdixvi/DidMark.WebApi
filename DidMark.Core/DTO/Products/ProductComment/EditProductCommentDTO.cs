using System.ComponentModel.DataAnnotations;

namespace DidMark.Core.DTO.Products.ProductComment
{
    public class EditProductCommentDTO
    {
        public long Id { get; set; }

        [Display(Name = "متن کامنت")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(1000, ErrorMessage = "تعداد کاراکتر های {0} نمیتواند بیشتر از {1} باشد")]
        public string CommentText { get; set; }

        [Display(Name = "امتیاز")]
        [Range(1, 5, ErrorMessage = "امتیاز باید بین 1 تا 5 باشد")]
        public int? Rating { get; set; }

        [Display(Name = "تایید شده")]
        public bool? IsApproved { get; set; }

        [Display(Name = "خوانده شده")]
        public bool? IsRead { get; set; }
    }
}