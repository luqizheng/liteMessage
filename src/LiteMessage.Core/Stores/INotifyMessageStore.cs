using System;
using System.Collections.Generic;
using System.Text;

namespace LiteMessage.Stores
{
    public interface INotifyMessageStore
    {
        Message Get(int id);
        void Add(Message message);
        void Update(Message message);

        void Delete(int id);

        IEnumerable<Message> List(string search, int pageIndex, int pageSize);
        int Count(string search);
    }

}
