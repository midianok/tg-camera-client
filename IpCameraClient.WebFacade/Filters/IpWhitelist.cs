using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace IpCameraClient.WebFacade.Filters
{
    public class IpWhitelistAttribute : ActionFilterAttribute
    {
        private readonly IEnumerable<IPAddress> _ipWhiteList;

        private readonly IPAddress _ipFrom;
        private readonly IPAddress _ipTo;

        public IpWhitelistAttribute(string ipFrom, string ipTo)
        {
            _ipFrom = IPAddress.Parse(ipFrom);
            _ipTo = IPAddress.Parse(ipTo);
        }
            
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var lowerBytes = _ipFrom.GetAddressBytes();
            var upperBytes = _ipTo.GetAddressBytes();
            var addressBytes = context.HttpContext.Connection.RemoteIpAddress.GetAddressBytes();

            bool lowerBoundary = true, upperBoundary = true;
            for (int i = 0; i < lowerBytes.Length && (lowerBoundary || upperBoundary); i++)
            {
                if ((lowerBoundary && addressBytes[i] < lowerBytes[i]) || (upperBoundary && addressBytes[i] > upperBytes[i]))
                    context.Result = new NotFoundResult();

                lowerBoundary &= (addressBytes[i] == lowerBytes[i]);
                upperBoundary &= (addressBytes[i] == upperBytes[i]);
            }
        }

    }
}
