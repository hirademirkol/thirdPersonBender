using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerraformingController : MonoBehaviour
{
    public Terrain Terrain;

    private TerrainData _terrainData;
    private Vector3 _terrainPosition;
    private Vector3 _terrainScale;
    
    struct coords
    {   
        public coords(int x, int y)
        {
            X = x;
            Y = y;
        }
        public coords(coords other)
        {
            X = other.X;
            Y = other.Y;
        }
        public static coords operator +(coords first, coords second)
        {
            return new coords(first.X + second.X, first.Y + second.Y);
        }

        public int X;
        public int Y;
    }

    void Start()
    {
        _terrainData = Terrain.terrainData;
        _terrainPosition = Terrain.transform.position;
        _terrainScale = _terrainData.heightmapScale;
    }

    public void ColumnAtMiddle(float height, int brushLength)
    {
        float[,] columnHeights = new float[brushLength, brushLength];
        //coords start = TransformCoordinate(-brushLength/2, -brushLength/2);
        coords start = TransformCoordinate(0, 0);
        for (int x = 0; x < brushLength; x++)
        {
            for (int y = 0; y < brushLength; y++)
            {
                columnHeights[x, y] = height;
            }
        }
        _terrainData.SetHeights(start.X-brushLength/2, start.Y-brushLength/2, columnHeights);
    }

    public void PullOffRock(Vector3 position)
    {   
        var radius = 1f;
        coords start = TransformCoordinate(position - new Vector3(radius, 0, radius));
        int diameterOnMap = TransformHorizontalLength(position.x + radius) - TransformHorizontalLength(position.x - radius);
        float[,] heights =  _terrainData.GetHeights(start.X, start.Y, diameterOnMap, diameterOnMap);
        _terrainData.SetHeights(start.X, start.Y, GetHeightsOnCircleOnMap(diameterOnMap, -1f, heights));
    }

    public void RaiseWall(Vector3 position, Vector3 normal)
    {
        var height = 3f;
        height /= _terrainScale.y;
        var width = 1f;
        var length = 4f;
        var angle = Vector3.Angle(normal, Vector3.right) * Mathf.Deg2Rad;
        angle = Mathf.PI/2 - Mathf.Abs((angle % Mathf.PI) - Mathf.PI/2);
        float brushHorizontal = length * Mathf.Cos(angle) + width * Mathf.Sin(angle);
        float brushVertical = length * Mathf.Sin(angle) + width * Mathf.Cos(angle);
        coords start = TransformCoordinate(position - new Vector3(brushVertical/2, 0f, brushHorizontal/2));
        var brush = new coords(TransformVerticalLength(brushVertical), TransformHorizontalLength(brushHorizontal));
        float[,] heights = _terrainData.GetHeights(start.X, start.Y, brush.X, brush.Y);

        for(int x = 0; x < brush.X; x++)
        {
            for(int y = 0; y < brush.Y; y++)
            {
                heights[y, x] += height;
            }
        }
            _terrainData.SetHeights(start.X, start.Y, heights);
    }

    private coords TransformCoordinate(float x, float z)
    {
        int x_terrain = Mathf.RoundToInt((x - _terrainPosition.x)/_terrainScale.x);
        int y_terrain = Mathf.RoundToInt((z - _terrainPosition.z)/_terrainScale.z);
        return new coords(x_terrain, y_terrain);
    }

    private coords TransformCoordinate(Vector3 position)
    {
        return TransformCoordinate(position.x, position.z);
    }

    private int TransformHorizontalLength(float length)
    {
        return Mathf.RoundToInt(length/_terrainScale.x);
    }

    private int TransformVerticalLength(float length)
    {
        return Mathf.RoundToInt(length/_terrainScale.z);
    }

    private float[,] GetHeightsOnCircleOnMap(int diameterOnMap, float height, float[,] heights)
    {   
        height /= _terrainScale.y;
        int radiusOnMap = diameterOnMap / 2;
        int rowLength = 0;

        if (diameterOnMap % 2 == 0)
        {
            for(int i = 1; i < radiusOnMap; i++)
            {
                rowLength = Mathf.FloorToInt(0.5f * Mathf.Sqrt(diameterOnMap*diameterOnMap - (2*i - 1)*(2*i - 1)));

                for(int j = radiusOnMap-rowLength; j < radiusOnMap; j++)
                {
                    heights[radiusOnMap-i+1,j] += height;
                    heights[radiusOnMap-i+1, diameterOnMap-j-1] += height;
                    heights[radiusOnMap+i, j] += height;
                    heights[radiusOnMap+i, diameterOnMap-j-1] += height;
                }
            }
        }
        else
        {
            rowLength = radiusOnMap;
            for (int j = radiusOnMap - rowLength; j < radiusOnMap; j++)
            {
                heights[radiusOnMap, j] += height;
                heights[radiusOnMap, diameterOnMap - j - 1] += height;
            }

            for(int i = 1; i < radiusOnMap; i++)
            {
                rowLength = Mathf.FloorToInt(0.5f * Mathf.Sqrt(diameterOnMap*diameterOnMap - 4 * i*i));
                for(int j = radiusOnMap-rowLength; j < radiusOnMap; j++)
                {
                    heights[radiusOnMap-i, j] += height;
                    heights[radiusOnMap-i, diameterOnMap-j-1] += height;
                    heights[radiusOnMap+i, j] += height;
                    heights[radiusOnMap+i, diameterOnMap-j-1] += height;
                }
                heights[radiusOnMap-i, radiusOnMap] += height;
                heights[radiusOnMap+i, radiusOnMap] += height;
            }
            heights[radiusOnMap, radiusOnMap] += height;
        }

        return heights;
    }
}