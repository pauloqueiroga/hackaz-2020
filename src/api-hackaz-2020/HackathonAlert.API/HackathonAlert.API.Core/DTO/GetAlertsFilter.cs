using System.Collections.Generic;

namespace HackathonAlert.API.Core.DTO
{
    public class GetAlertsFilter
    {
        public int MinutesToSearch { get; set; }
        public List<string> SourceIds { get; set; }
    }
}
