using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Tilemap))]
public class TilemapManager : MonoBehaviour
{
    public static TilemapManager instance;

    private Grid grid;
    private Tilemap plattformTilemap;
    [SerializeField]
    private Tilemap itemTilemap = null;

    private Queue<KeyValuePair<Vector3Int, TileBase>> orders;

    void Awake()
    {
        instance = this;
        orders = new Queue<KeyValuePair<Vector3Int, TileBase>>();
        //itemTilemap.GetComponent<TilemapRenderer>().enabled = false;
    }

    void Start()
    {
        grid = transform.GetComponentInParent<Grid>();
        plattformTilemap = transform.GetComponent<Tilemap>();
    }

    void Update()
    {
        while (orders.Count > 0)
        {
            KeyValuePair<Vector3Int, TileBase> newTileSetOrder = orders.Dequeue();
            plattformTilemap.SetTile(newTileSetOrder.Key, newTileSetOrder.Value);
            plattformTilemap.RefreshTile(newTileSetOrder.Key);
        }
    }

    public Vector3Int WorldToCell(Vector3 position)
    {
        return grid.WorldToCell(position);
    }

    public Vector3 CellToWorld(Vector3Int position)
    {
        return grid.CellToWorld(position) + plattformTilemap.tileAnchor;
    }

    public void ChangeToOtherTileNextFrame(Vector3Int cellPosition, TileBase tile)
    {
        //orders.Enqueue(new KeyValuePair<Vector3Int, TileBase>(cellPosition, tile));
        plattformTilemap.SetTile(cellPosition, tile);
        //plattformTilemap.RefreshTile(cellPosition);
    }

    public void HitTile(Vector3Int cellPosition, Vector2 normal)
    {
        TileBase plattformTile = plattformTilemap.GetTile(cellPosition);
        if (plattformTile != null)
        {
            (plattformTile as InteractableTile)?.Hit(cellPosition ,normal, this);
        }
    }

    public GameObject GetItemFromTile(Vector3Int cellPosition)
    {
        GameObject dropItem = null;
        TileBase tile = itemTilemap.GetTile(cellPosition);
        if (tile != null)
        {
            dropItem = (tile as ItemTile).dropItemPrefab;
            itemTilemap.SetTile(cellPosition, null);
            //itemTilemap.RefreshTile(cellPosition);
        }

        return dropItem;
    }
}
