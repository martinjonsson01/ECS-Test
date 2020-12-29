using Game.Graphics;

using Unity.Entities;

using UnityEngine;

namespace Game.Enemy
{
public class EnemyFighterVisual : IComponentData, IVisualData
{
    public Material Material { get; set; }
    public Mesh Mesh { get; set; }
}

[DisallowMultipleComponent]
public class EnemyFighterVisualSingletonAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public Material material;
    public Mesh mesh;

    public void Convert(Entity entity, EntityManager entityManager, GameObjectConversionSystem conversionSystem)
    {
        entityManager.AddComponentData(entity, new EnemyFighterVisual
        {
            Material = material,
            Mesh = mesh
        });
    }
}
}
