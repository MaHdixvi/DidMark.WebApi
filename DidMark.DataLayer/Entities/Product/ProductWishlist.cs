using DidMark.DataLayer.Entities.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DidMark.DataLayer.Entities.Products
{
    public class ProductWishlist : BaseEntity
    {
        #region properties

        [Display(Name = "کاربر")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public long UserId { get; set; }

        [Display(Name = "محصول")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public long ProductId { get; set; }

        #endregion

        #region relations

        [ForeignKey("ProductId")]
        public virtual Product.Product Product { get; set; }

        #endregion
    }
}