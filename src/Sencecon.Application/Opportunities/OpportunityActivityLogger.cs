using System.Globalization;
using Sencecon.Application.Common.Interfaces;
using Sencecon.Domain.Entities;
using Sencecon.Domain.Enums;

namespace Sencecon.Application.Opportunities;

public static class OpportunityActivityLogger
{
    public static void Log(IApplicationDbContext context, Guid opportunityId, string type, string text, Guid? userId)
    {
        context.OpportunityActivities.Add(new OpportunityActivity
        {
            OpportunityId = opportunityId,
            Type = type,
            Text = text,
            UserId = userId,
            Created = DateTimeOffset.UtcNow
        });
    }

    public static string FormatMoney(decimal value) => "$" + value.ToString("N0", CultureInfo.InvariantCulture);

    public static string StageLabel(OpportunityStage stage) => stage switch
    {
        OpportunityStage.SiteVisit => "Site visit",
        _ => stage.ToString()
    };
}
