using Core.Features.Product.Queries.Results;

namespace Core.Features.Product.Queries.Models
{
    public class GetProductByIdQuery : IRequest<Response<ProductResponse>>
    {
        public Guid Id { get; set; }

        public GetProductByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}