using System;
using System.Collections.Generic;
using System.Text;
using VkBotApi.Abstraction;
using VkBotApi.Enums;
using Newtonsoft.Json;
using System.Linq;

namespace VkBotApi.Models.Params
{
    /// <summary>
    /// Keyboard instance for bots. 5 × 10 for default keyboard size (Max keys - 40), 5 x 6 with Inline = true (Max keys - 10)
    /// </summary>
    public class KeyboardParams
    {
        /// <summary>
        /// Whether to hide the keyboard after the first use. The parameter is triggered only for buttons that send messages (field "type" - text, location) For "type" equal to open_app, vk_pay parameter is ignored.
        /// </summary>
        public bool OneTime { get; set; } = false;
        /// <summary>
        /// Should the keyboard be displayed inside the message.
        /// </summary>
        public bool Inline { get; set; } = false;
        /// <summary>
        /// Buttons
        /// </summary>
        public List<IButton> Buttons { get; set; }
        /// <summary>
        /// Build json
        /// </summary>
        /// <returns>json payload</returns>
        public string Build()
        {
            string json = "{";
            if (!Inline)
            {
                json += $"\"one_time\":{OneTime.ToString().ToLower()},";
            }
            else
            {
                json += $"\"inline\":{Inline.ToString().ToLower()},";
            }
            json += "\"buttons\":[";
            foreach(var button in Buttons)
            {
                if(button == Buttons.Last())
                {
                    json += button.Build();
                }
                else
                {
                    json += button.Build() + ",";
                }
            }
            json += "]}";
            return json;
        }
    }
    /// <summary>
    /// Text button. Sends a message with the text specified in the label.
    /// </summary>
    public class ButtonText : IButton
    {
        public string Type { get => "\"text\""; }
        /// <summary>
        /// Button text.
        /// </summary>
        public string Label { get; set; }
        /// <summary>
        /// The position of the button on the keyboard. 5 × 10 for default keyboard size (Max keys - 40), 5 x 6 with Inline = true (Max keys - 10)
        /// </summary>
        public int Position { get; set; }
        /// <summary>
        /// Button color.
        /// </summary>
        public ButtonColor Color { get; set; } = ButtonColor.Primary;

