using AutoMapper;
using Core.Base;
using Core.Features.authentication.Commands.Models;
using Domain.Enums;
using Domain.Models;
using Infrastructure.Repositories.OrganizationRepository;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Service;
using Service.Services;
using Service.Services.AuthenticationService;

namespace Core.Features.authentication.Commands.Handler
{
    public class UserCommandHandler : ResponseHandler, IRequestHandler<AddUserCommand, Response<JwtAuthResult>>
    {
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IOrganizationService _orgService;
        private readonly IOrganizationRepository _orgRepo;
        private readonly IAuthenticationService _authService; // إضافة سيرفيس التوكن

        public UserCommandHandler(IMapper mapper,
                                  UserManager<ApplicationUser> userManager,
                                  IOrganizationService orgService,
                                  IOrganizationRepository orgRepo,
                                  IAuthenticationService authService)
        {
            _userManager = userManager;
            _mapper = mapper;
            _orgService = orgService;
            _orgRepo = orgRepo;
            _authService = authService;
        }

        public async Task<Response<JwtAuthResult>> Handle(AddUserCommand request, CancellationToken cancellationToken)
        {
            // 1. تشيك تكرار الإيميل
            var user = await _userManager.FindByEmailAsync(request.Email!);
            if (user != null) return BadRequest<JwtAuthResult>("Email already exists");

            // 2. فتح Transaction
            // نعتمد على using لعمل Rollback تلقائي في حالة حدوث خطأ وعدم الوصول لـ Commit
            using var transaction = _orgRepo.BeginTransaction();

            try
            {
                // 3. إنشاء اليوزر
                var userIdentity = _mapper.Map<ApplicationUser>(request);
                var createdResult = await _userManager.CreateAsync(userIdentity, request.Password!);

                if (!createdResult.Succeeded)
                {
                    // لا حاجة لعمل Rollback يدوياً، الـ using سيقوم بذلك عند الخروج
                    var error = createdResult.Errors.FirstOrDefault()?.Description;
                    return BadRequest<JwtAuthResult>(error ?? "Failed to create user");
                }

                // 4. إنشاء المنظمة
                if (request.UserType != UserType.IndividualUser)
                {
                    var organization = new Domain.Models.Organization
                    {
                        Name = request.OrganizationName!,
                        Type = request.OrganizationType ?? OrganizationType.Farm,
                        Address = request.OrganizationAddress,
                        OwnerId = userIdentity.Id, // EF Core يتعامل بذكاء مع الـ Tracking هنا
                        CreatedAt = DateTime.UtcNow
                    };

                    await _orgService.CreateOrganizationAsync(organization);
                }

                // 5. توليد التوكن (تم نقلها قبل الـ Commit)
                // لضمان أنه في حالة فشل التوكن، لا يتم حفظ اليوزر في الداتا بيز
                var accessToken = await _authService.GenerateToken(userIdentity);

                // 6. تثبيت التغييرات (آخر خطوة تماماً)
                transaction.Commit();

                // 7. إرجاع النتيجة
                return Success(accessToken);
            }
            catch (Exception ex)
            {
                // لا تقم بعمل Rollback يدوياً إذا كنت تستخدم using، إلا إذا كنت تريد التعامل مع الخطأ وإكمال التنفيذ
                // transaction.Rollback(); // يفضل إزالتها لتجنب Exception إذا كان الخطأ في الـ Commit نفسه

                return BadRequest<JwtAuthResult>("An error occurred: " + ex.Message);
            }
        }
    }
}