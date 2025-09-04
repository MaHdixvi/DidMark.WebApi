using DidMark.Core.Services.Interfaces;
using DidMark.DataLayer.Entities.Access;
using DidMark.DataLayer.Entities.Account;
using DidMark.DataLayer.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DidMark.Core.Services.Implementations
{
    public class AccessService : IAccessService
    {
        #region constructor
        private readonly IGenericRepository<Role> _roleRepository;
        private readonly IGenericRepository<User> _userRepository;
        private readonly IGenericRepository<UserRole> _userRoleRepository;

        public AccessService(IGenericRepository<Role> roleRepository, IGenericRepository<User> userRepository, IGenericRepository<UserRole> userRoleRepository)
        {
            _roleRepository = roleRepository;
            _userRepository = userRepository;
            _userRoleRepository = userRoleRepository;
        }


        #endregion


        #region dispose
        public void Dispose()
        {
            _userRepository?.Dispose();
            _userRepository?.Dispose();
            _roleRepository?.Dispose();
        }
        #endregion
        #region user role
        public async Task<bool> CheckUserRole(long userId, string role)
        {
            var result = await _userRoleRepository.GetEntitiesQuery().AsQueryable().AnyAsync(s => s.UserId == userId && s.Role.RoleTitle == role);
            return result;
        }
        #endregion
    }

}
