using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LiteMessage.Stores;
using Microsoft.AspNetCore.Mvc;

namespace LiteMessage.Web.Controllers
{
    [Route("api/[controller]")]
    public class MessageController : Controller
    {
        private readonly INotifyMessageStore store;

        public MessageController(INotifyMessageStore store)
        {
            this.store = store;
        }
        // GET api/values
        [HttpGet("list"), SignResponse]
        public IEnumerable<Message> Get([FromQuery] string search, [FromQuery]int page, [FromQuery] int pageSize)
        {
            var f = store.List(search, page, pageSize);

            return f;
        }

        [HttpGet("count"), SignResponse]
        public int Count(string search)
        {
            return store.Count(search);
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
}
