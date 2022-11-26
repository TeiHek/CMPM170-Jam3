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
        if(!GameManager.Instance.MapManager.IsInBounds(tilePos + Vector3Int.left) && movement.x < -0.01f)
        {
            movement.x = 0f;
        }
        // Check Right
        if (!GameManager.Instance.MapManager.IsInBounds(tilePos + Vector3Int.right) && movement.x > 0.01f)
        {
            movement.x = 0f;
        }
        // Check up
        if (!GameManager.Instance.MapManager.IsInBounds(tilePos + Vector3Int.up) && movement.y > 0.01f)
        {
            movement.y = 0f;
        }
        // Check down
        if (!GameManager.Instance.MapManager.IsInBounds(tilePos + Vector3Int.down) && movement.y < -0.01f)
        {
            movement.y = 0f;
        }
        return movement;
    }

    // Click dependent on game state
    public void LeftClick(InputAction.CallbackContext context)
    {
        if (!context.started)
        {
            return;
        }

        TryLeftClick();
    }

    private void TryLeftClick()
    {
        GameManager.Instance.MapManager.DebugClick();
        if(GameManager.Instance.state == GameState.PlayerTurn)
        {
            Vector3Int pos = GameManager.Instance.MapManager.GetMousePosition();
            // Ensure mouse is in bounds before trying any action
            if(!GameManager.Instance.MapManager.IsInBounds(pos))
            {
                return;
            }

            // Actions if a unit is selected
            if(GameManager.Instance.MapManager.GetSelectedUnit() != null)
            {
                // Click on same unit
                if(GameManager.Instance.MapManager.GetUnitAt(pos) == GameManager.Instance.MapManager.GetSelectedUnit())
                {
                    // Bring up option menu
                    return;
                }

                // Click on another unit after first selection or tile unit cannot navigate to
                if(GameManager.Instance.MapManager.IsAllyUnit(pos) || !GameManager.Instance.MapManager.IsNavigable(pos) )
                {
                    print("Deselect");
                    // Deselect Unit
                    GameManager.Instance.MapManager.DeselectUnit();
                    return;
                }

                // Click on unoccupied tile
                if(GameManager.Instance.MapManager.GetUnitAt(pos) == null)
                {
                    // Note: Bring up a menu before choosing to move.
                    print("move");
                    GameManager.Instance.MapManager.MoveSelectedUnit(pos);
                    return;
                }
            }
            print("hi");
            // Check if allied unit and has not acted yet
            if(GameManager.Instance.MapManager.IsAllyUnit(pos) && GameManager.Instance.MapManager.GetUnitAt(pos).ableToAct)
            {
                print("Selected");
                GameManager.Instance.MapManager.SelectUnit(pos);
            }
        }
    }
}
