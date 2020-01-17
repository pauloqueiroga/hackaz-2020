using System.Collections.Generic;
using System.Linq;
using HackathonAlert.API.Core.DTO;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace HackathonAlert.Api.Tests.ServiceTests
{
    public class AlertServiceTests
    {
        [Test]
        public void AlertServiceShouldSuccessfullyCreateAlertWhenGivenValidRequest()
        {
            var contextFactory = TestHelper.GetInMemoryTestContextFactory();
            var sourceList = TestHelper.GetTestSourceList();
            var alertService = TestHelper.GetTestAlertService(contextFactory, sourceList);

            var previousCount = contextFactory.AlertIds.Count;

            var createAlertRequest = TestHelper.CreateRandomValidCreateAlertRequest();
            createAlertRequest.SourceId = sourceList.Sources.First().Guid;
            var result = alertService.CreateAlertAsync(createAlertRequest).Result as CreatedResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(201, result.StatusCode);
            Assert.AreEqual(createAlertRequest.AlarmId, (string)result.Value);
            var expectedCount = previousCount + 1;
            Assert.AreEqual(expectedCount, TestHelper.GetNumberOfAlertsInDb(contextFactory));
        }

        [Test]
        public void AlertServiceShouldReturnErrorIfGivenInvalidRequestValue()
        {
            var contextFactory = TestHelper.GetInMemoryTestContextFactory();
            var sourceList = TestHelper.GetTestSourceList();
            var alertService = TestHelper.GetTestAlertService(contextFactory, sourceList);

            var previousCount = contextFactory.AlertIds.Count;

            var createAlertRequest = TestHelper.CreateRandomValidCreateAlertRequest();
            createAlertRequest.SourceId = sourceList.Sources.First().Guid;
            //Invalid Field
            createAlertRequest.AlarmId = string.Empty;

            var result = alertService.CreateAlertAsync(createAlertRequest).Result as BadRequestObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
            Assert.AreEqual(previousCount, TestHelper.GetNumberOfAlertsInDb(contextFactory));
        }

        [Test]
        public void AlertServiceShouldReturnErrorIfGivenUnassignedGuid()
        {
            var contextFactory = TestHelper.GetInMemoryTestContextFactory();
            var sourceList = TestHelper.GetTestSourceList();
            var alertService = TestHelper.GetTestAlertService(contextFactory, sourceList);

            var previousCount = contextFactory.AlertIds.Count;

            // Random Guid is assigned here, so it's not valid
            var createAlertRequest = TestHelper.CreateRandomValidCreateAlertRequest();

            var result = alertService.CreateAlertAsync(createAlertRequest).Result as BadRequestObjectResult;

            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
            Assert.AreEqual(previousCount, TestHelper.GetNumberOfAlertsInDb(contextFactory));
        }

        [Test]
        public void AlertServiceShouldReturnAlertsIfGivenValidFilter()
        {
            var contextFactory = TestHelper.GetInMemoryTestContextFactory();
            var sourceList = TestHelper.GetTestSourceList();
            var alertService = TestHelper.GetTestAlertService(contextFactory, sourceList);

            var previousCount = contextFactory.AlertIds.Count;

            var guidToUse = sourceList.Sources.First().Guid; 
            // Need to create an alert with the guid we want to use
            var createAlertRequest = TestHelper.CreateRandomValidCreateAlertRequest();
            createAlertRequest.SourceId = guidToUse;
            var createResult = alertService.CreateAlertAsync(createAlertRequest).Result as CreatedResult;
            
            Assert.IsNotNull(createResult);
            Assert.AreEqual(201, createResult.StatusCode);
            Assert.AreEqual(createAlertRequest.AlarmId, (string)createResult.Value);

            // Creating a second alert under a different guid to make sure only one appears
            var secondGuidToUse = sourceList.Sources[1].Guid;
            createAlertRequest = TestHelper.CreateRandomValidCreateAlertRequest();
            createAlertRequest.SourceId = secondGuidToUse;
            createResult = alertService.CreateAlertAsync(createAlertRequest).Result as CreatedResult;

            Assert.IsNotNull(createResult);
            Assert.AreEqual(201, createResult.StatusCode);
            Assert.AreEqual(createAlertRequest.AlarmId, (string)createResult.Value);

            var filter = new GetAlertsFilter
            {
                MinutesToSearch = 60,
                SourceIds = new List<string> { guidToUse }
            };

            var actionResult = alertService.GetAlertsAsync(filter).Result;
            var statusResult = actionResult.Result as OkObjectResult;

            Assert.IsNotNull(statusResult);
            Assert.AreEqual(200, statusResult.StatusCode);

            Assert.AreEqual(1, ((List<AlertMessage>)statusResult.Value).Count);
        }

        [Test]
        public void AlertServiceShouldReturnNoAlertsIfGivenValidFilterButNoAlerts()
        {
            var contextFactory = TestHelper.GetInMemoryTestContextFactory();
            var sourceList = TestHelper.GetTestSourceList();
            var alertService = TestHelper.GetTestAlertService(contextFactory, sourceList);

            var previousCount = contextFactory.AlertIds.Count;

            var guidToUse = sourceList.Sources.First().Guid;

            var filter = new GetAlertsFilter
            {
                MinutesToSearch = 60,
                SourceIds = new List<string> { guidToUse }
            };

            var actionResult = alertService.GetAlertsAsync(filter).Result;
            var statusResult = actionResult.Result as OkObjectResult;

            Assert.IsNotNull(statusResult);
            Assert.AreEqual(200, statusResult.StatusCode);

            Assert.AreEqual(0, ((List<AlertMessage>)statusResult.Value).Count);
        }

        [Test]
        public void NoAlertsShouldBeReturnedInGetAlertsIfMinutesToSearchIsSmallerThanTheTimeSinceLastAlertReceieved()
        {
            var contextFactory = TestHelper.GetInMemoryTestContextFactory();
            var sourceList = TestHelper.GetTestSourceList();
            var alertService = TestHelper.GetTestAlertService(contextFactory, sourceList);

            // Very hardcoded for this test, but we are just trying to make sure this works
            // basically an alert was seeded with the team name "SampleTeam3" which is paired with this guid
            // The timeposted was given 24 hours ago
            var guidToUse = "SampleGuid3";

            var filter = new GetAlertsFilter
            {
                MinutesToSearch = 1,
                SourceIds = new List<string> { guidToUse }
            };

            var actionResult = alertService.GetAlertsAsync(filter).Result;
            var statusResult = actionResult.Result as OkObjectResult;

            Assert.IsNotNull(statusResult);
            Assert.AreEqual(200, statusResult.StatusCode);
            // Still a valid result, just nothing was retrieved
            Assert.AreEqual(0, ((List<AlertMessage>)statusResult.Value).Count);
        }
    }
}
