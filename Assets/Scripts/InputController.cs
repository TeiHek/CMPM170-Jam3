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
        if (GameManager.Instance.UIOpen || GameManager.Instance.listeningForTarget)
        {
            return;
        }
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
       // SoundManager.PlaySound("sfx_MouseClickGood", 1);


        if (GameManager.Instance.state == GameState.PlayerTurn)
        {
  
            Vector3Int pos = GameManager.Instance.MapManager.GetMousePosition();
            // Ensure mouse is in bounds before trying any action
            if(!GameManager.Instance.MapManager.IsInBounds(pos))
            {

                return;
            }
            BaseUnit selectedUnit = GameManager.Instance.MapManager.GetSelectedUnit();


            // Select an enemy to attack, deselect if target is out of range or not an enemy
            if (GameManager.Instance.listeningForTarget)
            {
                SoundManager.PlaySound("sfx_MouseClickGood", 1);
                GameManager.Instance.listeningForTarget = false;
                BaseUnit unit = GameManager.Instance.MapManager.GetUnitAt(pos);
                if (GameManager.Instance.MapManager.EnemyInAttackRange(unit))
                {
                    GameManager.Instance.MapManager.MoveAttack(GameManager.Instance.MapManager.GetTargetTile(), unit);
                }
                else GameManager.Instance.MapManager.DeselectUnit();
                return;
            }
            // Actions if a unit is selected
            else if (selectedUnit != null && !GameManager.Instance.UIOpen)
            {
    
                // Click on another unit after first selection or tile unit cannot navigate to
                if ( (GameManager.Instance.MapManager.IsUnit(pos) && GameManager.Instance.MapManager.GetUnitAt(pos) != GameManager.Instance.MapManager.GetSelectedUnit()) || 
                     !GameManager.Instance.MapManager.IsNavigable(pos) || !GameManager.Instance.MapManager.InSelectedUnitMoveRange(pos))
                {
                    SoundManager.PlaySound("sfx_MouseClick", 1);
                    print("Deselect");
                    // Deselect Unit
                    GameManager.Instance.MapManager.DeselectUnit();
                    return;
                }

                GameManager.Instance.MapManager.SelectTargetTile(pos);
                GameManager.Instance.MapManager.ClearMoveAttackTiles();
                GameManager.Instance.MapManager.ShowAttackTiles(pos, GameManager.Instance.MapManager.GetSelectedUnit());
                GameManager.Instance.UIMenuController.ShowActionButtons(pos, GameManager.Instance.MapManager.EnemyInAttackRange(pos, selectedUnit.GetMaxAttackRange(), selectedUnit.GetMaxAttackRange()));
            }
            // Check if allied unit and has not acted yet
            else if(GameManager.Instance.MapManager.IsAllyUnit(pos) && GameManager.Instance.MapManager.GetUnitAt(pos).ableToAct)
            {
                SoundManager.PlaySound("sfx_MouseClickGood", 1);
                //print("Selected");
                GameManager.Instance.MapManager.SelectUnit(pos);
                GameManager.Instance.MapManager.ShowMoveAttackRange(pos);
            }
        }
    }
}
