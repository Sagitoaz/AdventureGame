using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    [SerializeField] protected int health;
    [SerializeField] protected float speed;
    [SerializeField] protected int gems;
    [SerializeField] protected Transform pointA, pointB;
    [SerializeField] protected Vector3 currentTarget;
    protected Animator anim;
    protected SpriteRenderer sprite;
    [SerializeField] protected PlayerController player;
    [SerializeField] protected float attackRange = 1.5f;
    public virtual void Init()
    {
        anim = GetComponentInChildren<Animator>();
        sprite = GetComponentInChildren<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        currentTarget = pointB.position;
    }
    private void Start()
    {
        Init();
    }
    public virtual void Update() {
        if (anim.GetBool("InCombat") && !anim.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            anim.SetTrigger("Idle");
        }
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") && !anim.GetBool("InCombat"))
        {
            return;
        }
        Movement();
    }
    public virtual void Movement() {
        if (currentTarget == pointA.position) {
            sprite.flipX = true;
        } else {
            sprite.flipX = false;
        }
        if (transform.position.x == pointA.position.x) {
            currentTarget = pointB.position;
            anim.SetTrigger("Idle");
        } else if (transform.position.x == pointB.position.x) {
            currentTarget = pointA.position;
            anim.SetTrigger("Idle");
        }
        if (!anim.GetBool("InCombat")) {
            transform.position = Vector3.MoveTowards(transform.position, currentTarget, speed * Time.deltaTime);
        }
        float distance = Vector3.Distance(transform.position, player.transform.position);
        // Debug.Log("Local Position: " + transform.localPosition);
        //Debug.Log(this.name + " Distance to Player: " + distance);
        if (distance > 2.0f)
        {
            anim.SetBool("InCombat", false);
        }
    }
}
