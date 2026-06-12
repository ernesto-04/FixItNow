using FixItNow.Domain.Models.BookingRequest.DTOs.Reviews;
using FluentValidation;

namespace FixItNow.Application.Validators;

public class CreateReviewRequestValidator : AbstractValidator<CreateReviewRequest>
{
    public CreateReviewRequestValidator()
    {
        RuleFor(x => x.TicketId)
            .NotEmpty().WithMessage("Ticket reference is required.");

        RuleFor(x => x.Rating)
            .InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5.");

        RuleFor(x => x.Comment)
            .NotEmpty().WithMessage("Please leave a comment.")
            .MaximumLength(500).WithMessage("Comment cannot exceed 500 characters.");
    }
}