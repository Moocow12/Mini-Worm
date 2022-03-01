using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameManager GameManager;
    public Sprite PlayerBodySprite;

    private bool firstMove = false;
    private float moveRate = .3f;
    private Vector3 bufferedMoveDirection;
    private Vector3 moveDirection = Vector3.zero;
    private List<TailSegment> tailSegments = new List<TailSegment>();

    //update is called once per frame
    void Update()
    {
        float verticalInput = Input.GetAxis("Vertical");
        float horizontalInput = Input.GetAxis("Horizontal");

        //user input - up
        if (verticalInput > 0 && moveDirection != Vector3.down)
        {
            bufferedMoveDirection = Vector3.up;
            if (!firstMove)
            {
                StartCoroutine(MovePlayer());
                firstMove = true;
            }
        }
        //user input - down
        if (verticalInput < 0 && moveDirection != Vector3.up)
        {
            bufferedMoveDirection = Vector3.down;
            if (!firstMove)
            {
                StartCoroutine(MovePlayer());
                firstMove = true;
            }
        }
        //user input - right
        if (horizontalInput > 0 && moveDirection != Vector3.left)
        {
            bufferedMoveDirection = Vector3.right;
            if (!firstMove)
            {
                StartCoroutine(MovePlayer());
                firstMove = true;
            }
        }
        //user input - left
        if (horizontalInput < 0 && moveDirection != Vector3.right)
        {
            bufferedMoveDirection = Vector3.left;
            if (!firstMove)
            {
                StartCoroutine(MovePlayer());
                firstMove = true;
            }
        }
    }

    //add tail segment
    public void Grow()
    {
        TailSegment newSegment = new GameObject("Tail Segment").AddComponent<TailSegment>();
        newSegment.gameObject.AddComponent<SpriteRenderer>().sprite = PlayerBodySprite;
        
        if (tailSegments.Count > 0)         //if we have at least one tail segment already...
        {
            TailSegment lastSegment = tailSegments[tailSegments.Count - 1];
            newSegment.CurrentDirection = lastSegment.CurrentDirection;
            newSegment.transform.position = lastSegment.transform.position - lastSegment.CurrentDirection;
        }
        else                                //if this is our first tail segment...
        {
            newSegment.CurrentDirection = moveDirection;
            newSegment.transform.position = transform.position - moveDirection;
        }
        
        tailSegments.Add(newSegment);

        if (tailSegments.Count > 1)         //if we now have more than one tail segment...
        {
            tailSegments[tailSegments.Count - 2].FollowingSegment = tailSegments[tailSegments.Count - 1];       //give our second to last segment a tail segment reference
        }
    }

    public void IncreaseSpeed()
    {
        moveRate *= .9f;
    }

    public void ResetPlayer()
    {
        for (int i = 0; i < tailSegments.Count;)
        {
            Destroy(tailSegments[0].gameObject);
            tailSegments.RemoveAt(0);
        }
        firstMove = false;
        moveDirection = Vector3.zero;
        moveRate = .3f;
        transform.position = new Vector3(0, 0);
    }

    //returns a List<Vector3> with positions of all the player's tail segments
    public List<Vector3> GetSegmentPositions()
    {
        List<Vector3> retList = new List<Vector3>();
        foreach (TailSegment segment in tailSegments)
        {
            retList.Add(segment.transform.position);
        }
        return retList;
    }

    private IEnumerator MovePlayer()
    {
        moveDirection = bufferedMoveDirection;
        transform.Translate(moveDirection);
        if (tailSegments.Count > 0)
        {
            tailSegments[0].Follow(moveDirection);
        }
        GameManager.PlayerMove();
        yield return new WaitForSeconds(moveRate);
        StartCoroutine(MovePlayer());
    }
}
