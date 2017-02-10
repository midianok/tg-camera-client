using Microsoft.EntityFrameworkCore;
using IpCameraClient.Abstractions;
using System.Collections.Generic;
using System;
using System.Linq.Expressions;

namespace IpCameraClient.Repository
{
    public class EfRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        DbContext _context;
        DbSet<TEntity> _dbSet;
        Expression<Func<TEntity, object>> _includes;

        public EfRepository(DbContext context, Expression<Func<TEntity, object>> includes)
        {
            _context = context;
            _dbSet = _context.Set<TEntity>();
            _includes = includes;
        }

        public EfRepository(DbContext context)
        {
            _context = context;
            _dbSet = _context.Set<TEntity>();
        }

        public IEnumerable<TEntity> GetAll()
        {
            if (_includes != null)
                return _dbSet.Include(_includes);
            else
                return _dbSet;
        }

        public void Add(TEntity entity)
        {
            _dbSet.Add(entity);
            _context.SaveChanges();
        }

        public void Delete(TEntity entity)
        {
            _dbSet.Remove(entity);
            _context.SaveChanges();
        }

        public void Update(TEntity entity)
        {
            _dbSet.Update(entity);
            _context.SaveChanges();
        }
    }
}
