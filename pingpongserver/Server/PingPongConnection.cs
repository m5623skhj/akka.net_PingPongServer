using System;

using Akka.Actor;
using Akka.IO;

using MyMessage;
using RedisChattingServer;

namespace pingpongserver.Server
{
    public class PingPongConnection : UntypedActor
    {
        public IActorRef Connection { get; init; }
        public Redis RedisConnection { get; init; }

        Action<Message.MessageBase> pakcethandler;

        public PingPongConnection(IActorRef connection)
        {
            Connection = connection;
            RedisConnection = new Redis(this);
            pakcethandler += PacketHandle;
        }

        protected override void PreStart()
        {
            base.PreStart();
        }

        protected override void PostStop()
        {
            RedisConnection.Dispose();

            base.PostStop();
        }

        protected override void OnReceive(object message)
        {
            var ReceivedMessage = message as Tcp.Received;

            if(ReceivedMessage is Tcp.Received msg)
            {
                var RecvPacket = PacketGenerator.GetInst.MakePacket(ReceivedMessage.Data.ToString());
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

        private void PacketHandle(Message.MessageBase recvPacket)
        {
            recvPacket.PacketHandle(this);
        }

        public void SendPacket(Message.MessageBase packet)
        {
            Connection.Tell(Tcp.Write.Create(ByteString.FromBytes(PacketGenerator.GetInst.ClassToBytes(packet))));
        }
    }
}
