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
        public static IActionResult Success()
        {
            return new JsonResult(new { status = "Success" }) { StatusCode = 200 };
        }

        public static IActionResult Success(object returnData)
        {
            return new JsonResult(new { status = "Success", data = returnData }) { StatusCode = 200 };
        }

        public static IActionResult NotFound()
        {
            return new JsonResult(new { status = "NotFound" }) { StatusCode = 404 };
        }

        public static IActionResult NotFound(object returnData)
        {
            return new JsonResult(new { status = "NotFound", data = returnData }) { StatusCode = 404 };
        }

        public static IActionResult Error()
        {
            return new JsonResult(new { status = "Error" }) { StatusCode = 500 };
        }

        public static IActionResult Error(object returnData)
        {
            return new JsonResult(new { status = "Error", data = returnData }) { StatusCode = 500 };
        }

        public static IActionResult Unauthorized()
        {
            return new JsonResult(new { status = "Unauthorized" }) { StatusCode = 401 };
        }

        public static IActionResult Unauthorized(object returnData)
        {
            return new JsonResult(new { status = "Unauthorized", data = returnData }) { StatusCode = 401 };
        }

        public static IActionResult BadRequest()
        {
            return new JsonResult(new { status = "BadRequest" }) { StatusCode = 400 };
        }

        public static IActionResult BadRequest(object returnData)
        {
            return new JsonResult(new { status = "BadRequest", data = returnData }) { StatusCode = 400 };
        }

        public static IActionResult Conflict()
        {
            return new JsonResult(new { status = "Conflict" }) { StatusCode = 409 };
        }

        public static IActionResult Conflict(object returnData)
        {
            return new JsonResult(new { status = "Conflict", data = returnData }) { StatusCode = 409 };
        }

        public static IActionResult Created()
        {
            return new JsonResult(new { status = "Created" }) { StatusCode = 201 };
        }

        public static IActionResult Created(object returnData)
        {
            return new JsonResult(new { status = "Created", data = returnData }) { StatusCode = 201 };
        }

        public static IActionResult Forbidden()
        {
            return new JsonResult(new { status = "Forbidden" }) { StatusCode = 403 };
        }

        public static IActionResult Forbidden(object returnData)
        {
            return new JsonResult(new { status = "Forbidden", data = returnData }) { StatusCode = 403 };
        }
    }
}