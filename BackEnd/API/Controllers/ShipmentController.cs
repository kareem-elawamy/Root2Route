using System;
using System.Security.Claims;
using System.Threading.Tasks;
using API.Controllers.Shared;
using Core.Features.Shipping.Commands.Models;
using Core.Features.Shipping.Queries.Models;
using Domain.MetaData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Authorize]
    public class ShipmentController : BaseApiController
    {
        [HttpPost(Router.Shipment.AddAddress)]
        public async Task<IActionResult> AddShippingAddress([FromBody] AddShippingAddressCommand command)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            command.CurrentUserId = userId.Value;
            var response = await Mediator.Send(command);
            return NewResult(response);
        }

        [HttpGet(Router.Shipment.GetMyAddresses)]
        public async Task<IActionResult> GetMyShippingAddresses()
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            var query = new GetMyShippingAddressesQuery { CurrentUserId = userId.Value };
            var response = await Mediator.Send(query);
            return NewResult(response);
        }

        [HttpPost(Router.Shipment.Dispatch)]
        public async Task<IActionResult> DispatchOrder([FromBody] DispatchOrderCommand command)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            command.CurrentUserId = userId.Value;
            var response = await Mediator.Send(command);
            return NewResult(response);
        }

        [HttpPut(Router.Shipment.UpdateStatus)]
        public async Task<IActionResult> UpdateShipmentStatus([FromRoute] Guid id, [FromBody] UpdateShipmentStatusCommand command)
        {
            var userId = GetCurrentUserId();
            if (userId == null) return Unauthorized();

            command.ShipmentId = id;
            command.CurrentUserId = userId.Value;
            
            var response = await Mediator.Send(command);
            return NewResult(response);
        }

        #region Helpers
        private Guid? GetCurrentUserId()
        {
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdString)) return null;
            return Guid.Parse(userIdString);
        }
        #endregion
    }
}
