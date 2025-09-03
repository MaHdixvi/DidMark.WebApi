using DidMark.Core.Security.ContactUs;
using DidMark.Core.Services.Interfaces;
using DidMark.Core.Utilities.Common;
using Microsoft.AspNetCore.Mvc;

namespace DidMark.WebApi.Controllers
{
    public class ContactController : SiteBaseController
    {
        #region costructor

        private IContactUs contactUs;

        public ContactController(IContactUs contactUs)
        {
            this.contactUs = contactUs;
        }

        #endregion

        #region SubmitResume

        [HttpPost("SubmitContact")]
        public async Task<IActionResult> SubmitContact([FromBody] ContactUsDTO resume)
        {
            if (ModelState.IsValid)
            {
                var res = await contactUs.SendMessage(resume);
                switch (res)
                {
                    case ContactResult.Error:
                        return JsonResponseStatus.Error(new { info = "Error" });
                }
            }


            return JsonResponseStatus.Success();
        }

        #endregion
    }
}