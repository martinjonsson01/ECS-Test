using Unity.Collections;
using Unity.Physics;

namespace Game
{
public static class RaycastHelper
{
    public const int ProjectileCollidableLayer = 6;
    public const int ProjectileLayer = 7;
    public const int PlayerMoveInputRayLayer = 8;

    public static CollisionFilter LayerToFilter(int belongsToLayer, int collidesWithLayer = -1)
    {
        if (belongsToLayer == -1) return CollisionFilter.Zero;
        if (collidesWithLayer == -1) collidesWithLayer = belongsToLayer;

        var belongsMask = new BitField32();
        belongsMask.SetBits(belongsToLayer, true);

        var collidesWithMask = new BitField32();
        collidesWithMask.SetBits(collidesWithLayer, true);

        var filter = new CollisionFilter()
        {
            BelongsTo = collidesWithMask.Value,
            CollidesWith = collidesWithMask.Value
        };
        return filter;
    }
}
}
