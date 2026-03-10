using System;
using Core.Base;
using MediatR;

namespace Core.Features.Product.Command.Models
{
    public class DeleteProductCommand : IRequest<Response<string>>
    {
        public Guid Id { get; set; }
        public DeleteProductCommand(Guid id) { Id = id; }
    }
}