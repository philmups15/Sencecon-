using FluentValidation;

namespace Sencecon.Application.Opportunities.Commands.UploadOpportunityAttachments;

public class UploadOpportunityAttachmentsCommandValidator : AbstractValidator<UploadOpportunityAttachmentsCommand>
{
    private const int MaxFiles = 5;
    private const long MaxFileSizeBytes = 10 * 1024 * 1024;

    public UploadOpportunityAttachmentsCommandValidator()
    {
        RuleFor(v => v.Files)
            .NotEmpty()
            .WithMessage("At least one file is required.")
            .Must(files => files.Count <= MaxFiles)
            .WithMessage($"No more than {MaxFiles} files can be uploaded at once.");

        RuleFor(v => v.Title)
            .MaximumLength(200);

        RuleForEach(v => v.Files).ChildRules(file =>
        {
            file.RuleFor(f => f.FileName)
                .NotEmpty()
                .MaximumLength(260);

            file.RuleFor(f => f.Content.LongLength)
                .LessThanOrEqualTo(MaxFileSizeBytes)
                .WithMessage("Each file must be 10 MB or smaller.");
        });
    }
}
