using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using IpCameraClient.Infrastructure.Abstractions;
using LiteDB;

namespace IpCameraClient.Infrastructure.Repository
{
    public class LiteDbRepository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private readonly string _dbPath;
        
        public LiteDbRepository() =>
            _dbPath = $"{Directory.GetCurrentDirectory()}\\CameraClient.db";
        
        
        public IEnumerable<TEntity> GetAll()
        {
            using (var db = new LiteDatabase(_dbPath))
            {
                var collection = db.GetCollection<TEntity>();
                if (collection.Count() == 0) return new List<TEntity>();  
                return collection.FindAll().ToList();
            }
        }

        public TEntity GetById(int id)
        {
            using (var db = new LiteDatabase(_dbPath))
            {
                var collection = db.GetCollection<TEntity>();
                var item = collection.FindOne(Query.EQ("_id", id));
                if (item == null) throw new ArgumentException("Item not found");
                return item;
                    
            }
        }

        public void Add(TEntity entity)
        {
            using (var db = new LiteDatabase(_dbPath))
            {
                var collection = db.GetCollection<TEntity>();
                collection.Insert(entity);
            }
        }
        
        public void AddRange(IEnumerable<TEntity> entities)
        {
            using (var db = new LiteDatabase(_dbPath))
            {
                var collection = db.GetCollection<TEntity>();
                collection.Insert(entities);
            }
        }

        public void Update(TEntity entity)
        {
            using (var db = new LiteDatabase(_dbPath))
            {
                var collection = db.GetCollection<TEntity>();
                var updated = collection.Update(entity);
                if (!updated) throw new ArgumentException("Item not found");
            }
        }

        public void DeleteById(int id)
        {
            using (var db = new LiteDatabase(_dbPath))
            {
                var collection = db.GetCollection<TEntity>();
                var deleted = collection.Delete(id);
                if (!deleted) throw new ArgumentException("Item not found");
            }
        }
    }
}