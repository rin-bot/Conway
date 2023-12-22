using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class GridController : MonoBehaviour
{
    // Start is called before the first frame update
    public Material activeMaterial;
    public Material inactiveMaterial;
    public Material gridMaterial;
    private List<Vector3> activeTiles = new List<Vector3>();
    private List<Vector3> mouseSelects = new List<Vector3>();
    private bool spaceBar = false;
    private float timer = 0.0f;
    public Texture2D image;

    // Camera Stuff
    private Camera cam;
    Vector3 screenTR;
    Vector3 screenBL;

    // Used Meshes

    Mesh Verticalmesh;
    Mesh Horizontalmesh;
    Mesh square;


    void Start()
    {
        Verticalmesh = ShapeMesh.RectangleMesh(0.05f, 10000f);
        Horizontalmesh = ShapeMesh.RectangleMesh(10000f, 0.05f);
        square = ShapeMesh.RectangleMesh(1f, 1f);
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {

        Tile();
        GenGrid();
        if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
        {
            MakeActive();
        }
        timer += Time.deltaTime;


        if (Input.GetKeyDown("space"))
        {
            print("space press");
            spaceBar = !spaceBar;
        }

        if (spaceBar && timer >= 0.4f)
        {
            timer = 0;
            NextGen();
        }

        if (Input.GetKeyDown("o"))
        {
            var tiles = GetPositions();
            cam.transform.position = new Vector3(tiles[0].x, tiles[0].y, -10);
            activeTiles = tiles;
        }
    }

    void NextGen()
    {
        var watch = System.Diagnostics.Stopwatch.StartNew();

        List<Vector3> saveActives = new List<Vector3>(activeTiles);

        // For Tiles Off Screen;
        List<Vector3> allInactive = new List<Vector3>();

        for (int i = 0; i < saveActives.Count; i++)
        {
            var currTile = saveActives[i];
            for (int j = -1; j <= 1; j++)
            {
                for (int k = -1; k <= 1; k++)
                {
                    var pos = new Vector3(currTile.x + j, currTile.y + k, currTile.z);
                    if (saveActives.Contains(pos)) continue;
                    if (!allInactive.Contains(pos)) allInactive.Add(pos);
                }
            }
        }




        // Rule 1: Any live cell with fewer than two live neighbours dies, as if by underpopulation.
        // Rule 2: Any live cell with two or three live neighbours lives on to the next generation.
        // Rule 3: Any live cell with more than three live neighbours dies, as if by overpopulation.

        activeTiles.RemoveAll(tile => UnderOrOverPopulated(tile));


        // Rule 4: Any dead cell with exactly three live neighbours becomes a live cell, as if by reproduction.

        var newActives = allInactive.Where(tile => ActiveNeighbours(tile) == 3).ToList();
        activeTiles.AddRange(newActives);



        int ActiveNeighbours(Vector3 tile)
        {
            List<Vector3> activeNeighbours = new List<Vector3>();
            for (int j = -1; j <= 1; j++)
            {
                for (int k = -1; k <= 1; k++)
                {
                    var pos = new Vector3(tile.x + j, tile.y + k, tile.z);
                    if (saveActives.Contains(pos) && pos != tile) activeNeighbours.Add(pos);
                }
            }
            return activeNeighbours.Count();
        }

        watch.Stop();
        var elapsedMs = watch.ElapsedMilliseconds;
        print(elapsedMs);

        bool IsNeighbour(Vector3 house1, Vector3 house2)
        {
            float xDiffSquared = Mathf.Pow(house1.x - house2.x, 2);
            float yDiffSquared = Mathf.Pow(house1.y - house2.y, 2);

            if (xDiffSquared + yDiffSquared <= 2) return true;  
            return false;
        }

        bool UnderOrOverPopulated(Vector3 tile)
        {
            var activeNeighbours = ActiveNeighbours(tile);
            if (activeNeighbours < 2 || activeNeighbours > 3) return true;
            return false;
        }

        
       
    }

    bool isOnScreen(Vector3 tile)
    {
        screenTR = cam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
        screenBL = cam.ScreenToWorldPoint(new Vector3(0, 0, 0));

        if (tile.x > screenTR.x || tile.x < screenBL.x || tile.y > screenTR.y || tile.y < screenBL.y) return false;
        return true;
    }


    void MakeActive()
    {
        RenderParams rp = new RenderParams(activeMaterial);
        Matrix4x4 position;
        Mesh square = ShapeMesh.RectangleMesh(1f, 1f);

        screenTR = cam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
        screenBL = cam.ScreenToWorldPoint(new Vector3(0, 0, 0));
        Vector3 pos = cam.ScreenToWorldPoint(Input.mousePosition);
        pos.x = Mathf.Floor(pos.x) + 0.5f;
        pos.y = Mathf.Floor(pos.y) + 0.5f;
        pos.z = 0;
        if (activeTiles.Contains(pos))
        {
            if (Input.GetMouseButton(1))
            {
                activeTiles.RemoveAll(tile => tile == pos);
            }
            return;
        }
        if (Input.GetMouseButton(1)) return;

        position = Matrix4x4.Translate(pos);
        activeTiles.Add(pos);
    }



    void GenGrid()
    {
        RenderParams rp = new RenderParams(gridMaterial);
        List<Matrix4x4> positions = new List<Matrix4x4>();



        screenTR = cam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
        screenBL = cam.ScreenToWorldPoint(new Vector3(0, 0, 0));


        for (int i = -10; i <= Mathf.Ceil(screenTR.x - screenBL.x) + 10; i++)
        {
            positions.Add(Matrix4x4.Translate(new Vector3(Mathf.Floor(screenBL.x) + i, 0, -1)));
        }
        if (positions.Count > 0) Graphics.RenderMeshInstanced(rp, Verticalmesh, 0, positions.ToArray());

        positions = new List<Matrix4x4>();
        for (int i = -10; i <= Mathf.Ceil(screenTR.y - screenBL.y) + 10; i++)
        {
            positions.Add(Matrix4x4.Translate(new Vector3(0, Mathf.Floor(screenBL.y) + i, -1)));
        }

        if (positions.Count > 0) Graphics.RenderMeshInstanced(rp, Horizontalmesh, 0, positions.ToArray());

    }

    void Tile()
    {
        RenderParams activeRP = new RenderParams(activeMaterial);


        List<Matrix4x4> activePositions = new List<Matrix4x4>();


        Vector3 screenTR = cam.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, 0));
        Vector3 screenBL = cam.ScreenToWorldPoint(new Vector3(0, 0, 0));

        foreach (Vector3 tile in activeTiles)
        {
            activePositions.Add(Matrix4x4.Translate(tile));
        }

        if (activePositions.Count > 0)
        {
            Graphics.RenderMeshInstanced(activeRP, square, 0, activePositions.ToArray());
        }

    }


    public List<Vector3> GetPositions()
    {
        image = MakeGrey(image);
        Color[] pix = image.GetPixels();
        List<Vector3> positions = new List<Vector3>();
        for (int i = 0; i < image.width; i++)
        {
            for (int j = 0; j < image.height; j++)
            {
                Color col = image.GetPixel(i, j);
                if (col == Color.black)
                {
                    positions.Add(new Vector3(i + 0.5f, j + 0.5f, 0));
                }
            }
        }

        return positions;
    }
    private Texture2D MakeGrey(Texture2D tex)
    {
        var texColours = tex.GetPixels();
        for (int i = 0; i < texColours.Length; i++)
        {
            var greyValue = texColours[i].grayscale;
            greyValue = greyValue >= 0.5f ? 1 : 0;
            texColours[i] = new Color(greyValue, greyValue, greyValue, texColours[i].a);
        }
        tex.SetPixels(texColours);
        return tex;
    }
}

