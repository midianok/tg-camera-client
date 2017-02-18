using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace IpCameraClient.Abstractions
{
    public interface IRepository<TEntity> where TEntity : class
    {
        IEnumerable<TEntity> Entities { get; }
        void Add(TEntity entity);
        void Update(TEntity entity);
        void Delete(TEntity entity);
    }
}
