using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Core.Base;
using Core.Features.SystemSettings.Commands.Models;
using Domain.Models;
using Infrastructure.Repositories.SystemSettingRepository;
using Infrastructure.Repositories.AuditLogRepository;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Core.Features.SystemSettings.Commands.Handlers
{
    public class SystemSettingsCommandHandler : ResponseHandler,
        IRequestHandler<UpdatePlatformFeeCommand, Response<string>>
    {
        private readonly ISystemSettingRepository _settingsRepo;
        private readonly IAuditLogRepository _auditRepo;

        public SystemSettingsCommandHandler(ISystemSettingRepository settingsRepo, IAuditLogRepository auditRepo)
        {
            _settingsRepo = settingsRepo;
            _auditRepo = auditRepo;
        }

        public async Task<Response<string>> Handle(UpdatePlatformFeeCommand request, CancellationToken cancellationToken)
        {
            var settings = await _settingsRepo.GetTableNoTracking().FirstOrDefaultAsync(cancellationToken);
            if (settings == null)
            {
                settings = new SystemSetting { PlatformFeePercentage = request.NewFeePercentage };
                await _settingsRepo.AddAsync(settings);
            }
            else
            {
                var oldFee = settings.PlatformFeePercentage;
                settings.PlatformFeePercentage = request.NewFeePercentage;
                settings.LastUpdated = System.DateTime.UtcNow;
                await _settingsRepo.UpdateAsync(settings);

                // Add Audit Log
                var audit = new AuditLog
                {
                    UserId = request.AdminId,
                    Action = "Update Platform Fee",
                    EntityName = "SystemSetting",
                    OldValues = $"{{\"PlatformFeePercentage\": {oldFee}}}",
                    NewValues = $"{{\"PlatformFeePercentage\": {request.NewFeePercentage}}}"
                };
                await _auditRepo.AddAsync(audit);
            }

            return Success("Platform fee updated successfully.");
        }
    }
}
