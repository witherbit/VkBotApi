using System;
using System.Collections.Generic;
using System.Text;
using VkBotApi.Utils;

namespace VkBotApi.Models.Messages
{
    public class Message
    {
        public List<long> PeerIds = new List<long>();

        public void AddPeerId(long peerId)
        {
            try
            {
                PeerIds.Add(peerId);
            }
            catch
            {

            }
        }

        public string Text { get; set; }
        public int RandomId { get { return DateTime.Now.GetTimeStampInt(); } }

        /// <summary>
        /// from -90 to 90
        /// </summary>
        public float? Lat { get; set; }
        /// <summary>
        /// from -180 to 180
        /// </summary>
        public float? Long { get; set; }

        public long? ReplyTo { get; set; }

        public int? StickerId { get; set; }
    }
}
