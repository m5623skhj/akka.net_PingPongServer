using System;

using Akka.Actor;
using pingpongserver.Server;


/// <summary>
/// 참고 자료
/// https://getakka.net/articles/networking/io.html
/// https://blog.rajephon.dev/2018/12/08/akka-02/
/// </summary>

namespace pingpongserver
{
    class Program
    {
        static void Main(string[] args)
        {
            if(MySystem.System.GetInst.SystemStart() == false)
            {
                Console.WriteLine("System Start Error");
                return;
            }

            ActorSystem MyActorSystem = MySystem.System.GetInst.ActorSystem;

            IActorRef MyPingPongServer = MyActorSystem.ActorOf(Props.Create(() =>
                new PingPongServer(63325)), "MyPingPongServer");

            MyActorSystem.WhenTerminated.Wait();
        }
    }
}
