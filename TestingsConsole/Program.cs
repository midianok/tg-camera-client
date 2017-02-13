using System;
using IpCameraClient.Abstractions;
using IpCameraClient.Db;
using IpCameraClient.Model;
using IpCameraClient.Repository;
using System.Collections.Generic;
using System.Linq;

namespace TestingsConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var cameras = new EfRepository<Camera>(new SQLiteContext());
            var records = new EfRepository<Record>(new SQLiteContext());
            //repo.Add(new Camera
            //{
            //    Model = "123123"
            //});

            //repo.Add(new Camera
            //{
            //    Model = "3123123",
            //    Records = new List<Record>
            //    {
            //        new Record
            //        {
            //            DateTime = DateTime.Now,
            //            ImgLocation = "D:\test11223.jpg"
            //        }
            //    }
            //});
            var t2 = cameras.Entities.ToList();
        }
    }
}