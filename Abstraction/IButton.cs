using System;
using System.Collections.Generic;
using System.Text;
using VkBotApi.Enums;

namespace VkBotApi.Abstraction
{
    public interface IButton
    {
        string Type { get; }
        int Position { get; set; }
        ButtonColor Color { get; set; }
        string Build();
    }
}
