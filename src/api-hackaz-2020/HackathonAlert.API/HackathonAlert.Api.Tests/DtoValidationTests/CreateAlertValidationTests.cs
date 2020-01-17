using System.Linq;
using HackathonAlert.API.Core.DTO;
using HackathonAlert.API.Core.DTO.Validators;
using NUnit.Framework;

namespace HackathonAlert.Api.Tests.DtoValidationTests
{
    public class CreateAlertValidationTests
    {
        [Test]
        public void OnValidationCreateAlertValidatorShouldReturnNoErrorsWithValidMessage()
        {
            var createRequest = TestHelper.CreateRandomValidCreateAlertRequest();

            var validator = new CreateAlertValidator();

            var validationResult = validator.Validate(createRequest);

            Assert.IsTrue(validationResult.IsValid);
            Assert.IsEmpty(validationResult.Errors);
        }

        [Test]
        public void OnValidationCreateAlertValidatorShouldReturnErrorWithInValidSourceId()
        {
            var createRequest = TestHelper.CreateRandomValidCreateAlertRequest();
            createRequest.SourceId = "";

            var validator = new CreateAlertValidator();

            var validationResult = validator.Validate(createRequest);

            Assert.IsFalse(validationResult.IsValid);
            Assert.AreEqual(1, validationResult.Errors.Count);
            Assert.IsNotEmpty(validationResult.Errors.First().ErrorMessage);
        }

        [Test]
        public void OnValidationCreateAlertValidatorShouldReturnErrorWithInValidStreamId()
        {
            var createRequest = TestHelper.CreateRandomValidCreateAlertRequest();
            createRequest.StreamId = "";

            var validator = new CreateAlertValidator();

            var validationResult = validator.Validate(createRequest);

            Assert.IsFalse(validationResult.IsValid);
            Assert.AreEqual(1, validationResult.Errors.Count);
            Assert.IsNotEmpty(validationResult.Errors.First().ErrorMessage);
        }

        [Test]
        public void OnValidationCreateAlertValidatorShouldReturnErrorWithInValidAlarmId()
        {
            var createRequest = TestHelper.CreateRandomValidCreateAlertRequest();
            createRequest.AlarmId = "";

            var validator = new CreateAlertValidator();

            var validationResult = validator.Validate(createRequest);

            Assert.IsFalse(validationResult.IsValid);
            Assert.AreEqual(1, validationResult.Errors.Count);
            Assert.IsNotEmpty(validationResult.Errors.First().ErrorMessage);
        }

        [Test]
        public void OnValidationCreateAlertValidatorShouldReturnErrorWithUnspecifiedWarningType()
        {
            var createRequest = TestHelper.CreateRandomValidCreateAlertRequest();
            createRequest.Type = WarningTypeMessage.Unspecified;

            var validator = new CreateAlertValidator();

            var validationResult = validator.Validate(createRequest);

            Assert.IsFalse(validationResult.IsValid);
            Assert.AreEqual(1, validationResult.Errors.Count);
            Assert.IsNotEmpty(validationResult.Errors.First().ErrorMessage);
        }

        [Test]
        public void OnValidationCreateAlertValidatorShouldReturnErrorWithInvalidWarningType()
        {
            var createRequest = TestHelper.CreateRandomValidCreateAlertRequest();
            createRequest.Type = (WarningTypeMessage) 9;

            var validator = new CreateAlertValidator();

            var validationResult = validator.Validate(createRequest);

            Assert.IsFalse(validationResult.IsValid);
            Assert.AreEqual(1, validationResult.Errors.Count);
            Assert.IsNotEmpty(validationResult.Errors.First().ErrorMessage);
        }

        [Test]
        public void OnValidationCreateAlertValidatorShouldReturnErrorWithUnspecifiedRegion()
        {
            var createRequest = TestHelper.CreateRandomValidCreateAlertRequest();
            createRequest.Region = AlarmRegionMessage.Unspecified;

            var validator = new CreateAlertValidator();

            var validationResult = validator.Validate(createRequest);

            Assert.IsFalse(validationResult.IsValid);
            Assert.AreEqual(1, validationResult.Errors.Count);
            Assert.IsNotEmpty(validationResult.Errors.First().ErrorMessage);
        }

        [Test]
        public void OnValidationCreateAlertValidatorShouldReturnErrorWithInvalidRegion()
        {
            var createRequest = TestHelper.CreateRandomValidCreateAlertRequest();
            createRequest.Region = (AlarmRegionMessage)9;

            var validator = new CreateAlertValidator();

            var validationResult = validator.Validate(createRequest);

            Assert.IsFalse(validationResult.IsValid);
            Assert.AreEqual(1, validationResult.Errors.Count);
            Assert.IsNotEmpty(validationResult.Errors.First().ErrorMessage);
        }

        [Test]
        public void OnValidationCreateAlertValidatorShouldReturnErrorWithInvalidPosition()
        {
            var createRequest = TestHelper.CreateRandomValidCreateAlertRequest();
            createRequest.Position.Longitude = 200;

            var validator = new CreateAlertValidator();

            var validationResult = validator.Validate(createRequest);

            Assert.IsFalse(validationResult.IsValid);
            Assert.AreEqual(1, validationResult.Errors.Count);
            Assert.IsNotEmpty(validationResult.Errors.First().ErrorMessage);
        }

        [Test]
        public void OnValidationCreateAlertValidatorShouldReturnErrorWithInvalidTarget1Position()
        {
            var createRequest = TestHelper.CreateRandomValidCreateAlertRequest();
            createRequest.Target1Position.Longitude = 200;

            var validator = new CreateAlertValidator();

            var validationResult = validator.Validate(createRequest);

            Assert.IsFalse(validationResult.IsValid);
            Assert.AreEqual(1, validationResult.Errors.Count);
            Assert.IsNotEmpty(validationResult.Errors.First().ErrorMessage);
        }

        [Test]
        public void OnValidationCreateAlertValidatorShouldReturnErrorWithInvalidTarget2Position()
        {
            var createRequest = TestHelper.CreateRandomValidCreateAlertRequest();
            createRequest.Target2Position.Longitude = 200;

            var validator = new CreateAlertValidator();

            var validationResult = validator.Validate(createRequest);

            Assert.IsFalse(validationResult.IsValid);
            Assert.AreEqual(1, validationResult.Errors.Count);
            Assert.IsNotEmpty(validationResult.Errors.First().ErrorMessage);
        }

        [Test]
        public void OnValidationCreateAlertValidatorShouldReturnErrorWithInvalidTarget3Position()
        {
            var createRequest = TestHelper.CreateRandomValidCreateAlertRequest();
            createRequest.Target3Position.Longitude = 200;

            var validator = new CreateAlertValidator();

            var validationResult = validator.Validate(createRequest);

            Assert.IsFalse(validationResult.IsValid);
            Assert.AreEqual(1, validationResult.Errors.Count);
            Assert.IsNotEmpty(validationResult.Errors.First().ErrorMessage);
        }
    }
}
