using DidMark.DataLayer.Entities.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DidMark.DataLayer.Entities.Newsletter
{
    public class Campaign : BaseEntity
    {
        #region Properties

        [Display(Name = "عنوان کمپین")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(200, ErrorMessage = "تعداد کاراکتر های {0} نمیتواند بیشتر از {1} باشد")]
        public string Title { get; set; }

        [Display(Name = "موضوع ایمیل")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        [MaxLength(200, ErrorMessage = "تعداد کاراکتر های {0} نمیتواند بیشتر از {1} باشد")]
        public string Subject { get; set; }

        [Display(Name = "محتوا")]
        [Required(ErrorMessage = "لطفا {0} را وارد کنید")]
        public string Content { get; set; }

        [Display(Name = "تاریخ ارسال")]
        public DateTime? SentDate { get; set; }

        [Display(Name = "وضعیت")]
        public CampaignStatus Status { get; set; }

        [Display(Name = "تعداد ارسال شده")]
        public int SentCount { get; set; }

        [Display(Name = "تعداد تحویل شده")]
        public int DeliveredCount { get; set; }

        [Display(Name = "تعداد باز شده")]
        public int OpenedCount { get; set; }

        [Display(Name = "تعداد کلیک شده")]
        public int ClickedCount { get; set; }

        #endregion
    }

    public enum CampaignStatus
    {
        [Display(Name = "پیش‌نویس")]
        Draft = 0,

        [Display(Name = "در صف ارسال")]
        Queued = 1,

        [Display(Name = "در حال ارسال")]
        Sending = 2,

        [Display(Name = "ارسال شده")]
        Sent = 3,

        [Display(Name = "لغو شده")]
        Cancelled = 4
    }
}
