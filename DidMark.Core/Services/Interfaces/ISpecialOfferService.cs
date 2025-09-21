using DidMark.Core.DTO.Products.SpecialOffers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DidMark.Core.Services.Interfaces
{
    public interface ISpecialOfferService : IDisposable
    {
        Task<SpecialOfferDto> GetByIdAsync(long id);
        Task<List<SpecialOfferDto>> GetAllAsync();
        Task CreateOfferAsync(CreateSpecialOfferDto dto);
        Task UpdateOfferAsync(UpdateSpecialOfferDto dto);
        Task DeleteOfferAsync(long id);
        Task<SpecialOfferDto> GetActiveOfferAsync();

        Task AssignOfferToProductAsync(AssignOfferToProductDto dto);
        Task<bool> UpdateOfferForProductAsync(EditOfferProductDto dto);
        Task<bool> RemoveOfferFromProductAsync(long id);
    }
}