        public string Build()
        {
            string json = "[{\"action\":{\"type\":" + Type + $",\"label\":\"{Label}\",\"payload\":\"{{\\\"button\\\":\\\"{Position}\\\"}}\"}},\"color\":\"{Color.ToString().ToLower()}\"";
            json += "}]";
            return json;
        }
    }
    /// <summary>
    /// Callback button. Allows you to receive a notification about a button click without sending a message from the user and perform the necessary action.
    /// </summary>
    public class ButtonCallback : IButton
    {
        public string Type { get => "\"callback\""; }
        /// <summary>
        /// Button text.
        /// </summary>
        public string Label { get; set; }
        /// <summary>
        /// The position of the button on the keyboard. 5 × 10 for default keyboard size (Max keys - 40), 5 x 6 with Inline = true (Max keys - 10)
        /// </summary>
        public int Position { get; set; }
        /// <summary>
        /// Button color.
        /// </summary>
        public ButtonColor Color { get; set; } = ButtonColor.Primary;
        public string Build()
        {
            string json = "[{\"action\":{\"type\":" + Type + $",\"label\":\"{Label}\",\"payload\":\"{{\\\"button\\\":\\\"{Position}\\\"}}\"}},\"color\":\"{Color.ToString().ToLower()}\"";
            json += "}]";
            return json;
        }
    }
    /// <summary>
    /// Link button. Opens the specified link.
    /// </summary>
    public class ButtonOpenLink : IButton
    {
        public string Type { get => "\"open_link\""; }
        /// <summary>
        /// Button text.
        /// </summary>
        public string Label { get; set; }
        /// <summary>
        /// Link.
        /// </summary>
        public string Link { get; set; }
        /// <summary>
        /// The position of the button on the keyboard. 5 × 10 for default keyboard size (Max keys - 40), 5 x 6 with Inline = true (Max keys - 10)
        /// </summary>
        public int Position { get; set; }
        /// <summary>
        /// Button color.
        /// </summary>
        public ButtonColor Color { get; set; } = ButtonColor.Primary;
        public string Build()
        {
            string json = "[{\"action\":{\"type\":" + Type + $",\"label\":\"{Label}\",\"link\":\"{Link}\",\"payload\":\"{{\\\"button\\\":\\\"{Position}\\\"}}\"}},\"color\":\"{Color.ToString().ToLower()}\"";
            json += "}]";
            return json;
        }
    }
    /// <summary>
    /// Location button. By clicking, it sends the location to a dialog with the bot/conversation. This button always occupies the entire width of the keyboard!
    /// </summary>
    public class ButtonLocation : IButton
    {
        public string Type { get => "\"location\""; }
        /// <summary>
        /// The position of the button on the keyboard. 5 × 10 for default keyboard size (Max keys - 40), 5 x 6 with Inline = true (Max keys - 10)
        /// </summary>
        public int Position { get; set; }
        /// <summary>
        /// Button color.
        /// </summary>
        public ButtonColor Color { get; set; }
        public string Build()
        {
            string json = "[{\"action\":{\"type\":" + Type + $",\"payload\":\"{{\\\"button\\\":\\\"{Position}\\\"}}\"}},\"color\":\"{Color.ToString().ToLower()}\"";
            json += "}]";
            return json;
        }
    }
    /// <summary>
    /// VK Pay button. Opens the VKPay payment window with predefined parameters. The button is called "Pay via VK pay”, VK pay is displayed as a logo. This button always occupies the entire width of the keyboard!
    /// </summary>
    public class ButtonVKPay : IButton
    {
        public string Type { get => "\"vkpay\""; }
        /// <summary>
        /// The position of the button on the keyboard. 5 × 10 for default keyboard size (Max keys - 40), 5 x 6 with Inline = true (Max keys - 10)
        /// </summary>
        public int Position { get; set; }
        /// <summary>
        /// A string containing VKPay payment parameters and the application ID in the aid parameter, separated by &.
        /// </summary>
        public string Hash { get; set; }
        /// <summary>
        /// Button color.
        /// </summary>
        public ButtonColor Color { get; set; }
        public string Build()
        {
            string json = "[{\"action\":{\"type\":" + Type + $",\"hash\":\"{Hash}\",\"payload\":\"{{\\\"button\\\":\\\"{Position}\\\"}}\"}},\"color\":\"{Color.ToString().ToLower()}\"";
            json += "}]";
            return json;
        }
    }
    /// <summary>
    /// VK App button. Opens the specified VK App. This button always occupies the entire width of the keyboard!
    /// </summary>
    public class ButtonVKApp : IButton
    {
        public string Type { get => "\"open_app\""; }
        /// <summary>
        /// Button text.
        /// </summary>
        public string Label { get; set; }
        /// <summary>
        /// Id of the called application with the VK Apps type.
        /// </summary>
        public long AppId { get; set; }
        /// <summary>
        /// Id of the community where the application is installed, if you want to open it in the context of the community.
        /// </summary>
        public long OwnerId { get; set; }
        /// <summary>
        /// The position of the button on the keyboard. 5 × 10 for default keyboard size (Max keys - 40), 5 x 6 with Inline = true (Max keys - 10)
        /// </summary>
        public int Position { get; set; }
        /// <summary>
        /// The hash for navigation in the application will be passed in the launch parameters string after the character #.
        /// </summary>
        public string Hash { get; set; }
        /// <summary>
        /// Button color.
        /// </summary>
        public ButtonColor Color { get; set; }
        public string Build()
        {
            string json = "[{\"action\":{\"type\":" + Type + $",\"app_id\":{AppId},\"owner_id\":{OwnerId},\"label\":\"{Label}\",\"hash\":\"{Hash}\",\"payload\":\"{{\\\"button\\\":\\\"{Position}\\\"}}\"}},\"color\":\"{Color.ToString().ToLower()}\"";
            json += "}]";
            return json;
        }
    }
}
