using System.Collections;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEditor.Callbacks;
using UnityEngine;
using System.Collections.Generic;
using NUnit.Framework.Interfaces;

public enum BossState
{
    Idle, Chase, Attack
}
public class Pair<T1, T2>
{
    public T1 First { get; set; }
    public T2 Second { get; set; }

    public Pair(T1 first, T2 second)
    {
        First = first;
        Second = second;
    }
}
public class BossController : MonoBehaviour
{
    private Rigidbody2D rb2D;
    private Animator animator;

    private SpriteRenderer spriteRenderer;

    [SerializeField] private PlayerController playerController;
    public PlayerController PlayerController => playerController;

    [SerializeField] private BossHitBoxAtkController hitBoxAtk;


    [SerializeField] private float rangeDetect;
    [Header("Stats")]
    [SerializeField] private int _maxHP ;

    [SerializeField] private int _currentHP ;

    [SerializeField] private int _hpRegenPerSec;

    [Header("MovementSetting")]
    [SerializeField] private float speed;

    [Header("AtkSetting")]

    [SerializeField] private float rangeAtkJJ;
    [SerializeField] private int normalDamageAtkJJ;

    [SerializeField] private int critDamageAtkJJ;

    [SerializeField] private float coolDownAtkJJ = 1.5f;
    [SerializeField] private float curCoolDownAtkJJ = 0f;

    [SerializeField] private float rangeAtkJK;

    [SerializeField] private int normalDamageAtkJK;

    [SerializeField] private int critDamageAtkJK;

    [SerializeField] private float coolDownAtkJK = 2f;
    [SerializeField] private float curCoolDownAtkJK = 0f;

    [Header("DastAtkSetting")]
    [SerializeField] private float rangeDashAtk;
    [SerializeField] private int dashAtkDamage;
    [SerializeField] private float _dashSpeed= 15f;
    [SerializeField] private float coolDownDashAtk = 2f;

    [SerializeField] private float curCoolDownDashAtk = 0f;

    
    private float _originalGravityScale = 1f;

    [Header("KickAtkSetting")]
    
    [SerializeField] private float rangeKickAtk;
    [SerializeField] private int kickAtkDamage;

    [SerializeField] private float coolDownKickAtk = 2f;

    [SerializeField] private float curCoolDownKickAtk = 0f;

    private bool isDashAtk = false;
    private bool isAttack = false;
    private bool isDie = false;

    private bool unBeatable = false;

    private bool berserkerMode = false;
    public bool BerserkerMode => berserkerMode;

    private bool hitPlayer = false;
    private BossState curState;

    private Queue<Pair<string, int>> combo = new Queue<Pair<string, int>>();


