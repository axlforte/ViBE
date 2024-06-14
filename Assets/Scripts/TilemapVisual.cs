using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilemapVisual : MonoBehaviour
{
    [System.Serializable]
    public struct TmapSpriteUV{
        public Tmap.TmapObj.TmapSprite TmapSprite;
        public Vector2Int uv00;
    }

    private struct UVCoords{
        public Vector2 uv00;
    }

    [SerializeField] public TmapSpriteUV[] TmapSpriteUVArray;
    private NewGrid<Tmap.TmapObj> grid;
    public Transform ParallaxTransform;
    private Mesh mesh;
    private Mesh[,] fullMesh;
    public int MeshSize, verticalMeshOffset, horizontalMeshOffset;
    private GameObject[,] visualMeshArray;
    public GameObject BasicMesh;
    public Vector2 updatePos;
    private MeshCollider mc;
    private bool updateMesh, didChunkUpdate;
    public bool GameplayMeshUpdate, chunkedMeshes, doNearbyChunks;
    private Dictionary<Tmap.TmapObj.TmapSprite, UVCoords> uvCoordsDictionary;
    private float texWidth, texHeight;

    private void Awake() {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        mc = GetComponent<MeshCollider>();

        Texture tex = GetComponent<MeshRenderer>().material.mainTexture;
        texWidth = tex.width; 
        texHeight = tex.height;

        uvCoordsDictionary = new Dictionary<Tmap.TmapObj.TmapSprite, UVCoords>();

        foreach(TmapSpriteUV TmapSpriteUV in TmapSpriteUVArray){
            uvCoordsDictionary[TmapSpriteUV.TmapSprite] = new UVCoords{
                uv00 = new Vector2(TmapSpriteUV.uv00.x / texWidth, TmapSpriteUV.uv00.y / texHeight),
            };
        }
    }

    public void SetGrid(Tmap Tmap, NewGrid<Tmap.TmapObj> grid)
    {
        this.grid = grid;
        UpdateHeatMapVisual();

        grid.OnGridObjectChanged += Grid_OnGridObjectChanged;
        Tmap.OnLoaded += Tmap_OnLoaded;
        fullMesh = new Mesh[grid.GetHeight() / MeshSize, grid.GetWidth() / MeshSize];
        visualMeshArray = new GameObject[grid.GetHeight() / MeshSize, grid.GetWidth() / MeshSize];
        for (int w = 0; w < grid.GetWidth() / MeshSize; w++)
        {
            for (int h = 0; h < grid.GetHeight() / MeshSize; h++)
            {
                visualMeshArray[h, w] = Instantiate(BasicMesh, new Vector3(w * MeshSize + horizontalMeshOffset, h * MeshSize + verticalMeshOffset, 0), Quaternion.identity);
            }
        }
    }

    private void Tmap_OnLoaded(System.Object sender, System.EventArgs e){
        updateMesh = true;
    }

    private void Grid_OnGridObjectChanged(System.Object sender, NewGrid<Tmap.TmapObj>.OnGridObjectChangedEventArgs e) {
        updateMesh = true;
    }

    private void LateUpdate() {
        if (updateMesh) {
            updateMesh = false;
            if (chunkedMeshes)
            {
                if (GameplayMeshUpdate)
                {
                    UpdateVisualInSectionsInGameplay();
                }
                else
                {
                    UpdateVisualInSections();
                }
            }
            else
            {
                if (GameplayMeshUpdate)
                {
                    //UpdateHeatMapVisualInGameplay();
                }
                else
                {
                    UpdateTheWholeGodDamnedThing();
                }
            }
        }
    }

    private void UpdateHeatMapVisual()
    {
        MeshUtils.CreateEmptyMeshArrays(grid.GetWidth() * grid.GetHeight(), out Vector3[] vertices, out Vector2[] uv, out int[] triangles);

        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                int index = x * grid.GetHeight() + y;
                Vector3 quadSize = new Vector3(1, 1) * grid.GetCellSize();

                Tmap.TmapObj gridObj = grid.GetGridObject(x, y);
                Tmap.TmapObj.TmapSprite tmapSprite = gridObj.GetTmapSprite();
                Vector2 gridUV00, gridUV11;
                if (tmapSprite == Tmap.TmapObj.TmapSprite.None)
                {
                    UVCoords uvcoords = uvCoordsDictionary[tmapSprite];
                    gridUV00 = new Vector2(uvcoords.uv00.x + 16, uvcoords.uv00.y + 16);
                    gridUV11 = uvcoords.uv00;
                }
                else
                {
                    UVCoords uvcoords = uvCoordsDictionary[tmapSprite];
                    gridUV00 = uvcoords.uv00;
                    gridUV11 = new Vector2(uvcoords.uv00.x + 16, uvcoords.uv00.y + 16);
                    MeshUtils.AddToMeshArrays(vertices, uv, triangles, index, grid.GetWorldPosition(x, y) + quadSize * .5f, 0f, quadSize, gridUV00, gridUV11);
                }
            }
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
    }

    /*private void UpdateHeatMapVisualInGameplay()
    {
        MeshUtils.CreateEmptyMeshArrays(grid.GetWidth() * grid.GetHeight(), out Vector3[] vertices, out Vector2[] uv, out int[] triangles);

        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                int index = x * grid.GetHeight() + y;
                Vector3 quadSize = new Vector3(1, 1) * grid.GetCellSize();

                Tmap.TmapObj gridObj = grid.GetGridObject(x, y);
                Tmap.TmapObj.TmapSprite tmapSprite = gridObj.GetTmapSprite();
                Vector2 gridUV00, gridUV11;
                if (gridObj.BlacklistedCollisionTiles())
                {
                    UVCoords uvcoords = uvCoordsDictionary[tmapSprite];
                    gridUV00 = uvcoords.uv11;
                    gridUV11 = uvcoords.uv00;
                }
                else
                {
                    UVCoords uvcoords = uvCoordsDictionary[tmapSprite];
                    gridUV00 = uvcoords.uv00;
                    gridUV11 = uvcoords.uv11;

                    MeshUtils.AddToMeshArrays(vertices, uv, triangles, index, grid.GetWorldPosition(x, y) + quadSize * .5f, 0f, quadSize, gridUV00, gridUV11);
                }

            }
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
        GetComponent<MeshFilter>().sharedMesh.RecalculateBounds();
    }*/

    private void UpdateVisualInSections()
    {
        didChunkUpdate = false;
        for (int w = 0; w < grid.GetWidth() / MeshSize; w++)
        {
            for (int h = 0; h < grid.GetHeight() / MeshSize; h++)
            {
                if ((int)((updatePos.x - horizontalMeshOffset) / MeshSize) == w && (int)((updatePos.y - verticalMeshOffset) / MeshSize) == h)
                {
                    fullMesh[h, w] = UpdateHeatMapVisual(w * MeshSize + horizontalMeshOffset, h * MeshSize + verticalMeshOffset, (w + 1) * MeshSize + horizontalMeshOffset, (h + 1) * MeshSize + verticalMeshOffset);
                    didChunkUpdate = true;
                }
                else if ((int)((updatePos.x - horizontalMeshOffset + 1) / MeshSize) == w && (int)((updatePos.y - verticalMeshOffset + 1) / MeshSize) == h && doNearbyChunks)
                {
                    fullMesh[h, w] = UpdateHeatMapVisual(w * MeshSize + horizontalMeshOffset, h * MeshSize + verticalMeshOffset, (w + 1) * MeshSize + horizontalMeshOffset, (h + 1) * MeshSize + verticalMeshOffset);
                    didChunkUpdate = true;
                }
                else if ((int)((updatePos.x - horizontalMeshOffset + 1) / MeshSize) == w && (int)((updatePos.y - verticalMeshOffset - 1) / MeshSize) == h && doNearbyChunks)
                {
                    fullMesh[h, w] = UpdateHeatMapVisual(w * MeshSize + horizontalMeshOffset, h * MeshSize + verticalMeshOffset, (w + 1) * MeshSize + horizontalMeshOffset, (h + 1) * MeshSize + verticalMeshOffset);
                    didChunkUpdate = true;
                }
                else if ((int)((updatePos.x - horizontalMeshOffset - 1) / MeshSize) == w && (int)((updatePos.y - verticalMeshOffset - 1) / MeshSize) == h && doNearbyChunks)
                {
                    fullMesh[h, w] = UpdateHeatMapVisual(w * MeshSize + horizontalMeshOffset, h * MeshSize + verticalMeshOffset, (w + 1) * MeshSize + horizontalMeshOffset, (h + 1) * MeshSize + verticalMeshOffset);
                    didChunkUpdate = true;
                }
                else if ((int)((updatePos.x - horizontalMeshOffset - 1) / MeshSize) == w && (int)((updatePos.y - verticalMeshOffset + 1) / MeshSize) == h && doNearbyChunks)
                {
                    fullMesh[h, w] = UpdateHeatMapVisual(w * MeshSize + horizontalMeshOffset, h * MeshSize + verticalMeshOffset, (w + 1) * MeshSize + horizontalMeshOffset, (h + 1) * MeshSize + verticalMeshOffset);
                    didChunkUpdate = true;
                }

                if(didChunkUpdate){

                    Destroy(visualMeshArray[h, w].GetComponent<MeshFilter>().mesh);
                    visualMeshArray[h, w].GetComponent<MeshFilter>().mesh = fullMesh[h, w];
                }
            }
        }
    }

    public void UpdateTheWholeGodDamnedThing(){
        for (int w = 0; w < grid.GetWidth() / MeshSize; w++)
        {
            for (int h = 0; h < grid.GetHeight() / MeshSize; h++)
            {
                fullMesh[h, w] = UpdateHeatMapVisual(w * MeshSize + horizontalMeshOffset, h * MeshSize + verticalMeshOffset, (w + 1) * MeshSize + horizontalMeshOffset, (h + 1) * MeshSize + verticalMeshOffset);
                visualMeshArray[h, w].GetComponent<MeshFilter>().mesh = fullMesh[h, w];
            }
        }
    }

    private void UpdateVisualInSectionsInGameplay()
    {
        for (int w = 0; w < grid.GetWidth() / MeshSize; w++)
        {
            for (int h = 0; h < grid.GetHeight() / MeshSize; h++)
            {
                fullMesh[h, w] = UpdateHeatMapVisualInGameplay(w * MeshSize + horizontalMeshOffset, h * MeshSize + verticalMeshOffset, (w + 1) * MeshSize + horizontalMeshOffset, (h + 1) * MeshSize + verticalMeshOffset);
                visualMeshArray[h, w].GetComponent<MeshFilter>().mesh = fullMesh[h, w];
            }
        }
    }

    private Mesh UpdateHeatMapVisual(int Xa, int Ya, int Xb, int Yb)
    {
        Mesh mes = new Mesh();
        MeshUtils.CreateEmptyMeshArrays(grid.GetWidth() * grid.GetHeight(), out Vector3[] vertices, out Vector2[] uv, out int[] triangles);

        for (int x = Xa; x < Xb; x++)
        {
            for (int y = Ya; y < Yb; y++)
            {
                int index = (x - Xa) * grid.GetHeight() + (y - Ya);
                Vector3 quadSize = new Vector3(1, 1) * grid.GetCellSize();

                Tmap.TmapObj gridObj = grid.GetGridObject(x, y);
                Tmap.TmapObj.TmapSprite tmapSprite = gridObj.GetTmapSprite();
                Vector2 gridUV00, gridUV11;
                if (tmapSprite == Tmap.TmapObj.TmapSprite.None)
                {
                    UVCoords uvcoords = uvCoordsDictionary[tmapSprite];
                    gridUV00 = new Vector2(uvcoords.uv00.x + 16, uvcoords.uv00.y + 16);
                    gridUV11 = uvcoords.uv00;
                }
                else
                {
                    UVCoords uvcoords = uvCoordsDictionary[tmapSprite];
                    gridUV00 = uvcoords.uv00;
                    gridUV11 = new Vector2(uvcoords.uv00.x + (16 / texWidth), uvcoords.uv00.y + (16 / texHeight));
                    MeshUtils.AddToMeshArrays(vertices, uv, triangles, index, grid.GetWorldPosition(x - Xa, y - Ya) + quadSize * .5f, 0f, quadSize, gridUV00, gridUV11);
                }
            }
        }

        mes.vertices = vertices;
        mes.uv = uv;
        mes.triangles = triangles;
        return mes;
    }

    private Mesh UpdateHeatMapVisualInGameplay(int Xa, int Ya, int Xb, int Yb)
    {
        Mesh mes = new Mesh();
        MeshUtils.CreateEmptyMeshArrays(grid.GetWidth() * grid.GetHeight(), out Vector3[] vertices, out Vector2[] uv, out int[] triangles);

        for (int x = Xa; x < Xb; x++)
        {
            for (int y = Ya; y < Yb; y++)
            {
                int index = (x - Xa) * grid.GetHeight() + (y - Ya); 
                Vector3 quadSize = new Vector3(1, 1) * grid.GetCellSize();

                Tmap.TmapObj gridObj = grid.GetGridObject(x, y);
                Tmap.TmapObj.TmapSprite tmapSprite = gridObj.GetTmapSprite();
                Vector2 gridUV00, gridUV11;
                if (gridObj.BlacklistedRenderTiles())
                {
                    UVCoords uvcoords = uvCoordsDictionary[tmapSprite];
                    gridUV00 = new Vector2(uvcoords.uv00.x + 16, uvcoords.uv00.y + 16);
                    gridUV11 = uvcoords.uv00;
                }
                else
                {
                    UVCoords uvcoords = uvCoordsDictionary[tmapSprite];
                    gridUV00 = uvcoords.uv00;
                    gridUV11 = new Vector2(uvcoords.uv00.x + (16 / texWidth), uvcoords.uv00.y + (16 / texHeight));
                    MeshUtils.AddToMeshArrays(vertices, uv, triangles, index, grid.GetWorldPosition(x - Xa, y - Ya) + quadSize * .5f, 0f, quadSize, gridUV00, gridUV11);
                }
            }
        }

        mes.vertices = vertices;
        mes.uv = uv;
        mes.triangles = triangles;
        return mes;
    }
}
