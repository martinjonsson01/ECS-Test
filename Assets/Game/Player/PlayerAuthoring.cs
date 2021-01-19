using Unity.Entities;

using UnityEngine;

namespace Game.Player
{
[DisallowMultipleComponent]
public class PlayerAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager manager, GameObjectConversionSystem conversionSystem)
    {
        manager.AddComponents(entity, new ComponentTypes(
            typeof(PlayerTag)
        ));
    }
}
}
