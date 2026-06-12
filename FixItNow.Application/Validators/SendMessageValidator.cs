using FixItNow.Domain.Models.BookingRequest.DTOs.Chat;
using FluentValidation;

namespace FixItNow.Application.Validators;

public class SendMessageValidator : AbstractValidator<SendMessageDto>
{
    public SendMessageValidator()
    {
        RuleFor(x => x.Message)
            .NotEmpty().WithMessage("Message cannot be empty.")
            .MaximumLength(1000).WithMessage("Message cannot exceed 1000 characters.");

        RuleFor(x => x.TicketId)
            .NotEmpty().WithMessage("Ticket reference is required.");
    }
}