using System;
using System.Collections.Generic;
using LiteDB;
namespace LiteMessage.Stores.LiteDb
{
    public class LiteMessageSetting
    {
        public string StorePath { get; set; }
    }
    public class NotifyMessageStore : INotifyMessageStore
    {
        static NotifyMessageStore()
        {
            var mapper = BsonMapper.Global;

            mapper.Entity<Message>()

                .Ignore(x => x.ReadStatus);// ignore this property (do not store)


        }

        private readonly LiteDatabase db;

        public NotifyMessageStore(LiteDatabase setting)
        {
            this.db = setting;
        }
        public void Delete(int id)
        {

            CreateCollection(db);
            // Get customer collection
            var col = db.GetCollection<Message>();
            col.Delete(new BsonValue(id));


        }

        private void CreateCollection(LiteDatabase db)
        {
            //var col = db.CollectionExists(CollectionName);
            //if (!col)
            //{
            //    db.GetCollection<Message>(CollectionName).EnsureIndex(f => f.ModifyTime);
            //}
        }

        public void Add(Message order)
        {

            order.CreateTime = DateTime.Now;
            CreateCollection(db);
            // Get customer collection
            var col = db.GetCollection<Message>();
            var bs = col.Insert(order);
            order.Id = bs.AsInt32;

        }

        public void Update(Message order)
        {

            CreateCollection(db);
            order.ModifyTime = DateTimeOffset.Now;
            // Get customer collection
            var col = db.GetCollection<Message>();
            col.Update(order);
        }


        public IEnumerable<Message> List(string search, int pageIndex, int pageSize)
        {


            // Get customer collection
            var col = db.GetCollection<Message>();
            var query = Query.All("PublishTime", Query.Descending);

            if (!string.IsNullOrEmpty(search))
            {
                query = Query.And(
                    Query.Or(Query.Contains("Subject", search), Query.Contains("Content", search)),
                    query
                    );
            }
            var result = col.Find(query, pageIndex * pageSize, pageSize);
            return result ?? new List<Message>();

        }

        public Message Get(int id)
        {


            // Get customer collection
            var col = db.GetCollection<Message>();
            return col.FindById(new BsonValue(id));

        }

        public int Count(string search = null)
        {


            // Get customer collection
            var col = db.GetCollection<Message>();
            if (!string.IsNullOrEmpty(search))
                return col.Count(f => (f.Content.Contains(search) || f.Subject.Contains(search)));
            return col.Count();

        }


    }
}
