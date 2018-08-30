using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Clockwork.API.Models;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Clockwork.API.Controllers
{
    [Route("api/[controller]")]
    public class TimeZonesController : Controller
    {
        // GET: /<controller>/
        [HttpGet]
        public IEnumerable<TimeZones> Get()
        {
            ReadOnlyCollection<TimeZoneInfo> tzCollection;
            tzCollection = TimeZoneInfo.GetSystemTimeZones();
            var listTZ = new List<TimeZones>();
            var TZ = new TimeZones();

            foreach (var timezone in tzCollection)
            {
                TZ = new TimeZones();
                TZ.DisplayName = timezone.DisplayName;
                TZ.Id = timezone.Id;
                
                listTZ.Add(TZ);
            }

            return listTZ;                 
        }

        /* Get local time based on Timezone ID/name */
        [HttpGet("{id}")]
        public string Get(string id)
        {
            string results = string.Empty;

            try
            {
                TimeZoneInfo hwZone = TimeZoneInfo.FindSystemTimeZoneById(id);
                results = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, hwZone).ToString();
            }
            catch (TimeZoneNotFoundException)
            {
                results = "Unable to find the "+id+" zone in the registry.";
            }
            catch (InvalidTimeZoneException)
            {
                results = "Registry data on the " + id + " zone has been corrupted.";
            }

            return results;
        }
    }
}
