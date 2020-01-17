using FluentValidation;

namespace HackathonAlert.API.Core.DTO.Validators
{
    public class CreateAlertValidator : AbstractValidator<CreateAlertRequest>
    {
        public CreateAlertValidator()
        {
            RuleFor(createAlert => createAlert.AlarmId).NotEmpty().WithMessage("AlarmId must be specified.");
            RuleFor(createAlert => createAlert.SourceId).NotEmpty().WithMessage("SourceName must be specified.");
            RuleFor(createAlert => createAlert.StreamId).NotEmpty().WithMessage("StreamId must be specified.");

            RuleFor(createAlert => createAlert.Type).IsInEnum().WithMessage("AlertType must be a valid enum value.");

            RuleFor(createAlert => createAlert.Region).IsInEnum().WithMessage("AlarmRegion must be valid enum value.");

            RuleFor(createAlert => createAlert.Position).NotEmpty().SetValidator(new PositionMessageValidator());
            RuleFor(createAlert => createAlert.Target1Position).NotEmpty().SetValidator(new PositionMessageValidator());
            RuleFor(createAlert => createAlert.Target2Position).NotEmpty().SetValidator(new PositionMessageValidator());
            RuleFor(createAlert => createAlert.Target3Position).NotEmpty().SetValidator(new PositionMessageValidator());
        }
    }
}
