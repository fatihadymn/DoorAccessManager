using DoorAccessManager.Items.Models.Requests;
using FluentValidation;

namespace DoorAccessManager.Items.Validators
{
    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator()
        {
            RuleFor(x => x.Username).NotEmpty()
                                    .NotNull()
                                    .MinimumLength(3)
                                    .MaximumLength(50);

            RuleFor(x => x.Password).NotEmpty()
                                  .NotNull()
                                  .MinimumLength(3)
                                  .MaximumLength(50);

        }
    }
}
