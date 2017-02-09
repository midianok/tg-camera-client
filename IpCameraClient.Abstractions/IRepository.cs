using System.Collections.Generic;

namespace IpCameraClient.Abstractions
{
    public interface IRepository<T>
    {
        IEnumerable<T> Entities { get;}
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}
