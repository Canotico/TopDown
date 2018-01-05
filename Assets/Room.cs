using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    const float CELL_SIZE = 1f;

    public int width;
    public int height;
    public Rect bounds;

    SpriteRenderer debugRenderer;

    public void Start()
    {
        debugRenderer = GetComponent<SpriteRenderer>();
        //StartCoroutine(EscapeOverlap());
    }

    IEnumerator EscapeOverlap()
    {
        Room[] otherRooms = FindObjectsOfType<Room>();
        while(true)
        {
            for (int i = 0; i < otherRooms.Length; i++)
            {
                if (otherRooms[i] == this) continue;
                ResolveOverlap(otherRooms[i]);
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void Update()
    {
        SnapPositionToGrid();
        SnapSizeToGrid();
    }

    void SnapPositionToGrid()
    {
        Vector2 currentPosition = transform.position;
        currentPosition.x = CELL_SIZE * (int)(currentPosition.x / CELL_SIZE);
        if (width % 2 == 0)
            currentPosition.x += currentPosition.x < 0 ? -0.5f : 0.5f;
        currentPosition.y = CELL_SIZE * (int)(currentPosition.y / CELL_SIZE);
        if (height % 2 == 0)
            currentPosition.y += currentPosition.y < 0 ? -0.5f : 0.5f;
        transform.position = currentPosition;
        UpdateBounds();
    }

    void UpdateBounds()
    {
        bounds = new Rect(transform.position - (new Vector3(width, height)) * CELL_SIZE * 0.5f, new Vector2(width, height) * CELL_SIZE);
    }

    void SnapSizeToGrid()
    {
        width = (int)(CELL_SIZE * (int)(width / CELL_SIZE));
        height = (int)(CELL_SIZE * (int)(height / CELL_SIZE));
        debugRenderer.size = new Vector2(width, height);
    }

    public bool ResolveOverlap(Room other)
    {
        Vector2[] otherPoints = new Vector2[]
        {
            other.bounds.min,
            other.bounds.max,
            new Vector2(other.bounds.xMin, other.bounds.yMax),
            new Vector2(other.bounds.xMax, other.bounds.yMin)
        };

        bool isOverlapping = false;
        for (int i = 0; i < 4; i++)
        {
            if(otherPoints[i].x > bounds.min.x && otherPoints[i].x < bounds.max.x &&
                otherPoints[i].y > bounds.min.y && otherPoints[i].y < bounds.max.y)
            {
                isOverlapping = true;
                break;
            }
        }

        if(!isOverlapping)
        {
            if(otherPoints[0].x > bounds.min.x && otherPoints[1].x < bounds.max.x)
            {
                float deltaY = Mathf.Abs(other.transform.position.y - transform.position.y);
                if (deltaY < (other.bounds.height + bounds.height) / 2)
                {
                    Debug.DrawLine(transform.position, other.transform.position, Color.red, 0.1f);
                    isOverlapping = true;
                }
            }
            else if (otherPoints[0].y > bounds.min.y && otherPoints[1].y < bounds.max.y)
            {
                float deltaX = Mathf.Abs(other.transform.position.x - transform.position.x);
                if (deltaX < (other.bounds.width + bounds.width) / 2)
                {
                    Debug.DrawLine(transform.position, other.transform.position, Color.red, 0.1f);
                    isOverlapping = true;
                }
            }
        }

        if(isOverlapping)
        {
            Vector3 resolvingVector = (transform.position - other.transform.position).normalized;
            resolvingVector.x = Mathf.Sign(resolvingVector.x);
            resolvingVector.y = Mathf.Sign(resolvingVector.y);

            transform.position = resolvingVector * CELL_SIZE + transform.position;
            return false;
        }
        return true;
    }


    void OnDrawGizmos()
    {
        if(!Application.isPlaying)
            UpdateBounds();

        Gizmos.color = Color.yellow;

        Gizmos.DrawWireCube(bounds.center, bounds.size);
    }
}
