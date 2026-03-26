using Infrastructure.Base;
using Infrastructure.Data;
using Domain.Models;

namespace Infrastructure.Repositories.DiagnosisLogRepository
{
    public class DiagnosisLogRepository : GenericRepositoryAsync<DiagnosisLog>, IDiagnosisLogRepository
    {
        public DiagnosisLogRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
