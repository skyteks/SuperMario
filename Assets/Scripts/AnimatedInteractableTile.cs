using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
[CreateAssetMenu(fileName = "New Animated Interactable Tile", menuName = "Tiles/Animated Interactable Tile")]
public class AnimatedInteractableTile : AnimatedTile
{
    public TileBase changeToOnInteraction;

    public override void GetTileData(Vector3Int cellPos, ITilemap tilemap, ref TileData tileData)
    {
        base.GetTileData(cellPos, tilemap, ref tileData);
    }

    public virtual void Hit(Vector3Int cellPos, Vector2 normal, TilemapManager manager)
    {
        bool triggered = false;
        if (normal.y < 0f)
        {
            triggered = true;
        }

        if (triggered)
        {
            Interact(cellPos, normal, manager);
        }
    }

    protected virtual void Interact(Vector3Int cellPos, Vector2 normal, TilemapManager manager)
    {
        GameManager.Instance.StopPlayerMovementY();

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

        TryDropItem(cellPos, outDir, manager);

        manager.ChangeToOtherTileNextFrame(cellPos, changeToOnInteraction);
    }

    protected void TryDropItem(Vector3Int cellPos, Vector2 outDirection, TilemapManager manager)
    {
        GameObject dropItemPrefab = manager.GetItemFromTile(cellPos);
        if (dropItemPrefab != null)
        {
            GameObject dropInstance = Instantiate(dropItemPrefab, manager.CellToWorld(cellPos) + Vector3.down * 0.5f, Quaternion.identity);
            manager.StartCoroutine(MoveOneCellIntoDirection(dropInstance, outDirection));
            //dropInstance.transform.Translate(Vector3.up);
        }
    }

    private IEnumerator MoveOneCellIntoDirection(GameObject instance, Vector2 direction)
    {
        MovementController controller = instance.GetComponent<MovementController>();
        controller.ToggleFreeze(true);
        Vector3 startPos = instance.transform.position;
        Vector3 endPos = startPos + direction.ToVector3();
        float indexer = 0.25f;
        while (indexer != 1f)
        {
            instance.transform.position = Vector3.Lerp(startPos, endPos, indexer);
            yield return null;
            indexer = Mathf.Clamp01(indexer + Time.deltaTime);
        }
        controller.ToggleFreeze(false);
    }
}
