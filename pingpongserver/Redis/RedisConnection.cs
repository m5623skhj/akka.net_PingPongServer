using System;
using System.Collections.Generic;

using StackExchange.Redis;
using pingpongserver.Server;
using MyMessage.Chatting2User;

// https://infodbbase.tistory.com/135
namespace RedisConnectionChattingServer
{
    public class RedisConnection : IDisposable
    {
        private ConnectionMultiplexer redisConnection;
        private ISubscriber subscriber;

        private IDatabase DB;    
        public PingPongConnection userConnection { get; init; }

        private HashSet<string> subscribeChannelSet = new HashSet<string>();

        private bool isDisposed = false;

        public RedisConnection(PingPongConnection Connection)
        {
            userConnection = Connection;          

            Init();
        }

        ~RedisConnection() => Dispose(false);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (isDisposed)
            {
                return;
            }

            if (disposing)
            {
                ChannelUnsubscribeAll();
            }

            isDisposed = true;
        }

        bool Init(/*string host, int port*/)
        {
            subscribeChannelSet.Clear();

            try
            {
                //RedisConnectionConnection = ConnectionMultiplexer.Connect(host + ":" + port);
                redisConnection = ConnectionMultiplexer.Connect("127.0.0.1:6379");
                if(redisConnection.IsConnected)
                {
                    DB = redisConnection.GetDatabase();
                    return true;
                }

                return false;
            }
            catch(Exception e)
            {
                return false;
            }
        }

        public bool ChannelSubscribe(string channel)
        {
            subscribeChannelSet.Add(channel);

            subscriber = redisConnection.GetSubscriber();
            subscriber.SubscribeAsync(channel, (RedisChannel channel, RedisValue value) =>
            {
                if(value.IsNull == true)
                {
                    // log
                    return;
                }

                RecvChatting Packet = new RecvChatting();
                Packet.ChannelName = channel;
                Packet.ChatMessage = value;

                userConnection.SendPacket(Packet);
            });

            return true;
        }

        public void ChannelUnsubscribe(string channel)
        {
            if (subscribeChannelSet.Contains(channel) == true)
            {
                subscriber.UnsubscribeAsync(channel);
            }
        }

        public void ChannelUnsubscribeAll()
        {
            if (subscribeChannelSet.Count > 0)
            {
                subscriber.UnsubscribeAllAsync();
            }
        }

        public void ChannelPublish(string channel, string message)
        {
            if (string.IsNullOrEmpty(channel) || string.IsNullOrEmpty(message))
            {
                return;
            }

            subscriber.PublishAsync(channel, message);            
        }
    }
}
