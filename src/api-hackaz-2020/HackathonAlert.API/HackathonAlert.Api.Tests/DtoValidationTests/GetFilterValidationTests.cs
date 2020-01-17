using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HackathonAlert.API.Core.DTO;
using HackathonAlert.API.Core.DTO.Validators;
using HackathonAlert.API.Core.Settings;
using NUnit.Framework;

namespace HackathonAlert.Api.Tests.DtoValidationTests
{
    public class GetFilterValidationTests
    {
        [Test]
        public void OnValidationTheGetFilterValidatorShouldReturnNoErrorsGivenAValidFilter()
        {
            var filter = new GetAlertsFilter
            {
                MinutesToSearch = 60,
                SourceIds = new List<string>()
            };

            filter.SourceIds.Add("OneString");

            var validator = new GetFilterValidator();

            var validationResult = validator.Validate(filter);

            Assert.IsTrue(validationResult.IsValid);
            Assert.IsEmpty(validationResult.Errors);
        }

        [Test]
        public void OnValidationTheGetFilterValidatorShouldReturnErrorWhenGivenBelowMinMinutes()
        {
            var filter = new GetAlertsFilter
            {
                MinutesToSearch = 0,
                SourceIds = new List<string>()
            };

            filter.SourceIds.Add("OneString");

            var validator = new GetFilterValidator();

            var validationResult = validator.Validate(filter);

            Assert.IsFalse(validationResult.IsValid);
            Assert.AreEqual(1, validationResult.Errors.Count);
            Assert.IsNotEmpty(validationResult.Errors.First().ErrorMessage);
        }

        [Test]
        public void OnValidationTheGetFilterValidatorShouldReturnErrorWhenGivenAboveMaxMinutes()
        {
            var filter = new GetAlertsFilter
            {
                MinutesToSearch = 1500,
                SourceIds = new List<string>()
            };

            filter.SourceIds.Add("OneString");

            var validator = new GetFilterValidator();

            var validationResult = validator.Validate(filter);

            Assert.IsFalse(validationResult.IsValid);
            Assert.AreEqual(1, validationResult.Errors.Count);
            Assert.IsNotEmpty(validationResult.Errors.First().ErrorMessage);
        }

        [Test]
        public void OnValidationTheGetFilterValidatorShouldReturnErrorWhenGivenNoSourceIds()
        {
            var filter = new GetAlertsFilter
            {
                MinutesToSearch = 60,
                SourceIds = new List<string>()
            };

            var validator = new GetFilterValidator();

            var validationResult = validator.Validate(filter);

            Assert.IsFalse(validationResult.IsValid);
            Assert.AreEqual(1, validationResult.Errors.Count);
            Assert.IsNotEmpty(validationResult.Errors.First().ErrorMessage);
        }
    }
}
