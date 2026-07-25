using Microsoft.EntityFrameworkCore;
using Sencecon.Domain.Entities;

namespace Sencecon.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<TodoItem> TodoItems { get; }
    DbSet<User> Users { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
