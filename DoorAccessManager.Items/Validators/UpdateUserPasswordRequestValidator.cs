using DoorAccessManager.Items.Models.Requests;
using FluentValidation;

namespace DoorAccessManager.Items.Validators
{
    public class UpdateUserPasswordRequestValidator : AbstractValidator<UpdateUserPasswordRequest>
    {
        public UpdateUserPasswordRequestValidator()
        {
            RuleFor(x => x.OldPassword).NotEmpty()
                                       .NotNull()
                                       .MinimumLength(3)
                                       .MaximumLength(50);

            RuleFor(x => x.NewPassword).NotEmpty()
                                       .NotNull()
                                       .MinimumLength(3)
                                       .MaximumLength(50);
        }
    }
}
