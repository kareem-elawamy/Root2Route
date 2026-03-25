using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Base;
using Core.Features.Shipping.Queries.Models;
using Core.Features.Shipping.Queries.Results;
using Infrastructure.Repositories.ShippingAddressRepository;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Core.Features.Shipping.Queries.Handlers
{
    public class GetMyShippingAddressesQueryHandler : IRequestHandler<GetMyShippingAddressesQuery, Response<List<ShippingAddressResponse>>>
    {
        private readonly IShippingAddressRepository _shippingAddressRepository;

        public GetMyShippingAddressesQueryHandler(IShippingAddressRepository shippingAddressRepository)
        {
            _shippingAddressRepository = shippingAddressRepository;
        }

        public async Task<Response<List<ShippingAddressResponse>>> Handle(GetMyShippingAddressesQuery request, CancellationToken cancellationToken)
        {
            var addresses = await _shippingAddressRepository.GetTableNoTracking()
                .Where(sa => sa.UserId == request.CurrentUserId)
                .OrderByDescending(sa => sa.IsDefault)
                .ThenByDescending(sa => sa.CreatedAt)
                .Select(sa => new ShippingAddressResponse
                {
                    Id = sa.Id,
                    Label = sa.Label,
                    City = sa.City,
                    Street = sa.Street,
                    Phone = sa.Phone,
                    IsDefault = sa.IsDefault
                })
                .ToListAsync(cancellationToken);

            return new Response<List<ShippingAddressResponse>>(addresses) { Succeeded = true };
        }
    }
}
