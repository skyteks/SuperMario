using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mario : GameEntity
{
    public enum FormStates : int
    {
        dead,
        normal,
        super,
        fire,
    }

    public float sprintSpeed = 8f;
    public float maxSprintSpeed = 11f;
    public float maxSprintTriggerTime = 2f;
    private float currentSprintTriggerTime;
    private int lastSprintDirection;
    private bool isAtMaxSprint;

    private new Collider2D collider;
    private new SpriteRenderer renderer;
    public FormStates formState = FormStates.normal;
    public Dictionary<string, Texture> stateTextures;
    private bool lookUp;
    private bool lookDown;
}
