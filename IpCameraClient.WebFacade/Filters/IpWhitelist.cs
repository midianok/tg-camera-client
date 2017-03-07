using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace IpCameraClient.WebFacade.Filters
{
    public class IpWhitelistAttribute : ActionFilterAttribute
    {
        private readonly IPAddress _ipFrom;
        private readonly IPAddress _ipTo;

        public IpWhitelistAttribute(string ipFrom, string ipTo)
        {
            _ipFrom = IPAddress.Parse(ipFrom);
            _ipTo = IPAddress.Parse(ipTo);
        }
            
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var addressBytes = context.HttpContext.Connection.RemoteIpAddress.GetAddressBytes();
            var localIpV4 = IPAddress.Parse("127.0.0.1").GetAddressBytes();
            var localIpV6 = IPAddress.Parse("::1").GetAddressBytes();

            if (addressBytes.SequenceEqual(localIpV4) || addressBytes.SequenceEqual(localIpV6))
                return;

            var lowerBytes = _ipFrom.GetAddressBytes();
            var upperBytes = _ipTo.GetAddressBytes();
            

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
