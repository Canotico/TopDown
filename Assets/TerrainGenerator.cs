using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class TerrainGenerator : MonoBehaviour
{
    Texture2D texture;

    const int HEIGHTMAP_RESOLUTION = 512;

    float[,] heightMap;
    public float noiseScale = 10f;
    public int riverSeed = 1337;
    public int riverWidth = 3;
    int previousRiverSeed;
    int previousRiverWidth;
    float previousNoiseScale;

	// Use this for initialization
	void Start ()
    {
        texture = new Texture2D(HEIGHTMAP_RESOLUTION, HEIGHTMAP_RESOLUTION);
        
        heightMap = new float[HEIGHTMAP_RESOLUTION, HEIGHTMAP_RESOLUTION];
        previousNoiseScale = 0f;
        previousRiverWidth = riverWidth;
        previousRiverSeed = riverSeed;
    }
	
	// Update is called once per frame
	void FixedUpdate ()
    {
		if(noiseScale != previousNoiseScale ||
            riverWidth != previousRiverWidth ||
            riverSeed != previousRiverSeed)
        {
            AddHeights();
            AddRiver();
            ApplyHeightmap();
            previousNoiseScale = noiseScale;
            previousRiverWidth = riverWidth;
            previousRiverSeed = riverSeed;
        }
	}

    void ResetHeightMap()
    {
        heightMap = new float[HEIGHTMAP_RESOLUTION, HEIGHTMAP_RESOLUTION];
    }

    void AddRiver()
    {
        Random.InitState(riverSeed);
        int[] start = new int[] { 0, Random.Range(0, HEIGHTMAP_RESOLUTION) };
        int[] end = new int[] { Random.Range(0, HEIGHTMAP_RESOLUTION), HEIGHTMAP_RESOLUTION - 1};

        int[] current = start;
        int halfWidth = riverWidth / 2;

        while (current[0] != end[0] && current[1] != end[1])
        {
            int dirX = (int)Mathf.Sign(end[0] - start[0]);
            int dirY = (int)Mathf.Sign(end[1] - start[1]);
            current[0] += dirX;
            current[1] += dirY;

            if (current[0] < 0 || current[0] > HEIGHTMAP_RESOLUTION)
                continue;
            if (current[1] < 0 || current[1] > HEIGHTMAP_RESOLUTION)
                continue;

            Debug.DrawRay(new Vector3(transform.position.x + current[0] / HEIGHTMAP_RESOLUTION, 0, transform.position.z + current[1] / HEIGHTMAP_RESOLUTION), Vector3.up * 1, Color.yellow, 10f);
            

            heightMap[current[0], current[1]] = 0;

            AvgNeighborsHeight(current[0], current[1]);

            for (int i = 0; i < halfWidth; i++)
            {
                AvgNeighborsHeight(current[0]+i, current[1]);
                AvgNeighborsHeight(current[0]-i, current[1]);
            }
        }

    }

    void AvgNeighborsHeight(int x, int y)
    {
        int kernelHalfSize = 2;
        float avg = 0;
        for (int i = Mathf.Max(x - kernelHalfSize, 0); i < Mathf.Min(x + kernelHalfSize, HEIGHTMAP_RESOLUTION); i++)
        {
            for (int j = Mathf.Max(y - kernelHalfSize, 0); j < Mathf.Min(y + kernelHalfSize, HEIGHTMAP_RESOLUTION); j++)
            {
                avg += heightMap[i, j] * 0.8f;
            }
        }

        avg /= kernelHalfSize * 2 + 1;

        for (int i = Mathf.Max(x - kernelHalfSize, 0); i < Mathf.Min(x + kernelHalfSize, HEIGHTMAP_RESOLUTION); i++)
        {
            for (int j = Mathf.Max(y - kernelHalfSize, 0); j < Mathf.Min(y + kernelHalfSize, HEIGHTMAP_RESOLUTION); j++)
            {
                heightMap[i, j] = avg;
            }
        }
    }

    void AddHeights()
    {
        float[] noise = new float[2];
        for (int i = 0; i < HEIGHTMAP_RESOLUTION; i++)
        {
            for (int j = 0; j < HEIGHTMAP_RESOLUTION; j++)
            {
                noise[0] = (float)i / HEIGHTMAP_RESOLUTION * noiseScale;
                noise[1] = (float)j / HEIGHTMAP_RESOLUTION * noiseScale;
                heightMap[i, j] = Mathf.PerlinNoise(noise[0], noise[1]);
            }
        }
    }

    void ApplyHeightmap()
    {
        for (int i = 0; i < HEIGHTMAP_RESOLUTION; i++)
        {
            for (int j = 0; j < HEIGHTMAP_RESOLUTION; j++)
            {
                float greyscale = heightMap[i, j];
                texture.SetPixel(i,j,new Color(greyscale, greyscale, greyscale));
            }
        }

        texture.Apply();

        GetComponent<Renderer>().material.SetTexture("_MainTex", texture);
    }
}
