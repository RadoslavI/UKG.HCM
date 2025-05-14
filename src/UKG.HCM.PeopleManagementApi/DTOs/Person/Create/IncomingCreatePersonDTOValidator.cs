using FluentValidation;

namespace UKG.HCM.PeopleManagementApi.DTOs.Person.Create;

public class IncomingCreatePersonDTOValidator : AbstractValidator<IncomingCreatePersonDTO>
{
    public IncomingCreatePersonDTOValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .MaximumLength(50);

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .MaximumLength(50);

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress();

        RuleFor(x => x.Role)
            .IsInEnum()
            .WithMessage("Role is invalid");
    }
}