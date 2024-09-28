using UnityEngine;
using System; // 引入事件相关的命名空间

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float dashSpeed = 20f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 1f;
    public float detectionRadius = 5f; // 自动瞄准的检测半径

    private float dashTime;
    private float dashCooldownTime;
    private Vector3 dashDirection;

    private int healthMax = 3;
    private int health;

    // 定义健康值变化事件
    public event Action<int, int> OnHealthChanged; // 传递当前健康值和最大健康值

    private void Start()
    {
        health = healthMax;
        NotifyHealthChanged(); // 初始化时通知UI更新
    }

    private void Update()
    {
        if (dashTime > 0)
        {
            DashMove();
        }
        else
        {
            NormalMove();

            if (dashCooldownTime > 0)
            {
                dashCooldownTime -= Time.deltaTime;
            }

            if (Input.GetKeyDown(KeyCode.Space) && dashCooldownTime <= 0)
            {
                StartDash();
            }
        }
    }

    private void NormalMove()
    {
        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");

        Vector3 move = new Vector3(moveX, moveY, 0f);
        transform.position += move * moveSpeed * Time.deltaTime;
    }

    private void DashMove()
    {
        transform.position += dashDirection * dashSpeed * Time.deltaTime;

        dashTime -= Time.deltaTime;

        if (dashTime <= 0)
        {
            dashCooldownTime = dashCooldown;
        }
    }

    private void StartDash()
    {
        GameObject nearestEnemy = FindNearestEnemy();

        if (nearestEnemy != null)
        {
            dashDirection = (nearestEnemy.transform.position - transform.position).normalized;
        }
        else
        {
            float moveX = Input.GetAxis("Horizontal");
            float moveY = Input.GetAxis("Vertical");

            if (moveX == 0 && moveY == 0)
            {
                dashDirection = transform.up; // 默认向上冲撞
            }
            else
            {
                dashDirection = new Vector3(moveX, moveY, 0f).normalized;
            }
        }

        dashTime = dashDuration;
    }

    private GameObject FindNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject nearestEnemy = null;
        float shortestDistance = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);

            if (distanceToEnemy <= detectionRadius && distanceToEnemy < shortestDistance)
            {
                shortestDistance = distanceToEnemy;
                nearestEnemy = enemy;
            }
        }

        return nearestEnemy;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (dashTime > 0)
            {
                Destroy(collision.gameObject);
                GameManager.Instance.AddScore(5);
            }
            else
            {
                GetHurt();
                Destroy(collision.gameObject);
            }
        }
    }

    private void GetHurt()
    {
        health = Mathf.Clamp(health - 1, 0, healthMax);
        NotifyHealthChanged(); // 通知UI更新健康值
        if (health == 0)
        {
            GameManager.Instance.RestartGame();
        }
    }

    // 通知UI更新健康值的方法
    private void NotifyHealthChanged()
    {
        OnHealthChanged?.Invoke(health, healthMax);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}


