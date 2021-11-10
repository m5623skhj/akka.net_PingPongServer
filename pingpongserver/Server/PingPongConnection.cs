using System;

using Akka.Actor;
using Akka.IO;

using MyMessage;
using RedisConnectionChattingServer;

namespace pingpongserver.Server
{
    public class PingPongConnection : UntypedActor, IDisposable
    {
        public IActorRef connection { get; init; }
        public RedisConnection redisConnection { get; init; }

        Action<IMessage> pakcethandler;
        private bool isDisposed = false;

        ~PingPongConnection() => Dispose(false);

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
                redisConnection.Dispose();
            }

            isDisposed = true;
        }

        public PingPongConnection(IActorRef connection)
        {
            this.connection = connection;
            redisConnection = new RedisConnection(this);
            pakcethandler += PacketHandle;
        }

        protected override void PreStart()
        {
            base.PreStart();
        }

        protected override void PostStop()
        {
            Dispose();
            base.PostStop();
        }

        protected override void OnReceive(object message)
        {
            var ReceivedMessage = message as Tcp.Received;

            if(ReceivedMessage is Tcp.Received msg)
            {
                var RecvPacket = PacketGenerator.GetInst.CreateMessage(ReceivedMessage.Data.ToString());
                if(RecvPacket == null)
                {
                    this.Self.GracefulStop(TimeSpan.FromMilliseconds(10));
                    return;
                }
                pakcethandler?.Invoke(RecvPacket);
            }
            // 클라이언트에서 연결 종료
            else
            {
                this.Self.GracefulStop(TimeSpan.FromMilliseconds(10));
            }
        }

        protected override void Unhandled(object message)
        {
            // logging?
            Console.WriteLine("UnHandled message received {0}", message.ToString());
        }

        private void PacketHandle(IMessage recvPacket)
        {
            recvPacket.PacketHandle(this);
        }

        public void SendPacket(IMessage packet)
        {
            connection.Tell(Tcp.Write.Create(ByteString.FromBytes(PacketGenerator.GetInst.ClassToBytes(packet))));
        }
    }
}
