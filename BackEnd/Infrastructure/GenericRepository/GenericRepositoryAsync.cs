using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Domain.Models;
using Domain.Common; // تأكد من إضافة هذا الـ Namespace عشان BaseEntity

namespace Infrastructure.Base
{
    // 👇 التعديل 1: غيرنا الـ Constraint عشان نضمن إن الـ T عنده IsDeleted
    // لو مش عايز تربطه بـ BaseEntity ممكن تسيبها class بس التعديل اللي تحت أهم
    public class GenericRepositoryAsync<T> : IGenericRepositoryAsync<T> where T : BaseEntity
    {
        #region Vars / Props

        protected readonly ApplicationDbContext _dbContext;

        #endregion

        #region Constructor(s)
        public GenericRepositoryAsync(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        #endregion

        #region Actions

        // 👇 التعديل 2 (الأهم): استبدال FindAsync
        public virtual async Task<T?> GetByIdAsync(Guid id)
        {
            // FindAsync أحياناً بيتجاهل فلتر الحذف لو العنصر في الميموري
            // FirstOrDefaultAsync بيضمن إن الاستعلام يروح للداتابيز ويطبق شرط !IsDeleted
            return await _dbContext.Set<T>().FirstOrDefaultAsync(x => x.Id == id);
        }

        public IQueryable<T> GetTableNoTracking()
        {
            // دي شغالة تمام لأن الـ Global Query Filter في الـ DbContext بيطبق عليها تلقائي
            return _dbContext.Set<T>().AsNoTracking().AsQueryable();
        }

        public IQueryable<T> GetTableAsTracking()
        {
            // ودي كمان شغالة تمام
            return _dbContext.Set<T>().AsQueryable();
        }

        public virtual async Task AddRangeAsync(ICollection<T> entities)
        {
            await _dbContext.Set<T>().AddRangeAsync(entities);
            await _dbContext.SaveChangesAsync();
        }

        public virtual async Task<T> AddAsync(T entity)
        {
            await _dbContext.Set<T>().AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return entity;
        }

        public virtual async Task UpdateAsync(T entity)
        {
            _dbContext.Set<T>().Update(entity);
            await _dbContext.SaveChangesAsync();
        }

        // 👇 ملاحظة بخصوص الحذف:
        // بما إنك عدلت الـ SaveChanges في الـ DbContext عشان تحول الحذف لـ Soft Delete
        // فتقدر تسيب الكود ده زي ما هو (Remove)، والـ Context هيحوله لـ Update IsDeleted=true
        public virtual async Task DeleteAsync(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
            await _dbContext.SaveChangesAsync();
        }

        public virtual async Task DeleteRangeAsync(ICollection<T> entities)
        {
            foreach (var entity in entities)
            {
                // نفس الفكرة، الـ Context هيعترض الحالة Deleted ويحولها
                _dbContext.Entry(entity).State = EntityState.Deleted;
            }
            await _dbContext.SaveChangesAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }

        public IDbContextTransaction BeginTransaction()
        {
            return _dbContext.Database.BeginTransaction();
        }

        public void Commit()
        {
            _dbContext.Database.CommitTransaction();
        }

        public void RollBack()
        {
            _dbContext.Database.RollbackTransaction();
        }

        public virtual async Task UpdateRangeAsync(ICollection<T> entities)
        {
            _dbContext.Set<T>().UpdateRange(entities);
            await _dbContext.SaveChangesAsync();
        }
        #endregion
    }
}