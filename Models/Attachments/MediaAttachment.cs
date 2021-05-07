using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace VkBotApi.Models.Attachments
{
    [Serializable]
    public abstract class MediaAttachment
    {
        protected abstract string Alias { get; }
        [JsonProperty("id")]
        public long? Id { get; set; }
        [JsonProperty("owner_id")]
        public long? OwnerId { get; set; }
        [JsonProperty("access_key")]
        public string AccessKey { get; set; }

        public override string ToString()
        {
            var result = $"{Alias}{OwnerId}_{Id}";

            return string.IsNullOrWhiteSpace(AccessKey)
                ? result
                : $"{result}_{AccessKey}";
        }
    }
}
