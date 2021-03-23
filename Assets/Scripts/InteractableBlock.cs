using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class InteractableBlock : MonoBehaviour
{
    public TileBase changeInto;
    public bool destroyOnChange;

    public virtual void Hit(Vector2 normal, TilemapManager manager)
    {
        bool triggered = false;
        if (normal.y < 0f)
        {
            triggered = true;
        }

        if (triggered)
        {
            Interact(normal, manager);
        }
    }

    protected virtual void Interact(Vector2 normal, TilemapManager manager)
    {
        manager.ChangeToOtherTileNextFrame(manager.WorldToCell(transform.position), changeInto);
    }
}
