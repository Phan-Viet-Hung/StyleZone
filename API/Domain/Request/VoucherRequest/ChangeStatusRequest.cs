using DAL_Empty.Models;

namespace API.Domain.Request.VoucherRequest
{
    public class ChangeStatusRequest
    {
        public VoucherStatus Status { get; set; }

    }
}
