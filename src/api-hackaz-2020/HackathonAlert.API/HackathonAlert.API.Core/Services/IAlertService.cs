using System.Collections.Generic;
using System.Threading.Tasks;
using HackathonAlert.API.Core.DTO;
using Microsoft.AspNetCore.Mvc;

namespace HackathonAlert.API.Core.Services
{
    public interface IAlertService
    {
        Task<ActionResult<List<AlertMessage>>> GetAlertsAsync(GetAlertsFilter getAlertsFilter);
        Task<IActionResult> CreateAlertAsync(CreateAlertRequest createAlertRequest);
    }
}
