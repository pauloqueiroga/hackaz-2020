using FluentValidation;

namespace HackathonAlert.API.Core.DTO.Validators
{
    public class PositionMessageValidator : AbstractValidator<PositionMessage>
    {
        public PositionMessageValidator()
        {
            RuleFor(position => position.Latitude).GreaterThan(-90).LessThan(90).WithMessage("Latitude must greater than -90 and less than 90.");
            RuleFor(position => position.Longitude).GreaterThan(-180).LessThan(180).WithMessage("Longitude must be greater than -180 and less than 180.");
            RuleFor(position => position.Heading).GreaterThanOrEqualTo(0).LessThanOrEqualTo(360).WithMessage("Heading must be equal to or greater than 0 and less than or equal to 360.");
            // TODO: Can we go backwards?
            RuleFor(position => position.Velocity).GreaterThanOrEqualTo(0).WithMessage("Velocity must be greater than or equal to 0.");
        }
    }
}