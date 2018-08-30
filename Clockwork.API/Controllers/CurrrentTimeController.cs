using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Clockwork.API.Models;

namespace Clockwork.API.Controllers
{
    [Route("api/[controller]")]
    public class CurrentTimeController : Controller
    {
        // GET api/currenttime/all
        [HttpGet]
        public IEnumerable<CurrentTimeQuery> Get()
        {
            var db = new ClockworkContext();
            var returnVal = new List<CurrentTimeQuery>();
            var currentTimeQuery = new CurrentTimeQuery();

            foreach (CurrentTimeQuery clockwork in db.CurrentTimeQueries)
            {
                currentTimeQuery = new CurrentTimeQuery();
                currentTimeQuery.CurrentTimeQueryId = clockwork.CurrentTimeQueryId;
                currentTimeQuery.ClientIp = clockwork.ClientIp;
                currentTimeQuery.Time = clockwork.Time;
                currentTimeQuery.UTCTime = clockwork.UTCTime;
                currentTimeQuery.TimeZoneName = clockwork.TimeZoneName;
                returnVal.Add(currentTimeQuery);
            }

            return returnVal;
        }

        // GET api/currenttime/timezone id
        [HttpGet("{id}")]
        public IActionResult Get(string id)
        {
            var returnVal = new CurrentTimeQuery();
            try
            {
                TimeZoneInfo hwZone = TimeZoneInfo.FindSystemTimeZoneById(id);
                var utcTime = DateTime.UtcNow;
                var serverTime = DateTime.Now;
                var ip = this.HttpContext.Connection.RemoteIpAddress.ToString();
                    
                returnVal.UTCTime = utcTime;
                returnVal.ClientIp = ip;
                returnVal.Time = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, hwZone);
                returnVal.TimeZoneName = id;

                using (var db = new ClockworkContext())
                {                                        
                    db.CurrentTimeQueries.Add(returnVal);
                    var count = db.SaveChanges();
                    Console.WriteLine("{0} records saved to database", count);

                    Console.WriteLine();
                    foreach (var CurrentTimeQuery in db.CurrentTimeQueries)
                    {
                        Console.WriteLine(" - {0}", CurrentTimeQuery.UTCTime);
                    }
                }
            }
            catch (TimeZoneNotFoundException)
            {
                return NotFound();
            }
            catch (InvalidTimeZoneException)
            {
                return NotFound();
            }
            
            return Ok(returnVal);
        }
    }
}
