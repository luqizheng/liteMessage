using System.Linq;
using System.Collections.Generic;
using LiteDB;
namespace LiteMessage.Stores.LiteDb
{
    public class ReadStatusStore : IReadStatusStore
    {
        private readonly LiteDatabase db;

        public ReadStatusStore(LiteDatabase db)
        {
            this.db = db;
        }

        public int Count(string userId, bool read)
        {
            var col = db.GetCollection<ReadStatus>();
            var queryUserId = Query.EQ("UserId", new BsonValue(userId));
            var contain = Query.EQ("Read", new BsonValue(read));
            return col.Count(queryUserId);
        }

        public IEnumerable<ReadStatus> GetReadStatus(string userId, params int[] messageId)
        {


            // Get customer collection
            var col = db.GetCollection<ReadStatus>();
            var queryUserId = Query.EQ("UserId", new BsonValue(userId));

            var queryMessageId = Query.In("MessageId", messageId.Select(f => new BsonValue(f)));
            var bs = col.Find(Query.And(queryUserId, queryMessageId));
            return bs;


        }

        public void Save(ReadStatus status)
        {


            // Get customer collection
            var col = db.GetCollection<ReadStatus>();

            var bs = col.FindOne(f => f.MessageId == status.MessageId && f.UserId == status.UserId);
            if (bs != null)
            {
                status.Id = bs.Id;
                col.Update(status);
            }
            else
            {
                col.Insert(status);
            }



        }
    }
}
