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
        RuleFor(x => x.PhoneNumber)
            .Must(phone => string.IsNullOrWhiteSpace(phone) ||
                           System.Text.RegularExpressions.Regex.IsMatch(phone, @"^0[0-9]{9,12}$"))
            .WithMessage("Enter a valid Indonesian phone number (e.g. 08123456789)");
    }
}