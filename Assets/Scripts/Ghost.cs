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
        transform.DOMove(newPos, 1)
            .SetEase(Ease.InOutSine)
            // .SetEase(Ease.Linear)
            .SetSpeedBased(true)
            .OnComplete(MoveToRandomDir);
    }
}
