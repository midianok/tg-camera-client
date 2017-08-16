using System.Collections.Generic;
using System.Linq;
using IpCameraClient.Infrastructure.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace IpCameraClient.Infrastructure.Repository
{
    public class EfRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private readonly DbContext _context;
        private readonly DbSet<TEntity> _dbSet;

        public EfRepository(DbContext context)
        {
            _context = context;
            _dbSet = _context.Set<TEntity>();
        }

        public IEnumerable<TEntity> GetAll() => _dbSet.AsEnumerable();

        public TEntity GetById(int id) => _dbSet.Find(id);

        public void Add(TEntity entity) => _dbSet.Add(entity);
        
        public void Delete(TEntity entity) => _dbSet.Remove(entity);
        
        public void Update(TEntity entity) => _dbSet.Update(entity);
        
        public void SaveChanges() => _context.SaveChanges();

    }
}
