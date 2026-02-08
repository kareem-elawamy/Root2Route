using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Base;
using MediatR;

namespace Core.Features.Farm.Commands.Models
{
    public class CreateFarmCommand : IRequest<Response<string>>
    {
        public string Name { get; set; } = null!;
        public string? Location { get; set; }
        public Guid OrganizationId { get; set; }

    }
}