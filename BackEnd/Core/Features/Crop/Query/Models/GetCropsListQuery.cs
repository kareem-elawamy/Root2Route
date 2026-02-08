using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Base;
using Core.Features.Crop.Query.Result;
using MediatR;

namespace Core.Features.Crop.Query.Models
{
    public class GetCropsListQuery : IRequest<Response<List<CropResponse>>>
    {

    }
}