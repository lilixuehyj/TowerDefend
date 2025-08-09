using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolObj : MonoBehaviour
{
    public bool IsReleased { get; private set; }

    public void MarkReleased() => IsReleased = true;
    public void MarkAcquired() => IsReleased = false;
}
