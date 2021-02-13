using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class that contains animation information that is used across animated GameObjects
/// </summary>
public class Animated : MonoBehaviour
{
    public const string MOVING_SIDEWAYS = "movingSideways";
    public const string MOVING_UP = "movingUp";
    public const string MOVING_DOWN = "movingDown";
    public const string IDLE = "idle";

    public const string IDLE_OBJECT = "idleObject";
    public const string DOWN_OBJECT = "movingDownObject";
    public const string UP_OBJECT = "movingUpObject";
    public const string SIDEWAYS_OBJECT = "movingSidewaysObject";

    /// <summary>
    /// Possible animations for the GameObject
    /// </summary>
    protected enum characterAnimation
    {
        Idle,
        Left,
        Right,
        Up,
        Down,
        IdleObject,
        LeftObject,
        RightObject,
        UpObject,
        DownObject
    };

    /// <summary>
    /// The current animation being used by the sprite
    /// </summary>
    protected characterAnimation currentCharacterAnimation = characterAnimation.Down;

    /// <summary>
    /// Whether the GameObject sprite is flipped
    /// </summary>
    protected bool flipped = false;

    /// <summary>
    /// Flips the sprite to face the other direction
    /// </summary>
    protected void flipSprite()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
        flipped = !flipped;
    }
}
