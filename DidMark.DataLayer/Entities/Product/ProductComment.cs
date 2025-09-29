using DidMark.DataLayer.Entities.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DidMark.DataLayer.Entities.Comments
{
    public class ProductComment : BaseEntity
    {
        #region properties

        [Display(Name = "محصول")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public long ProductId { get; set; }

        [Display(Name = "کاربر")]
        public long? UserId { get; set; }

        [Display(Name = "نام کاربر")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(100, ErrorMessage = "تعداد کاراکتر های {0} نمیتواند بیشتر از {1} باشد")]
        public string UserName { get; set; }

        [Display(Name = "ایمیل کاربر")]
        [MaxLength(150, ErrorMessage = "تعداد کاراکتر های {0} نمیتواند بیشتر از {1} باشد")]
        [EmailAddress(ErrorMessage = "فرمت ایمیل معتبر نیست")]
        public string UserEmail { get; set; }

        [Display(Name = "متن کامنت")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(1000, ErrorMessage = "تعداد کاراکتر های {0} نمیتواند بیشتر از {1} باشد")]
        public string CommentText { get; set; }

        [Display(Name = "امتیاز")]
        [Range(1, 5, ErrorMessage = "امتیاز باید بین 1 تا 5 باشد")]
        public int? Rating { get; set; }

        [Display(Name = "کامنت والد")]
        public long? ParentId { get; set; }

        [Display(Name = "تایید شده")]
        public bool IsApproved { get; set; }

        [Display(Name = "خوانده شده")]
        public bool IsRead { get; set; }

        [Display(Name = "IP کاربر")]
        [MaxLength(50, ErrorMessage = "تعداد کاراکتر های {0} نمیتواند بیشتر از {1} باشد")]
        public string UserIp { get; set; }

        #endregion

        #region relations

        [ForeignKey("ProductId")]
        public virtual Product.Product Product { get; set; }

        [ForeignKey("ParentId")]
        public virtual ProductComment Parent { get; set; }

        public virtual ICollection<ProductComment> Replies { get; set; }

        #endregion
    }
}