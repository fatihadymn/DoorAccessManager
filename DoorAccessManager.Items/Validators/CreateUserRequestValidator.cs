using DoorAccessManager.Items.Models.Requests;
using FluentValidation;

namespace DoorAccessManager.Items.Validators
{
    public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
    {
        public CreateUserRequestValidator()
        {
            RuleFor(x => x.Role).IsInEnum();

            RuleFor(x => x.Name).NotEmpty()
                                .NotNull()
                                .MinimumLength(1)
                                .MaximumLength(50);

            RuleFor(x => x.Username).NotNull()
                                    .MinimumLength(1)
                                    .NotEmpty()
                                    .MaximumLength(50);

            RuleFor(x => x.Password).NotNull()
                                    .NotEmpty() 
                                    .MinimumLength(3)
                                    .MaximumLength(50);

        }
    }
}
