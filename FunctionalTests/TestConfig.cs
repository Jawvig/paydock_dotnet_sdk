﻿using Paydock_dotnet_sdk.Services;

namespace FunctionalTests
{
    public static class TestConfig
    {
        public const string SecretKey = "";
        public const string GatewayId = "";
        public const string PublicKey = "";

        public static void Init()
        {
            Config.Initialise(Environment.Sandbox, SecretKey, PublicKey);
        }
    }
}
