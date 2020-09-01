using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Enemy : MovingObject
{
    [SerializeField] private int playerDamage;
    [SerializeField] private AudioClip enemy1AttackSounds;
    [SerializeField] private AudioClip enemy2AttackSounds;
    
    private static readonly int EnemyAttack = Animator.StringToHash("enemyAttack");
    
    private Animator animator;
    private Transform target;
    private bool skipMove;

    protected override void Start()
    {
        GameBehavior.Instance.AddEnemyToList(this);
        animator = GetComponent<Animator>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        base.Start();
    }

    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        if (skipMove)
        {
            skipMove = false;
            return;
        }
        
        base.AttemptMove<T>(xDir, yDir);

        skipMove = true;
    }

    public void MoveEnemy()
    {
        var xDir = 0;
        var yDir = 0;
        
        //bool canMove = Move()        
        
        if (Mathf.Abs(target.position.x - transform.position.x) < float.Epsilon)
            yDir = target.position.y > transform.position.y ? 1 : -1;
        else
        {
            xDir = target.position.x > transform.position.x ? 1 : -1;
        }
            
        
        AttemptMove<Player>(xDir, yDir);
    }

    protected override void OnCantMove<T>(T component)
    {
        var hitPlayer = component as Player;
        
        animator.SetTrigger(EnemyAttack);
        
        hitPlayer.LoseFood(playerDamage);
        
        GameSound.Instance.RandomizeSfx(enemy1AttackSounds, enemy2AttackSounds);
        
    }
}
