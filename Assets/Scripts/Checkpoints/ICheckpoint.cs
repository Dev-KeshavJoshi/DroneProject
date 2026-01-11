using UnityEngine;

public interface ICheckpoint
{
    void Enter();
    void UpdateCheckpoint();
    void Exit();
    bool IsCompleted { get; }
}
