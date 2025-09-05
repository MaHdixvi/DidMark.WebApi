using System.ComponentModel.DataAnnotations;

namespace DidMark.Core.DTO.Users
{
    public class EditUserByAdminDTO
    {
        public long Id { get; set; }

        [Display(Name = "نام")]
        [MaxLength(100)]
        public string? FirstName { get; set; }

        [Display(Name = "نام خانوادگی")]
        [MaxLength(100)]
        public string? LastName { get; set; }

        [Display(Name = "نام کاربری")]
        [MaxLength(50)]
        public string? Username { get; set; }

        [Display(Name = "ایمیل")]
        [EmailAddress(ErrorMessage = "ایمیل وارد شده معتبر نیست")]
        public string? Email { get; set; }

        [Display(Name = "شماره تماس")]
        [Phone(ErrorMessage = "شماره تماس وارد شده معتبر نیست")]
        public string? PhoneNumber { get; set; }

        [Display(Name = "کد ملی")]
        [MaxLength(10, ErrorMessage = "کد ملی نمی‌تواند بیشتر از 10 رقم باشد")]
        public string? NationalCode { get; set; }

        [Display(Name = "شهر")]
        [MaxLength(100)]
        public string? City { get; set; }

        [Display(Name = "استان")]
        [MaxLength(100)]
        public string? Province { get; set; }

        [Display(Name = "آدرس")]
        [MaxLength(250)]
        public string? Address { get; set; }
    }
}
