using LiteDB;
using System;
using System.Collections.Generic;
using System.Text;

namespace LiteMessage.Stores.LiteDb
{
    public class Mapping
    {
        static Mapping()
        {
            var mapper = BsonMapper.Global;

            mapper.Entity<Message>()

                .Ignore(x => x.ReadStatus);// ignore this property (do not store)


        }
    }
}
