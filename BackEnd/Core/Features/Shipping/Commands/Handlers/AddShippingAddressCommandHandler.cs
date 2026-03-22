using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Base;
using Core.Features.Shipping.Commands.Models;
using Domain.Models;
using Infrastructure.Repositories.ShippingAddressRepository;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Core.Features.Shipping.Commands.Handlers
{
    public class AddShippingAddressCommandHandler : IRequestHandler<AddShippingAddressCommand, Response<Guid>>
    {
        private readonly IShippingAddressRepository _shippingAddressRepository;

        public AddShippingAddressCommandHandler(IShippingAddressRepository shippingAddressRepository)
        {
            _shippingAddressRepository = shippingAddressRepository;
        }

        public async Task<Response<Guid>> Handle(AddShippingAddressCommand request, CancellationToken cancellationToken)
        {
            // If setting as default, unset all other defaults for this user
            if (request.IsDefault)
            {
                var existingDefaults = await _shippingAddressRepository.GetTableAsTracking()
                    .Where(sa => sa.UserId == request.CurrentUserId && sa.IsDefault)
                    .ToListAsync(cancellationToken);

                foreach (var addr in existingDefaults)
                {
                    addr.IsDefault = false;
                }

                if (existingDefaults.Any())
                    await _shippingAddressRepository.UpdateRangeAsync(existingDefaults);
            }

            var address = new ShippingAddress
            {
                UserId = request.CurrentUserId,
                Label = request.Label,
                City = request.City,
                Street = request.Street,
                Phone = request.Phone,
                IsDefault = request.IsDefault
            };

            await _shippingAddressRepository.AddAsync(address);
            await _shippingAddressRepository.SaveChangesAsync();

            return new Response<Guid>(address.Id) { Succeeded = true };
        }
    }
}
