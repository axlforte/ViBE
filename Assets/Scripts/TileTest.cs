using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMonkey.Utils;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TileTest : MonoBehaviour
{
    [SerializeField] private TilemapVisual tilemapVisual;
    private Tmap tiles;
    private Tmap.TmapObj.TmapSprite tilemapSprite, ObjectTile;
    public PanelMinimizer ToolMin, SaveMin, DetailsMin;
    public TMP_Text Hud;
    public GameObject rectangleSelection, Cam, PlayerPrefab, CollisionPrefab, gridPrefab, BGPrefab, LevelEndPrefab, temp, LeftSlopeCollPrefab, RightSlopeCollPrefab;
    private GameObject player, tempGridObject;
    private GameObject[,] CollisionGrid, BGs;
    private GameObject[] Entities;
    public EditTool tool;
    public EditTool[] toolList;
    public int width, height;
    private string pos, toolName, tile;
    Vector3 mouseWorldPos, mouseScreenPos, NormMouseScreenPos, startPos, endPos;
    private bool Testing;
    public bool ProperGameplay;
    private RockmanControll rkmn;
    public int ObjectIndex, TileIndex, ToolIndex;
    public Tmap.TmapObj.TmapSprite[] possibleTiles, possibleObjects, fullList;

    private int UILayer;
    private bool overUI;
    public DetailsPanel dp;
    public int skinIndex, tileSize, toolIndex, detailIndex, BGIndex;

    public GameObject[] ObjectsToSpawn;
    private bool[] UsableTile;

    public TMP_InputField inputField;
    private float speed;

    public RectTransform MinMapCamPos;

    public Sprite[] BgImages;

    public enum EditTool
    {
        Default,
        Erase,
        Paint,
        Rectangle,
        Detail,
        Object,
        Spikes,
        Water,
        Background,
        Test
    }

    private void Start(){
        UILayer = LayerMask.NameToLayer("UI");

        tiles = new Tmap(width,height,1,Vector3.zero);
        CollisionGrid = new GameObject[height, width];
        tiles.SetTilemapVisual(tilemapVisual);
        tool = EditTool.Default;
        tilemapSprite = Tmap.TmapObj.TmapSprite.Ground;
        ObjectTile = Tmap.TmapObj.TmapSprite.RideroidG;
        toolName = tool.ToString();
        tile = tilemapSprite.ToString();
        Cam.transform.position = new Vector3(width / 4, height / 2, -10);

        skinIndex = PlayerPrefs.GetInt("skin");
        tileSize = 0;
        foreach (Tmap.TmapObj.TmapSprite pieceType in Enum.GetValues(typeof(Tmap.TmapObj.TmapSprite)))
        {
            tileSize++;
        }

        fullList = new Tmap.TmapObj.TmapSprite[tileSize];
        UsableTile = new bool[tileSize];

        tileSize = 0;
        foreach (Tmap.TmapObj.TmapSprite pieceType in Enum.GetValues(typeof(Tmap.TmapObj.TmapSprite)))
        {
            fullList[tileSize] = pieceType;
            tileSize++;
            ////Debug.Log(pieceType);
        }
        for (int e = 0; e < tilemapVisual.TmapSpriteUVArray.Length; e++)
        {
            for (int f = 0; f < fullList.Length; f++)
            {
                if (tilemapVisual.TmapSpriteUVArray[e].TmapSprite == fullList[f])
                {
                    UsableTile[e] = true;
                }
            }
        }

        MakeBackgrounds();

        dp.ttest = this;
        dp.BGs = BgImages;
        ChangeSliderSize();

        
    }

    private void Update()
    {
        if (Testing)
        {
            TestMode();
        } else 
        if (ProperGameplay)
        {
            Gameplay();
        } else 
        {
            EditMode();
        }

        if (Input.GetKey(KeyCode.M) && Testing)
        {
            Testing = !Testing;
            tilemapVisual.GameplayMeshUpdate = false;
            Destroy(player);
        }

        dp.tileIndex = TileIndex;
        dp.objectIndex = ObjectIndex;
        dp.OnImage = !(tool == EditTool.Object);
        dp.toolIndex = toolIndex;
    }

    private void FixedUpdate()
    {
        if (Testing)
        {
            FixedTestMode();
        } else
        {
            FixedEditMode();
        }
    }

    private void EditMode()
    {
        mouseWorldPos = UtilsClass.GetMouseWorldPosition();
        mouseScreenPos = Input.mousePosition;

        ////Debug.Log(IsPointerOverUIElement() ? "Over UI" : "Not over UI");
        overUI = IsPointerOverUIElement();

        if (mouseWorldPos.x < 2)
        {
            mouseWorldPos = new Vector3(2, mouseWorldPos.y, 0);
        }
        if (mouseWorldPos.y < 2)
        {
            mouseWorldPos = new Vector3(mouseWorldPos.x, 2, 0);
        }
        if (mouseWorldPos.x > width - 2.1f)
        {
            mouseWorldPos = new Vector3(width - 2.1f, mouseWorldPos.y, 0);
        }
        if (mouseWorldPos.y > height - 2.1f)
        {
            mouseWorldPos = new Vector3(mouseWorldPos.x, height - 2.1f, 0);
        }

        if (GetMousePressed())
        {

            pos = mouseWorldPos + "";
            if (tool == EditTool.Default && Input.GetMouseButtonDown(0))
            {
                tiles.SetTilemapSprite(mouseWorldPos, tilemapSprite, true);
                tilemapVisual.updatePos = new Vector2(mouseWorldPos.x, mouseWorldPos.y);
                RegenerateCollision();
            }
            else if (tool == EditTool.Paint)
            {
                tiles.SetTilemapSprite(mouseWorldPos, tilemapSprite, true);
                tilemapVisual.updatePos = new Vector2(mouseWorldPos.x, mouseWorldPos.y);
                RegenerateCollision();
            }
            else if (tool == EditTool.Detail && Input.GetMouseButtonDown(0))
            {
                tiles.SetTilemapSprite(mouseWorldPos, tilemapSprite, false);
                tilemapVisual.updatePos = new Vector2(mouseWorldPos.x, mouseWorldPos.y);
                RegenerateCollision();
            }
            else if (tool == EditTool.Background && Input.GetMouseButtonDown(0))
            {
                BGs[(int)((mouseWorldPos.x - 2) / 16), (int)((mouseWorldPos.y - 2) / 14)].GetComponent<SpriteRenderer>().sprite = BgImages[BGIndex];
                RegenerateCollision();
            }
            else if (tool == EditTool.Rectangle)
            {
                if (GetMouseDown())
                {
                    startPos = new Vector3((int)(mouseWorldPos.x + 0.5f), (int)(mouseWorldPos.y + 0.5f), 0);
                    tilemapVisual.chunkedMeshes = false;
                }
                else
                {
                    rectangleSelection.transform.position = startPos - ((startPos - endPos) / 2);
                    rectangleSelection.transform.localScale = startPos - endPos;
                    endPos = new Vector3((int)(mouseWorldPos.x + 0.5f), (int)(mouseWorldPos.y + 0.5f), 0);
                }
            }
            else if (tool == EditTool.Erase)
            {
                tiles.SetTilemapSprite(mouseWorldPos, Tmap.TmapObj.TmapSprite.None, true);
                tilemapVisual.updatePos = new Vector2(mouseWorldPos.x, mouseWorldPos.y);
                RegenerateCollision();
            }
            else if (tool == EditTool.Object) 
            {
                tiles.SetTilemapSprite(mouseWorldPos, ObjectTile, true);
                tilemapVisual.updatePos = new Vector2(mouseWorldPos.x, mouseWorldPos.y);
                RegenerateCollision();
            } 
            else if (tool == EditTool.Test)
            {
                player = Instantiate(PlayerPrefab, new Vector3(mouseWorldPos.x, mouseWorldPos.y, 0), Cam.transform.rotation);
                rkmn = player.GetComponent<RockmanControll>();
                rkmn.cam = Cam.transform;
                rkmn.x = mouseWorldPos.x;
                rkmn.y = mouseWorldPos.y;
                tilemapVisual.GameplayMeshUpdate = true;
                tiles.SetTilemapSprite(new Vector3(3,3,0), tilemapSprite, false);
                tilemapVisual.updatePos = new Vector2(mouseWorldPos.x, mouseWorldPos.y);
                Testing = true;
                if (!ToolMin.minimized)
                {
                    ToolMin.minimized = true;
                }

                if (!SaveMin.minimized)
                {
                    SaveMin.minimized = true;
                }

                if (!DetailsMin.minimized)
                {
                    DetailsMin.minimized = true;
                }
                AddEnemies();
            }
        }
        else
        {
            if (GetMouseUp() && tool == EditTool.Rectangle)
            {//get Bigelow to help with this shit if he has time
                int height = 1, width = 1, xOffset, yOffset;
                if (startPos.x < endPos.x)
                {
                    xOffset = (int)startPos.x;
                }
                else
                {
                    xOffset = (int)endPos.x;
                }

                if (startPos.y < endPos.y)
                {
                    yOffset = (int)startPos.y;
                }
                else
                {
                    yOffset = (int)endPos.y;
                }

                width = (int)Mathf.Abs(endPos.x - startPos.x);
                height = (int)Mathf.Abs(endPos.y - startPos.y);
                for (int w = 0; w < width; w++)
                {
                    for (int h = 0; h < height; h++)
                    {
                        tiles.SetTilemapSprite(new Vector3(w + xOffset, h + yOffset, 0), tilemapSprite, true);
                        //tilemapVisual.updatePos = new Vector2(w + xOffset, h + yOffset);
                        //RegenerateCollision();
                    }
                }
                tilemapVisual.UpdateTheWholeGodDamnedThing();
                RegenerateCollision();
                tilemapVisual.chunkedMeshes = true;
                //rectangleSelection.SetActive(false);
            }
            else
            {
                rectangleSelection.transform.position = Vector3.Lerp(rectangleSelection.transform.position, new Vector3((int)mouseWorldPos.x + 0.5f, (int)mouseWorldPos.y + 0.5f, 0), 0.25f);
                rectangleSelection.transform.localScale = Vector3.one;
            }
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            if (PlayerPrefs.GetInt("skin") == 1)
            {
                PlayerPrefs.SetInt("skin", 0);
            } else
            {
                PlayerPrefs.SetInt("skin", 1);
            }
            skinIndex = PlayerPrefs.GetInt("skin");
        }

        if (!ToolMin.minimized)
        {
            Hud.text = "Tool: " + toolName + "\nTile: " + tile;
        }
        else
        {
            Hud.text = toolName + ",  " + tile;
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            ShiftToolsForward();
            if (tool == EditTool.Default || tool == EditTool.Rectangle)
            {
                ShiftTilesBack();
                ShiftTilesForward();
                tilemapVisual.doNearbyChunks = true;
            }
            else if (tool == EditTool.Object)
            {
                ShiftObjectsBack();
                ShiftObjectsForward();
                tilemapVisual.doNearbyChunks = false;
            }
            else if (tool == EditTool.Background)
            {
                ShiftBGsBack();
                ShiftBGsForward();
                tilemapVisual.doNearbyChunks = false;
            }
            else if (tool == EditTool.Detail)
            {
                ShiftDetailTilesBack();
                ShiftDetailTilesForward();
                tilemapVisual.doNearbyChunks = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            if (tool == EditTool.Object)
            {
                ShiftObjectsForward();
            }
            else if (tool == EditTool.Detail)
            {
                ShiftDetailTilesForward();
            }
            else if(tool == EditTool.Background)
            {
                ShiftBGsForward();
            }
            else
            {
                ShiftTilesForward();
            }
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            Save();
            ////Debug.Log("saved!");
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            Load();
            ////Debug.Log("loaded!");
        }
    }

    private void FixedEditMode()
    {

        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) {
            speed = (195 / 256f);
        } else
        {
            speed = (39 / 256f);
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            Cam.transform.position = new Vector3(Cam.transform.position.x, Cam.transform.position.y - speed, -10);
        }

        if (Input.GetKey(KeyCode.UpArrow))
        {
            Cam.transform.position = new Vector3(Cam.transform.position.x, Cam.transform.position.y + speed, -10);
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            Cam.transform.position = new Vector3(Cam.transform.position.x - speed, Cam.transform.position.y, -10);
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            Cam.transform.position = new Vector3(Cam.transform.position.x + speed, Cam.transform.position.y, -10);
        }

        MinMapCamPos.offsetMin = new Vector2(((Cam.transform.position.x / width) * 200) - 2.5f, ((Cam.transform.position.y / height) * 88) - 2);
        MinMapCamPos.offsetMax = new Vector2(((Cam.transform.position.x / width) * 200) + 2.5f, ((Cam.transform.position.y / height) * 88) + 2);
    }

    private void TestMode()
    {
        if (Input.GetKey(KeyCode.L) && !ProperGameplay)
        {
            Destroy(player);
            Testing = false;
            tilemapVisual.GameplayMeshUpdate = true;
            Entities = new GameObject[height * width];
        }

        if (!ToolMin.minimized)
        {
            Hud.text = "Return to Editing mode (L) \nTesting";
        }
        else
        {
            Hud.text = "Return to editing (L)";
        }
    }

    private void FixedTestMode()
    {

    }

    private void RegenerateCollision()
    {
        int skip = 0;
        for (int h = 0; h < height; h++)
        {
            for (int w = 0; w < width; w++)
            {
                skip = 0;
                if (!tiles.grid.GetGridObject(w, h).BlacklistedCollisionTiles())
                {
                    if (CollisionGrid[h, w] == null)
                    {
                        CollisionGrid[h, w] = Instantiate(CollisionPrefab, new Vector3(w + 0.5f, h + 0.5f, 0), Cam.transform.rotation);
                    }
                    CheckForGreedableMesh(w, h, out skip);
                    w += skip;
                } 
                else 
                {
                    Destroy(CollisionGrid[h, w]);
                }
            }
        }
    }

    private void CheckForGreedableMesh(int x, int y, out int skip)
    {
        bool Meshing = true;
        skip = 0;
        int meshDistance = 0;
        if (tiles.grid.GetGridObject(x - 1, y).BlacklistedCollisionTiles())
        {
            ////Debug.Log("im in the middle!");
            skip = 0;
            return;
        }

        while (Meshing)
        {
            ////Debug.Log("Rendering the mesh");
            meshDistance++;
            if (tiles.grid.GetGridObject(x + meshDistance, y).BlacklistedCollisionTiles() || tiles.grid.GetGridObject(x + meshDistance, y).tmapSprite == Tmap.TmapObj.TmapSprite.None)
            {
                Meshing = false;
            }
        }

        if (meshDistance > 1)
        {
            Destroy(CollisionGrid[y, x]);
            CollisionGrid[y, x] = Instantiate(CollisionPrefab, new Vector3(x + (meshDistance / 2f), y + 0.5f, 0), Cam.transform.rotation);
            CollisionGrid[y, x].gameObject.transform.localScale = new Vector3(meshDistance, CollisionGrid[y, x].gameObject.transform.localScale.y, 1);
            for (int l = 0; l < meshDistance - 1; l++)
            {
                Destroy(CollisionGrid[y, x + l + 1]);
            }
            skip = meshDistance - 1;
        }
    }

    private void AddEnemies()
    {
        Entities = new GameObject[width*height];
        for (int h = 0; h < height; h++)
        {
            for (int w = 0; w < width; w++)
            {
                if (tiles.grid.GetGridObject(w, h).IsEnemy())
                {
                    if (tiles.grid.GetGridObject(w, h).tmapSprite == Tmap.TmapObj.TmapSprite.RideroidG)
                    {
                        Entities[h * w] = Instantiate(ObjectsToSpawn[0], new Vector3(w,h,0), Cam.transform.rotation);
                    } else if (tiles.grid.GetGridObject(w, h).tmapSprite == Tmap.TmapObj.TmapSprite.Ladder)
                    {
                        if(LadderLength(w,h + 1) > 1){ 
                        } else {
                            temp = Instantiate(CollisionPrefab, new Vector3(w,h,0), Cam.transform.rotation);
                            temp.name = "Ladder (Clone)";
                            temp.tag = "ladder";
                            temp.GetComponent<BoxCollider2D>().isTrigger = true;
                            if(LadderLength(w,h) > 1){
                                temp.gameObject.transform.position = new Vector3(w,h - (LadderLength(w,h) / 2),0);
                                temp.gameObject.transform.localScale = new Vector3(1,LadderLength(w,h),1);
                            }
                        }
                    } else if (tiles.grid.GetGridObject(w, h).tmapSprite == Tmap.TmapObj.TmapSprite.FlameMammothBoss)
                    {
                        Entities[h * w] = Instantiate(ObjectsToSpawn[1], new Vector3(w,h,0), Cam.transform.rotation);
                        Entities[h * w].GetComponent<FlameMammoth>().player = player;
                        Entities[h * w].GetComponent<FlameMammoth>().x = w * 16;
                        Entities[h * w].GetComponent<FlameMammoth>().y = h * 16;
                    } else if (tiles.grid.GetGridObject(w, h).tmapSprite == Tmap.TmapObj.TmapSprite.StormEagleBoss)
                    {
                        Entities[h * w] = Instantiate(ObjectsToSpawn[2], new Vector3(w,h,0), Cam.transform.rotation);
                        Entities[h * w].GetComponent<FlameMammoth>().player = player;
                        Entities[h * w].GetComponent<FlameMammoth>().x = w * 16;
                        Entities[h * w].GetComponent<FlameMammoth>().y = h * 16;
                    } else if (tiles.grid.GetGridObject(w, h).tmapSprite == Tmap.TmapObj.TmapSprite.GroundSlopeR1)
                    {
                        Entities[h * w] = Instantiate(RightSlopeCollPrefab, new Vector3(w + 1f,h,0), Cam.transform.rotation);
                    } else if (tiles.grid.GetGridObject(w, h).tmapSprite == Tmap.TmapObj.TmapSprite.GroundSlopeL1)
                    {
                        Entities[h * w] = Instantiate(LeftSlopeCollPrefab, new Vector3(w + 1f,h,0), Cam.transform.rotation);
                    } else if (tiles.grid.GetGridObject(w, h).tmapSprite == Tmap.TmapObj.TmapSprite.playerStart && ProperGameplay)
                    {
                        player = Instantiate(PlayerPrefab, new Vector3(mouseWorldPos.x, mouseWorldPos.y, 0), Cam.transform.rotation);
                        rkmn = player.GetComponent<RockmanControll>();
                        rkmn.cam = Cam.transform;
                        rkmn.x = w;
                        rkmn.y = h;
                    } else
                    {

                    }
                }
            }
        }
    }

    public int LadderLength(int x, int y){
        if(tiles.grid.GetGridObject(x, y).tmapSprite == Tmap.TmapObj.TmapSprite.Ladder){
            return LadderLength(x,y-1) + 1;
        }
        return 1;
    }

    public void Ping(int index){
        if (tool == EditTool.Default || tool == EditTool.Rectangle || tool == EditTool.Paint)
        {
            TileIndex = index;
            tilemapSprite = possibleTiles[TileIndex];
            dp.text.text = possibleTiles[TileIndex].ToString();
        }
        else if (tool == EditTool.Background)
        {
            BGIndex = index;
            dp.text.text = BgImages[BGIndex].ToString();
            dp.text.text = dp.text.text.Substring(0,BgImages[BGIndex].ToString().Length - 20);
        }
        else if (tool == EditTool.Detail)
        {
            detailIndex = index;
            tilemapSprite = fullList[detailIndex];
            dp.text.text = fullList[TileIndex].ToString();
        }
        else if (tool == EditTool.Object)
        {
            ObjectIndex = index;
            ObjectTile = possibleObjects[ObjectIndex];
            dp.text.text = possibleObjects[ObjectIndex].ToString();
        }
    }

    private void ChangeSliderSize(){
        if (tool == EditTool.Default || tool == EditTool.Rectangle || tool == EditTool.Paint)
        {
            dp.CreateButtons(possibleTiles.Length);
            for(int e = 0; e < possibleTiles.Length; e++){
                dp.Buttons[e].transform.GetChild(0).GetComponent<TMP_Text>().text = possibleTiles[e].ToString();
            }
        }
        else if (tool == EditTool.Erase)
        {
            dp.CreateButtons(possibleTiles.Length);
            for(int e = 0; e < possibleTiles.Length; e++){
                dp.Buttons[e].transform.GetChild(0).GetComponent<TMP_Text>().text = possibleTiles[e].ToString();
            }
        }
        else if (tool == EditTool.Background)
        {
            dp.CreateButtons(BgImages.Length);
            for(int e = 0; e < BgImages.Length; e++){
                dp.Buttons[e].transform.GetChild(0).GetComponent<TMP_Text>().text = BgImages[e].ToString();
                dp.Buttons[e].transform.GetChild(0).GetComponent<TMP_Text>().text = dp.Buttons[e].transform.GetChild(0).GetComponent<TMP_Text>().text.Substring(0,BgImages[e].ToString().Length - 20);
            }
        }
        else if (tool == EditTool.Detail)
        {
            dp.CreateButtons(fullList.Length);
            for(int e = 0; e < fullList.Length; e++){
                dp.Buttons[e].transform.GetChild(0).GetComponent<TMP_Text>().text = fullList[e].ToString();
            }
        }
        else if (tool == EditTool.Object)
        {
            dp.CreateButtons(possibleObjects.Length);
            for(int e = 0; e < possibleObjects.Length; e++){
                dp.Buttons[e].transform.GetChild(0).GetComponent<TMP_Text>().text = possibleObjects[e].ToString();
            }
        }
        else if (tool == EditTool.Test)
        {
            dp.CreateButtons(1);
            for(int e = 0; e < 1; e++){
                dp.Buttons[e].transform.GetChild(0).GetComponent<TMP_Text>().text = "Test";
            }
        }
        else
        {

        }
    }

    private void Gameplay(){
        Testing = true;
        ////Debug.Log(PlayerPrefs.GetString("Level To Load"));
        try
        {
            if (PlayerPrefs.GetString("Level File Path") == "None") {
                LoadSpecific(PlayerPrefs.GetString("Level To Load"));
            } else
            {
                LoadSpecific(PlayerPrefs.GetString("Level To Load"), PlayerPrefs.GetString("Level File Path"));
            }
        } catch (Exception e)
        {
            inputField.text = e + "\n" + PlayerPrefs.GetString("Level To Load") + "\n" + PlayerPrefs.GetString("Level File Path");
        }

        for (int h = 0; h < height; h++)
        {
            for (int w = 0; w < width; w++)
            {
                if (tiles.grid.GetGridObject(w, h).tmapSprite == Tmap.TmapObj.TmapSprite.playerStart)
                {
                    player = Instantiate(PlayerPrefab, new Vector3(mouseWorldPos.x, mouseWorldPos.y, 0), Cam.transform.rotation);
                    rkmn = player.GetComponent<RockmanControll>();
                    rkmn.cam = Cam.transform;
                    rkmn.x = w;
                    rkmn.y = h;
                    tilemapVisual.GameplayMeshUpdate = true;
                    tiles.SetTilemapSprite(new Vector3(3,3,0), tilemapSprite, false);
                }else if (tiles.grid.GetGridObject(w, h).tmapSprite == Tmap.TmapObj.TmapSprite.winPoint)
                {
                    Instantiate(LevelEndPrefab, new Vector3(w, h + 0.5f, 0), Cam.transform.rotation);
                    tilemapVisual.GameplayMeshUpdate = true;
                    tiles.SetTilemapSprite(new Vector3(3,3,0), tilemapSprite, false);
                }
            }
        }

        Testing = true;


        tilemapVisual.UpdateTheWholeGodDamnedThing();
    }

    private void CheckForBounds()
    {
        if (mouseWorldPos.x == 0)
        {

        }
    }

    public void ShiftToolsBack()
    {
        toolIndex--;
        if (toolIndex < 0)
        {
            toolIndex = toolList.Length - 1;
        }
        tool = toolList[toolIndex];
        ChangeSliderSize();
        toolName = tool.ToString();
    }

    public void ShiftBGsForward()
    {
        BGIndex++;
        if (BGIndex > BgImages.Length - 1)
        {
            BGIndex = 0;
        }
    }

    public void ShiftBGsBack()
    {
        BGIndex--;
        if (BGIndex < 0)
        {
            BGIndex = BgImages.Length - 1;
        }
    }

    public void ShiftToolsForward()
    {
        toolIndex++;
        if (toolIndex > toolList.Length - 1)
        {
            toolIndex = 0;
        }
        tool = toolList[toolIndex];
        ChangeSliderSize();
        toolName = tool.ToString();
    }

    public void ShiftTilesBack()
    {
        TileIndex--;
        if (TileIndex < 0)
        {
            TileIndex = possibleTiles.Length - 1;
        }
        tilemapSprite = possibleTiles[TileIndex];
        tile = tilemapSprite.ToString();
        CleanUpTileName();
    }

    public void ShiftTilesForward()
    {
        TileIndex++;
        if (TileIndex > possibleTiles.Length - 1)
        {
            TileIndex = 0;
        }
        tilemapSprite = possibleTiles[TileIndex];
        tile = tilemapSprite.ToString();
        CleanUpTileName();
    }

    public void ShiftDetailTilesBack()
    {
        detailIndex--;
        if (detailIndex < 0)
        {
            detailIndex = fullList.Length - 1;
        }
        tilemapSprite = fullList[detailIndex];
        tile = tilemapSprite.ToString();
        if (!UsableTile[detailIndex])
        {
            ShiftDetailTilesBack();
        }
        CleanUpTileName();
    }

    public void ShiftDetailTilesForward()
    {
        detailIndex++;
        if (detailIndex > fullList.Length - 1)
        {
            detailIndex = 0;
        }
        tilemapSprite = fullList[detailIndex];
        tile = tilemapSprite.ToString();
        if (!UsableTile[detailIndex])
        {
            ShiftDetailTilesForward();
        }
        CleanUpTileName();
    }

    public void ShiftObjectsForward()
    {
        ObjectIndex++;
        if (ObjectIndex > possibleObjects.Length - 1)
        {
            ObjectIndex = 0;
        }
        ObjectTile = possibleObjects[ObjectIndex];
        tile = tilemapSprite.ToString();
    }

    public void ShiftObjectsBack()
    {
        ObjectIndex--;
        if (ObjectIndex < 0)
        {
            ObjectIndex = possibleObjects.Length - 1;
        }
        ObjectTile = possibleObjects[ObjectIndex];
        tile = tilemapSprite.ToString();
    }

    private void CleanUpTileName(){
        if(tile.Equals("SigBR")){
            tile = "Sigma";
        } else if(tile.Equals("DirtCTR")){
            tile = "Dirt";
        } else if(tile.Equals("CyberATR")){
            tile = "Cyber";
        } else if(tile.Equals("SigFloorL")){
            tile = "Sigma Floor";
        } else{
            //lol
        }
    }

    public bool IsPointerOverUIElement()
    {
        return IsPointerOverUIElement(GetEventSystemRaycastResults());
    }

    private bool IsPointerOverUIElement(List<RaycastResult> eventSystemRaysastResults)
    {
        for (int index = 0; index < eventSystemRaysastResults.Count; index++)
        {
            RaycastResult curRaysastResult = eventSystemRaysastResults[index];
            if (curRaysastResult.gameObject.layer == UILayer)
                return true;
        }
        return false;
    }

    static List<RaycastResult> GetEventSystemRaycastResults()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> raysastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raysastResults);
        return raysastResults;
    }

    private void LoadGridLines()
    {
        for (int e = 0; e < width / 16; e++)
        {
            tempGridObject = Instantiate(gridPrefab, new Vector3((e * 16) + 2, (height / 2) + 2, 10), Cam.transform.rotation);
            tempGridObject.transform.localScale = new Vector3(0.1f, height, 1);
        }

        for (int f = 0; f < height / 11; f++)
        {
            tempGridObject = Instantiate(gridPrefab, new Vector3((width / 2) + 2, (f * 14) + 2, 10), Cam.transform.rotation);
            tempGridObject.transform.localScale = new Vector3(width, 0.1f, 1);
        }
    }

    private void MakeBackgrounds()
    {
        BGs = new GameObject[(width / 16), (height / 16)];
        for (int e = 0; e < (width / 16); e++)
        {
            for (int f = 0; f < (height / 16); f++)
            {
                BGs[e, f] = Instantiate(BGPrefab, new Vector3((e * 16) + 10, (f * 14) + 9, 10), Cam.transform.rotation);
            }
        }
    }

    private bool GetMouseDown()
    {
        if (overUI) {
            return false;
        } else {
            return Input.GetMouseButtonDown(0);
        }
    }

    private bool GetMousePressed()
    {
        if(overUI) {
            return false;
        } else
        {
            return Input.GetMouseButton(0);
        }
    }

    private bool GetMouseUp()
    {
        if (overUI)
        {
            return false;
        }
        else
        {
            return Input.GetMouseButtonUp(0);
        }
    }

    public void Save(){
        for(int f = 0; f < BGs.Length; f++){
            try{
                tiles.grid.GetGridObject(f, 0).Arg1 = GetWhichBgIndexItWas(BGs[f / 16, f % 16].GetComponent<SpriteRenderer>().sprite);
                ////Debug.Log(tiles.grid.GetGridObject(f, 0).Arg1);
            } catch (Exception e) {
                //Debug.LogError(e + "\n The index was " + f);
            }
        }
        tiles.Save(inputField.text);
    }

    public void Load()
    {
        tiles.Load();
        
        tilemapVisual.UpdateTheWholeGodDamnedThing();

        LoadBgs();
        RegenerateCollision();
    }

    public void LoadSpecific()
    {
        tiles.LoadSpecific(inputField.text);
        //Debug.Log(inputField.text);
        tilemapVisual.UpdateTheWholeGodDamnedThing();

        LoadBgs();
        RegenerateCollision();
    }

    public void LoadSpecific(string level)
    {
        //Debug.Log(level);
        tiles.LoadSpecific(level);

        tilemapVisual.UpdateTheWholeGodDamnedThing();

        LoadBgs();
        RegenerateCollision();
    }

    public void LoadSpecific(string level, string dir)
    {
        ////Debug.Log(level + " " + dir);
        tiles.LoadSpecific(level, dir);

        tilemapVisual.UpdateTheWholeGodDamnedThing();

        LoadBgs();
        RegenerateCollision();
    }

    private void LoadBgs(){
        for(int f = 0; f < BGs.Length; f++){
            //Debug.Log(BGs.Length);
            try {
                BGs[f / 16, f % 16].GetComponent<SpriteRenderer>().sprite = BgImages[tiles.grid.GetGridObject(f, 0).Arg1];
                ////Debug.Log(tiles.grid.GetGridObject(f, 0).Arg1);
            } catch (Exception e) {
                //Debug.LogError(e + "\n The index was " + f);
            }
        }
    }

    private int GetWhichBgIndexItWas(Sprite bg){
        for(int a = 0; a < BgImages.Length; a++){
            if(BgImages[a] == bg){
                return a;
            }
        }
        return 0;
    }
}
