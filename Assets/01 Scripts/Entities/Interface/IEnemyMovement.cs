using UnityEngine;

public interface IEnemyMovement
{
    void SetDestination(Vector3 destination);
    void Stop();
    void Resume();

    bool HasReachedDestination { get; }
}