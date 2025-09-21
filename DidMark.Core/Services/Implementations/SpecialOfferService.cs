using DidMark.Core.DTO.Products.SpecialOffers;
using DidMark.Core.Services.Interfaces;
using DidMark.DataLayer.Entities.Offers;
using DidMark.DataLayer.Entities.Product;
using DidMark.DataLayer.Repository;
using Microsoft.EntityFrameworkCore;

public class SpecialOfferService : ISpecialOfferService
{
    private readonly IGenericRepository<SpecialOffer> _specialOfferRepository;
    private readonly IGenericRepository<SpecialOfferProduct> _specialOfferProductRepository;

    public SpecialOfferService(
        IGenericRepository<SpecialOffer> specialOfferRepository,
        IGenericRepository<SpecialOfferProduct> specialOfferProductRepository)
    {
        _specialOfferRepository = specialOfferRepository;
        _specialOfferProductRepository = specialOfferProductRepository;
    }

    #region SpecialOffer CRUD

    public async Task<SpecialOfferDto> GetByIdAsync(long id)
    {
        var offer = await _specialOfferRepository.GetEntityById(id);
        if (offer == null) return null;

        return new SpecialOfferDto
        {
            Id = offer.Id,
            Title = offer.Title,
            DiscountPercent = offer.DiscountPercent,
            StartDate = offer.StartDate,
            EndDate = offer.EndDate,
            IsActive = offer.IsDelete
        };
    }

    public async Task<List<SpecialOfferDto>> GetAllAsync()
    {
        return _specialOfferRepository.GetEntitiesQuery()
            .Select(o => new SpecialOfferDto
            {
                Id = o.Id,
                Title = o.Title,
                DiscountPercent = o.DiscountPercent,
                StartDate = o.StartDate,
                EndDate = o.EndDate,
                IsActive = o.IsDelete
            }).ToList();
    }

    public async Task CreateOfferAsync(CreateSpecialOfferDto dto)
    {
        var offer = new SpecialOffer
        {
            Title = dto.Title,
            DiscountPercent = dto.DiscountPercent,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            IsDelete = true
        };

        await _specialOfferRepository.AddEntity(offer);
        await _specialOfferRepository.SaveChanges();
    }

    public async Task UpdateOfferAsync(UpdateSpecialOfferDto dto)
    {
        var offer = await _specialOfferRepository.GetEntityById(dto.Id);
        if (offer == null) return;

        offer.Title = dto.Title;
        offer.DiscountPercent = dto.DiscountPercent;
        offer.StartDate = dto.StartDate;
        offer.EndDate = dto.EndDate;
        offer.IsDelete = dto.IsActive;

        _specialOfferRepository.UpdateEntity(offer);
        await _specialOfferRepository.SaveChanges();
    }

    public async Task DeleteOfferAsync(long id)
    {
        var offer = await _specialOfferRepository.GetEntityById(id);
        if (offer == null) return;

        _specialOfferRepository.RemoveEntity(offer);
        await _specialOfferRepository.SaveChanges();
    }

    public async Task<SpecialOfferDto> GetActiveOfferAsync()
    {
        var now = DateTime.Now;

        var offer = _specialOfferRepository.GetEntitiesQuery()
            .Where(o => !o.IsDelete && o.StartDate <= now && o.EndDate >= now)
            .OrderByDescending(o => o.DiscountPercent)
            .FirstOrDefault();

        if (offer == null) return null;

        return new SpecialOfferDto
        {
            Id = offer.Id,
            Title = offer.Title,
            DiscountPercent = offer.DiscountPercent,
            StartDate = offer.StartDate,
            EndDate = offer.EndDate,
            IsActive = offer.IsDelete
        };
    }

    #endregion

    #region Product Offers (Many-to-Many)

    public async Task AssignOfferToProductAsync(AssignOfferToProductDto dto)
    {
        var existing = await _specialOfferProductRepository.GetEntitiesQuery()
            .FirstOrDefaultAsync(op => op.ProductId == dto.ProductId && op.SpecialOfferId == dto.OfferId);

        if (existing != null) return;

        var entity = new SpecialOfferProduct
        {
            ProductId = dto.ProductId,
            SpecialOfferId = dto.OfferId
        };

        await _specialOfferProductRepository.AddEntity(entity);
        await _specialOfferProductRepository.SaveChanges();
    }

    public async Task<bool> UpdateOfferForProductAsync(EditOfferProductDto dto)
    {
        var existing = await _specialOfferProductRepository.GetEntityById(dto.Id);
        if (existing == null) return false;

        existing.SpecialOfferId = dto.NewOfferId;
        _specialOfferProductRepository.UpdateEntity(existing);
        await _specialOfferProductRepository.SaveChanges();
        return true;
    }

    public async Task<bool> RemoveOfferFromProductAsync(long id)
    {
        var existing = await _specialOfferProductRepository.GetEntityById(id);
        if (existing == null) return false;

        _specialOfferProductRepository.RemoveEntity(existing);
        await _specialOfferProductRepository.SaveChanges();
        return true;
    }
    public void Dispose() { _specialOfferRepository?.Dispose(); }

    #endregion
}
