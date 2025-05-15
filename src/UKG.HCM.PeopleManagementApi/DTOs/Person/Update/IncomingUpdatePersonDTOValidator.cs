using FluentValidation;

using UKG.HCM.Shared.Constants;

namespace UKG.HCM.PeopleManagementApi.DTOs.Person.Update;

public class IncomingUpdatePersonDTOValidator : AbstractValidator<IncomingUpdatePersonDTO>
{
    public IncomingUpdatePersonDTOValidator()
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
            .NotEmpty()
            .Must(role => ApplicationRoles.All.Contains(role))
            .WithMessage("Role is invalid");
    }
}