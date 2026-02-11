using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Features.Organization.Commands.Models;
using Domain.Constants;
using Infrastructure.Data;
using Infrastructure.Repositories.OrganizationRepository;
using Service.Services;
using Service.Services.FileService;
using Service.Services.OrganizationMemberService;
using Service.Services.OrganizationRoleService;

namespace Core.Features.Organization.Commands.Handler
{
    public class CreateOrganizationHandler : ResponseHandler, IRequestHandler<CreateOrganizationCommand, Response<string>>
    {
        private readonly IOrganizationService _organizationService;
        private readonly IMapper _mapper;
        private readonly IFileService _fileService;
        private readonly IOrganizationRoleService _organizationRoleService;
        private readonly IOrganizationMemberService _organizationMemberService;
        private readonly ApplicationDbContext _dbContext;
        public CreateOrganizationHandler(IOrganizationService organizationService, IMapper mapper, IFileService fileService, IOrganizationRoleService organizationRoleService, IOrganizationMemberService organizationMemberService, ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            _organizationMemberService = organizationMemberService;
            _organizationService = organizationService;
            _mapper = mapper;
            _fileService = fileService;
            _organizationRoleService = organizationRoleService;
        }

        public async Task<Response<string>> Handle(CreateOrganizationCommand request, CancellationToken cancellationToken)
        {
            // 1. استخدام Transaction لضمان سلامة البيانات
            using var transaction = _dbContext.Database.BeginTransaction(); // أو عبر الـ DbContext

            try
            {
                // 2. Mapping
                var organization = _mapper.Map<Domain.Models.Organization>(request);

                // 3. Execution (الخدمة هنا يجب أن تتعامل مع رفع اللوجو وترجع الـ Entity)
                var result = await _organizationService.CreateOrganizationAsync(organization, request.Logo);

                if (result == "Exists")
                    return BadRequest<string>("Organization with the same name already exists.");

                // 4. إنشاء الـ Role (يفضل وضع القيم الثابتة في Constants)
                var ownerRole = new Domain.Models.OrganizationRole
                {
                    Name = "Owner",
                    OrganizationId = organization.Id,
                    Permissions = OrganizationsPermissions.GetAll().Select(p => new OrganizationRolePermission
                    {
                        PermissionsClaim = p
                    }).ToList() // نفترض أن هذا يعيد قائمة بكل الـ Permissions كقيم
                };
                await _organizationRoleService.CreateOrganizationRole(ownerRole);

                // 5. إنشاء الـ Membership
                var ownerMembership = new Domain.Models.OrganizationMember
                {
                    OrganizationId = organization.Id,
                    UserId = organization.OwnerId,
                    OrganizationRoleId = ownerRole.Id,
                    IsActive = true
                };
                await _organizationMemberService.AddOrganizationMemberAsync(ownerMembership);

                // تثبيت كل العمليات
                transaction.Commit();

                return Created(result);
            }
            catch (Exception ex)
            {
                // في حالة حدوث أي خطأ، الـ using سيضمن عمل Rollback تلقائياً
                return BadRequest<string>($"حدث خطأ أثناء إنشاء المنظمة: {ex.Message}");
            }
        }
    }
}