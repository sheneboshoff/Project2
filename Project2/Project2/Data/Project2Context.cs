using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Project2.Models;

namespace Project2.Data
{
    public class Project2Context : DbContext
    {
        public Project2Context (DbContextOptions<Project2Context> options)
            : base(options)
        {
        }

        public DbSet<Project2.Models.PhotoViewModel> PhotoViewModel { get; set; }
    }
}
