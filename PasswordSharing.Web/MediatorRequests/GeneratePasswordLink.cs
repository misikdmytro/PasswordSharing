using System;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PasswordSharing.Contracts;
using PasswordSharing.Events;
using PasswordSharing.Events.Contracts;
using PasswordSharing.Exceptions;
using PasswordSharing.Models;
using PasswordSharing.Web.Exceptions;
using PasswordSharing.Web.Models;

namespace PasswordSharing.Web.MediatorRequests
{
    public class GeneratePasswordLinkRequest : IRequest<GenerateLinkModel>
    {
        public string[] Passwords { get; }
        public int ExpiresIn { get; }

        public GeneratePasswordLinkRequest(string[] passwords, int expiresIn)
        {
            Passwords = passwords;
            ExpiresIn = expiresIn;

        }
    }

    public class GeneratePasswordLinkHandler : IRequestHandler<GeneratePasswordLinkRequest, GenerateLinkModel>
    {
        private readonly IPasswordEncryptor _passwordEncryptor;
        private readonly IGroupEventHandler<PasswordGroupCreated> _eventHandler;
        private readonly ILogger<GeneratePasswordLinkHandler> _logger;
        private readonly IRsaKeyGenerator _keyGenerator;

        public GeneratePasswordLinkHandler(IPasswordEncryptor passwordEncryptor,
            IGroupEventHandler<PasswordGroupCreated> eventHandler, 
            ILogger<GeneratePasswordLinkHandler> logger, IRsaKeyGenerator keyGenerator)
        {
            _passwordEncryptor = passwordEncryptor;
            _eventHandler = eventHandler;
            _logger = logger;
            _keyGenerator = keyGenerator;
        }

        public async Task<GenerateLinkModel> Handle(GeneratePasswordLinkRequest request, CancellationToken cancellationToken)
        {
            var key = _keyGenerator.GenerateKey();

            var passwordGroup = new PasswordGroup
            {
                ExpiresAt = DateTime.Now.Add(TimeSpan.FromSeconds(request.ExpiresIn)),
                Passwords = request.Passwords.Select(password => Handle(password, key)).ToArray(),
                Status = PasswordStatus.Valid
            };

            await _eventHandler.When(new PasswordGroupCreated(passwordGroup));

            return new GenerateLinkModel
            {
                Key = _keyGenerator.ToString(key),
                PasswordGroupId = passwordGroup.Id
            };
        }

        private Password Handle(string passwordStr, RSAParameters key)
        {
            try
            {
                var password = _passwordEncryptor.Encode(passwordStr, key);
                
                _logger.LogDebug($"Password model - {JsonConvert.SerializeObject(password)}");

                return password;
            }
            catch (BadLengthException)
            {
                const string message = "One of passwords is too long";
                _logger.LogError(message);

                throw new HttpResponseException(HttpStatusCode.BadRequest, message);
            }
        }
    }
}
