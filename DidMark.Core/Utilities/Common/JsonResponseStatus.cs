using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DidMark.Core.Utilities.Common
{
    public static class JsonResponseStatus
    {
        public static JsonResult Success()
        {
            return new JsonResult(new { status = "Success" });
        }

        public static JsonResult Success(object returnData)
        {
            return new JsonResult(new { status = "Success", data = returnData });
        }

        public static JsonResult NotFound()
        {
            return new JsonResult(new { status = "NotFound" });
        }

        public static JsonResult NotFound(object returnData)
        {
            return new JsonResult(new { status = "NotFound", data = returnData });
        }

        public static JsonResult Error()
        {
            return new JsonResult(new { status = "Error" });
        }

        public static JsonResult Error(object returnData)
        {
            return new JsonResult(new { status = "Error", data = returnData });
        }
        public static JsonResult UnAthorized()
        {
            return new JsonResult(new { status = "UnAthorized" });
        }

        public static JsonResult UnAthorized(object returnData)
        {
            return new JsonResult(new { status = "UnAthorized", data = returnData });
        }
        public static JsonResult NoAccess()
        {
            return new JsonResult(new { status = "Error" });
        }

        public static JsonResult NoAccess(object returnData)
        {
            return new JsonResult(new { status = "Error", data = returnData });
        }
    }
}