using UnityEngine;
using UnityEditor;

public class GenerateTerrain : ScriptableWizard
{
    //Data
    private Vector3[] vertices;

    //Terrain Options
    [Header("Terrain Options")]
    public int depth = 10; //height of bumps
    public int width = 256; //map height
    public int height = 256; //map width
    public float scale = 5f; //density of bumps
    public float offsetX = 100f;
    public float offsetY = 100f;
    public bool restoreDefaults = false;

    private int oldDepth = 10;
    private int oldWidth = 256;
    private int oldHeight = 256;
    private float oldScale = 5f;
    private float oldOffsetX = 100f;
    private float oldOffsetY = 100f;

    //Randomization options
    [Header("Randomization Options")]
    public bool randomizeDepth = false;
    public bool randomizeScale = false;
    public bool randomizeOffsetX = true;
    public bool randomizeOffsetY = true;

    [MenuItem("Custom Tools/Generate Terrain")]
    static void CreateWizard()
    {
        ScriptableWizard.DisplayWizard<GenerateTerrain>("Generate Terrain", "Apply", "Generate Random");
    }

    private void Update()
    {
        if (restoreDefaults)
        {
            restoreDefaults = false;
            depth = 10;
            width = 256;
            height = 256;
            scale = 5f;
            offsetX = 100f;
            offsetY = 100f;

            oldDepth = depth;
            oldWidth = width;
            oldHeight = height;
            oldScale = scale;
            oldOffsetX = offsetX;
            oldOffsetY = offsetY;
        }

        if (depth != oldDepth)
        {
            oldDepth = depth;
        }

        if (width != oldWidth)
        {
            oldWidth = width;
        }

        if (height != oldHeight)
        {
            oldHeight = height;
        }

        if (scale != oldScale)
        {
            oldScale = scale;
        }

        if (offsetX != oldOffsetX)
        {
            oldOffsetX = offsetX;
        }

        if (offsetY != oldOffsetY)
        {
            oldOffsetY = offsetY;
        }
    }

    void OnWizardOtherButton()
    {
        if (randomizeDepth)
        {
            depth = Random.Range(5, 25);
        }
        if (randomizeScale)
        {
            scale = Random.Range(5f, 25f);
        }
        if (randomizeOffsetX)
        {
            offsetX = Random.Range(0f, 9999f);
        }
        if (randomizeOffsetY)
        {
            offsetY = Random.Range(0f, 9999f);
        }
    }

    void OnWizardCreate()
    {
        GameObject gameObject = GameObject.FindGameObjectWithTag("Terrain");
        Terrain terrain = gameObject.GetComponent<Terrain>();
        terrain.terrainData = GenTerrain(terrain.terrainData);
    }

    TerrainData GenTerrain(TerrainData terrainData)
    {
        terrainData.heightmapResolution = width + 1;

        terrainData.size = new Vector3(width, depth, height);

        terrainData.SetHeights(0, 0, GenerateHeights());
        return terrainData;
    }
    float[,] GenerateHeights()
    {
        vertices = new Vector3[(width + 1) * (height + 1)];
        float[,] heights = new float[width, height];

        for (int i = 0, x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                heights[x, y] = CalculateHeight(x, y);
                vertices[i] = new Vector3(x, heights[x,y], y);
                i++;
            }
        }

        return heights;
    }

    float CalculateHeight(int x, int y)
    {
        float xCoord = (float)x / width * scale + offsetX;
        float yCoord = (float)y / height * scale + offsetY;

        return Mathf.PerlinNoise(xCoord, yCoord);
    }
}
