using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HackathonAlert.API.Core.DTO;
using HackathonAlert.API.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSwag.Annotations;

namespace HackathonAlert.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [OpenApiTag("Alert", Description = "It's the Alert API")]
    public class AlertController : ControllerBase
    {
        private readonly ILogger<AlertController> _logger;
        private readonly IAlertService _alertOperations;

        public AlertController(ILogger<AlertController> logger, IAlertService alertOperations)
        {
            _logger = logger;
            _alertOperations = alertOperations;
        }

        [HttpGet("{sourceIds}")]
        public async Task<ActionResult<List<AlertMessage>>> Get(string sourceIds, int minutesToSearch = 60)
        {
            var listSources = new List<string>();
            if (!string.IsNullOrEmpty(sourceIds))
            {
                listSources = sourceIds.Split(',').ToList();
            }

            var getAlertsFilter = new GetAlertsFilter
            {
                MinutesToSearch = minutesToSearch,
                SourceIds = listSources
            };

            var result = await _alertOperations.GetAlertsAsync(getAlertsFilter);

            return result;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PostNewAlert(CreateAlertRequest createAlertRequest)
        {
            var result = await _alertOperations.CreateAlertAsync(createAlertRequest);
            return result;
        }
    }
}
