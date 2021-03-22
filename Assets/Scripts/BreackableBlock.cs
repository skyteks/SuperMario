using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BreackableBlock : InteractableBlock
{
    public GameObject dropPrefab;
    public TileBase turnInto;

    public override void HitBlock(Vector2 point)
    {
        point = transform.InverseTransformPoint(point);

        Vector3 outDir;
        if (point.y > 0.5f)
        {
            outDir = Vector2.down;
        }
        else if (point.y < -0.5f)
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

        ChangeToOtherTile(turnInto);
    }
}
