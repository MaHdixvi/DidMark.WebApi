using DidMark.DataLayer.Entities.Account;
using DidMark.DataLayer.Entities.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DidMark.DataLayer.Entities.Access
{
    public class UserRole : BaseEntity
    {
        #region properties

        public long UserId { get; set; }

        public long RoleId { get; set; }

        #endregion

        #region Relations

        public virtual User User { get; set; }

        public virtual Role Role { get; set; }

        #endregion
    }
}