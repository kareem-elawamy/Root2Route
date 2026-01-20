using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Infrastructure.Base
{
    // هذا الكلاس هو التنفيذ الفعلي (Implementation) للمستودع العام
    // T : class تعني أن هذا المستودع يتعامل مع Reference Types فقط (Entities)
    public class GenericRepositoryAsync<T> : IGenericRepositoryAsync<T> where T : class
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


        #region Methods

        #endregion

        #region Actions
        // دالة لجلب عنصر واحد عن طريق الـ ID (Primary Key)
        public virtual async Task<T> GetByIdAsync(object id)
        {
            // FindAsync هي الأسرع للبحث عن الـ Primary Key لأنها بتدور في الـ Cache الأول
            return await _dbContext.Set<T>().FindAsync(id);
        }

        // دي أسرع بكتير في الأداء من الاستعلام العادي
        public IQueryable<T> GetTableNoTracking()
        {
            return _dbContext.Set<T>().AsNoTracking().AsQueryable();
        }

        // Add entity to database asynchronously and save changes
        public virtual async Task AddRangeAsync(ICollection<T> entities)
        {
            await _dbContext.Set<T>().AddRangeAsync(entities);
            await _dbContext.SaveChangesAsync();

        }
        // Add single entity to database asynchronously and save changes
        public virtual async Task<T> AddAsync(T entity)
        {
            await _dbContext.Set<T>().AddAsync(entity);
            await _dbContext.SaveChangesAsync();

            return entity;
        }
        // Update entity in database asynchronously and save changes

        public virtual async Task UpdateAsync(T entity)
        {
            _dbContext.Set<T>().Update(entity);
            await _dbContext.SaveChangesAsync();

        }
        // Delete entity from database asynchronously and save changes
        public virtual async Task DeleteAsync(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
            await _dbContext.SaveChangesAsync();
        }
        // Delete multiple entities from database asynchronously and save changes
        public virtual async Task DeleteRangeAsync(ICollection<T> entities)
        {
            foreach (var entity in entities)
            {
                _dbContext.Entry(entity).State = EntityState.Deleted;
            }
            await _dbContext.SaveChangesAsync();
        }
        // Save changes to the database asynchronously
        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }


        // Transaction Management
        public IDbContextTransaction BeginTransaction()
        {


            return _dbContext.Database.BeginTransaction();
        }
        // Commit the current transaction
        public void Commit()
        {
            _dbContext.Database.CommitTransaction();

        }
        // Revert the transaction in case of an error.
        public void RollBack()
        {
            _dbContext.Database.RollbackTransaction();

        }
        // Get table as tracking (for updates)
        public IQueryable<T> GetTableAsTracking()
        {
            return _dbContext.Set<T>().AsQueryable();

        }
        // Update multiple entities in database asynchronously and save changes
        public virtual async Task UpdateRangeAsync(ICollection<T> entities)
        {
            _dbContext.Set<T>().UpdateRange(entities);
            await _dbContext.SaveChangesAsync();
        }
        #endregion
    }
}