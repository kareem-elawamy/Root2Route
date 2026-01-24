using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Base;
using MediatR;

namespace Core.Features.PlantInfo.Commands.Models
{
    public class DeletePlantInfoCommand : IRequest<Response<string>>
    {
        public Guid Id { get; set; }
        public DeletePlantInfoCommand(Guid id)
        {
            Id = id;

        }
    }
}