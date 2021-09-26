using Akka.Actor;

namespace MySystem
{
    class System : MyUtils.Singleton<System>
    {
        public ActorSystem ActorSystem;

        ~System()
        {
            SystemShutdown();
        }

        public bool SystemStart()
        {
            ActorSystem = ActorSystem.Create("MyActorSystem");

            return true;
        }

        public void SystemShutdown()
        {
            ActorSystem.Terminate();
        }
    }

}
