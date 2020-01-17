using System.Linq;
using HackathonAlert.API.Core.DTO.Validators;
using NUnit.Framework;

namespace HackathonAlert.Api.Tests.DtoValidationTests
{
    public class PositionMessageValidationTests
    {
        [Test]
        public void OnValidationTheCreateAlertValidatorShouldReturnNoErrorsWithValidPositionMessage()
        {
            var positionMessage = TestHelper.CreateRandomValidPositionMessage();

            var validator = new PositionMessageValidator();

            var validationResult = validator.Validate(positionMessage);

            Assert.IsTrue(validationResult.IsValid);
            Assert.IsEmpty(validationResult.Errors);
        }

        [Test]
        public void OnValidationTheCreateAlertValidatorShouldReturnErrorWithInvalidLatitudeMin()
        {
            var positionMessage = TestHelper.CreateRandomValidPositionMessage();
            positionMessage.Latitude = -90;

            var validator = new PositionMessageValidator();

            var validationResult = validator.Validate(positionMessage);

            Assert.IsFalse(validationResult.IsValid);
            Assert.AreEqual(1, validationResult.Errors.Count);
            Assert.IsNotEmpty(validationResult.Errors.First().ErrorMessage);
        }

        [Test]
        public void OnValidationTheCreateAlertValidatorShouldReturnErrorWithInvalidLatitudeMax()
        {
            var positionMessage = TestHelper.CreateRandomValidPositionMessage();
            positionMessage.Latitude = 90;

            var validator = new PositionMessageValidator();

            var validationResult = validator.Validate(positionMessage);

            Assert.IsFalse(validationResult.IsValid);
            Assert.AreEqual(1, validationResult.Errors.Count);
            Assert.IsNotEmpty(validationResult.Errors.First().ErrorMessage);
        }

        [Test]
        public void OnValidationTheCreateAlertValidatorShouldReturnErrorWithInvalidLongitudeMin()
        {
            var positionMessage = TestHelper.CreateRandomValidPositionMessage();
            positionMessage.Longitude = -180;

            var validator = new PositionMessageValidator();

            var validationResult = validator.Validate(positionMessage);

            Assert.IsFalse(validationResult.IsValid);
            Assert.AreEqual(1, validationResult.Errors.Count);
            Assert.IsNotEmpty(validationResult.Errors.First().ErrorMessage);
        }

        [Test]
        public void OnValidationTheCreateAlertValidatorShouldReturnErrorWithInvalidAltitude()
        {
            var positionMessage = TestHelper.CreateRandomValidPositionMessage();
            positionMessage.Altitude = -5180;

            var validator = new PositionMessageValidator();

            var validationResult = validator.Validate(positionMessage);

            Assert.IsFalse(validationResult.IsValid);
            Assert.AreEqual(1, validationResult.Errors.Count);
            Assert.IsNotEmpty(validationResult.Errors.First().ErrorMessage);
        }

        [Test]
        public void OnValidationTheCreateAlertValidatorShouldReturnErrorWithInvalidHeadingMin()
        {
            var positionMessage = TestHelper.CreateRandomValidPositionMessage();
            positionMessage.Heading = -1;

            var validator = new PositionMessageValidator();

            var validationResult = validator.Validate(positionMessage);

            Assert.IsFalse(validationResult.IsValid);
            Assert.AreEqual(1, validationResult.Errors.Count);
            Assert.IsNotEmpty(validationResult.Errors.First().ErrorMessage);
        }

        [Test]
        public void OnValidationTheCreateAlertValidatorShouldReturnErrorWithInvalidHeadingMax()
        {
            var positionMessage = TestHelper.CreateRandomValidPositionMessage();
            positionMessage.Heading = 361;

            var validator = new PositionMessageValidator();

            var validationResult = validator.Validate(positionMessage);

            Assert.IsFalse(validationResult.IsValid);
            Assert.AreEqual(1, validationResult.Errors.Count);
            Assert.IsNotEmpty(validationResult.Errors.First().ErrorMessage);
        }

        [Test]
        public void OnValidationTheCreateAlertValidatorShouldReturnErrorWithInvalidVelocity()
        {
            var positionMessage = TestHelper.CreateRandomValidPositionMessage();
            positionMessage.Velocity = -1;

            var validator = new PositionMessageValidator();

            var validationResult = validator.Validate(positionMessage);

            Assert.IsFalse(validationResult.IsValid);
            Assert.AreEqual(1, validationResult.Errors.Count);
            Assert.IsNotEmpty(validationResult.Errors.First().ErrorMessage);
        }
    }
}
