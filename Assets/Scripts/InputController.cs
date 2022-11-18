using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour
{
    [SerializeField] private float cameraMoveSpeed;
    private Rigidbody2D rb;
    private Vector2 targetMovement;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        targetMovement = Vector2.zero;
    }

    private void Update()
    {
        ApplyMoveCamera();
    }

    public void MoveCamera(InputAction.CallbackContext context)
    {
        targetMovement = context.ReadValue<Vector2>() * cameraMoveSpeed;
    }

    private void ApplyMoveCamera()
    {
        rb.velocity = LimitCameraMovement(targetMovement);
    }

    private Vector2 LimitCameraMovement(Vector2 movement)
    {
        Vector3Int tilePos = GameManager.Instance.MapManager.grid.WorldToCell(transform.position);
        // Check left
        if(!GameManager.Instance.MapManager.InBounds(tilePos + Vector3Int.left) && movement.x < -0.01f)
        {
            movement.x = 0f;
        }
        // Check Right
        if (!GameManager.Instance.MapManager.InBounds(tilePos + Vector3Int.right) && movement.x > 0.01f)
        {
            movement.x = 0f;
        }
        // Check up
        if (!GameManager.Instance.MapManager.InBounds(tilePos + Vector3Int.up) && movement.y > 0.01f)
        {
            movement.y = 0f;
        }
        // Check down
        if (!GameManager.Instance.MapManager.InBounds(tilePos + Vector3Int.down) && movement.y < -0.01f)
        {
            movement.y = 0f;
        }
        return movement;
    }

    // Click dependent on game state
}
