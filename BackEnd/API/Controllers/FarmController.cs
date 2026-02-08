using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Controllers.Shared;
using Core.Features.Farm.Commands.Models;
using Domain.MetaData;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    public class FarmController : BaseApiController
    {
        [HttpPost(Router.Farm.CreateFarm)]
        public async Task<IActionResult> CreateFarm([FromForm] CreateFarmCommand command)
        {
             var response = await Mediator.Send(command);

            return NewResult(response);
        }
    }
}