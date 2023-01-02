using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private int _hp = 1;
    [SerializeField] private GameObject _deathEffectPrefab;
    [SerializeField] private float _moveSpeed = 1;

    private SpriteRenderer _renderer;
    private Rigidbody2D _rigidbody2D;
    private PlayerController _player;

    void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
        _rigidbody2D = GetComponent<Rigidbody2D>();
        _player = FindObjectOfType<PlayerController>();
    }

    void FixedUpdate()
    {
        float xDirToPlayer = Mathf.Sign(_player.transform.position.x - transform.position.x);
        float moveSpeed = xDirToPlayer * Time.deltaTime * _moveSpeed;
        _rigidbody2D.MovePosition(transform.position + Vector3.right * moveSpeed);

        UpdateLookDir();
    }

    private Sequence seq;
    
    public void ReceiveDamage()
    {
        _hp--;
        if (_hp <= 0)
        {
            Destroy(gameObject);
            Instantiate(_deathEffectPrefab, transform.position, Quaternion.identity);
            seq.Kill();
        }
        else
        {
            seq = DOTween.Sequence();
            seq.Append(_renderer.DOColor(Color.red, 0.1f));
            seq.AppendInterval(0.1f);
            seq.Append(_renderer.DOColor(Color.white, 0.1f));
        }
    }

    private void UpdateLookDir()
    {
        float lookDir = Mathf.Sign(transform.position.x - _player.transform.position.x);
        //Flip Player left and right based on movement
        Vector3 newScale = transform.localScale;
        newScale.x = lookDir * Mathf.Abs(newScale.x);
        transform.localScale = newScale;
    }
}