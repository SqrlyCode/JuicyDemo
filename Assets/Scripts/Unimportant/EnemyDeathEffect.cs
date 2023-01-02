using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDeathEffect : MonoBehaviour
{
    private Animator _animator;
    
    private void Awake()
    {
        _animator = GetComponent<Animator>();
        
    }

    private void Update()
    {
        //Destroy once finished playing the animation
        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.normalizedTime >= 1)
        {
            Destroy(gameObject);
        }
    }
}
