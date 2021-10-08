using System;
using System.Collections.Generic;
using System.Text;

namespace VkBotApi.MessagesController
{
    public class Config
    {
        public string DefaultChapter { get; set; } = "default";
        public string DefaultSystemChapter { get; } = "system";
        public string DefaultPermissionDeniedMessage { get; set; }
        public string DefaultNotFoundMessage { get; set; }
    }
}
