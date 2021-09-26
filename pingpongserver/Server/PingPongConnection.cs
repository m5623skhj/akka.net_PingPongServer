using System;

using Akka.Actor;
using Akka.IO;

using MyMessage;

namespace pingpongserver.Server
{
    public class PingPongConnection : UntypedActor
    {
        private readonly IActorRef Connection;

        public PingPongConnection(IActorRef connection)
        {
            Connection = connection;
        }

        protected override void OnReceive(object message)
        {
            var ReceivedMessage = message as Tcp.Received;
            if (ReceivedMessage != null)
            {
                var RecvPacket = PacketGenerator.GetInst.MakePacket(ReceivedMessage.Data.ToString());
                PacketHandle(RecvPacket);
            }
            else
            {
                Unhandled(message);
            }
        }

        protected override void Unhandled(object message)
        {
            // logging?
            Console.WriteLine("UnHandled message received {0}", message.ToString());
        }

        private void PacketHandle(Message.MessageBase RecvPacket)
        {
            RecvPacket.PacketHandle(Connection);
        }
    }
}
