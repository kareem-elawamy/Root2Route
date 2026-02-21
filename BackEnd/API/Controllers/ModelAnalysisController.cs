using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Controllers.Shared;
using Core.Features.ModelAI.Query.Models;
using Domain.MetaData;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    public class ModelAnalysisController : BaseApiController
    {
        private readonly IMediator _mediator;

        public ModelAnalysisController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPost(Router.AnalyzePlants.AnalyzePlant)]
        public async Task<IActionResult> AnalyzePlant([FromForm] AnalyzePlantImageQuery query)
        {
            // إرسال الـ Query للـ Handler من خلال MediatR
            var response = await _mediator.Send(query);

            // هنا بنعمل return بناءً على حالة الـ Response
            if (response.Succeeded)
            {
                return Ok(response);
            }

            return BadRequest(response);
        }

    }
}