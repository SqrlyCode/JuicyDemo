using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
public class Ghost : MonoBehaviour
{
    public Vector2 _minPos;
    public Vector2 _maxPos;
    void Awake()
    {
        MoveToRandomDir();
    }

    void MoveToRandomDir()
    {
        Vector2 newPos = new Vector2(Random.Range(_minPos.x, _maxPos.x), Random.Range(_minPos.y, _maxPos.y));
        
        //TODO: The ghost should continually move towards a random Direction
        transform.DOMove(newPos, 2)
            .SetEase(Ease.InOutSine)
            .SetDelay(2)
            .OnComplete(MoveToRandomDir);
    }
}
