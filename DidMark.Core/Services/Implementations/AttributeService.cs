using DidMark.Core.DTO.Products;
using DidMark.Core.Services.Interfaces;
using DidMark.DataLayer.Entities.Product;
using DidMark.DataLayer.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DidMark.Core.Services.Implementations
{
    public class AttributeService : IAttributeService, IDisposable
    {
        private readonly IGenericRepository<PAttribute> _attributeRepo;

        public AttributeService(IGenericRepository<PAttribute> attributeRepo)
        {
            _attributeRepo = attributeRepo;
        }

        #region Attribute CRUD

        public async Task<List<AttributeDto>> GetAllAttributes()
        {
            return await _attributeRepo.GetEntitiesQuery()
                .Select(a => new AttributeDto
                {
                    Id = a.Id,
                    Name = a.Name,
                    CategoryId = a.CategoryId
                })
                .ToListAsync();
        }

        public async Task<AttributeDto> GetAttributeById(long id)
        {
            return await _attributeRepo.GetEntitiesQuery()
                .Where(a => a.Id == id)
                .Select(a => new AttributeDto
                {
                    Id = a.Id,
                    Name = a.Name,
                    CategoryId = a.CategoryId
                })
                .FirstOrDefaultAsync();
        }

        public async Task<List<AttributeDto>> GetAttributesByCategoryId(long categoryId)
        {
            return await _attributeRepo.GetEntitiesQuery()
                .Where(a => a.CategoryId == categoryId)
                .Select(a => new AttributeDto
                {
                    Id = a.Id,
                    Name = a.Name,
                    CategoryId = a.CategoryId
                })
                .ToListAsync();
        }

        public async Task AddAttribute(CreateAttributeDto dto)
        {
            var entity = new PAttribute
            {
                Name = dto.Name,
                CategoryId = dto.CategoryId,
                CreateDate = DateTime.Now,
                IsDelete = false
            };

            await _attributeRepo.AddEntity(entity);
            await _attributeRepo.SaveChanges();
        }

        public async Task UpdateAttribute(long id, EditAttributeDto dto)
        {
            var entity = await _attributeRepo.GetEntityById(id);
            if (entity == null) return;

            entity.Name = dto.Name;
            entity.CategoryId = dto.CategoryId;

            _attributeRepo.UpdateEntity(entity);
            await _attributeRepo.SaveChanges();
        }

        public async Task<bool> DeleteAttribute(long id)
        {
            var entity = await _attributeRepo.GetEntityById(id);
            if (entity == null) return false;

            entity.IsDelete = true; // soft delete
            _attributeRepo.UpdateEntity(entity);
            await _attributeRepo.SaveChanges();

            return true;
        }

        #endregion

        public void Dispose()
        {
            _attributeRepo?.Dispose();
        }
    }
}
