using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Timeline;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 5;
    [SerializeField] private LayerMask _enemyHitMask;
    [SerializeField] private int _hitpoints = 3;
    

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
            Enemy closestEnemy = GetClosestEnemy();
            if (closestEnemy != null)
                StartCoroutine(Attack(closestEnemy));
        }
    }

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

    private IEnumerator Attack(Enemy enemy)
    {
        //TODO: Dash to Enemy
        Vector3 dashTarget = GetPosInFrontOfEnemy(enemy, 1.7f);
        UpdateLookDir(enemy.transform);
        // if (Mathf.Abs(enemy.transform.position.x - transform.position.x) > 1.7f)
        if(Vector3.Distance(enemy.transform.position, transform.position) > 1.7f)
        {
            Tween dashTween = transform.DOMove(dashTarget, 0.5f)
                .SetEase(Ease.InOutCubic);
        }

        _animator.SetTrigger("Attack");
        yield return new WaitForEndOfFrame();

        //As long as Attack-Animation is playing dead damage in front of player
        AnimatorStateInfo animatorStateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        while (animatorStateInfo.IsName("HeroAttack") && animatorStateInfo.normalizedTime < 1)
        {
            float attackDistance = 1.7f;
            float attackRadius = 0.3f;
            RaycastHit2D[] rayHits = Physics2D.CircleCastAll(transform.position, attackDistance, Vector2.right * _lookDir,
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

    private Enemy GetClosestEnemy()
    {
        Enemy closestEnemy = null;
        float curMinDistance = 9999;
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        foreach (var curEnemy in enemies)
        {
            float distanceToCurEnemy = Vector3.Distance(curEnemy.transform.position, transform.position);
            if (distanceToCurEnemy < curMinDistance)
            {
                closestEnemy = curEnemy;
                curMinDistance = distanceToCurEnemy;
            }
        }

        return closestEnemy;
    }

    private Vector3 GetPosInFrontOfEnemy(Enemy enemy, float distanceToObject)
    {
        Vector3 dirToEnemy = enemy.transform.position - transform.position;
        return transform.position + dirToEnemy - dirToEnemy.normalized * distanceToObject;
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

    private void OnCollisionStay2D(Collision2D collision)
    {
        Enemy collidingEnemy = collision.gameObject.GetComponent<Enemy>();
        if (collidingEnemy != null && _isInvulnerable == false)
        {
            _hitpoints--;
            if (_hitpoints <= 0)
            {
                Destroy(gameObject);
            }
            else
            {
                _isInvulnerable = true;
                _animator.SetTrigger("Hurt");
                Sequence seq = DOTween.Sequence();
                seq.Append(_renderer.DOColor(Color.red, 0.2f));
                seq.AppendInterval(0.1f);
                seq.Append(_renderer.DOColor(Color.white, 0.2f));
                seq.AppendInterval(0.1f);
                seq.Append(_renderer.DOColor(Color.red, 0.2f));
                seq.AppendInterval(0.1f);
                seq.Append(_renderer.DOColor(Color.white, 0.2f));
                seq.OnComplete(() =>
                {
                    _isInvulnerable = false;
                    _animator.SetTrigger("Recover");
                });
            }
        }
    }
}