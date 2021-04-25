using System;

namespace SimpleECS
{
    struct Position
    {
        public float X, Y;
    }

    struct Velocity
    {
        public float X, Y;
    }

    struct Fart
    {
        public int Power;
    }

    struct SystemTimer : IDisposable
    {
        string name;
        System.Diagnostics.Stopwatch watch;

        public SystemTimer(string name)
        {
            this.name = name;
            watch = new System.Diagnostics.Stopwatch();
            watch.Start();
        }

        void IDisposable.Dispose() => Console.WriteLine($"{name}:\t\t{watch.ElapsedTicks}");
    }

    class Program
    {
        static void Main(string[] args)
        {
            var registry = new Registry(100);

            for (var i = 0; i < 100; i++)
            {
                var entity = registry.Create();
                registry.AddComponent<Position>(entity, new Position { X = i * 10, Y = i * 10 });
                registry.AddComponent<Velocity>(entity, new Velocity { X = 2, Y = 2 });

                if (i % 5 == 0) registry.AddComponent<Fart>(entity, new Fart { Power = 666 });
            }

            RunPrinterSystem(registry);
            RunVelocitySystem(registry);
            RunVelocityGroupSystem(registry);
            RunPrinterSystem(registry);

            using (new SystemTimer("RunVelocitySystem"))
                RunVelocitySystem(registry);

            using (new SystemTimer("RunVelocityGroupSystem"))
                RunVelocityGroupSystem(registry);
        }

        static void RunVelocitySystem(Registry registry)
        {
            var view = registry.View<Velocity, Position>();
            foreach (var entity in view)
            {
                ref Position pos = ref registry.GetComponent<Position>(entity);
                ref Velocity vel = ref registry.GetComponent<Velocity>(entity);
                pos.X += vel.X;
                pos.Y += vel.Y;
            }
        }

        static void RunVelocityGroupSystem(Registry registry)
        {
            var view = registry.Group<Velocity, Position>();
            foreach (var entity in view)
            {
                ref Position pos = ref registry.GetComponent<Position>(entity);
                ref Velocity vel = ref registry.GetComponent<Velocity>(entity);
                pos.X += vel.X;
                pos.Y += vel.Y;
            }
        }

        static void RunPrinterSystem(Registry registry)
        {
            Console.WriteLine("----- Printer -----");
            var view = registry.View<Velocity, Position, Fart>();
            foreach (var entity in view)
            {
                var pos = registry.GetComponent<Position>(entity);
                Console.WriteLine($"entity: {entity}, pos: {pos.X},{pos.Y}");
            }
        }
    }
}
