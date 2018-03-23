namespace LiteMessage
{
    public class ReadStatus
    {
        public int Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int MessageId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool IsDeleted { get; set; }
        /// <summary>
        /// 是否已经°
        /// </summary>
        public bool Read { get; set; }

        public override int GetHashCode()
        {
            return (MessageId.ToString() + UserId).GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var t = base.Equals(obj);
            if (t)
            {
                return true;
            }
            var compare = obj as ReadStatus;
            if (compare == null)
                return false;
            return Read == compare.Read && IsDeleted == compare.IsDeleted && MessageId == compare.MessageId && UserId == compare.UserId;
        }
    }
}
