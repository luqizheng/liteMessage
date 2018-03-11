using System.Linq;
using System.Collections.Generic;
using LiteDB;
namespace LiteMessage.Stores.LiteDb
{
    public class ReadStatusStore : IReadStatusStore
    {
        private readonly LiteMessageSetting setting;

        public ReadStatusStore(LiteMessageSetting setting)
        {
            this.setting = setting;
        }
        public IEnumerable<ReadStatus> GetReadStatus(string userId, params int[] messageId)
        {
            using (var db = new LiteDatabase(setting.StorePath))
            {

                // Get customer collection
                var col = db.GetCollection<ReadStatus>();
                var queryUserId = Query.EQ("UserId", new BsonValue(userId));

                var queryMessageId = Query.In("MessgeId", messageId.Select(f => new BsonValue(f)));
                var bs = col.Find(Query.And(queryUserId, queryMessageId));
                return bs;

            }
        }
    }
}
