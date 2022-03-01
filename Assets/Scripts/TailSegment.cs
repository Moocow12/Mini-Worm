using UnityEngine;

public class TailSegment : MonoBehaviour
{
    public TailSegment FollowingSegment = null;
    public Vector3 CurrentDirection = Vector3.zero;

    public void Follow(Vector3 nextDirection)
    {
        transform.Translate(CurrentDirection);
        if (FollowingSegment != null)
        {
            FollowingSegment.Follow(CurrentDirection);
        }
        CurrentDirection = nextDirection;
    }
}
