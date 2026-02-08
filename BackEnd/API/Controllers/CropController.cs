using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Controllers.Shared;
using Core.Features.Crop.Command.Models;
using Core.Features.Crop.Query.Models;
using Domain.MetaData;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    public class CropController : BaseApiController
    {
        [HttpGet(Router.Crop.List)]
        public async Task<IActionResult> GetCropsList()
        {
            var response = await Mediator.Send(new GetCropsListQuery());
            return NewResult(response);
        }

        // =====================================================
        // 2. Get Crop By Id (عرض محصول معين)
        // =====================================================
        [HttpGet(Router.Crop.GetById)] // api/v1/crop/getbyid/{id}
        public async Task<IActionResult> GetCropById([FromRoute] Guid id)
        {
            var response = await Mediator.Send(new GetCropByIdQuery(id));
            return NewResult(response);
        }

        // =====================================================
        // 3. Get Crops By Farm Id (عرض محاصيل مزرعة معينة)
        // =====================================================
        // مثال: api/Crop/Farm/xxxx-xxxx-xxxx
        [HttpGet(Router.Crop.GetByFarmId)] // api/v1/crop/getbyfarmid/{id}
        public async Task<IActionResult> GetCropsByFarmId([FromRoute] Guid farmId)
        {
            // ستحتاج لإنشاء هذا الكويري في الـ Core إذا لم يكن موجوداً
            // var response = await Mediator.Send(new GetCropsByFarmIdQuery(farmId));
            // return NewResult(response);
            return Ok(); // مؤقتاً حتى تنشئ الكويري
        }

        // =====================================================
        // 4. Create Crop (إضافة محصول)
        // =====================================================
        [HttpPost(Router.Crop.Create)] // api/v1/crop/create
        public async Task<IActionResult> Create([FromBody] CreateCropCommand command)
        {
            var response = await Mediator.Send(command);
            return NewResult(response);
        }

        // =====================================================
        // 5. Update Crop (تعديل محصول)
        // =====================================================
        [HttpPut(Router.Crop.Edit)] // api/v1/crop/edit
        public async Task<IActionResult> Update([FromBody] UpdateCropCommand command)
        {
            var response = await Mediator.Send(command);
            return NewResult(response);
        }

        // =====================================================
        // 6. Delete Crop (حذف محصول - Soft Delete)
        // =====================================================
        [HttpDelete(Router.Crop.Delete)]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var response = await Mediator.Send(new DeleteCropCommand(id));
            return NewResult(response);
        }

        // =====================================================
        // 7. Register Harvest (تسجيل الحصاد - Custom Action)
        // =====================================================
        // هذا الـ Endpoint خاص بتغيير حالة المحصول إلى "تم الحصاد"
        // مثال: api/Crop/Harvest
        [HttpPost(Router.Crop.RegisterHarvest)]
        public async Task<IActionResult> RegisterHarvest([FromBody] RegisterHarvestCommand command)
        {
            var response = await Mediator.Send(command);
            return NewResult(response);
        }
    }
}