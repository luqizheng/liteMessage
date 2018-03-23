using System;

namespace LiteMessage
{
    public class Message
    {
        public Message()
        {

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
        /// <summary>
        /// 发布时间
        /// </summary>
        public DateTimeOffset PublishTime { get; set; }
        /// <summary>
        /// 读取状态
        /// </summary>
        public ReadStatus ReadStatus { get; set; }



    }


}
