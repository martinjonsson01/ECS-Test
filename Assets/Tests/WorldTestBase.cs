using NUnit.Framework;

using Unity.Entities;

namespace Tests
{
public abstract class WorldTestBase
{
    protected EntityManager EntityManager;
    protected World World;

    [SetUp]
    public virtual void SetUp()
    {
        World = World.DefaultGameObjectInjectionWorld = new World("Test World");
        EntityManager = World.EntityManager;
    }
}
}
