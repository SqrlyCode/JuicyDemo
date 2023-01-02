using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Rendering.Universal;
using UnityEngine.Timeline;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 5;
    [SerializeField] private LayerMask _enemyHitMask;


    private Rigidbody2D _rigidbody2D;
    private Animator _animator;
    private SpriteRenderer _renderer;
    private float _lookDir = 1; //-1 left, 1 right
    private Vector3 _lastPos; //Position where the Player was last frame
    private bool _isInvulnerable;

    void Awake()
    {
        _animator = GetComponent<Animator>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _renderer = GetComponent<SpriteRenderer>();
        _lastPos = transform.position;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            //TODO: determine closest Enemy
            Enemy closestEnemy = null;
            StartCoroutine(Attack(closestEnemy));
        }
    }


    private IEnumerator Attack(Enemy enemy)
    {
        //TODO: Set to position in front of enemy
        Vector3 dashDestination = Vector3.zero;
        if (enemy != null)
            UpdateLookDir(enemy.transform);

        //TODO: Replace with DoTween
        transform.position = dashDestination;

        _animator.SetTrigger("Attack");
        yield return new WaitForEndOfFrame();

        //As long as Attack-Animation is playing dead damage in front of player
        AnimatorStateInfo animatorStateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        while (animatorStateInfo.IsName("HeroAttack") && animatorStateInfo.normalizedTime < 1)
        {
            animatorStateInfo = _animator.GetCurrentAnimatorStateInfo(0);
            float attackDistance = 1.7f;
            float attackRadius = 0.3f;
            RaycastHit2D[] rayHits = Physics2D.CircleCastAll(transform.position, attackDistance,
                Vector2.right * _lookDir,
                attackRadius, _enemyHitMask);

            //Deal damage to enemy
            foreach (RaycastHit2D rayHit in rayHits)
            {
                Collider2D hitEnemyCollider = rayHit.collider;
                if (hitEnemyCollider != null)
                {
                    Enemy hitEnemy = hitEnemyCollider.GetComponent<Enemy>();
                    hitEnemy.ReceiveDamage();
                }
            }

            if (rayHits.Length > 0)
                break;
            yield return new WaitForEndOfFrame();
        }
    }

    //TODO: GetClosestEnemy()

    //TODO: GetPosInFrontOfEnemy

    #region No Changes are required here

    void FixedUpdate()
    {
        //Move Player
        float horInput = Input.GetAxis("Horizontal");
        Vector2 moveSpeed = new Vector2(horInput, 0) * (Time.deltaTime * _moveSpeed);
        _rigidbody2D.MovePosition(_rigidbody2D.position + Vector3.down * (-Physics2D.gravity * Time.deltaTime) +
                                  moveSpeed);
        _animator.SetFloat("MoveSpeed", Mathf.Abs(horInput));

        UpdateLookDir();
    }

    private void UpdateLookDir(Transform target = null)
    {
        if (target == null)
        {
            float deltaX = transform.position.x - _lastPos.x;
            if (Mathf.Abs(deltaX) > 0.1f * Time.deltaTime)
            {
                _lookDir = Mathf.Sign(deltaX);
            }
        }
        else
        {
            float deltaX = target.position.x - transform.position.x;
            _lookDir = Mathf.Sign(deltaX);
        }

        //Flip Player left and right based on movement
        Vector3 newScale = transform.localScale;
        newScale.x = _lookDir * Mathf.Abs(newScale.x);
        transform.localScale = newScale;

        _lastPos = transform.position;
    }

    #endregion
}