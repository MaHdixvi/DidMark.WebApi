using DidMark.Core.DTO.Contact;
using DidMark.Core.DTO.Faq;
using DidMark.Core.Services.Implementations;
using DidMark.Core.Services.Interfaces;
using DidMark.DataLayer.Entities.Site;
using DidMark.DataLayer.Repository;
using Microsoft.EntityFrameworkCore;

public class UserQuestionService : IUserQuestionService
{
    private readonly IGenericRepository<UserQuestion> _questionRepo;
    private readonly ISmsService _smsService;

    public UserQuestionService(
        IGenericRepository<UserQuestion> questionRepo,
        ISmsService smsService)
    {
        _questionRepo = questionRepo;
        _smsService = smsService;
    }

    public async Task<UserQuestionDto> GetByIdAsync(long id)
    {
        var q = await _questionRepo.GetEntityById(id);
        if (q == null) return null;

        return new UserQuestionDto
        {
            Id = q.Id,
            Question = q.Question,
            FullName = q.FullName,
            CreateDate = q.CreateDate,
            IsAnswered = q.IsAnswered,
            Answer = q.Answer
        };
    }

    public async Task<List<UserQuestionDto>> GetAllAsync()
    {
        return await _questionRepo.GetEntitiesQuery()
            .OrderByDescending(x => x.CreateDate)
            .Select(q => new UserQuestionDto
            {
                Id = q.Id,
                Question = q.Question,
                FullName = q.FullName,
                CreateDate = q.CreateDate,
                IsAnswered = q.IsAnswered,
                Answer = q.Answer
            }).ToListAsync();
    }

    public async Task<bool> AskQuestionAsync(CreateUserQuestionDto dto, string phoneNumber, string fullName)
    {
        if (string.IsNullOrWhiteSpace(dto.Question)) return false;

        var q = new UserQuestion
        {
            Question = dto.Question,
            FullName = fullName,
            PhoneNumber = phoneNumber,
            CreateDate = DateTime.Now,
            IsAnswered = false,
            IsDelete = false
        };

        await _questionRepo.AddEntity(q);
        await _questionRepo.SaveChanges();
        return true;
    }

    public async Task<bool> AnswerQuestionAsync(UpdateUserQuestionDto dto)
    {
        var q = await _questionRepo.GetEntityById(dto.Id);
        if (q == null) return false;

        q.Answer = dto.Answer;
        q.IsAnswered = dto.IsAnswered;

        _questionRepo.UpdateEntity(q);
        await _questionRepo.SaveChanges();

        if (dto.IsAnswered && !string.IsNullOrWhiteSpace(q.PhoneNumber))
        {
            var smsText = $"کاربر گرامی، پاسخ سوال شما:\n\"{q.Question}\"\n\n{dto.Answer}";
            await _smsService.SendCustomSmsAsync(q.PhoneNumber, smsText);
        }

        return true;
    }

    public void Dispose()
    {
        _questionRepo?.Dispose();
    }
}
