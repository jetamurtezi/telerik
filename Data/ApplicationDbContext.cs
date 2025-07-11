﻿using Microsoft.EntityFrameworkCore;
using telerik.Models;

namespace telerik.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Book> Book { get; set; }
    }
}
