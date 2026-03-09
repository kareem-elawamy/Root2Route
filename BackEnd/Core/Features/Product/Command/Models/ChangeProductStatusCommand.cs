using System;
using Core.Base;
using Domain.Enums;
using MediatR;

namespace Core.Features.Product.Command.Models
{
    public class ChangeProductStatusCommand : IRequest<Response<string>>
    {
        public Guid ProductId { get; set; }
        public ProductStatus Status { get; set; }
        public string? RejectionReason { get; set; }
    }
}