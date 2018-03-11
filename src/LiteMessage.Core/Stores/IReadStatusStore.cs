using System.Collections.Generic;

namespace LiteMessage.Stores
{
    public interface IReadStatusStore
    {
        IEnumerable<ReadStatus> GetReadStatus(string userId, params int[] messageId);
    }

}
