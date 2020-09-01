using System.Numerics;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class Player : MovingObject
{
    [SerializeField] private Text foodText;
    [SerializeField] private AudioClip[] moveSounds;
    [SerializeField] private AudioClip[] eatSounds;
    [SerializeField] private AudioClip[] drinkSounds;
    [SerializeField] private AudioClip[] attackSounds;
    [SerializeField] private AudioClip gameOverSound;
        
    private int wallDamage = 1;
    private int pointsPerFood = 10;
    private int pointsPerSoda = 10;
    private float restartLevelDelay = 1f;
    private Animator animator;
    private int food;
    private static readonly int PlayerAttack = Animator.StringToHash("playerAttack");
    private static readonly int PlayerHit = Animator.StringToHash("playerHit");
    private Vector2 touchOrigin = -Vector2.one;

    protected override void Start()
    {
        animator = GetComponent<Animator>();
        food = GameBehavior.Instance.playerFoodPoints;
        foodText.text = "Food: " + food;
        transform.position = new Vector3(0,0,0);
        base.Start();
    }
    
    private void OnDisable()
    {
        GameBehavior.Instance.playerFoodPoints = food;
    }

    private void Update()
    {
        if (!GameBehavior.Instance.playersTurn) return;
        if (GameBehavior.Instance.doingSetup) return;

        var horizontal = 0;
        var vertical = 0;

    #if UNITY_STANDALONE
        horizontal = (int) Input.GetAxisRaw("Horizontal");
        vertical = (int) Input.GetAxisRaw("Vertical");

        if (horizontal != 0) vertical = 0;
        
    
    #else
        if (Input.touchCount > 0)
        {
            Touch myTouch = Input.touches[0];
            if (myTouch.phase) == TouchPhase.Began
            {
                touchOrigin = myTouch.position;
            }
            else if(myTouch.phase == TouchPhase.Ended && touchOrigin.x >= 0)
        }
            
            
        
        
        if (horizontal != 0 || vertical != 0)
            AttemptMove<Wall>(horizontal, vertical);
    #endif
    }

   

    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        food--;
        foodText.text = "Food: " + food;
        base.AttemptMove<T>(xDir, yDir);

        RaycastHit2D hit;
        
        if (Move(xDir, yDir, out hit))
            GameSound.Instance.RandomizeSfx(moveSounds);
        
        CheckIfGameIsOver();
        GameBehavior.Instance.playersTurn = false;
    }

    private void CheckIfGameIsOver()
    {
        if (food <= 0)
        {
            GameSound.Instance.PlaySingle(gameOverSound);
            GameSound.Instance.musicSource.Stop();
            GameBehavior.Instance.GameOver();
        }
            
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Exit"))
        {
            Invoke(nameof(Restart), restartLevelDelay);
            enabled = false;
        } else if (other.CompareTag("Food"))
        {
            food += pointsPerFood;
            foodText.text = "+" + pointsPerFood + " Food: " + food;
            GameSound.Instance.RandomizeSfx(eatSounds);
            other.gameObject.SetActive(false);
        } else if (other.CompareTag("Soda"))
        {
            food += pointsPerSoda;
            foodText.text = "+" + pointsPerSoda + " Food: " + food;
            GameSound.Instance.RandomizeSfx(eatSounds);
            other.gameObject.SetActive(false);
        }
    }

    protected override void OnCantMove<T>(T component)
    {
        var hitWall = component as Wall;
        hitWall.DamageWall(wallDamage);
        animator.SetTrigger(PlayerAttack);
        GameSound.Instance.RandomizeSfx(attackSounds);
    }

    private void Restart()
    {
        SceneManager.LoadScene("MainScene");
    }

    public void LoseFood(int loss)
    {
        animator.SetTrigger(PlayerHit);
        food -= loss;
        foodText.text = "-" + loss + " Food: " + food;
        CheckIfGameIsOver();
    }
}
