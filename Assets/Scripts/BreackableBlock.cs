using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BreackableBlock : InteractableBlock
{
    public GameObject dropPrefab;

    protected override void Interact(Vector2 normal, TilemapManager manager)
    {
        Vector3 outDir;
        if (normal.y > 0f)
        {
            outDir = Vector2.down;
        }
        else if (normal.y < 0f)
        {
            outDir = Vector2.up;
        }
        else
        {
            outDir = Vector2.up;
        }

        if (dropPrefab != null)
        {
            GameObject dropInstance = Instantiate(dropPrefab, transform.position + outDir, Quaternion.identity);
        }

        base.Interact(normal, manager);
    }
}
