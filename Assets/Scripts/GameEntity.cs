using UnityEngine;

public class GameEntity : MonoBehaviour
{
    public float movementSpeed = 5;
    public float gravity = 3f;
    public bool useGravity = true;
    protected Range jumpVelocity;
    public Range jumpHeight = new Range(0.8f, 4.3f);
    public float jumpDuration;

    protected Vector2 velocity;
    public Vector2 lastVelocity {private set; get;}
    
}