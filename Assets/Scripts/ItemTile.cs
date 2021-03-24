using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ItemTile : TileBase
{
    public GameObject dropItemPrefab;

    public override void GetTileData(Vector3Int location, ITilemap tileMap, ref TileData tileData)
    {
        tileData.transform = Matrix4x4.identity;
        tileData.color = Color.white;
        if (dropItemPrefab != null)
        {
            tileData.sprite = dropItemPrefab.GetComponentInChildren<SpriteRenderer>().sprite;
        }
        tileData.colliderType = Tile.ColliderType.None;
    }
}
