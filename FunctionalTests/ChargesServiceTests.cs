﻿using NUnit.Framework;
using Paydock_dotnet_sdk.Services;
using Paydock_dotnet_sdk.Models;
using System;
using System.Linq;

namespace FunctionalTests
{
    [TestFixture]
    public class ChargesServiceTests
    {
        string secretKey = "";
        string gatewayId = "";

        [Test]
        public void SimpleCharge()
        {
            Config.Initialise(Paydock_dotnet_sdk.Services.Environment.Sandbox, secretKey);

            var charge = new ChargeRequest
            {
                amount = 10,
                currency = "AUD",
                customer = new Customer
                {
                    payment_source = new PaymentSource
                    {
                        gateway_id = gatewayId,
                        card_name = "Test Name",
                        card_number = "4111111111111111",
                        card_ccv = "123",
                        expire_month = "10",
                        expire_year = "2020"
                    }
                }
            };

            var result = new Charges().Add(charge);

            Assert.IsTrue(result.IsSuccess);
        }


        [Test]
        public void GetCharges()
        {
            Config.Initialise(Paydock_dotnet_sdk.Services.Environment.Sandbox, secretKey);

            var result = new Charges().Get();
            Assert.IsTrue(result.IsSuccess);
        }

        [Test]
        public void GetChargesWithSearch()
        {
            Config.Initialise(Paydock_dotnet_sdk.Services.Environment.Sandbox, secretKey);

            var result = new Charges().Get(new GetChargeRequest { gateway_id = gatewayId });
            Assert.IsTrue(result.IsSuccess);
        }

        [Test]
        public void GetSingleCharge()
        {
            Config.Initialise(Paydock_dotnet_sdk.Services.Environment.Sandbox, secretKey);

            var chargeList = new Charges().Get();
            var chargeId = chargeList.resource.data.First()._id;
            var result = new Charges().Get(chargeId);
            Assert.IsTrue(result.IsSuccess);
        }
    }
}
