using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiteMessage.Dto;
using LiteMessage.Stores;
using Microsoft.AspNetCore.Mvc;

namespace LiteMessage.Web.Controllers
{
    [Route("api/[controller]")]
    public class MessageController : Controller
    {
        private readonly INotifyMessageStore store;
        private readonly IReadStatusStore readStatusStore;

        public MessageController(INotifyMessageStore store, IReadStatusStore readStatusStore)
        {
            this.store = store;
            this.readStatusStore = readStatusStore;
        }
        // GET api/values
        [HttpGet("list"), SignResponse]
        public IEnumerable<Message> Get([FromQuery] string search, [FromQuery]int page, [FromQuery] int pageSize, [FromQuery] string userId)
        {
            var messages = store.List(search, page, pageSize).ToList();

            var ary = userId != null ? readStatusStore.GetReadStatus(userId, messages.Select(s => s.Id).ToArray())
                .Distinct(new Self()).ToDictionary(status => status.MessageId, status => status)
                : new Dictionary<int, ReadStatus>()
                ;
            foreach (var message in messages)
            {
                if (ary.ContainsKey(message.Id))
                {
                    message.ReadStatus = ary[message.Id];
                }
                else
                {
                    message.ReadStatus = new ReadStatus()
                    {
                        MessageId = message.Id,
                        UserId = userId,
                        Read = false,
                    };
                }
            }


            return messages;
        }

        [HttpGet("count"), SignResponse]
        public int Count(string search)
        {
            return store.Count(search);
        }
        [HttpGet("Count/{userId}/unread")]
        public int CountNew(string userid)
        {
            var messageAll = store.Count();
            return messageAll = readStatusStore.Count(userid, true);
        }
        [HttpPut("Mark/{id}")]
        public void Mark(int id, [FromBody]MarkReadDto dto)
        {
            var r = store.Get(id);

            readStatusStore.Save(new ReadStatus
            {
                MessageId = id,
                UserId = dto.User,
                Read = dto.Read
            });

        }

        // GET api/values/5
        [HttpGet("{id}"), SignResponse]
        public Message Get(int id)
        {
            return store.Get(id);
        }

        // POST api/values
        [HttpPost, SignResponse]
        public int Post([FromBody]Message value)
        {
            store.Add(value);
            return value.Id;
        }

        // PUT api/values/5
        [HttpPut("{id}"), SignResponse]
        public void Put(int id, [FromBody]Message value)
        {
            value.Id = id;
            store.Update(value);
        }

        // DELETE api/values/5
        [HttpDelete("{id}"), SignResponse]
        public void Delete(int id)
        {
            store.Delete(id);
        }
        [HttpGet("work")]
        public string Work()
        {
            return "Work";
        }
    }

    public class Self : EqualityComparer<ReadStatus>
    {
        public override bool Equals(ReadStatus x, ReadStatus y)
        {
            return x.MessageId == y.MessageId && x.UserId == x.UserId;
        }

        public override int GetHashCode(ReadStatus obj)
        {
            return obj.GetHashCode();
        }
    }


}
