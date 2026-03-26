using System;
using Core.Base;
using MediatR;

namespace Core.Features.SystemSettings.Commands.Models
{
    public class UpdatePlatformFeeCommand : IRequest<Response<string>>
    {
        public decimal NewFeePercentage { get; set; }
        public Guid AdminId { get; set; }
    }
}
