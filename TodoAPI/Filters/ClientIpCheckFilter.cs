using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace TodoAPI.Filters
{
    public class ClientIpCheckFilter : ActionFilterAttribute
    {
        private readonly IConfiguration _config;
        public ClientIpCheckFilter(IConfiguration config)
        {
            _config = config;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var remoteIp = context.HttpContext.Connection.RemoteIpAddress;
            Log.Information($"Request from Remote IP Address: {remoteIp}");

            string[] ip = _config["AdminSafeList"].Split(';');

            byte[] bytes = remoteIp.GetAddressBytes();
            bool badIp = true;

            foreach (var address in ip)
            {               
                try
                {
                    IPAddress testIp;

                    if (address.Contains("::1"))
                    {
                        testIp = IPAddress.IPv6Loopback;
                    }
                    else
                    {
                        testIp = IPAddress.Parse(address);
                    }

                    if (testIp.GetAddressBytes().SequenceEqual(bytes))
                    {
                        badIp = false;
                        break;
                    }
                    
                }
                catch (FormatException fex)
                {
                    Log.Error($"ClientIpFilterCheck.cs: Error validating IP: {fex.Message}");
                    badIp = true;
                    break;
                }
                               
            }

            if (badIp)
            {
                Log.Warning($"Forbidden Request from Remote IP address {remoteIp}");
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            }

            base.OnActionExecuting(context);
        }
    }
}
