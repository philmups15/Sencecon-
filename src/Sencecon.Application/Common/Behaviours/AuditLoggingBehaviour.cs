using System.Text;
using MediatR;
using Sencecon.Application.Common.Interfaces;
using Sencecon.Domain.Entities;

namespace Sencecon.Application.Common.Behaviours;

public class AuditLoggingBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUserService;

    public AuditLoggingBehaviour(IApplicationDbContext context, ICurrentUserService currentUserService)
    {
        _context = context;
        _currentUserService = currentUserService;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var response = await next();

        var requestTypeName = typeof(TRequest).FullName ?? string.Empty;

        if (requestTypeName.Contains(".Commands.", StringComparison.Ordinal))
        {
            _context.AuditLogEntries.Add(new AuditLogEntry
            {
                UserId = _currentUserService.UserId,
                Action = Humanize(typeof(TRequest).Name),
                Created = DateTimeOffset.UtcNow
            });

            await _context.SaveChangesAsync(cancellationToken);
        }

        return response;
    }

    private static string Humanize(string commandTypeName)
    {
        var name = commandTypeName.EndsWith("Command", StringComparison.Ordinal)
            ? commandTypeName[..^"Command".Length]
            : commandTypeName;

        var builder = new StringBuilder();

        foreach (var c in name)
        {
            if (char.IsUpper(c) && builder.Length > 0)
            {
                builder.Append(' ');
            }

            builder.Append(c);
        }

        return builder.ToString();
    }
}
