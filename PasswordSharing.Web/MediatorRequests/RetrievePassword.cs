using System;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PasswordSharing.Contexts;
using PasswordSharing.Contracts;
using PasswordSharing.Events;
using PasswordSharing.Events.Contracts;
using PasswordSharing.Models;
using PasswordSharing.Repositories;
using PasswordSharing.Web.Exceptions;

namespace PasswordSharing.Web.MediatorRequests
{
    public class RetrievePasswordRequest : IRequest<string[]>
    {
        public int PasswordGroupId { get; }
        public string Key { get; }

        public RetrievePasswordRequest(string key, int passwordGroupId)
        {
            Key = key;
            PasswordGroupId = passwordGroupId;
        }
    }

    public class RetrievePasswordHandler : IRequestHandler<RetrievePasswordRequest, string[]>
    {
        private readonly IPasswordEncryptor _passwordEncryptor;
        private readonly IContextFactory<ApplicationContext> _factory;
        private readonly IGroupEventHandler<PasswordGroupStatusChanged> _eventHandler;
        private readonly ILogger<RetrievePasswordHandler> _logger;
        private readonly IRsaKeyGenerator _keyGenerator;

        public RetrievePasswordHandler(IPasswordEncryptor passwordEncryptor,
            IGroupEventHandler<PasswordGroupStatusChanged> eventHandler,
            ILogger<RetrievePasswordHandler> logger, IContextFactory<ApplicationContext> factory,
            IRsaKeyGenerator keyGenerator)
        {
            _passwordEncryptor = passwordEncryptor;
            _eventHandler = eventHandler;
            _logger = logger;
            _factory = factory;
            _keyGenerator = keyGenerator;
        }

        public async Task<string[]> Handle(RetrievePasswordRequest request, CancellationToken cancellationToken)
        {
            using (var context = _factory.CreateContext())
            {
                var passwordGroupRepository = new PasswordGroupRepository(context);
                var passwordGroup = await passwordGroupRepository
                    .SingleOrDefaultAsync(x => x.Id == request.PasswordGroupId);

                if (passwordGroup == null)
                {
                    const string message = "Link not exists";
                    _logger.LogError(message);

                    throw new HttpResponseException(HttpStatusCode.BadRequest, message);
                }

                if (passwordGroup.Status != PasswordStatus.Valid)
                {
                    var message = PrepareErrorMessage(passwordGroup);
                    _logger.LogError(message);

                    throw new HttpResponseException(HttpStatusCode.BadRequest, message);
                }

                if (passwordGroup.ExpiresAt < DateTime.Now)
                {
                    var model = new PasswordGroupStatusChanged(passwordGroup, PasswordStatus.Expired);
                    await _eventHandler.When(model);

                    var message = PrepareErrorMessage(passwordGroup);
                    _logger.LogError(message);

                    throw new HttpResponseException(HttpStatusCode.BadRequest, message);
                }

                var key = _keyGenerator.FromString(request.Key);

                var result = passwordGroup.Passwords
                    .Select(password => Handle(password, key))
                    .ToArray();

                try
                {
                    await _eventHandler.When(new PasswordGroupStatusChanged(passwordGroup, PasswordStatus.Used));
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    _logger.LogError(ex.Message);

                    // repeat again to return correct reason
                    return await Handle(request, cancellationToken);
                }

                return result;
            }
        }

        private string Handle(Password password, RSAParameters key)
        {
            _logger.LogDebug($"Password model - {JsonConvert.SerializeObject(password)}");

            try
            {
                _logger.LogInformation($"Trying to retrieve password with ID {password.Id}");

                var result = _passwordEncryptor.Decode(password, key);

                return result;
            }
            catch (Exception)
            {
                const string message = "Incorrect link";
                _logger.LogError(message);

                throw new HttpResponseException(HttpStatusCode.BadRequest, message);
            }
        }

        private string PrepareErrorMessage(PasswordGroup @group)
        {
            switch (@group.Status)
            {
                case PasswordStatus.Valid:
                    throw new ArgumentException("Password is valid");
                case PasswordStatus.Expired:
                    return $"Link expired at {@group.ExpiresAt}";
                case PasswordStatus.Used:
                    return "Link has been already used";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
