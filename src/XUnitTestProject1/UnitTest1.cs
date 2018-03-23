using LiteMessage.Client;
using System;
using Xunit;

namespace XUnitTestProject1
{
    public class UnitTest1
    {
        public string Url = "http://localhost:53587";
        public string AppKey = "{36F2F0D5-C5B8-4138-91DD-654BE99021B6}";

        [Fact]
        public void Test()
        {
            LiteMessageService service = new LiteMessageService(AppKey, Url);
        }
        [Fact]
        public void Test1()
        {
            var message = new LiteMessage.Message()
            {

                Content = "content",
                Subject = "subject",
                ReadStatus=new LiteMessage.ReadStatus()
                {
                    IsDeleted=false,
                    MessageId=1,
                    UserId="kjkj",
                }
            };


            LiteMessageService service = new LiteMessageService(AppKey, Url);
            service.Add(message);

            //message.Content = "contentn1";
            //message.Subject = "subject1";

            //service.Update(message);

            //var msg = service.Get(message.Id);

            //Assert.Equal(msg.Content, message.Content);
            //Assert.Equal(msg.Subject, message.Subject);
            //Assert.Equal(msg.Id, message.Id);

            var messages = service.List(0, 100);
        }
    }
}
