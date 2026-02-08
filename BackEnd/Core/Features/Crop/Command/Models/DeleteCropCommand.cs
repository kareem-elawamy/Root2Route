using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Base;
using MediatR;

namespace Core.Features.Crop.Command.Models
{
    public class DeleteCropCommand : IRequest<Response<string>>
    {
        public Guid Id { get; set; }
        public DeleteCropCommand(Guid id)
        {
            Id = id;

        }
    }
}