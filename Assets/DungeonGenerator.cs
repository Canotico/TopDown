using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    const int MAX_WIDTH = 30;
    const int MIN_WIDTH = 10;
    const int MAX_HEIGHT = 30;
    const int MIN_HEIGHT = 10;

    public int spawnRadius = 30;
    public int spawnCount = 40;

    public GameObject roomPrefab;

    public bool useSeed = false;
    public int seed = 1337;

    public Room[] rooms;

    private void Start()
    {

        if (useSeed)
            Random.InitState(seed);
        else
            seed = Random.seed;

        rooms = new Room[spawnCount];
        for (int i = 0; i < spawnCount; i++)
        {
            rooms[i] = Instantiate(roomPrefab, transform.position + (Vector3)Random.insideUnitCircle * spawnRadius, Quaternion.identity, transform).GetComponent<Room>();
            rooms[i].width = Random.Range(MIN_WIDTH, MAX_WIDTH);
            rooms[i].height = Random.Range(MIN_HEIGHT, MAX_HEIGHT);
        }
        StartCoroutine(ResolveOverlaps());
    }

    IEnumerator ResolveOverlaps()
    {
        yield return new WaitForEndOfFrame();
        bool overlapsResolved = false;
        while (!overlapsResolved)
        {
            overlapsResolved = true;
            for (int i = 0; i < rooms.Length; i++)
            {
                rooms[i].overlapping = false;
                for (int j = 0; j < rooms.Length; j++)
                {
                    if (rooms[i] == rooms[j]) continue;
                    if (!rooms[i].resolvingOverlap && rooms[i].IsOverlapping(rooms[j]))
                    {
                        overlapsResolved = false;
                        rooms[i].overlapping = true;
                        rooms[i].ResolveOverlap(rooms[j]);
                    }
                }
            }
            yield return new WaitForFixedUpdate();
        }
        Debug.Log("DONE");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;

        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}
