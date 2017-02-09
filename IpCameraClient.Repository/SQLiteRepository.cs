using Microsoft.EntityFrameworkCore;
using IpCameraClient.Db;
using IpCameraClient.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace IpCameraClient.Repository
{
    class SQLiteRepository : IRepository<Record>
    {
        private readonly SQLiteContext _context;

        public IEnumerable<Record> Entities => _context.Records;

        public SQLiteRepository() => _context = new SQLiteContext();

        public void Add(Record entity)
        {
            _context.Records.Add(entity);
            _context.SaveChangesAsync();
        }

        public void Delete(Record entity)
        {
            _context.Records.Remove(entity);
            _context.SaveChangesAsync();
        }

        public void Update(Record entity)
        {
            _context.Records.Update(entity);
            _context.SaveChangesAsync();
        }
    }
}
