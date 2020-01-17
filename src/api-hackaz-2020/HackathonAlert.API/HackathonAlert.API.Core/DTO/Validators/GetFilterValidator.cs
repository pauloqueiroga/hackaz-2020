using FluentValidation;

namespace HackathonAlert.API.Core.DTO.Validators
{
    public class GetFilterValidator : AbstractValidator<GetAlertsFilter>
    {
        public GetFilterValidator()
        {
            RuleFor(filter => filter.MinutesToSearch).GreaterThanOrEqualTo(1).LessThanOrEqualTo(1440)
                .WithMessage("Filter on Minutes must be greater than or equal to 1 minutes or less than or equal to 1440 minutes");
            RuleFor(filter => filter.SourceIds).NotEmpty()
                .WithMessage("At least 1 source Id Guid must be given to retrieve alerts");
        }
    }
}
