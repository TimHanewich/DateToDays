using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Text.Encodings;
using System.Text;

namespace DateToDays
{
    public static class HttpTriggerCSharp1
    {
        
        [FunctionName("DateToDays")]
        public static HttpResponseMessage DateToDays([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest request, ILogger log)
        {
            string datestr = request.Query["date"];

            //Parse it
            log.LogInformation("Parsing provided date.");
            DateTime dt;
            try
            {
                dt = DateTime.Parse(datestr);
                log.LogInformation("Date received and successfully parsed: " + datestr);
            }
            catch
            {
                log.LogError("Unable to parse date: " + datestr);
                HttpResponseMessage emsg = new HttpResponseMessage();
                emsg.StatusCode = HttpStatusCode.BadRequest;
                emsg.Content = new StringContent("Unable to parse date '" + datestr + "'.");
                return emsg;
            }

            //Get the number of days since 1/1/2000
            log.LogInformation("Counting number of days since 1/1/2000");
            TimeSpan ts = dt - DateTime.Parse("1/1/2000");
            int days = Convert.ToInt32(Math.Round(ts.TotalDays,0));

            //Encode it
            log.LogInformation("Encoding");
            DaysReturn dr = new DaysReturn();
            dr.NumberOfDays = days;
            string as_json = JsonConvert.SerializeObject(dr);

            //Return it
            log.LogInformation("Returning package");
            HttpResponseMessage msg = new HttpResponseMessage();
            msg.Content = new StringContent(as_json, Encoding.UTF8, "application/json");
            msg.StatusCode = HttpStatusCode.OK;
            return msg;
        }

        [FunctionName("IsAfterNow")]
        public static HttpResponseMessage IsAfterNow([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req, ILogger log)
        {
            string datestr = req.Query["date"];

            //Parse it
            log.LogInformation("Parsing provided date.");
            DateTime dt;
            try
            {
                dt = DateTime.Parse(datestr);
                log.LogInformation("Date received and successfully parsed: " + datestr);
            }
            catch
            {
                log.LogError("Unable to parse date: " + datestr);
                HttpResponseMessage emsg = new HttpResponseMessage();
                emsg.StatusCode = HttpStatusCode.BadRequest;
                emsg.Content = new StringContent("Unable to parse date '" + datestr + "'.");
                return emsg;
            }

            //Evaluate
            log.LogInformation("Evaluating...");
            bool toreturn = false;
            if (dt > DateTime.Now)
            {
                toreturn = true;
            }
            else
            {
                toreturn = false;
            }


            //Encode
            log.LogInformation("Encoding package");
            ConditionalReturn cr = new ConditionalReturn();
            cr.Value = toreturn;
            string as_json = JsonConvert.SerializeObject(cr);

            //Return it
            log.LogInformation("Returning");
            HttpResponseMessage msg = new HttpResponseMessage();
            msg.Content = new StringContent(as_json, Encoding.UTF8, "application/json");
            msg.StatusCode = HttpStatusCode.OK;
            return msg;
        }

        public class DaysReturn
        {
            public int NumberOfDays {get; set;}
        }

        public class ConditionalReturn
        {
            public bool Value {get; set;}
        }

    }
}
