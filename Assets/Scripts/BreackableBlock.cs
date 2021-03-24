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

        DropItem(outDir, manager);

        base.Interact(normal, manager);
    }

    protected void DropItem(Vector2 outDirection, TilemapManager manager)
    {
        if (dropPrefab == null)
        {
            return;
        }
        GameObject dropInstance = Instantiate(dropPrefab, transform.position, Quaternion.identity);
        manager.StartCoroutine(MoveOneCellIntoDirection(dropInstance, outDirection));
    }

    private IEnumerator MoveOneCellIntoDirection(GameObject instance, Vector2 direction)
    {
        Rigidbody2D rigid = instance.GetComponent<Rigidbody2D>();
        rigid?.Freeze(true);
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + direction.ToVector3();
        float indexer = 0f;
        while (indexer != 1f)
        {
            indexer += Time.deltaTime;
            instance.transform.position = Vector3.Lerp(startPos, endPos, indexer);
            yield return null;
        }
        rigid?.Freeze(false);
    }
}
