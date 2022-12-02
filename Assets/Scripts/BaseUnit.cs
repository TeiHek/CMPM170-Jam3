using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using static EnemyAI;

public abstract class BaseUnit : MonoBehaviour
{
    public enum AttackType {
        Arrow, Fire, Lance, Metal, Thunder, None
    };

    public AttackType Type;

    public int maxHp;
    public int hp;
    public int moveRange;
    public int minAttackRange;
    public int maxAttackRange;
    public int attackDamage;
    public bool ableToAct;
    public TileAffinity affinity;
    public VisualEffect damage;
    public VisualEffect attack;
    public bool isStream;



    // Start is called before the first frame update
    public virtual void Start()
    {
    }

    // Update is called once per frame
    public virtual void Update()
    {

    }

    public virtual void AddUnit()
    {

    }

    public int GetMoveRange()
    {
        return moveRange;
    }

    public int GetMinAttackRange()
    {
        return minAttackRange;
    }

    public int GetMaxAttackRange()
    {
        return maxAttackRange;
    }

    public IEnumerator playAnim()
    {
        attack.Play();
        yield return new WaitForSeconds(0.25f);
        attack.Stop();
    }

    public IEnumerator MovePosition(List<Vector3Int> path)
    {
        // 4 is a hardcoded value for this but the movement speed for this is acceptable but should not change between units
        float step = 4 * Time.deltaTime;
        GameState prevState = GameManager.Instance.state;
        GameManager.Instance.state = GameState.UnitInTransit;
        Vector3Int prevPos = GetTile();
        while (path.Count > 0)
        {
            Vector3 midTile = GameManager.Instance.MapManager.grid.CellToWorld(path[0]) + GameManager.Instance.MapManager.grid.cellSize / 2;
            transform.position = Vector2.MoveTowards(transform.position, midTile, step);

            if(Vector2.Distance(transform.position, midTile) < 0.0001f)
            {
                path.RemoveAt(0);
            }
            yield return null;
        }
        yield return new WaitForSeconds(0.05f);
        GameManager.Instance.state = prevState;
        UpdateLocation(prevPos, GetTile());
    }

    public IEnumerator AttackUnit(BaseUnit target)
    {
        // 4 is a hardcoded value for this but the movement speed for this is acceptable but should not change between units
        float step = 4 * Time.deltaTime;
        Vector3 targetPos = Vector3.Normalize(target.transform.position - transform.position) * 0.5f;
        //print(targetPos);
        Vector3 returnPos = transform.position;
        List<Vector3> path = new List<Vector3> { transform.position + targetPos, returnPos };
        bool attackApplied = false;
        if (isStream)
        {
            StartCoroutine(playAnim());
            target.ReceiveDamage(attackDamage);
            target.damage.Play();
        }
        else
        {
            while (path.Count > 0)
            {
                transform.position = Vector2.MoveTowards(transform.position, path[0], step);

                if (Vector2.Distance(transform.position, path[0]) < 0.0001f)
                {
                    path.RemoveAt(0);
                }
                // jank solution but apply damage when unit is closest to targetPos, or when path length is 1
                if (!attackApplied && path.Count == 1)
                {
                    attackApplied = true;
                    attack.Play();
                    target.damage.Play();
                    target.ReceiveDamage(attackDamage);
                }
                yield return null;
            }
        }
    }

    //helper function that will play sound
    void playAttackSound()
    {
        if (this.Type == AttackType.Arrow)
        {
            SoundManager.PlaySound("sfx_HitArrow", 1);
            //PlaySound("sfx_MouseButton", 1);
        }
        else if (this.Type == AttackType.Fire)
        {
            SoundManager.PlaySound("sfx_HitFire", 1);
        }
        else if (this.Type == AttackType.Lance)
        {
            SoundManager.PlaySound("sfx_HitLance", 1);
        }
        else if (this.Type == AttackType.Metal)
        {
            SoundManager.PlaySound("sfx_HitMetal", 1);
        }
        else if (this.Type == AttackType.Thunder)
        {
            SoundManager.PlaySound("sfx_HitThunder", 1);
            Debug.Log("ss");
        }
        else
        {
            Debug.Log("Type not defined, no sfx will be played");
        }

    }
    public IEnumerator MoveAttack(List<Vector3Int> path, BaseUnit target)
    {
        yield return StartCoroutine(MovePosition(path));
        playAttackSound();
        yield return StartCoroutine(AttackUnit(target));

    }

    public void ReceiveDamage(int damage)
    {
        hp -= damage;
        if(hp <= 0)
        {
            // Remove and destroy the GameObject if it dies
            GameManager.Instance.MapManager.removeUnit(GetTile());
            gameObject.SetActive(false);
        }
    }

    public Vector3Int GetTile()
    {
        // Get the cell position based on GameObject position
        return GameManager.Instance.MapManager.grid.WorldToCell(transform.position);
    }

    protected void SnapToGrid()
    {
        transform.position = GameManager.Instance.MapManager.grid.CellToWorld(GetTile()) + GameManager.Instance.MapManager.grid.cellSize / 2;
    }

    public void UpdateLocation(Vector3Int prevPos, Vector3Int newPos)
    {
        GameManager.Instance.MapManager.UpdateUnitLocation(this, prevPos, newPos);
    }



}
