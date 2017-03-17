﻿using System;
using Paydock_dotnet_sdk.Models;
using Newtonsoft.Json;

namespace Paydock_dotnet_sdk.Services
{
    /// <summary>
    /// Provides abstraction over the /charges endpoint for the APU
    /// </summary>
    public class Charges : ICharges
    {
        protected IServiceHelper _serviceHelper;
        
        /// <summary>
        /// Service locator style constructor
        /// </summary>
        public Charges()
        {
            _serviceHelper = new ServiceHelper();
        }

        /// <summary>
        /// Dependency injection constructor to enable testing
        /// </summary>
        public Charges(IServiceHelper serviceHelper)
        {
            _serviceHelper = serviceHelper;
        }

        /// <summary>
        /// Add a charge
        /// </summary>
        /// <param name="request">Charge data</param>
        /// <returns>Charge response</returns>
        [RequiresConfig]
        public ChargeResponse Add(ChargeRequest request)
        {
            var requestData = JsonConvert.SerializeObject(request, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            var responseJson = _serviceHelper.CallPaydock("charges", HttpMethod.POST, requestData);

            var response = (ChargeResponse) JsonConvert.DeserializeObject(responseJson, typeof(ChargeResponse));
            response.JsonResponse = responseJson;
            return response;
        }

        /// <summary>
        /// Retrieve full list of charges, limited to 1000
        /// </summary>
        /// <returns>List of charges</returns>
        [RequiresConfig]
        public ChargeItemsResponse Get()
        {
            var responseJson = _serviceHelper.CallPaydock("charges", HttpMethod.GET, "");

            var response = (ChargeItemsResponse)JsonConvert.DeserializeObject(responseJson, typeof(ChargeItemsResponse));
            response.JsonResponse = responseJson;
            return response;
        }

        /// <summary>
        /// Retrieve a filtered list of charges, limited to 1000
        /// </summary>
        /// <param name="request">filter parameters</param>
        /// <returns>List of charges</returns>
        [RequiresConfig]
        public ChargeItemsResponse Get(GetChargeRequest request)
        {
            var url = "charges";

            if (request.skip.HasValue)
                url = addUrlParameter(url, "skip", request.skip.Value.ToString());

            if (request.limit.HasValue)
                url = addUrlParameter(url, "limit", request.limit.Value.ToString());

            if (request.subscription_id != null)
                url = addUrlParameter(url, "subscription_id", request.subscription_id);

            if (request.gateway_id != null)
                url = addUrlParameter(url, "gateway_id", request.gateway_id);

            if (request.company_id != null)
                url = addUrlParameter(url, "company_id", request.company_id);

            if (request.created_at_from.HasValue)
                url = addUrlParameter(url, "created_at.from", request.created_at_from.Value.ToString());

            if (request.created_at_to.HasValue)
                url = addUrlParameter(url, "created_at.to", request.created_at_to.Value.ToString());

            if (request.search != null)
                url = addUrlParameter(url, "search", request.search);

            if (request.status != null)
                url = addUrlParameter(url, "status", request.status);

            if (request.archived.HasValue)
                url = addUrlParameter(url, "archived", request.archived.Value.ToString());

            var response = _serviceHelper.CallPaydock("charges", HttpMethod.GET, "");

            return (ChargeItemsResponse)JsonConvert.DeserializeObject(response, typeof(ChargeItemsResponse));
        }

        private string addUrlParameter(string currrentUrl, string name, string value)
        {
            if (!currrentUrl.Contains("?"))
                currrentUrl += "?";
            else
                currrentUrl += "&";

            return Uri.EscapeUriString(name) + "=" + Uri.EscapeUriString(value);
        }

        /// <summary>
        /// Retrive a single charge
        /// </summary>
        /// <param name="chargeId">id of the charge to retreive</param>
        /// <returns>charge data</returns>
        [RequiresConfig]
        public ChargeItemResponse Get(string chargeId)
        {
            chargeId = Uri.EscapeUriString(chargeId);
            var responseJson = _serviceHelper.CallPaydock("charges/" + chargeId, HttpMethod.GET, "");

            var response = (ChargeItemResponse)JsonConvert.DeserializeObject(responseJson, typeof(ChargeItemResponse));
            response.JsonResponse = responseJson;
            return response;
        }

        /// <summary>t
        /// Refund a transaction
        /// </summary>
        /// <param name="chargeId">id of the charge to refund</param>
        /// <param name="amount">amount to refund, can be used to issue partial refunds</param>
        /// <returns>information on the transaction</returns>
        [RequiresConfig]
        public RefundResponse Refund(string chargeId, decimal amount)
        {
            chargeId = Uri.EscapeUriString(chargeId);
            var json = string.Format("{{\"amount\" : \"{0}\"}}", amount);
            var responseJson = _serviceHelper.CallPaydock(string.Format("charges/{0}/refunds", chargeId), HttpMethod.POST, json);

            var response = (RefundResponse)JsonConvert.DeserializeObject(responseJson, typeof(RefundResponse));
            response.JsonResponse = responseJson;
            return response;
        }

        /// <summary>
        /// Archive a transaction
        /// </summary>
        /// <param name="chargeId">id of the charge to archive</param>
        /// <returns>information on the transaction</returns>
        [RequiresConfig]
        public RefundResponse Archive(string chargeId)
        {
            chargeId = Uri.EscapeUriString(chargeId);
            var responseJson = _serviceHelper.CallPaydock(string.Format("charges/{0}", chargeId), HttpMethod.DELETE, "");

            var response = (RefundResponse)JsonConvert.DeserializeObject(responseJson, typeof(RefundResponse));
            response.JsonResponse = responseJson;
            return response;
        }
    }
}
