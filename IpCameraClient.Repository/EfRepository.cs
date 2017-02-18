using Microsoft.EntityFrameworkCore;
using IpCameraClient.Abstractions;
using System.Collections.Generic;

namespace IpCameraClient.Repository
{
    public class EfRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        DbContext _context;
        DbSet<TEntity> _dbSet;

        public IEnumerable<TEntity> Entities
        {
            get
            {
                return _dbSet;
            }
        }

        public EfRepository(DbContext context)
        {
            _context = context;
            _dbSet = _context.Set<TEntity>();
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
