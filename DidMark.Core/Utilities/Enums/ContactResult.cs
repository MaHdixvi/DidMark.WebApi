using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DidMark.Core.Utilities.Enums
{
    public enum ContactResult
    {
        Success,       // پیام با موفقیت ذخیره شد
        Error,         // خطا در ذخیره پیام
        NotFound,      // پیام پیدا نشد
        InvalidData    // داده‌های ورودی نامعتبر بود
    }
}
