using FixItNow.Domain.Models.BookingRequest.DTOs.Technicians;
using FluentValidation;

namespace FixItNow.Application.Validators;

public class UpdateTechnicianProfileValidator : AbstractValidator<UpdateTechnicianProfileDto>
{
    public UpdateTechnicianProfileValidator()
    {
        RuleFor(x => x.Bio)
            .MaximumLength(500).WithMessage("Bio cannot exceed 500 characters.")
            .When(x => x.Bio is not null);

        RuleFor(x => x.Location)
            .NotEmpty().WithMessage("Location is required.")
            .MaximumLength(200).WithMessage("Location cannot exceed 200 characters.");

        RuleFor(x => x.Skills)
            .NotEmpty().WithMessage("Please list at least one skill.")
            .MaximumLength(300).WithMessage("Skills cannot exceed 300 characters.");

        RuleFor(x => x.YearsExperience)
            .GreaterThanOrEqualTo(0).WithMessage("Years of experience cannot be negative.")
            .LessThanOrEqualTo(50).WithMessage("Please enter a realistic value for years of experience.");
    }
}