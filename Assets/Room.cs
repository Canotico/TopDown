using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    const float CELL_SIZE = 1f;

    public int width;
    public int height;
    public Rect bounds;

    public bool overlapping = false;
    public bool resolvingOverlap = false;

    SpriteRenderer debugRenderer;
    Vector3 previousResolveVector;

    public void Start()
    {
        debugRenderer = GetComponent<SpriteRenderer>();
        previousResolveVector = Vector3.zero;
    }

    public void Update()
    {
        SnapPositionToGrid();
        SnapSizeToGrid();
        UpdateBounds();
        if (overlapping)
            debugRenderer.color = Color.red;
        else
            debugRenderer.color = Color.white;
    }

    public void FixedUpdate()
    {
        resolvingOverlap = false;
    }

    void SnapPositionToGrid()
    {
        Vector2 currentPosition = transform.position;
        currentPosition.x = CELL_SIZE * (int)(currentPosition.x / CELL_SIZE);
        if (width % 2 == 0)
            currentPosition.x += transform.position.x < 0 ? -0.5f : 0.5f;
        currentPosition.y = CELL_SIZE * (int)(currentPosition.y / CELL_SIZE);
        if (height % 2 == 0)
            currentPosition.y += transform.position.y < 0 ? -0.5f : 0.5f;
        transform.position = currentPosition;
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

    public bool IsOverlapping (Room other)
    {
        Vector2[] otherPoints = new Vector2[]
        {
            other.bounds.min,
            other.bounds.max,
            new Vector2(other.bounds.xMin, other.bounds.yMax),
            new Vector2(other.bounds.xMax, other.bounds.yMin),
            other.bounds.center
        };

        bool isOverlapping = false;
        for (int i = 0; i < 5; i++)
        {
            if ((otherPoints[i].x > bounds.min.x && otherPoints[i].x < bounds.max.x &&
                otherPoints[i].y > bounds.min.y && otherPoints[i].y < bounds.max.y))
            {
                Debug.DrawLine(transform.position, otherPoints[i], Color.magenta, Time.fixedDeltaTime);
                isOverlapping = true;
                break;
            }
        }

        if (!isOverlapping)
        {
            if (otherPoints[0].x >= bounds.min.x && otherPoints[1].x <= bounds.max.x ||
                bounds.min.x >= otherPoints[0].x && bounds.max.x <= otherPoints[1].x)
            {
                float deltaY = Mathf.Abs(other.transform.position.y - transform.position.y);
                if (deltaY < (other.bounds.height + bounds.height) * CELL_SIZE / 2)
                {
                    Debug.DrawLine(transform.position, other.transform.position, Color.red, Time.fixedDeltaTime);
                    isOverlapping = true;
                }
            }
            else if (otherPoints[0].y >= bounds.min.y && otherPoints[1].y <= bounds.max.y ||
                bounds.min.y >= otherPoints[0].y && bounds.max.y <= otherPoints[1].y)
            {
                float deltaX = Mathf.Abs(other.transform.position.x - transform.position.x);
                if (deltaX < (other.bounds.width + bounds.width) * CELL_SIZE  / 2)
                {
                    Debug.DrawLine(transform.position, other.transform.position, Color.red, Time.fixedDeltaTime);
                    isOverlapping = true;
                }
            }

        }
        return isOverlapping;
    }

    public bool ResolveOverlap(Room other)
    {
        resolvingOverlap = true;
        Vector3 resolveVector = (transform.position - other.transform.position).normalized;
        if (resolveVector == Vector3.zero)
            resolveVector = Vector3.right;
        else
        {
            if (Mathf.Abs(resolveVector.x) >= 0.3f)
                resolveVector.x = Mathf.Sign(resolveVector.x);
            if (Mathf.Abs(resolveVector.y) >= 0.3f)
                resolveVector.y = Mathf.Sign(resolveVector.y);
        }

        transform.position = resolveVector * CELL_SIZE + transform.position;

        SnapPositionToGrid();
        UpdateBounds();

        return false;
    }


    void OnDrawGizmos()
    {
        if(!Application.isPlaying)
            UpdateBounds();

        Gizmos.color = Color.yellow;

        Gizmos.DrawWireCube(bounds.center, bounds.size);
    }
}
