using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HiddenInteractableTile : InteractableTile
{
    public Sprite playerSprite;
    public Sprite editorSprite;

    private new Sprite sprite
    {
        get
        {
            return Application.isPlaying ? playerSprite : editorSprite;
        }
    }

    public override void GetTileData(Vector3Int location, ITilemap tileMap, ref TileData tileData)
    {
        base.GetTileData(location, tileMap, ref tileData);
        tileData.sprite = sprite;
    }

    public override bool GetTileAnimationData(Vector3Int location, ITilemap tileMap, ref TileAnimationData tileAnimationData)
    {
        Sprite[] tmp = new Sprite[1];
        tmp[0] = playerSprite;
        tileAnimationData.animatedSprites = tmp;
        return true;
    }
}
