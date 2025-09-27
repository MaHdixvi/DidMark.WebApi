using System.Collections.Generic;
using System.Threading.Tasks;
using DidMark.Core.DTO.Contact;
using DidMark.Core.DTO.Faq;

public interface IUserQuestionService : IDisposable
{
    Task<UserQuestionDto> GetByIdAsync(long id);
    Task<List<UserQuestionDto>> GetAllAsync();
    Task<bool> AskQuestionAsync(CreateUserQuestionDto dto, string phoneNumber, string fullName);
    Task<bool> AnswerQuestionAsync(UpdateUserQuestionDto dto);
}

