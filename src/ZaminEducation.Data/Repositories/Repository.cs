﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ZaminEducation.Data.DbContexts;
using ZaminEducation.Data.IRepositories;

namespace ZaminEducation.Data.Repositories
{
    public class Repository<TSource> : IRepository<TSource> where TSource : class
    {
        protected readonly ZaminEducationDbContext dbContext;
        protected readonly DbSet<TSource> dbSet;

        public Repository(ZaminEducationDbContext dbContext)
        {
            this.dbContext = dbContext;
            this.dbSet = dbContext.Set<TSource>();
        }

        public async Task<TSource> AddAsync(TSource entity)
        {
            var entry = await dbSet.AddAsync(entity);

            return entry.Entity;
        }

        public void Delete(TSource entity)
            => dbSet.Remove(entity);

        public IQueryable<TSource> GetAll(Expression<Func<TSource, bool>> expression = null, string include = null, bool isTracking = true)
        {
            IQueryable<TSource> query = expression is null ? dbSet : dbSet.Where(expression);

            if (!string.IsNullOrEmpty(include))
                query = query.Include(include);

            if (!isTracking)
                query = query.AsNoTracking();

            return query;
        }

        public async Task<TSource> GetAsync(Expression<Func<TSource, bool>> expression = null, string include = null)
            => await GetAll(expression, include).FirstOrDefaultAsync();

        public TSource Update(TSource entity)
            => dbSet.Update(entity).Entity;

        public async Task SaveChangesAsync()
            => await dbContext.SaveChangesAsync();
    }
}