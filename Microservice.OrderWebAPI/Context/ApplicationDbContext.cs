using Microservice.OrderWebAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace Microservice.OrderWebAPI.Context;

public sealed class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Order> Orders { get; set; }
}
