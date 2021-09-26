using System;
using System.Net;

using Akka.Actor;
using Akka.IO;

namespace pingpongserver.Server
{
    class PingPongServer : UntypedActor
    {
        public PingPongServer(int port)
        {
            Context.System.Tcp().Tell(new Tcp.Bind(Self, new IPEndPoint(IPAddress.Any, port)));
        }

        protected override void OnReceive(object message)
        {
            if(message is Tcp.Bound)
            {
                Akka.IO.Tcp.Bound Bound = message as Tcp.Bound;
                Console.WriteLine("Listening on {0}", Bound.LocalAddress);
            }
            else if(message is Tcp.Connected)
            {
                var connection = Context.ActorOf(Props.Create(() =>
                    new PingPongConnection(Sender)));
                Sender.Tell(new Tcp.Register(connection));
            }
            else
            {
                Unhandled(message);
            }
        }
    }
}