    private void UpdateSpriteDirection()
    {
        Vector2 dir = playerController.transform.position - gameObject.transform.position;
        if (dir.x < 0.01f)
        {
            spriteRenderer.flipX = true;
            hitBoxAtk.gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 180));
        }
        else
        {
            spriteRenderer.flipX = false;
            hitBoxAtk.gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        }
    }
    private float GetRangeFromBossToPlayer()
    {
        Vector2 dir = playerController.transform.position - gameObject.transform.position;
        return dir.sqrMagnitude;
    }
    private Vector2 GetDirFromBossToPlayer() {
        Vector2 dir = playerController.transform.position - gameObject.transform.position;
        dir.y = 0;
        return dir; 
    }
    public void GetDamaged(int damaged)
    {
        if (!unBeatable)
        {
            _currentHP -= damaged;
        }
        if (_currentHP <= 0)
        {
            _currentHP = 0;
        }

        animator.SetTrigger(GameConfig.BOSS_HURT_TRIGGER);

        if (_currentHP == 0)
        {
            animator.SetTrigger(GameConfig.BOSS_DIE_TRIGGER);
            isDie = true;
        }
        else if (_currentHP < _maxHP / 2)
        {
            if (berserkerMode == false)
            {
                coolDownDashAtk = 3f;
            }
            berserkerMode = true;
            
        }
    }
    private void DashAtk()
    {
        unBeatable = true;
        hitBoxAtk.SetDamage(dashAtkDamage);
        isAttack = true;
        isDashAtk = true;
        hitPlayer = false;
        animator.SetTrigger(GameConfig.BOSS_DASH_ATK_TRIGGER);
        
    }
    private void AtkJK()
    {
        isAttack = true;
        animator.SetTrigger(GameConfig.BOSS_ATKJK1_TRIGGER);
    }
    public void SetHitPlayer()
    {
        hitPlayer = true;
    }
    private void EndAtk()
    {

        if (hitPlayer && combo.Count > 0)
        {
            hitBoxAtk.SetDamage(combo.Peek().Second);
            animator.SetBool(combo.Peek().First, true);
            
            combo.Dequeue();
            isAttack = true;
            hitPlayer = false;   
            unBeatable = false;
            isDashAtk = false;
        }
        else
        {
            isAttack = false;
            unBeatable = false;
            isDashAtk = false;
            combo.Clear();
            animator.SetBool(GameConfig.BOSS_ATKJJ2_BOOL, false);
            animator.SetBool(GameConfig.BOSS_ATKJK2_BOOL, false);
            animator.SetBool(GameConfig.BOSS_ATKJK3_BOOL, false);
            animator.ResetTrigger(GameConfig.BOSS_ATKJJ1_TRIGGER);
            animator.ResetTrigger(GameConfig.BOSS_ATKJK1_TRIGGER);
            animator.ResetTrigger(GameConfig.BOSS_DASH_ATK_TRIGGER);
        }
    }
    private void Stop()
    {
        rb2D.linearVelocity = new Vector2(0, 0);
        
        
    }
    private void OnMove()
    {
        rb2D.linearVelocity = GetDirFromBossToPlayer().normalized * speed;
        
        
    }
    private void OnIdle()
    {
        animator.SetBool(GameConfig.BOSS_CHASE_BOOL, false);
        float range = GetRangeFromBossToPlayer();
        if (range <= rangeDetect * rangeDetect)
        {
            curState = BossState.Chase;
        }
        
    }
    private void OnChase()
    {
        animator.SetBool(GameConfig.BOSS_CHASE_BOOL, true);
        float range = GetRangeFromBossToPlayer();
        if (range <= rangeDashAtk * rangeDashAtk)
        {
            curState = BossState.Attack;
            animator.SetBool(GameConfig.BOSS_CHASE_BOOL, false);
            Stop();
        }
        else if (range <= rangeDetect * rangeDetect)
        {
            
            OnMove();
            
        }
        else
        {
            curState = BossState.Idle;
            Stop();
        }
    }
    private void OnAtk()
    {
        
        float range = GetRangeFromBossToPlayer();
        
        if (range <= rangeDashAtk * rangeDashAtk && curCoolDownDashAtk >= coolDownDashAtk)
        {
            curCoolDownDashAtk = 0f;
            DashAtk();
        }
        else if (range <= rangeAtkJK * rangeAtkJK && curCoolDownAtkJK >= coolDownAtkJK)
        {
            curCoolDownAtkJK = 0f;
            isAttack = true;
            hitPlayer = false;
            animator.SetTrigger(GameConfig.BOSS_ATKJK1_TRIGGER);
            hitBoxAtk.SetDamage(critDamageAtkJK);
            combo.Enqueue(new Pair<string, int>(GameConfig.BOSS_ATKJK2_BOOL, normalDamageAtkJK));
            combo.Enqueue(new Pair<string, int>(GameConfig.BOSS_ATKJK3_BOOL, critDamageAtkJK));
        }
        else if (range <= rangeKickAtk * rangeKickAtk && curCoolDownKickAtk >= coolDownKickAtk)
        {
            unBeatable = true;
            curCoolDownKickAtk = 0f;
            isAttack = true;
            hitPlayer = false;
            hitBoxAtk.SetDamage(kickAtkDamage);
            animator.SetTrigger(GameConfig.BOSS_KICK_TRIGGER);


        }
        else if (range <= rangeAtkJJ * rangeAtkJJ && curCoolDownAtkJJ >= coolDownAtkJJ)
        {
            curCoolDownAtkJJ = 0f;
            isAttack = true;
            hitPlayer = false;
            animator.SetTrigger(GameConfig.BOSS_ATKJJ1_TRIGGER);
            hitBoxAtk.SetDamage(normalDamageAtkJJ);
            combo.Enqueue(new Pair<string, int>(GameConfig.BOSS_ATKJJ2_BOOL, critDamageAtkJK));

        }
        else if (range <= rangeDashAtk * rangeDashAtk)
        {
            return;
        }
        else if (range <= rangeDetect * rangeDetect)
        {

            curState = BossState.Chase;
            OnMove();
        }
        else
        {
            curState = BossState.Idle;
            Stop();
        }
    }
    private void UpdateCoolDown()
    {
        curCoolDownDashAtk += Time.deltaTime;
        curCoolDownAtkJK += Time.deltaTime;
        curCoolDownAtkJJ += Time.deltaTime;
        curCoolDownKickAtk += Time.deltaTime;
    }

    void Awake()
    {
        animator = GetComponent<Animator>();
        rb2D = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        curState = BossState.Idle;
    }


    void FixedUpdate()
    {
        if (!isAttack)
        {
            UpdateSpriteDirection();
        }

        if (isDashAtk)
        {
            rb2D.MovePosition(new Vector2(playerController.transform.position.x, transform.position.y));
            isDashAtk = false;
            
        }
    }

    void Update()
    {


        if (isDie)
        {
            return;
        }
        if (isAttack)
        {
            return;
        }
        UpdateCoolDown();
        switch (curState)
        {
            case BossState.Idle:
                Debug.Log("IDLE");
                OnIdle();
                break;
            case BossState.Chase:
                Debug.Log("Chase");
                OnChase();
                break;
            case BossState.Attack:
                Debug.Log("ATTACK");
                OnAtk();
                break;
        }

    }
}
