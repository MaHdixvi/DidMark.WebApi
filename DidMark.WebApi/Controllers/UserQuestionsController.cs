using DidMark.Core.DTO.Faq;
using DidMark.Core.Utilities.Common;
using DidMark.Core.Utilities.Enums;
using DidMark.DataLayer.Entities.Account;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/v1/[controller]")]
public class UserQuestionsController : ControllerBase
{
    private readonly IUserQuestionService _questionService;

    public UserQuestionsController(IUserQuestionService questionService)
    {
        _questionService = questionService;
    }

    // کاربر سوال می‌پرسه
    [HttpPost("ask")]
    public async Task<IActionResult> Ask([FromBody] CreateUserQuestionDto dto)
    {
        // شماره تلفن و نام کاربر رو از توکن می‌گیریم
        var phone = User?.FindFirst("phoneNumber")?.Value ?? "unknown";
        var fullName = User?.FindFirst("username")?.Value ?? "کاربر";

        var result = await _questionService.AskQuestionAsync(dto, phone, fullName);
        if (!result) return JsonResponseStatus.Success(new { message = "سوال معتبر نیست" });

        return JsonResponseStatus.Success(new { message = "سوال شما ثبت شد" });
    }

    // ادمین لیست سوالات
    [HttpGet("all")]
    public async Task<IActionResult> GetAll()
    {
        var list = await _questionService.GetAllAsync();
        return Ok(list);
    }

    [HttpGet("{id:long}")]
    public async Task<IActionResult> GetById(long id)
    {
        var result = await _questionService.GetByIdAsync(id);
        return result == null
            ? NotFound(new { message = "سؤال پیدا نشد" })
            : Ok(result);
    }

    // ادمین پاسخ می‌دهد
    [HttpPost("answer")]
    public async Task<IActionResult> Answer([FromBody] UpdateUserQuestionDto dto)
    {
        var result = await _questionService.AnswerQuestionAsync(dto);
        if (!result) return NotFound(new { message = "سوال یافت نشد" });

        return Ok(new { message = "پاسخ ذخیره شد" });
    }
}
