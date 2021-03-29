using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new SpriteToFontData", menuName = "ScriptableObject/SpriteToFontData")]
public class SpriteToFontData : ScriptableObject
{
    public Sprite[] numbers = new Sprite[10];
}
