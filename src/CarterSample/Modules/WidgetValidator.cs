using FluentValidation;

namespace CarterSample
{
    public class WidgetValidator : AbstractValidator<Widget>
    {
        public WidgetValidator()
        {
            RuleFor(x => x.Description).NotEmpty().WithMessage("is required");
        }
    }
}