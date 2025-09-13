using DidMark.Core.DTO.Products;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DidMark.Core.Services.Interfaces
{
    public interface IAttributeService
    {
        Task<List<AttributeDto>> GetAllAttributes();
        Task<AttributeDto> GetAttributeById(long id);
        Task<List<AttributeDto>> GetAttributesByCategoryId(long categoryId);

        Task AddAttribute(CreateAttributeDto dto);
        Task UpdateAttribute(long id, EditAttributeDto dto);
        Task<bool> DeleteAttribute(long id);
    }
}
