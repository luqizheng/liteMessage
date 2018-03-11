using System;

namespace LiteMessage
{
    public class Message
    {
        public Message()
        {
            CreateTime = DateTime.Now;
            ModifyTime = DateTime.Now;
        }
        /// <summary>
        /// 
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Subject { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTimeOffset CreateTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTimeOffset ModifyTime { get; set; }


    }


}
