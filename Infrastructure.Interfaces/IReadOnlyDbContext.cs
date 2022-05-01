using Entities;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Interfaces
{
    public interface IReadOnlyDbContext
    {
        DbSet<Order> Orders { get; }
    }
}