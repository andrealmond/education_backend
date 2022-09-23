﻿using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ZaminEducation.Data.IRepositories
{
    public interface IRepository<TSource> where TSource : class
    {
        IQueryable<TSource> GetAll(Expression<Func<TSource, bool>> expression = null, string[] includes = null, bool isTracking = true);
        Task<TSource> AddAsync(TSource entity);
        Task<TSource> GetAsync(Expression<Func<TSource, bool>> expression, string[] includes);
        TSource Update(TSource entity);
        void Delete(TSource entity);
        Task SaveChangesAsync();
    }
}