using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation.Results;
using HackathonAlert.API.Core.Domain;
using HackathonAlert.API.Core.DTO;
using HackathonAlert.API.Core.DTO.Validators;
using HackathonAlert.API.Core.Infrastructure;
using HackathonAlert.API.Core.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HackathonAlert.API.Core.Services
{
    public class AlertService : IAlertService
    {
        private readonly IAlertContextFactory _contextFactory;
        private SourceList _sourcesList;

        public AlertService(IAlertContextFactory contextFactory, SourceList sourceList)
        {
            _contextFactory = contextFactory;
            _sourcesList = sourceList;
        }

        public async Task<ActionResult<List<AlertMessage>>> GetAlertsAsync(GetAlertsFilter getAlertsFilter)
        {
            var result = await Task.Run(() => GetAlerts(getAlertsFilter));
            return result;
        }

        private ActionResult<List<AlertMessage>> GetAlerts(GetAlertsFilter getAlertsFilter)
        {
            var validator = new GetFilterValidator();
            var validationResult = validator.Validate(getAlertsFilter);

            if (!validationResult.IsValid)
            {
                return (ActionResult)GetActionResultFromErrors(validationResult);
            }

            // Key is TeamName, Value is Guid
            var kvDict = new Dictionary<string, string>();

            foreach (var guid in getAlertsFilter.SourceIds)
            {
                var teamName = GetTeamNameFromGuid(guid);

                if (string.IsNullOrEmpty(teamName) || teamName.Contains("Unassigned"))
                {
                    return new NotFoundObjectResult("Guid was not found with assigned team name.");
                }

                kvDict[teamName] = guid;
            }

            var timeToLookFor = DateTime.UtcNow.Subtract(new TimeSpan(0, getAlertsFilter.MinutesToSearch, 0));

            using (var context = _contextFactory.AlertContext())
            {
                var query = context.Alerts.AsQueryable().Include("Positions");

                query = query.Where(alr => alr.TimePosted > timeToLookFor);

                if (kvDict.Keys.Count > 0)
                {
                    query = query.Where(alr => kvDict.Keys.Contains(alr.SourceName));
                }

                var result = query.Select(alert => alert.ToAlertMessage()).ToList();

                foreach (var alertMessage in result)
                {
                    alertMessage.SourceId = kvDict[alertMessage.SourceId];
                }

                return new OkObjectResult(result);
            }
        }

        public async Task<IActionResult> CreateAlertAsync(CreateAlertRequest createAlertRequest)
        {
            var result = await Task.Run(() => CreateAlert(createAlertRequest));
            return result;
        }

        private IActionResult CreateAlert(CreateAlertRequest createAlertRequest)
        {
            var validator = new CreateAlertValidator();
            var validationResult = validator.Validate(createAlertRequest);

            if (!validationResult.IsValid)
            {
                return GetActionResultFromErrors(validationResult);
            }

            var teamName = GetTeamNameFromGuid(createAlertRequest.SourceId);

            if (string.IsNullOrEmpty(teamName) || teamName.Contains("Unassigned"))
            {
                return new BadRequestObjectResult("Guid was not found with assigned team name.");
            }

            var newAlert = Alert.FromCreateRequest(createAlertRequest);
            newAlert.SourceName = teamName;
            newAlert.TimePosted = DateTime.UtcNow;

            using (var context = _contextFactory.AlertContext())
            {
                return SaveAlertToDb(context, newAlert);
            }
        }

        private string GetTeamNameFromGuid(string guid)
        {
            foreach (var source in _sourcesList.Sources.Where(source => source.Guid == guid))
            {
                return source.TeamName;
            }

            return string.Empty;
        }

        private IActionResult SaveAlertToDb(AlertApiContext context, Alert newAlert)
        {
            context.Alerts.Add(newAlert);

            try
            {
                var numAffected = context.SaveChanges();

                // Alert, and the 4 positions
                if (numAffected < 5)
                {
                    // TODO: Log
                    return new StatusCodeResult(500);
                }
            }
            catch (DbUpdateException ex)
            {
                // TODO: Log
                return new StatusCodeResult(500);
            }

            return new CreatedResult("", newAlert.AlarmId);
        }

        private static IActionResult GetActionResultFromErrors(ValidationResult validationResult)
        {
            var stringBuilder = new StringBuilder();
            foreach (var validationResultError in validationResult.Errors)
            {
                stringBuilder.Append(validationResultError.ErrorMessage);
                stringBuilder.Append(" ");
            }

            return new BadRequestObjectResult(stringBuilder.ToString().Trim());
        }
    }
}
