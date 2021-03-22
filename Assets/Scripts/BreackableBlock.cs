using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BreackableBlock : InteractableBlock
{
    public GameObject dropPrefab;
    public TileBase turnInto;

    public override void HitBlock(Vector2 fromDir)
    {
        Vector3 outDir;
        if (fromDir.y > 0.5f)
        {
            outDir = Vector2.up;
        }
        else if (fromDir.y < -0.5f)
        {
            outDir = Vector2.down;
        }
        else
        {
            outDir = Vector2.up;
        }

        if (dropPrefab != null)
        {
            GameObject dropInstance = Instantiate(dropPrefab, transform.position + outDir, Quaternion.identity);
        }

        ChangeToOtherTile(turnInto);
    }
}
