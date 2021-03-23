using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Tilemap))]
public class TilemapManager : MonoBehaviour
{
    public static TilemapManager instance;

    private Grid grid;
    private Tilemap tilemap;

    private Queue<KeyValuePair<Vector3Int, TileBase>> orders;

    void Awake()
    {
        instance = this;
        orders = new Queue<KeyValuePair<Vector3Int, TileBase>>();
    }

    void Start()
    {
        grid = transform.GetComponentInParent<Grid>();
        tilemap = transform.GetComponent<Tilemap>();
    }

    void Update()
    {
        while (orders.Count > 0)
        {
            KeyValuePair<Vector3Int, TileBase> newTileSetOrder = orders.Dequeue();
            tilemap.SetTile(newTileSetOrder.Key, newTileSetOrder.Value);
            tilemap.RefreshTile(newTileSetOrder.Key);
        }
    }

    public Vector3Int WorldToCell(Vector3 position)
    {
        return grid.WorldToCell(position);
    }

    public void ChangeToOtherTileNextFrame(Vector3Int cellPosition, TileBase tile)
    {
        orders.Enqueue(new KeyValuePair<Vector3Int, TileBase>(cellPosition, tile));
        //Destroy(gameObject);
    }

    public void HitTile(Vector3Int cellPosition, Vector2 normal)
    {
        tilemap.GetInstantiatedObject(cellPosition)?.GetComponent<InteractableBlock>()?.Hit(normal, this);
    }
}
