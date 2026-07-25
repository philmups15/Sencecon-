using Sencecon.Domain.Common;
using Sencecon.Domain.Enums;

namespace Sencecon.Domain.Entities;

public class User : BaseAuditableEntity
{
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.User;

    public ICollection<TodoItem> TodoItems { get; set; } = new List<TodoItem>();
}
