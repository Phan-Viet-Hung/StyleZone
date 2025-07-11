using DAL_Empty.Models;

namespace API.Domain.Request.VoucherRequest
{
    public class BulkStatusChangeRequest
    {
        public List<Guid> Ids { get; set; }
        public VoucherStatus Status { get; set; }
    }
}
