using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace tr.Models
{
    public class SubjectDbContext : DbContext 
    {
        public SubjectDbContext(DbContextOptions<SubjectDbContext> options)
            : base (options)
        { }

        public DbSet<Subject> Subjects { get; set; }
    }
}
