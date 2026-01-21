using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Controllers.Shared;
using Core.Features.PlantInfo.Queries.Models;
using Domain.MetaData;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    public class PalntInfoController : BaseApiController
    {
        [HttpGet(Router.PlantInfo.GetAllPlantInfos)]
        public async Task<IActionResult> GetAllPlantInfos()
        {
            var response = await Mediator.Send(new GetAllPlantInfosQuery());
            return NewResult(response);
        }
    }
}