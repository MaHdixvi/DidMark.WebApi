using DidMark.DataLayer.Entities.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DidMark.DataLayer.Entities.Product
{
    public class ProductCategories : BaseEntity
    {
        #region properties

        public long? ParentId { get; set; }
        [Display(Name = "عنوان")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(150, ErrorMessage = "تعداد کاراکتر های {0} نمیتواند بیشتر از {1} باشد")]
        public string Title { get; set; }
        [Display(Name = "عنوان لینک")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(150, ErrorMessage = "تعداد کاراکتر های {0} نمیتواند بیشتر از {1} باشد")]
        public string UrlTitle { get; set; }
        #endregion
        #region relations

        [ForeignKey(name: "ParentId")]
        public virtual ProductCategories ParentCategories { get; set; }
        public virtual ICollection<ProductSelectedCategories> ProductSelectedCategories { get; set; }
        #endregion
    }
}
