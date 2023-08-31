using Application.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Persistence.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Repositories
{
    public class WriteRepository<T> : IWriteRepository<T> where T : class
    {
        readonly private OrganizationDbContext _context;

        public WriteRepository(OrganizationDbContext context)
        {
            _context = context;
        }

        public DbSet<T> Table => _context.Set<T>();

        public async Task<bool> AddAsync(T model)

        {
            EntityEntry entityEntry = await Table.AddAsync(model);
            SaveAsync();
            return entityEntry.State == EntityState.Added;
        }
      

        public bool Remove(int id)
        {
            EntityEntry entityEntry = Table.Remove(Table.Find(id));
            SaveAsync();

            return entityEntry.State == EntityState.Deleted;
        }
        public bool Update(T model)
        {
            EntityEntry entityEntry = Table.Update(model);
            SaveAsync();
            return entityEntry.State == EntityState.Modified;

        }
        public Task<int> SaveAsync()
        { return _context.SaveChangesAsync(); }

    }
}
