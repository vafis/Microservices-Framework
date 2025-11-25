using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Eid.Microservices.Core
{
    [Serializable]
    public class AccessTokenResponse
    {
        [JsonProperty(PropertyName = "access_token")]
        public string AccessToken { get; set; }
        [JsonProperty(PropertyName = "token_type")]
        public string TokenType { get; set; }
        [JsonProperty(PropertyName = "expires_in")]
        public int ExpiresIn { get; set; }
        [JsonProperty(PropertyName = "refresh_token")]
        public string RefreshToken { get; set; }
        public string IdToken { get; set; }
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        public string GrantType { get; set; }
    }
}
