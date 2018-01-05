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

    public Room[] rooms;

    private void Start()
    {
        rooms = new Room[spawnCount];
        Random.InitState((int)(Time.realtimeSinceStartup * 100f));
        for (int i = 0; i < spawnCount; i++)
        {
            rooms[i] = Instantiate(roomPrefab, transform.position + (Vector3)Random.insideUnitCircle, Quaternion.identity, transform).GetComponent<Room>();
            rooms[i].width = Random.Range(MIN_WIDTH, MAX_WIDTH);
            rooms[i].height = Random.Range(MIN_HEIGHT, MAX_HEIGHT);
        }
        StartCoroutine(ResolveOverlaps());
    }

    IEnumerator ResolveOverlaps()
    {
        bool overlapsResolved = false;
        while (!overlapsResolved)
        {
            //overlapsResolved = true;
            for (int i = 0; i < rooms.Length; i++)
            {
                for (int j = 0; j < rooms.Length; j++)
                {
                    if (rooms[i] == rooms[j]) continue;
                    rooms[i].ResolveOverlap(rooms[j]);
                }
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;

        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}
