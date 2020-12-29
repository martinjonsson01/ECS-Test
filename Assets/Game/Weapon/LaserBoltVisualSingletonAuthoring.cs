using Game.Graphics;

using Unity.Entities;

using UnityEngine;

namespace Game.Weapon
{
public class LaserBoltVisual : IComponentData, IVisualData
{
    public Material Material { get; set; }
    public Mesh Mesh { get; set; }
}

[DisallowMultipleComponent]
public class LaserBoltVisualSingletonAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public Material material;
    public Mesh mesh;

    public void Convert(Entity entity, EntityManager manager, GameObjectConversionSystem conversionSystem)
    {
        manager.AddComponentData(entity, new LaserBoltVisual
        {
            Material = material,
            Mesh = mesh
        });
    }
}
}