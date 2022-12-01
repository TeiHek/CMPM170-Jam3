using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMenuController : MonoBehaviour
{
    public Canvas canvas;
    public GameObject moveButton;
    public GameObject attackButton;
    public GameObject cancelButton;
    public GameObject turnFadeEffect;
    [Range(0f, 1f)] public float screenAlphaRate;
    private GameObject playerTurnText;
    private GameObject enemyTurnText;

    // Start is called before the first frame update
    void Start()
    {
        moveButton = GameObject.Instantiate(moveButton);
        attackButton = GameObject.Instantiate(attackButton);
        cancelButton = GameObject.Instantiate(cancelButton);
        moveButton.transform.SetParent(canvas.transform, false);
        attackButton.transform.SetParent(canvas.transform, false);
        cancelButton.transform.SetParent(canvas.transform, false);
        HideActionButtons();
        turnFadeEffect = GameObject.Instantiate(turnFadeEffect);
        turnFadeEffect.transform.SetParent(canvas.transform, false);
        // hardcoded but these are part of a prefab
        playerTurnText = turnFadeEffect.transform.GetChild(0).gameObject;
        enemyTurnText = turnFadeEffect.transform.GetChild(1).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowActionButtons(Vector3Int tile, bool targetInRange)
    {
        GameManager.Instance.UIOpen = true;
        Vector3 midtile = GameManager.Instance.MapManager.grid.CellToLocal(tile) + GameManager.Instance.MapManager.grid.cellSize / 2;
        if (targetInRange)
        {
            moveButton.SetActive(true);
            attackButton.SetActive(true);
            cancelButton.SetActive(true);
            moveButton.transform.position = Camera.main.WorldToScreenPoint(midtile + Vector3.right * 0.95f);
            attackButton.transform.position = Camera.main.WorldToScreenPoint(midtile + Vector3.right * 0.95f + Vector3.up);
            cancelButton.transform.position = Camera.main.WorldToScreenPoint(midtile + Vector3.right * 0.95f + Vector3.down);
        }
        else
        {
            moveButton.SetActive(true);
            cancelButton.SetActive(true);
            moveButton.transform.position = Camera.main.WorldToScreenPoint(midtile + Vector3.right * 0.95f + Vector3.up * 0.5f);
            cancelButton.transform.position = Camera.main.WorldToScreenPoint(midtile + Vector3.right * 0.95f + Vector3.down * 0.5f);
        }
    }

    public void HideActionButtons()
    {
        moveButton.SetActive(false);
        attackButton.SetActive(false);
        cancelButton.SetActive(false);
        GameManager.Instance.UIOpen = false;
    }
    public IEnumerator TurnFade(GameState state)
    {
        if (state == GameState.PlayerTurn)
        {
            playerTurnText.SetActive(true);
            enemyTurnText.SetActive(false);
        }
        else
        {
            playerTurnText.SetActive(false);
            enemyTurnText?.SetActive(true);
        }
        while (turnFadeEffect.GetComponent<CanvasGroup>().alpha < 0.999f)
        {
            turnFadeEffect.GetComponent<CanvasGroup>().alpha += screenAlphaRate * Time.deltaTime;
            yield return null;
        }
        yield return new WaitForSeconds(0.5f);
        while (turnFadeEffect.GetComponent<CanvasGroup>().alpha > 0.001f)
        {
            turnFadeEffect.GetComponent<CanvasGroup>().alpha -= screenAlphaRate * Time.deltaTime;
            yield return null;
        }
        turnFadeEffect.GetComponent<CanvasGroup>().alpha = 0;
    }
}
