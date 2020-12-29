using UnityEngine;

namespace Game.Graphics
{
public interface IVisualData
{
    Material Material { get; set; }
    Mesh Mesh { get; set; }
}
}
