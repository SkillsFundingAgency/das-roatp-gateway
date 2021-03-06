﻿using Newtonsoft.Json;

namespace SFA.DAS.RoatpGateway.Web.Settings
{
    public class ClientApiAuthentication : IClientApiAuthentication
    {
        [JsonRequired]
        public string TenantId { get; set; }

        [JsonRequired]
        public string ClientId { get; set; }

        [JsonRequired]
        public string ClientSecret { get; set; }

        [JsonRequired]
        public string ResourceId { get; set; }

        [JsonRequired]
        public string ApiBaseAddress { get; set; }
    }
}
