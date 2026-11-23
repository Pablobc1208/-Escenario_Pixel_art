using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public float speed = 5f;        // tiles per second (1 tile = 16px = 1 unit)
    public float jumpSpeed = 12.4f; // about 2.6 tiles of height at gravityScale 3

    // Escenario de 512x288 px = 32x18 casillas, centrado en (0,0)
    public float levelWidth = 32f;
    public float levelHeight = 18f;
    public Vector2 levelCenter = Vector2.zero;

    Rigidbody2D rb;
    SpriteRenderer sr;
    Camera cam;
    float move;
    bool jump, grounded;

#if UNITY_6000_0_OR_NEWER
    Vector2 Vel { get => rb.linearVelocity; set => rb.linearVelocity = value; }
#else
    Vector2 Vel { get => rb.velocity; set => rb.velocity = value; }
#endif

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        cam = Camera.main;
        rb.gravityScale = 3f;
        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.sharedMaterial = new PhysicsMaterial2D { friction = 0f }; // no sticking to walls
    }

    void Update()
    {
        // Controles con el Input System nuevo: A/D o flechas para moverse, Espacio para saltar
        Keyboard teclado = Keyboard.current;
        if (teclado == null) return;

        move = 0;
        if (teclado.aKey.isPressed || teclado.leftArrowKey.isPressed) move = -1;
        if (teclado.dKey.isPressed || teclado.rightArrowKey.isPressed) move = 1;
        if (teclado.spaceKey.wasPressedThisFrame) jump = true;
        if (move != 0) sr.flipX = move < 0;
    }

    void FixedUpdate()
    {
        Vel = new Vector2(move * speed, Vel.y);
        if (jump && grounded) Vel = new Vector2(Vel.x, jumpSpeed);
        jump = false;
        grounded = false; // OnCollisionStay2D sets it again before the next step
    }



    void OnCollisionStay2D(Collision2D c)
    {
        foreach (var p in c.contacts)
            if (p.normal.y > 0.5f) grounded = true;
    }
}