using System;

using Akka.Actor;
using Akka.IO;

using MyMessage;
using RedisChattingServer;

namespace pingpongserver.Server
{
    public class PingPongConnection : UntypedActor
    {
        // readonly 는 const 대신 사용하는 용도
        // 최초 생성시 set 후 수정방지하려면 아래와 같이 사용 
        public IActorRef Connection { get; init; }
        public readonly Redis RedisConnection;

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

            /*/
            // 좀 더 간결
            if(ReceivedMessage is Tcp.Received msg)
            {
                
            }

            /*/
            //  원래코드 
            if (ReceivedMessage != null)
            {
                var RecvPacket = PacketGenerator.GetInst.MakePacket(ReceivedMessage.Data.ToString());
                pakcethandler?.Invoke(RecvPacket);
            }
            //*/
            
            // 클라이언트에서 연결 종료
            else
            {
                this.Self.GracefulStop(TimeSpan.FromMilliseconds(10));
                //Context.Stop(Self);
            }
            //*/
        }

        protected override void Unhandled(object message)
        {
            // logging?
            Console.WriteLine("UnHandled message received {0}", message.ToString());
        }

        // 이런 느낌의 함수는
        private void PacketHandle(Message.MessageBase recvPacket)
        {
            recvPacket.PacketHandle(this);
        }

        // C# 델리게이트 검색해보기 
        Action<Message.MessageBase> pakcethandler;

        public void SendPacket(Message.MessageBase packet)
        {
            /*/
            Connection.Tell(Tcp.Write.Create(ByteString.FromBytes(PacketGenerator.GetInst.ClassToBytes(packet))));
            //*/
            Connection.Tell("TestString");
            //*/
        }

    }
}
