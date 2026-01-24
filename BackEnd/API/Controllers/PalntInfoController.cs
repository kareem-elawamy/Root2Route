using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Controllers.Shared;
using Core.Features.PlantInfo.Commands.Models;
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
        [HttpPost(Router.PlantInfo.CreatePlantInfo)]
        public async Task<IActionResult> CreatePlantInfo([FromForm] CreatePlantInfoCommand command)
        {

            if (command.Image == null)
            {
                return BadRequest("الملف لم يصل! تأكد من Postman وأن اسم الحقل هو 'Image'");
            }
            var response = await Mediator.Send(command);

            return NewResult(response);
        }
        [HttpPut(Router.PlantInfo.EditPlantInfo)]
        public async Task<IActionResult> EditPlantInfo([FromForm] EditPlantInfoCommand command)
        {
            // if (command.Image == null)
            // {
            //     return BadRequest("الملف لم يصل! تأكد من Postman وأن اسم الحقل هو 'Image'");
            // }
            var response = await Mediator.Send(command);

            return NewResult(response);
        }
        [HttpDelete(Router.PlantInfo.DeletePlantInfo)]
        public async Task<IActionResult> DeletePlantInfo([FromForm] Guid id)
        {
            return NewResult(await Mediator.Send(new DeletePlantInfoCommand(id)));

        }
    }
}