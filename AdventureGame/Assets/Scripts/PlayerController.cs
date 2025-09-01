using System.Data;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // Components
    private Rigidbody2D _rb;
    private Animator _animator;
    private SpriteRenderer _spriteRenderer;

    // Stats
    [Header("Stats")]
    [SerializeField] private int _maxHP = 100;
    [SerializeField] private int _maxMP = 100;
    [SerializeField] private int _currentHP = 100;
    [SerializeField] private int _currentMP = 100;
    [SerializeField] private int _hpRegenPerSec = 1;
    [SerializeField] private int _mpRegenPerSec = 1;
    private float _regenTimer = 0f;

    // Movement
    [Header("Movement Settings")]
    [SerializeField] private float _walkSpeed = 5f;
    [SerializeField] private float _runSpeed = 10f;

    // Jump
    [Header("Jump Settings")]
    [SerializeField] private float _jumpForce = 10f;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private float _groundRayLength = 0.2f;
    [SerializeField] private float _groundRaySpread = 0.2f;
    [SerializeField] private bool _isGrounded = false;

    // Attack
    [Header("Attack Settings")]
    [SerializeField] private bool _isAttacking = false;
    [SerializeField] private bool _canQueue = false;
    [SerializeField] private int _comboStep = 0;
    [SerializeField] private int _queueStep = 0;
    [SerializeField] private bool _isAirAttacking = false;
    [SerializeField] private bool _isDashAttacking = false;
    [SerializeField] private bool _isUAttacking = false;
    [SerializeField] private bool _isKUAttacking = false;
    [SerializeField] private bool _isUltimate = false;

    // Dash
    [Header("Dash Settings")]
    [SerializeField] private float _dashSpeed = 15f;
    private float _originalGravityScale = 1f;

    //Defend
    [Header("Defend Settings")]
    [SerializeField] private bool _isDefending = false;

    #region Unity Method
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _originalGravityScale = _rb.gravityScale;
    }
    private void Update()
    {
        HandleDefend();
        HandleJump();
        HandleMovement();
        HandleAttack();
        HandleRegen();
    }
    private void FixedUpdate()
    {
        CheckGround();
        if (_isDefending) return;

        if (_isUltimate) return;

        if (_isDashAttacking)
        {
            float dashDirection = _spriteRenderer.flipX ? -1f : 1f;
            _rb.linearVelocity = new Vector2(dashDirection * _dashSpeed, 0f);
        }
        if (_isKUAttacking)
        {
            float kuDirection = _spriteRenderer.flipX ? -1f : 1f;
            Vector2 kuAtkVelocity = new Vector2(kuDirection, -1f).normalized * _dashSpeed;
            _rb.linearVelocity = kuAtkVelocity;
            if (_isGrounded)
            {
                OnKUAttackEnd();
            }
        }
    }
    #endregion

    #region HP/MP
    private void HandleRegen()
    {
        _regenTimer += Time.deltaTime;
        if (_regenTimer >= 1f)
        {
            _currentHP = Mathf.Min(_currentHP + _hpRegenPerSec, _maxHP);
            _currentMP = Mathf.Min(_currentMP + _mpRegenPerSec, _maxMP);
            _regenTimer = 0f;
        }
    }

    public bool UseMana(int amount)
    {
        if (_currentMP >= amount)
        {
            _currentMP -= amount;
            return true;
        }
        return false;
    }
    #endregion

    #region Movement
    private void HandleMovement()
    {

        float horizontalInput = Input.GetAxisRaw(GameConfig.HORIZONTAL_INPUT);
        //Flip Player
        UpdateSpriteDirection(horizontalInput);
        // Stop Movement when Defending
        if (_isDefending) return;
        //Change Animation
        bool isRunning = Input.GetKey(KeyCode.Space);
        float blend = Mathf.Abs(horizontalInput) > 0.1f ? (isRunning ? 1f : 0.5f) : 0f;
        _animator.SetFloat(GameConfig.MOVING_STATE, blend);
        float moveSpeed = isRunning ? _runSpeed : _walkSpeed;
        _rb.linearVelocity = new Vector2(horizontalInput * moveSpeed, _rb.linearVelocity.y);
    }

    private void UpdateSpriteDirection(float horizontalInput)
    {
        if (horizontalInput == 0) return;
        _spriteRenderer.flipX = horizontalInput < 0;
    }
    #endregion

    #region Attack
    private void HandleAttack()
    {
        if (_isDefending) return;

        if (_isUltimate) return;

        if (Input.GetKeyDown(KeyCode.J) && !_isDashAttacking && !_isKUAttacking && !_isUAttacking)
        {
            if (_isGrounded && !_isAirAttacking)
            {
                if (!_isAttacking)
                {
                    StartAttackCombo(1);
                }
                else if (_canQueue && _comboStep < 3)
                {
                    QueueNext(_comboStep + 1);
                }
            }
            else
            {
                if (!_isAirAttacking)
                {
                    StartAirAttack();
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.L) && !_isDashAttacking && !_isAirAttacking && !_isUAttacking && !_isKUAttacking)
        {
            StartDashAttack();
        }
        if (Input.GetKeyDown(KeyCode.U) && !_isDashAttacking && !_isAirAttacking)
        {
            if (!_isAttacking)
            {
                if (_isGrounded && !_isUAttacking)
                {
                    StartUAttack();
                }
                else
                {
                    if (!_isKUAttacking)
                    {
                        StartKUAttack();
                    }
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.I) && !_isAttacking && _isGrounded && !_isDefending)
        {
            StartUltimate();
        }
    }
    private void CancelAttack()
    {
        _isAttacking = false;
        _isAirAttacking = false;
        _isUAttacking = false;
        _isKUAttacking = false;
        _comboStep = 0;
        _animator.SetBool(GameConfig.ATTACK_QUEUE, false);
        _animator.SetInteger(GameConfig.ATTACK_STATE, 0);
        _animator.ResetTrigger(GameConfig.IS_ATTACK);
        _animator.ResetTrigger(GameConfig.AIR_ATK_TRIGGER);
        _animator.ResetTrigger(GameConfig.U_ATK_TRIGGER);
        _animator.ResetTrigger(GameConfig.K_U_ATK_TRIGGER);
    }
    #endregion

    #region Attack Combo
    private void StartAttackCombo(int index)
    {
        _isAttacking = true;
        _comboStep = index;
        _animator.SetTrigger(GameConfig.IS_ATTACK);
        _animator.SetInteger(GameConfig.ATTACK_STATE, index);
        _animator.SetBool(GameConfig.ATTACK_QUEUE, false);
    }
    private void QueueNext(int nextIndex)
    {
        _queueStep = nextIndex;
        _animator.SetInteger(GameConfig.ATTACK_STATE, nextIndex);
        _animator.SetBool(GameConfig.ATTACK_QUEUE, true);
    }

    #region Animation Event
    public void OnStartAttack()
    {
        _isAttacking = true;
        _canQueue = false;
        _queueStep = 0;
        _animator.SetBool(GameConfig.ATTACK_QUEUE, false);
    }
    public void OpenCombo()
    {
        _canQueue = true;
    }
    public void CloseCombo()
    {
        _canQueue = false;
    }
    public void OnAttackEnd()
    {
        if (_queueStep != 0)
        {
            _comboStep = _queueStep;
            _queueStep = 0;
        }
        else
        {
            _isAttacking = false;
            _comboStep = 0;
            _animator.SetBool(GameConfig.ATTACK_QUEUE, false);
            _animator.SetInteger(GameConfig.ATTACK_STATE, 0);
        }
    }
    #endregion
    private void CancelAtkCombo()
    {
        _queueStep = 0;
        _comboStep = 0;
        _animator.SetBool(GameConfig.ATTACK_QUEUE, false);
        _animator.SetInteger(GameConfig.ATTACK_STATE, 0);
    }
    #endregion

    #region Air Attack
    private void StartAirAttack()
    {
        _isAirAttacking = true;
        _isAttacking = true;
        CancelAtkCombo();
        _animator.SetTrigger(GameConfig.AIR_ATK_TRIGGER);
    }
    #endregion

    #region Dash Attack
    private void StartDashAttack()
    {
        if (!UseMana(20)) return;
        _isDashAttacking = true;
        _isAttacking = true;
        CancelAtkCombo();
        _animator.SetTrigger(GameConfig.DASH_ATK_TRIGGER);
        _rb.gravityScale = 0f;
    }

    // Animation Event
    public void OnDashAttackEnd()
    {
        CancelDash();
        CancelAttack();
    }
    private void CancelDash()
    {
        _isDashAttacking = false;
        _rb.gravityScale = _originalGravityScale;
    }
    #endregion

    #region U_Attack
    private void StartUAttack()
    {
        if (!UseMana(10)) return;
        _isUAttacking = true;
        _isAttacking = true;
        CancelAtkCombo();
        _animator.SetTrigger(GameConfig.U_ATK_TRIGGER);
    }

    // Animation Event
    public void OnUAttackEnd()
    {
        CancelAttack();
    }
    #endregion

    #region K_U_Attack
    private void StartKUAttack()
    {
        if (!UseMana(15)) return;
        _isKUAttacking = true;
        _isAttacking = true;
        CancelAtkCombo();
        _animator.SetTrigger(GameConfig.K_U_ATK_TRIGGER);
    }

    // Animation Event
    public void OnKUAttackEnd()
    {
        CancelAttack();
    }
    #endregion

    #region Ultimate
    private void StartUltimate()
    {
        if (!UseMana(50)) return;
        _isUltimate = true;
        _isAttacking = true;
        CancelAtkCombo();
        _animator.SetTrigger(GameConfig.ULTIMATE_TRIGGER);
        _rb.linearVelocity = Vector2.zero;
        GameManager.Instance.SetGameSpeed(0.5f);
    }

    // Animation Event
    public void OnUltimateEnd()
    {
        _isUltimate = false;
        CancelAttack();
        GameManager.Instance.SetGameSpeed(1f);
    }
    #endregion

    #region Defend
    private void HandleDefend()
    {
        if (Input.GetKeyDown(KeyCode.S) && _isGrounded)
        {
            CancelAttack();
            CancelDash();
            _rb.linearVelocity = new Vector2(0, _rb.linearVelocity.y);
            _isDefending = true;
            _animator.SetTrigger(GameConfig.DEF_TRIGGER);
        }
    }
    // Animation Event
    public void OnDefendEnd()
    {
        _isDefending = false;
    }
    #endregion

    #region Jump
    private void HandleJump()
    {
        if (_isDefending) return;

        if (_isUltimate) return;

        if (Input.GetKeyDown(KeyCode.K) && _isGrounded && !_isDashAttacking)
        {
            CancelAttack();
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, _jumpForce);
            _animator.SetTrigger(GameConfig.JUMP_TRIGGER);
        }
    }
    private void CheckGround()
    {
        Vector2 o = _groundCheck ? (Vector2)_groundCheck.position : (Vector2)transform.position;
        Vector2 oL = o + Vector2.left * _groundRaySpread;
        Vector2 oR = o + Vector2.right * _groundRaySpread;

        bool hitC = Physics2D.Raycast(o, Vector2.down, _groundRayLength, _groundLayer);
        bool hitL = Physics2D.Raycast(oL, Vector2.down, _groundRayLength, _groundLayer);
        bool hitR = Physics2D.Raycast(oR, Vector2.down, _groundRayLength, _groundLayer);

        _isGrounded = hitC || hitL || hitR;

        _animator.SetBool(GameConfig.IS_GROUND, _isGrounded);
        if (!_isGrounded && _isAttacking && !_isAirAttacking && !_isKUAttacking)
        {
            CancelAttack();
        }
        if (_isGrounded && (_isAirAttacking || _isKUAttacking))
        {
            CancelAttack();
        }
        bool falling = !_isGrounded && _rb.linearVelocity.y < 0.01f;
        _animator.SetBool(GameConfig.FALLING_STATE, falling);
    }
    private void OnDrawGizmosSelected()
    {
        if (!_groundCheck) return;

        Vector3 o = _groundCheck.position;
        Vector3 oL = o + Vector3.left * _groundRaySpread;
        Vector3 oR = o + Vector3.right * _groundRaySpread;

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(o, o + Vector3.down * _groundRayLength);
        Gizmos.DrawLine(oL, oL + Vector3.down * _groundRayLength);
        Gizmos.DrawLine(oR, oR + Vector3.down * _groundRayLength);
    }
    #endregion

    public void Damage(int amount)
    {
        _currentHP -= amount;
        if (_currentHP <= 0)
        {
            _currentHP = 0;
            // Handle Player Death (not implemented)
        }
    }

}