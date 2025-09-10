using DidMark.DataLayer.Entities.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DidMark.Core.DTO.TransactionLog
{
    public class CancelTransactionFilterDto
    {
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
        public TransactionFor? TransactionFor { get; set; }
        public TransactionStatus? Status { get; set; }
    }

}
