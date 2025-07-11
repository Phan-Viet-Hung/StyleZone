﻿using System.ComponentModel.DataAnnotations;

namespace StyleZone_API.Domain.Request.VoucherRequest
{
    public class UpdateVoucherRequest : CreateVoucherRequest
    {
        [Required(ErrorMessage = "ID là bắt buộc")]
        public Guid Id { get; set; }
    }
}
