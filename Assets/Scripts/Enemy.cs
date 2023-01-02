using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private int _hp = 1;
    [SerializeField] private GameObject _deathEffectPrefab;
    [SerializeField] private float _moveSpeed = 1;


    private Animator _animator;
    private Rigidbody2D _rigidbody2D;
    private PlayerController _player;

    void Awake()
    {
        _animator = GetComponent<Animator>();
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

    public void ReceiveDamage()
    {
        _hp--;
        if (_hp <= 0)
        {
            Destroy(gameObject);
            Instantiate(_deathEffectPrefab, transform.position, Quaternion.identity);
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