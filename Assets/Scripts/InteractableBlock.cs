using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Collider2D))]
public class InteractableBlock : MonoBehaviour
{
    protected Grid grid;
    protected Tilemap tilemap;

    protected Collider2D hitbox;

    public Vector3Int cellPosition
    {
        get
        {
            return grid.WorldToCell(transform.position);
        }
    }

    protected void Awake()
    {
        grid = GetComponentInParent<Grid>();
        tilemap = GetComponentInParent<Tilemap>();
        hitbox = GetComponent<Collider2D>();
    }

    public virtual void HitBlock(Vector2 fromDir)
    {

    }

    protected void ChangeToOtherTile(TileBase tile)
    {
        tilemap.SetTile(cellPosition, tile);
    }
}
