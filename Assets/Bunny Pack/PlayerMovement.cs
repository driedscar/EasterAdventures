using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;
    public float jumpForce = 7f;

    [Header("UI")]
    public TextMeshProUGUI scoreText;
    public GameObject life1;
    public GameObject life2;
    public GameObject life3;
    public GameObject gameOverPanel;
    public GameObject panelNextLevel;

    [Header("Fall Limit")]
    public float fallLimitY = -6f;

    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sr;

    private bool isGrounded;
    private bool isInvincible = false;

    private Vector3 startPos;

    private int score = 0;
    private int life = 3;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        sr = GetComponentInChildren<SpriteRenderer>();

        startPos = transform.position;

        panelNextLevel.SetActive(false);

        UpdateScoreUI();
        gameOverPanel.SetActive(false);
    }

    void Update()
    {
        float move = Input.GetAxis("Horizontal");

        // GERAK
        rb.linearVelocity = new Vector2(move * moveSpeed, rb.linearVelocity.y);

        // ANIMASI JALAN
        anim.SetBool("isWalking", move != 0 && isGrounded);

        // FLIP
        if (move > 0)
            sr.flipX = false;
        else if (move < 0)
            sr.flipX = true;

        // JUMP
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            anim.SetTrigger("jumpTrigger");
        }

        // CEK JATUH
        if (transform.position.y < fallLimitY)
        {
            LoseLife();
        }
    }

    // DIPANGGIL DARI ANIMATION EVENT
    public void DoJump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        isGrounded = false;
        anim.SetBool("isGrounded", false);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            anim.SetBool("isGrounded", true);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
            anim.SetBool("isGrounded", false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // COIN
        if (collision.CompareTag("Coin"))
        {
            score += 10;
            UpdateScoreUI();
            Destroy(collision.gameObject);
        }

        // ENEMY
        if (collision.CompareTag("Enemy"))
        {
            HitEnemy();
        }

        // PORTAL
        if (collision.CompareTag("Portal"))
        {
            Time.timeScale = 0f;
            panelNextLevel.SetActive(true);
        }
    }

    // KENA MUSUH
    void HitEnemy()
    {
        if (isInvincible)
            return;

        life--;

        if (life == 2)
            life3.SetActive(false);
        else if (life == 1)
            life2.SetActive(false);
        else if (life <= 0)
        {
            life1.SetActive(false);
            GameOver();
            return;
        }

        StartCoroutine(Blink());
    }

    // JATUH KE BAWAH
    void LoseLife()
    {
        transform.position = startPos;
        rb.linearVelocity = Vector2.zero;

        life--;

        if (life == 2)
            life3.SetActive(false);
        else if (life == 1)
            life2.SetActive(false);
        else if (life <= 0)
        {
            life1.SetActive(false);
            GameOver();
        }
    }

    // EFEK KERLAP-KERLIP
    IEnumerator Blink()
    {
        isInvincible = true;

        for (int i = 0; i < 6; i++)
        {
            sr.enabled = false;
            yield return new WaitForSeconds(0.15f);

            sr.enabled = true;
            yield return new WaitForSeconds(0.15f);
        }

        isInvincible = false;
    }

    void GameOver()
    {
        gameOverPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    void UpdateScoreUI()
    {
        scoreText.text = "Score: " + score;
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void BackToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    public void NextLevelYes()
    {
        Time.timeScale = 1f;

        int currentScene = SceneManager.GetActiveScene().buildIndex;
        int lastLevel = SceneManager.sceneCountInBuildSettings - 1;

        if (currentScene >= lastLevel)
        {
            SceneManager.LoadScene(0);
        }
        else
        {
            SceneManager.LoadScene(currentScene + 1);
        }
    }

    public void NextLevelNo()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }
}