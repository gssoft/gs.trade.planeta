using System;
using Newtonsoft.Json;

namespace OwinTest_02
{
    public class Token
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
        [JsonProperty("token_type")]
        public string TokenType { get; set; }
        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }
        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }
        [JsonProperty("error")]
        public string Error { get; set; }

        public DateTime ExpireDateTime { get; private set; }

        public void SetExpireDateTime()
        {
            ExpireDateTime = DateTime.Now.AddSeconds(
                ExpiresIn > 5 * 60
                    ? ExpiresIn - 5*60
                    : ExpiresIn);
        }
    }
}
