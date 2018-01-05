﻿using Newtonsoft.Json;
using Paydock_dotnet_sdk.Models;
using Paydock_dotnet_sdk.Tools;
using System;

namespace Paydock_dotnet_sdk.Services
{
    public class Tokens : ITokens
	{
        protected IServiceHelper _serviceHelper;
        protected string _overrideConfigPublicKey = null;

        /// <summary>
        /// Service locator style constructor
        /// </summary>
        public Tokens(string overrideConfigPublicKey = null)
        {
            _serviceHelper = new ServiceHelper();
            _overrideConfigPublicKey = overrideConfigPublicKey;
        }

        /// <summary>
        /// Dependency injection constructor to enable testing
        /// <param name="serviceHelper">Service helper class to perform HTTP requests</param>
        /// <param name="overrideConfigSecretKey">Use a custom public key rather than the value in shared config, defaults to null</param>
        /// </summary>
        public Tokens(IServiceHelper serviceHelper, string overrideConfigPublicKey = null)
        {
            _serviceHelper = serviceHelper;
            _overrideConfigPublicKey = overrideConfigPublicKey;
        }

        /// <summary>
        /// Creates a one-time token on Paydock
        /// </summary>
        /// <param name="request">payment data to create the token</param>
        /// <returns>One-Time token</returns>
        [RequiresConfig]
        public TokenResponse Create(TokenRequest request)
        {
            var requestData = SerializeHelper.Serialize(request);

			var publicKey = Config.PublicKey;
			if (!string.IsNullOrEmpty(_overrideConfigPublicKey))
				publicKey = _overrideConfigPublicKey;

			var responseJson = _serviceHelper.CallPaydock("payment_sources/tokens?public_key=" + Uri.EscapeUriString(publicKey), HttpMethod.POST, requestData, excludeSecretKey: true);

            var response = SerializeHelper.Deserialize<TokenResponse>(responseJson);
            response.JsonResponse = responseJson;
            return response;

        }
    }
}
