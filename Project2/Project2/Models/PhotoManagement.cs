using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Project2.Models;

namespace Project2.Models
{
    public class PhotoManagement:DbContext
    {
        public PhotoManagement(DbContextOptions<PhotoManagement> options):base(options)
        {

        }
        public DbSet<Project2.Models.Photo> Photo { get; set; }
    }
}
