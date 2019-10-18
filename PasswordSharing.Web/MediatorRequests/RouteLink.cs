using System;
using System.Net;
using System.Security.Policy;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using PasswordSharing.Web.Configs;
using PasswordSharing.Web.Exceptions;

namespace PasswordSharing.Web.MediatorRequests
{
    public class RouteLinkRequest : IRequest<string>
    {
        public RouteLinkRequest(IUrlHelper urlHelper, Guid passwordGroupId, string key)
        {
            UrlHelper = urlHelper;
            PasswordGroupId = passwordGroupId;
            Key = key;
        }

        public IUrlHelper UrlHelper { get; }
        public Guid PasswordGroupId { get; }
        public string Key { get; }
    }

    public class RouteLinkResponse : IRequestHandler<RouteLinkRequest, string>
    {
        private readonly FabioConfig _fabioConfig;
        private readonly ConsulConfig _consulConfig;

        public RouteLinkResponse(FabioConfig fabioConfig, ConsulConfig consulConfig)
        {
            _fabioConfig = fabioConfig;
            _consulConfig = consulConfig;
        }

        public Task<string> Handle(RouteLinkRequest request, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrWhiteSpace(_fabioConfig?.Url))
            {
                if (string.IsNullOrWhiteSpace(_consulConfig?.ServiceName))
                {
                    throw new HttpResponseException(HttpStatusCode.InternalServerError, "Service configured wrong");
                }

                var fabioUrl = _fabioConfig.Url;
                var serviceName = _consulConfig.ServiceName;

                return Task.FromResult($"{fabioUrl.Trim('/')}/{serviceName}/api/password/" +
                                       $"{request.PasswordGroupId}?key={HttpUtility.UrlEncode(request.Key)}");
            }

            return Task.FromResult(request.UrlHelper.Link("GetPassword", new
            {
                passwordGroupId = request.PasswordGroupId,
                key = request.Key,
                controller = "password"
            }));
        }
    }
}
