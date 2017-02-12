using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Net;

namespace IpCameraClient.WebFacade.Filters
{
    public class IpWhitelistAttribute : ActionFilterAttribute
    {
        private readonly string[] _ipWhiteList;

        public IpWhitelistAttribute(string[] ipWhiteList)=>
            _ipWhiteList = ipWhiteList;
        

        public override void OnActionExecuting(ActionExecutingContext context)
        {

            //var ip = IPAddress.Parse("176.100.77.158");
            
            //if (context.HttpContext.Connection.RemoteIpAddress != ip)
            //    context.Result = new JsonResult("No");

        }
    }
}
