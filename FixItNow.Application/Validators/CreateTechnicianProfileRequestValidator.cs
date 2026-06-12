using FixItNow.Domain.Models.BookingRequest.DTOs.Technicians;
using FluentValidation;

public class CreateTechnicianProfileRequestValidator : AbstractValidator<CreateTechnicianProfileRequest>
{
    public CreateTechnicianProfileRequestValidator()
    {
        RuleFor(x => x.Skills)
            .NotEmpty().WithMessage("Skills are required.")
            .MaximumLength(300).WithMessage("Skills cannot exceed 300 characters.");

        RuleFor(x => x.Location)
            .NotEmpty().WithMessage("Location is required.")
            .MaximumLength(200).WithMessage("Location cannot exceed 200 characters.");

        RuleFor(x => x.Bio)
            .MaximumLength(500).WithMessage("Bio cannot exceed 500 characters.")
            .When(x => x.Bio is not null);
    }
}