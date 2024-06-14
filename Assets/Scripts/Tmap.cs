using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tmap
{
    public event EventHandler OnLoaded;
    public bool TileRulesApply;
    public NewGrid<TmapObj> grid;

    public Tmap(int width, int height, float cellSize, Vector3 originPosition) {
        grid = new NewGrid<TmapObj>(width, height, cellSize, originPosition, (NewGrid<TmapObj> g, int x, int y) => new TmapObj(g, x, y));
    }

    public void SetTilemapSprite(Vector3 worldPosition, TmapObj.TmapSprite TmapSprite, bool tileRulesApply) {
        TmapObj TmapObj = grid.GetGridObject(worldPosition);
        if (TmapObj != null) {
            TmapObj.SetTmapSprite(TmapSprite, tileRulesApply);
        }
    }

    public void SetTilemapVisual(TilemapVisual tilemapVisual){
        tilemapVisual.SetGrid(this, grid);
    }

    public void Save()
    {
        List<TmapObj.SaveObj> TmapObjSaveList = new List<TmapObj.SaveObj>();
        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                TmapObj TmapObj = grid.GetGridObject(x, y);
                TmapObjSaveList.Add(TmapObj.Save());
            }
        }

        SaveObj saveObj = new SaveObj { TmapObjSaveArray = TmapObjSaveList.ToArray() };

        SaveSystem.SaveObject(saveObj);
    }

    public void Save(string filename)
    {
        List<TmapObj.SaveObj> TmapObjSaveList = new List<TmapObj.SaveObj>();
        for (int x = 0; x < grid.GetWidth(); x++)
        {
            for (int y = 0; y < grid.GetHeight(); y++)
            {
                TmapObj TmapObj = grid.GetGridObject(x, y);
                TmapObjSaveList.Add(TmapObj.Save());
            }
        }

        SaveObj saveObj = new SaveObj { TmapObjSaveArray = TmapObjSaveList.ToArray() };

        SaveSystem.SaveObject(filename, saveObj, false);
    }

    public void Load()
    {
        SaveObj saveobj = SaveSystem.LoadMostRecentObject<SaveObj>();

        foreach (TmapObj.SaveObj TmapObjSaveObj in saveobj.TmapObjSaveArray)
        {
            TmapObj TmapObj = grid.GetGridObject(TmapObjSaveObj.x, TmapObjSaveObj.y);
            TmapObj.Load(TmapObjSaveObj);
        }
        OnLoaded?.Invoke(this, EventArgs.Empty);
    }

    public void LoadSpecific(string file)
    {
        SaveObj saveobj = SaveSystem.LoadObject<SaveObj>(file);

        foreach (TmapObj.SaveObj TmapObjSaveObj in saveobj.TmapObjSaveArray)
        {
            TmapObj TmapObj = grid.GetGridObject(TmapObjSaveObj.x, TmapObjSaveObj.y);
            TmapObj.Load(TmapObjSaveObj);
        }
        OnLoaded?.Invoke(this, EventArgs.Empty);
    }

    public void LoadSpecific(string file, string dir)
    {
        SaveObj saveobj = SaveSystem.LoadObject<SaveObj>(file, dir);

        foreach (TmapObj.SaveObj TmapObjSaveObj in saveobj.TmapObjSaveArray)
        {
            TmapObj TmapObj = grid.GetGridObject(TmapObjSaveObj.x, TmapObjSaveObj.y);
            TmapObj.Load(TmapObjSaveObj);
        }
        OnLoaded?.Invoke(this, EventArgs.Empty);
    }

    public void LoadFinalLevel(){
        SaveObj saveobj = SaveSystem.LoadMostRecentObject<SaveObj>();

        foreach(TmapObj.SaveObj TmapObjSaveObj in saveobj.TmapObjSaveArray){
            TmapObj TmapObj = grid.GetGridObject(TmapObjSaveObj.x,TmapObjSaveObj.y);
            if(TmapObj.GetTmapSprite() == Tmap.TmapObj.TmapSprite.None){
                TmapObj.SetTmapSprite(TmapObj.TmapSprite.Null, false);
            }
            TmapObj.Load(TmapObjSaveObj);
        }
        OnLoaded?.Invoke(this, EventArgs.Empty);
    }

    public class SaveObj{
        public TmapObj.SaveObj[] TmapObjSaveArray;
    }

    public class TmapObj {
        // t = top, l = left, r = right, d = down, tr = top right
        // m = middle, o = outer
        // adding a tile in the middle of the list will fuck the ones later in the list up. so DONT DO IT!
        public enum TmapSprite {
            None,
            Null,
            Ground,
            Pipe,
            Ice,
            GroundT,GroundB,GroundL,GroundR,GroundTR,GroundBR,GroundTL,GroundBL,GroundTB,GroundRL,
            Tower,TowerTR,TowerTL,TowerL,TowerR,TowerT,
            PipeTR,PipeTL,PipeT,PipeB,
            DirtCTR,DirtCTL,DirtCBR,DirtCBL,DirtTR,DirtTL,DirtOTR,DirtOTL,DirtOBR,DirtOBL,DirtRT,DirtRB,DirtLT,DirtLB,DirtBL,DirtBR,
            DigLabour,
            Spiky,
            RideroidG,
            ChillGunner,
            Scriver,
            SmallHealth,LargeHealth,
            SmallEnergy,LargeEnergy,
            ExtraLife,
            HeartTank,
            ArmorStation,
            Ladder,
            Sand,SandT,SandL,SandR,SandB,SandTR,SandTL,SandBR,SandBL,
            CyberATR,CyberATL,CyberABR,CyberABL,CyberBTR,CyberBTL,CyberBBL,CyberBBR,
            GirderOR,GirderOL,GirderML,GirderMR,
            ScaffOL,ScaffML,ScaffMR,ScaffOR,
            Beam,BeamDetail,
            SigBR,SigBL,SigML,SigMR,SigTL,SigTR,
            SigFloorL,SigFloorM,SigFloorR,
            playerStart,
            winPoint,
            Mammoth, MammothT, MammothB, MammothR, MammothL, MammothTR, MammothTL, MammothBL, MammothBR, MammothR2, MammothL2,
            Seahorse, SeahorseL, SeahorseL2,
            Handlebar, ScaffHandle, GuardRail, GuardRailL, GuardRailR, Fencing, FencingR, FencingL, 
            MamWall, MamWallL, MamWallR, MamWallT, MamWallTR, MamWallTL,
            MamCornerTR, MamCornerTL, MamCornerBR, MamCornerBL,
            WeeWoo, WeeWooL, WeeWooR, WeeWoo2,
            BreakableBlock,
            Blizzard,BlizzardT, BlizzardT2, BlizzardB, BlizzardB2, BlizzardL, BlizzardR, BlizzardBR, BlizzardBL,
            Acid,AcidT, AcidB, AcidL, AcidR, AcidTL, AcidTR, AcidBL, AcidBR,
            Centipede, CentipedeT, CentipedeB, L, CentipedeR, CentipedeTL, CentipedeTR, CentipedeBL, CentipedeBR,
            Buffalo, BuffaloL, BuffaloR, BuffaloT, BuffaloTL, BuffaloTR, BuffaloDetail, BuffaloDetail2, BuffaloDetail3, BuffaloDetail4,
            BuffaloPipe, BuffaloPipe2, BuffaloPipeTop,
            SafetyTL, SafetyTR, SafetyBL, SafetyBR,
            FlameMammothBoss,StormEagleBoss,
            GroundSlopeR1,GroundSlopeR2, GroundSlopeL1, GroundSlopeL2,
        }

        private NewGrid<Tmap.TmapObj> grid;
        private int x, y;
        public int Arg1, Arg2;
        public TmapSprite tmapSprite;

        public TmapObj(NewGrid<TmapObj> grid, int x, int y) {
            this.grid = grid;
            this.x = x;
            this.y = y;
        }

        public void SetTmapSprite(TmapSprite tmapSprite, bool tileRulesApply)
        {
            this.tmapSprite = tmapSprite;
            //Debug.Log(grid);
            if (tileRulesApply) {
                TileRules();
                grid.GetGridObject(x, y + 1).TileRules();
                grid.TriggerGridObjectChanged(x, y + 1);
                grid.GetGridObject(x, y - 1).TileRules();
                grid.TriggerGridObjectChanged(x, y - 1);
                grid.GetGridObject(x + 1, y).TileRules();
                grid.TriggerGridObjectChanged(x + 1, y);
                grid.GetGridObject(x - 1, y).TileRules();
                grid.TriggerGridObjectChanged(x - 1, y);
                grid.GetGridObject(x + 2, y).TileRules();
                grid.TriggerGridObjectChanged(x + 2, y);
                grid.GetGridObject(x - 2, y).TileRules();
                grid.TriggerGridObjectChanged(x - 2, y);
            }
            grid.TriggerGridObjectChanged(x, y);
        }

        public TmapSprite GetTmapSprite()
        {
            return tmapSprite;
        }

        public override string ToString() {
            return tmapSprite.ToString();
        }

        [System.Serializable]
        public class SaveObj {
            public TmapSprite tmapSprite;
            public int x, y, Arg1, Arg2;
        }


        public SaveObj Save() {
            return new SaveObj {
                tmapSprite = tmapSprite,
                x = x,
                y = y,
                Arg1 = Arg1,
                Arg2 = Arg2,
            };
        }

        public void Load(SaveObj saveObj) {
            tmapSprite = saveObj.tmapSprite;
            Arg1 = saveObj.Arg1;
            Arg2 = saveObj.Arg2;
        }

        private void TileRules()
        {
            if (IsGround())
            {
                GenericTileRules(TmapSprite.Ground, TmapSprite.GroundT, TmapSprite.GroundB, TmapSprite.GroundL, TmapSprite.GroundR, TmapSprite.GroundTR, TmapSprite.GroundTL, TmapSprite.GroundBR, TmapSprite.GroundBL);
            }
            else if (IsTower())
            {
                TowerTileRules(TmapSprite.Tower, TmapSprite.TowerT, TmapSprite.TowerL, TmapSprite.TowerR, TmapSprite.TowerTR, TmapSprite.TowerTL);
            }
            else if (IsSand())
            {
                GenericTileRules(TmapSprite.Sand, TmapSprite.SandT, TmapSprite.SandB, TmapSprite.SandL, TmapSprite.SandR, TmapSprite.SandTR, TmapSprite.SandTL, TmapSprite.SandBR, TmapSprite.SandBL);
            }
            else if (IsCyber())
            {
                tmapSprite = Repeating4X4Pattern(
                    TmapSprite.CyberBTL, 
                    TmapSprite.CyberABR, 
                    TmapSprite.CyberATL, 
                    TmapSprite.CyberBBR, 
                    TmapSprite.CyberBBR, 
                    TmapSprite.CyberATL, 
                    TmapSprite.CyberABR, 
                    TmapSprite.CyberBTL, 
                    TmapSprite.CyberBTR, 
                    TmapSprite.CyberATR, 
                    TmapSprite.CyberABL, 
                    TmapSprite.CyberBBL, 
                    TmapSprite.CyberBBL, 
                    TmapSprite.CyberATR, 
                    TmapSprite.CyberABL, 
                    TmapSprite.CyberBTR);
            }
            else if (SameTile(TmapSprite.Mammoth, 
                    TmapSprite.MammothT, 
                    TmapSprite.MammothB, 
                    TmapSprite.MammothL, 
                    TmapSprite.MammothR, 
                    TmapSprite.MammothTR, 
                    TmapSprite.MammothTL, 
                    TmapSprite.MammothBR, 
                    TmapSprite.MammothBL))
            {
                GenericTileRules(TmapSprite.Mammoth, 
                    TmapSprite.MammothT, 
                    TmapSprite.MammothB, 
                    TmapSprite.MammothL, 
                    TmapSprite.MammothR, 
                    TmapSprite.MammothTR, 
                    TmapSprite.MammothTL, 
                    TmapSprite.MammothBR, 
                    TmapSprite.MammothBL);
            }
            else if (SameTile(TmapSprite.SigFloorM, 
                    TmapSprite.SigFloorM, 
                    TmapSprite.SigFloorL, 
                    TmapSprite.SigFloorR))
            {
                HorizPipe(TmapSprite.SigFloorM, 
                    TmapSprite.SigFloorM, 
                    TmapSprite.SigFloorL, 
                    TmapSprite.SigFloorR);
            }
            else if (SameTile(TmapSprite.SigBR, 
                    TmapSprite.SigBL, 
                    TmapSprite.SigMR, 
                    TmapSprite.SigML, 
                    TmapSprite.SigTR, 
                    TmapSprite.SigTL))
            {
                TwoWideTower(TmapSprite.SigBL, 
                    TmapSprite.SigBR, 
                    TmapSprite.SigML, 
                    TmapSprite.SigMR, 
                    TmapSprite.SigTR, 
                    TmapSprite.SigTL);
            }
            else if (SameTile(TmapSprite.WeeWoo, 
                    TmapSprite.WeeWoo2, 
                    TmapSprite.WeeWooL, 
                    TmapSprite.WeeWooR))
            {
                HorizPipe(TmapSprite.WeeWoo, 
                    TmapSprite.WeeWoo2, 
                    TmapSprite.WeeWooL, 
                    TmapSprite.WeeWooR);
            }
            else if (SameTile(TmapSprite.Seahorse, 
                    TmapSprite.SeahorseL, 
                    TmapSprite.SeahorseL2))
            {
                ThickWallTileRules(TmapSprite.SeahorseL, 
                    TmapSprite.SeahorseL2, 
                    TmapSprite.Seahorse);
            }
            else if (SameTile(TmapSprite.Fencing, 
                    TmapSprite.FencingL, 
                    TmapSprite.FencingR))
            {
                HorizPipe(TmapSprite.Fencing,
                    TmapSprite.Fencing, 
                    TmapSprite.FencingL, 
                    TmapSprite.FencingR);
            }
            else if (SameTile(TmapSprite.GuardRail, 
                    TmapSprite.GuardRailL, 
                    TmapSprite.GuardRailR))
            {
                HorizPipe(TmapSprite.GuardRail,
                    TmapSprite.GuardRail, 
                    TmapSprite.GuardRailL, 
                    TmapSprite.GuardRailR);
            }
            else if (SameTile(TmapSprite.BuffaloPipe, 
                    TmapSprite.BuffaloPipe2, 
                    TmapSprite.BuffaloPipeTop))
            {
                VertPipe(TmapSprite.BuffaloPipe,
                    TmapSprite.BuffaloPipe, 
                    TmapSprite.BuffaloPipe2, 
                    TmapSprite.BuffaloPipeTop);
            }
            else if (SameTile(TmapSprite.SafetyBL, 
                    TmapSprite.SafetyBR, 
                    TmapSprite.SafetyTL, 
                    TmapSprite.SafetyTR))
            {
                Repeating2X2(TmapSprite.SafetyBL, 
                    TmapSprite.SafetyBR, 
                    TmapSprite.SafetyTL, 
                    TmapSprite.SafetyTR);
            }
            else if (SameTile(TmapSprite.Blizzard, 
                    TmapSprite.Blizzard, 
                    TmapSprite.Blizzard, 
                    TmapSprite.BlizzardT, 
                    TmapSprite.BlizzardB, 
                    TmapSprite.BlizzardR, 
                    TmapSprite.BlizzardL, 
                    TmapSprite.BlizzardBL, 
                    TmapSprite.BlizzardBR))
            {
                GenericTileRules(TmapSprite.Blizzard, 
                    TmapSprite.BlizzardT, 
                    TmapSprite.BlizzardB, 
                    TmapSprite.BlizzardL, 
                    TmapSprite.BlizzardR, 
                    TmapSprite.BlizzardT, 
                    TmapSprite.BlizzardT, 
                    TmapSprite.BlizzardBR, 
                    TmapSprite.BlizzardBL);
            }
            else if (IsDirt())
            {
                FourByFourTileRules(TmapSprite.DirtCTR, TmapSprite.DirtCTL, TmapSprite.DirtCBR, TmapSprite.DirtCBL, TmapSprite.DirtOTR, TmapSprite.DirtOTL, TmapSprite.DirtOBR, TmapSprite.DirtOBL, TmapSprite.DirtRT, TmapSprite.DirtRB, TmapSprite.DirtLT, TmapSprite.DirtLB, TmapSprite.DirtTR, TmapSprite.DirtTL, TmapSprite.DirtBR, TmapSprite.DirtBL);
            } else if((grid.GetGridObject(x + 1, y).tmapSprite == TmapSprite.GroundSlopeR2 || grid.GetGridObject(x - 1, y).tmapSprite == TmapSprite.GroundSlopeR2) && grid.GetGridObject(x, y).tmapSprite == TmapSprite.GroundSlopeR2){
                grid.GetGridObject(x, y).tmapSprite = TmapSprite.GroundSlopeR1;
            } else if((grid.GetGridObject(x + 1, y).tmapSprite == TmapSprite.GroundSlopeL1 || grid.GetGridObject(x - 1, y).tmapSprite == TmapSprite.GroundSlopeL1) && grid.GetGridObject(x, y).tmapSprite == TmapSprite.GroundSlopeL1){
                grid.GetGridObject(x, y).tmapSprite = TmapSprite.GroundSlopeL2;
            }
        }

        private void GenericTileRules(TmapSprite Center, TmapSprite Top, TmapSprite Bottom, TmapSprite Left, TmapSprite Right, TmapSprite TopRight, TmapSprite TopLeft, TmapSprite BottomRight, TmapSprite BottomLeft)
        {

            if (grid.GetGridObject(x, y - 1).SameTile(Center, Top, Bottom, Left, Right, TopRight, TopLeft, BottomRight, BottomLeft) && !grid.GetGridObject(x, y + 1).SameTile(Center, Top, Bottom, Left, Right, TopRight, TopLeft, BottomRight, BottomLeft))
            {// this is the top!
                if (!grid.GetGridObject(x + 1, y).SameTile(Center, Top, Bottom, Left, Right, TopRight, TopLeft, BottomRight, BottomLeft) && grid.GetGridObject(x - 1, y).SameTile(Center, Top, Bottom, Left, Right, TopRight, TopLeft, BottomRight, BottomLeft))
                {// this is the top right corner!
                    grid.GetGridObject(x, y).tmapSprite = TopRight;
                }
                else if (!grid.GetGridObject(x - 1, y).SameTile(Center, Top, Bottom, Left, Right, TopRight, TopLeft, BottomRight, BottomLeft) && grid.GetGridObject(x + 1, y).SameTile(Center, Top, Bottom, Left, Right, TopRight, TopLeft, BottomRight, BottomLeft))
                {// this is the top left corner!
                    grid.GetGridObject(x, y).tmapSprite = TopLeft;
                }
                else
                {// this is the bottom!
                    grid.GetGridObject(x, y).tmapSprite = Top;
                }
            }
            else if (!grid.GetGridObject(x, y - 1).SameTile(Center, Top, Bottom, Left, Right, TopRight, TopLeft, BottomRight, BottomLeft) && grid.GetGridObject(x, y + 1).SameTile(Center, Top, Bottom, Left, Right, TopRight, TopLeft, BottomRight, BottomLeft))
            {// this is the bottom check!
                if (!grid.GetGridObject(x + 1, y).SameTile(Center, Top, Bottom, Left, Right, TopRight, TopLeft, BottomRight, BottomLeft) && grid.GetGridObject(x - 1, y).SameTile(Center, Top, Bottom, Left, Right, TopRight, TopLeft, BottomRight, BottomLeft))
                {// this is the bottom right corner!
                    grid.GetGridObject(x, y).tmapSprite = BottomRight;
                }
                else if (!grid.GetGridObject(x - 1, y).SameTile(Center, Top, Bottom, Left, Right, TopRight, TopLeft, BottomRight, BottomLeft) && grid.GetGridObject(x + 1, y).SameTile(Center, Top, Bottom, Left, Right, TopRight, TopLeft, BottomRight, BottomLeft))
                {// this is the bottom left corner!
                    grid.GetGridObject(x, y).tmapSprite = BottomLeft;
                }
                else
                {// this is the bottom!
                    grid.GetGridObject(x, y).tmapSprite = Bottom;
                }
            }
            else if (!grid.GetGridObject(x + 1, y).SameTile(Center, Top, Bottom, Left, Right, TopRight, TopLeft, BottomRight, BottomLeft) && grid.GetGridObject(x - 1, y).SameTile(Center, Top, Bottom, Left, Right, TopRight, TopLeft, BottomRight, BottomLeft))
            {// this is the right!
                grid.GetGridObject(x, y).tmapSprite = Right;
            }
            else if (!grid.GetGridObject(x - 1, y).SameTile(Center, Top, Bottom, Left, Right, TopRight, TopLeft, BottomRight, BottomLeft) && grid.GetGridObject(x + 1, y).SameTile(Center, Top, Bottom, Left, Right, TopRight, TopLeft, BottomRight, BottomLeft))
            {// this is the left!
                grid.GetGridObject(x, y).tmapSprite = Left;
            }
            else if (grid.GetGridObject(x, y - 1).tmapSprite != TmapSprite.None && grid.GetGridObject(x, y + 1).tmapSprite != TmapSprite.None)
            {// this is the center!
                grid.GetGridObject(x, y).tmapSprite = Center;
            }
        }

        private void TowerTileRules(TmapSprite Center, TmapSprite Top, TmapSprite Left, TmapSprite Right, TmapSprite TopRight, TmapSprite TopLeft)
        {
            if (grid.GetGridObject(x, y - 1).SameTile(Center, Top, Left, Right, TopRight, TopLeft) && !grid.GetGridObject(x, y + 1).SameTile(Center, Top, Left, Right, TopRight, TopLeft))
            {// this is the top!
                if (!grid.GetGridObject(x + 1, y).SameTile(Center, Top, Left, Right, TopRight, TopLeft) && grid.GetGridObject(x - 1, y).SameTile(Center, Top, Left, Right, TopRight, TopLeft))
                {// this is the top right corner!
                    grid.GetGridObject(x, y).tmapSprite = TopRight;
                }
                else if (!grid.GetGridObject(x - 1, y).SameTile(Center, Top, Left, Right, TopRight, TopLeft) && grid.GetGridObject(x + 1, y).SameTile(Center, Top, Left, Right, TopRight, TopLeft))
                {// this is the top left corner!
                    grid.GetGridObject(x, y).tmapSprite = TopLeft;
                }
                else
                {// this is the bottom!
                    grid.GetGridObject(x, y).tmapSprite = Top;
                }
            }
            else if (!grid.GetGridObject(x + 1, y).SameTile(Center, Top, Left, Right, TopRight, TopLeft) && grid.GetGridObject(x - 1, y).SameTile(Center, Top, Left, Right, TopRight, TopLeft))
            {// this is the right!
                grid.GetGridObject(x, y).tmapSprite = Right;
            }
            else if (!grid.GetGridObject(x - 1, y).SameTile(Center, Top, Left, Right, TopRight, TopLeft) && grid.GetGridObject(x + 1, y).SameTile(Center, Top, Left, Right, TopRight, TopLeft))
            {// this is the left!
                grid.GetGridObject(x, y).tmapSprite = Left;
            }
            else if (grid.GetGridObject(x - 1, y).SameTile(Center, Top, Left, Right, TopRight, TopLeft) && grid.GetGridObject(x + 1, y).SameTile(Center, Top, Left, Right, TopRight, TopLeft))
            {// this is the center!
                grid.GetGridObject(x, y).tmapSprite = Center;
            }
        }

        private void FourByFourTileRules(
            TmapSprite CenterTopRight,
            TmapSprite CenterTopLeft,
            TmapSprite CenterBottomRight,
            TmapSprite CenterBottomLeft,
            TmapSprite CornerTopRight,
            TmapSprite CornerTopLeft,
            TmapSprite CornerBottomRight,
            TmapSprite CornerBottomLeft,
            TmapSprite RightTop,
            TmapSprite RightBottom,
            TmapSprite LeftTop,
            TmapSprite LeftBottom,
            TmapSprite TopRight,
            TmapSprite TopLeft,
            TmapSprite BottomRight,
            TmapSprite BottomLeft
            )
        {
            if (grid.GetGridObject(x, y - 1).SameTile(CenterTopRight, CenterTopLeft, CenterBottomRight, CenterBottomLeft, CornerTopRight, CornerTopLeft, CornerBottomRight, CornerBottomLeft, RightTop, RightBottom, LeftTop, LeftBottom, TopRight, TopLeft, BottomRight, BottomLeft) && !grid.GetGridObject(x, y + 1).SameTile(CenterTopRight, CenterTopLeft, CenterBottomRight, CenterBottomLeft, CornerTopRight, CornerTopLeft, CornerBottomRight, CornerBottomLeft, RightTop, RightBottom, LeftTop, LeftBottom, TopRight, TopLeft, BottomRight, BottomLeft))
            {// this is the top!
                if (!grid.GetGridObject(x + 1, y).SameTile(CenterTopRight, CenterTopLeft, CenterBottomRight, CenterBottomLeft, CornerTopRight, CornerTopLeft, CornerBottomRight, CornerBottomLeft, RightTop, RightBottom, LeftTop, LeftBottom, TopRight, TopLeft, BottomRight, BottomLeft) && grid.GetGridObject(x - 1, y).SameTile(CenterTopRight, CenterTopLeft, CenterBottomRight, CenterBottomLeft, CornerTopRight, CornerTopLeft, CornerBottomRight, CornerBottomLeft, RightTop, RightBottom, LeftTop, LeftBottom, TopRight, TopLeft, BottomRight, BottomLeft))
                {// this is the top right corner!
                    grid.GetGridObject(x, y).tmapSprite = CornerTopRight;
                }
                else if (!grid.GetGridObject(x - 1, y).SameTile(CenterTopRight, CenterTopLeft, CenterBottomRight, CenterBottomLeft, CornerTopRight, CornerTopLeft, CornerBottomRight, CornerBottomLeft, RightTop, RightBottom, LeftTop, LeftBottom, TopRight, TopLeft, BottomRight, BottomLeft) && grid.GetGridObject(x + 1, y).SameTile(CenterTopRight, CenterTopLeft, CenterBottomRight, CenterBottomLeft, CornerTopRight, CornerTopLeft, CornerBottomRight, CornerBottomLeft, RightTop, RightBottom, LeftTop, LeftBottom, TopRight, TopLeft, BottomRight, BottomLeft))
                {// this is the top left corner!
                    grid.GetGridObject(x, y).tmapSprite = CornerTopLeft;
                }
                else
                {// this is the top!
                    if (grid.GetGridObject(x - 1, y).tmapSprite == TopRight || grid.GetGridObject(x + 1, y).tmapSprite == TopRight)
                    {
                        grid.GetGridObject(x, y).tmapSprite = TopLeft;
                    } else {
                        grid.GetGridObject(x, y).tmapSprite = TopRight;
                    }
                }
            }
            else if (!grid.GetGridObject(x, y - 1).SameTile(CenterTopRight, CenterTopLeft, CenterBottomRight, CenterBottomLeft, CornerTopRight, CornerTopLeft, CornerBottomRight, CornerBottomLeft, RightTop, RightBottom, LeftTop, LeftBottom, TopRight, TopLeft, BottomRight, BottomLeft) && grid.GetGridObject(x, y + 1).SameTile(CenterTopRight, CenterTopLeft, CenterBottomRight, CenterBottomLeft, CornerTopRight, CornerTopLeft, CornerBottomRight, CornerBottomLeft, RightTop, RightBottom, LeftTop, LeftBottom, TopRight, TopLeft, BottomRight, BottomLeft))
            {// this is the bottom check!
                if (!grid.GetGridObject(x + 1, y).SameTile(CenterTopRight, CenterTopLeft, CenterBottomRight, CenterBottomLeft, CornerTopRight, CornerTopLeft, CornerBottomRight, CornerBottomLeft, RightTop, RightBottom, LeftTop, LeftBottom, TopRight, TopLeft, BottomRight, BottomLeft) && grid.GetGridObject(x - 1, y).SameTile(CenterTopRight, CenterTopLeft, CenterBottomRight, CenterBottomLeft, CornerTopRight, CornerTopLeft, CornerBottomRight, CornerBottomLeft, RightTop, RightBottom, LeftTop, LeftBottom, TopRight, TopLeft, BottomRight, BottomLeft))
                {// this is the bottom right corner!
                    grid.GetGridObject(x, y).tmapSprite = CornerBottomRight;
                }
                else if (!grid.GetGridObject(x - 1, y).SameTile(CenterTopRight, CenterTopLeft, CenterBottomRight, CenterBottomLeft, CornerTopRight, CornerTopLeft, CornerBottomRight, CornerBottomLeft, RightTop, RightBottom, LeftTop, LeftBottom, TopRight, TopLeft, BottomRight, BottomLeft) && grid.GetGridObject(x + 1, y).SameTile(CenterTopRight, CenterTopLeft, CenterBottomRight, CenterBottomLeft, CornerTopRight, CornerTopLeft, CornerBottomRight, CornerBottomLeft, RightTop, RightBottom, LeftTop, LeftBottom, TopRight, TopLeft, BottomRight, BottomLeft))
                {// this is the bottom left corner!
                    grid.GetGridObject(x, y).tmapSprite = CornerBottomLeft;
                }
                else
                {// this is the bottom!
                    if (grid.GetGridObject(x - 1, y).tmapSprite == BottomRight || grid.GetGridObject(x + 1, y).tmapSprite == BottomRight)
                    {
                        grid.GetGridObject(x, y).tmapSprite = BottomLeft;
                    }
                    else
                    {
                        grid.GetGridObject(x, y).tmapSprite = BottomRight;
                    }
                }
            }
            else if (!grid.GetGridObject(x + 1, y).SameTile(CenterTopRight, CenterTopLeft, CenterBottomRight, CenterBottomLeft, CornerTopRight, CornerTopLeft, CornerBottomRight, CornerBottomLeft, RightTop, RightBottom, LeftTop, LeftBottom, TopRight, TopLeft, BottomRight, BottomLeft) && grid.GetGridObject(x - 1, y).SameTile(CenterTopRight, CenterTopLeft, CenterBottomRight, CenterBottomLeft, CornerTopRight, CornerTopLeft, CornerBottomRight, CornerBottomLeft, RightTop, RightBottom, LeftTop, LeftBottom, TopRight, TopLeft, BottomRight, BottomLeft))
            {// this is the right!
                if (grid.GetGridObject(x, y - 1).tmapSprite == RightTop || grid.GetGridObject(x, y + 1).tmapSprite == RightTop)
                {
                    grid.GetGridObject(x, y).tmapSprite = RightBottom;
                }
                else
                {
                    grid.GetGridObject(x, y).tmapSprite = RightTop;
                }
            }
            else if (!grid.GetGridObject(x - 1, y).SameTile(CenterTopRight, CenterTopLeft, CenterBottomRight, CenterBottomLeft, CornerTopRight, CornerTopLeft, CornerBottomRight, CornerBottomLeft, RightTop, RightBottom, LeftTop, LeftBottom, TopRight, TopLeft, BottomRight, BottomLeft) && grid.GetGridObject(x + 1, y).SameTile(CenterTopRight, CenterTopLeft, CenterBottomRight, CenterBottomLeft, CornerTopRight, CornerTopLeft, CornerBottomRight, CornerBottomLeft, RightTop, RightBottom, LeftTop, LeftBottom, TopRight, TopLeft, BottomRight, BottomLeft))
            {// this is the left!
                if(grid.GetGridObject(x - 1, y).tmapSprite == LeftTop || grid.GetGridObject(x + 1, y).tmapSprite == LeftTop)
                {
                    grid.GetGridObject(x, y).tmapSprite = LeftBottom;
                }
                else
                {
                    grid.GetGridObject(x, y).tmapSprite = LeftTop;
                }
            }
            else
            {
                grid.GetGridObject(x, y).tmapSprite = Repeating2X2Pattern(CenterTopRight, CenterTopLeft, CenterBottomRight, CenterBottomLeft);
            }
        }

        private void HorizPipe(TmapSprite Center, TmapSprite Center2, TmapSprite LeftCap, TmapSprite RightCap){
            if(grid.GetGridObject(x - 1, y).SameTile(Center, Center2, LeftCap, RightCap) && !grid.GetGridObject(x + 1, y).SameTile(Center, Center2, LeftCap, RightCap)){
                grid.GetGridObject(x, y).tmapSprite = RightCap;
            } else if(grid.GetGridObject(x + 1, y).SameTile(Center, Center2, LeftCap, RightCap) && !grid.GetGridObject(x - 1, y).SameTile(Center, Center2, LeftCap, RightCap)){
                grid.GetGridObject(x, y).tmapSprite = LeftCap;
            } else {
                if(x % 2 == 1){
                    grid.GetGridObject(x, y).tmapSprite = Center2;
                } else {
                    grid.GetGridObject(x, y).tmapSprite = Center;
                }
            }
        }

        private void VertPipe(TmapSprite Center, TmapSprite Center2, TmapSprite TopCap, TmapSprite BottomCap){
            if(grid.GetGridObject(x, y - 1).SameTile(Center, Center2, TopCap, BottomCap) && !grid.GetGridObject(x, y + 1).SameTile(Center, Center2, TopCap, BottomCap)){
                grid.GetGridObject(x, y).tmapSprite = BottomCap;
            } else if(grid.GetGridObject(x, y + 1).SameTile(Center, Center2, TopCap, BottomCap) && !grid.GetGridObject(x, y - 1).SameTile(Center, Center2, TopCap, BottomCap)){
                grid.GetGridObject(x, y).tmapSprite = TopCap;
            } else {
                if(y % 2 == 1){
                    grid.GetGridObject(x, y).tmapSprite = Center2;
                } else {
                    grid.GetGridObject(x, y).tmapSprite = Center;
                }
            }
        }

        private void TwoWideTower(TmapSprite BottomLeft, TmapSprite BottomRight, TmapSprite Left, TmapSprite Right, TmapSprite TopRight, TmapSprite TopLeft){
            if(grid.GetGridObject(x, y - 1).SameTile(BottomLeft, BottomRight, Left, Right, TopRight, TopLeft) && !grid.GetGridObject(x, y + 1).SameTile(BottomLeft, BottomRight, Left, Right, TopRight, TopLeft)){
                if(x % 2 == 1){
                    grid.GetGridObject(x, y).tmapSprite = TopRight;
                } else {
                    grid.GetGridObject(x, y).tmapSprite = TopLeft;
                }
            } else if(!grid.GetGridObject(x, y - 1).SameTile(BottomLeft, BottomRight, Left, Right, TopRight, TopLeft) && grid.GetGridObject(x, y + 1).SameTile(BottomLeft, BottomRight, Left, Right, TopRight, TopLeft)){
                if(x % 2 == 1){
                    grid.GetGridObject(x, y).tmapSprite = BottomRight;
                } else {
                    grid.GetGridObject(x, y).tmapSprite = BottomLeft;
                }
            } else {
                if(x % 2 == 1){
                    grid.GetGridObject(x, y).tmapSprite = Right;
                } else {
                    grid.GetGridObject(x, y).tmapSprite = Left;
                }
            }
        }

        private TmapSprite Repeating4X4Pattern(
            TmapSprite CenterTopRight,
            TmapSprite CenterTopLeft,
            TmapSprite CenterBottomRight,
            TmapSprite CenterBottomLeft,
            TmapSprite CornerTopRight,
            TmapSprite CornerTopLeft,
            TmapSprite CornerBottomRight,
            TmapSprite CornerBottomLeft,
            TmapSprite RightTop,
            TmapSprite RightBottom,
            TmapSprite LeftTop,
            TmapSprite LeftBottom,
            TmapSprite TopRight,
            TmapSprite TopLeft,
            TmapSprite BottomRight,
            TmapSprite BottomLeft)
        {
            if (x % 4 == 0 && y % 4 == 3)
            {
                return CornerTopLeft;
            }
            else if (x % 4 == 1 && y % 4 == 3)
            {
                return TopLeft;
            }
            else if (x % 4 == 2 && y % 4 == 3)
            {
                return TopRight;
            }
            else if (x % 4 == 3 && y % 4 == 3)
            {
                return CornerTopRight;
            }
            else if (x % 4 == 0 && y % 4 == 2)
            {
                return LeftTop;
            }
            else if (x % 4 == 1 && y % 4 == 2)
            {
                return CenterTopLeft;
            }
            else if (x % 4 == 2 && y % 4 == 2)
            {
                return CenterTopRight;
            }
            else if (x % 4 == 3 && y % 4 == 2)
            {
                return RightTop;
            }
            else if (x % 4 == 0 && y % 4 == 1)
            {
                return LeftBottom;
            }
            else if (x % 4 == 1 && y % 4 == 1)
            {
                return CenterBottomLeft;
            }
            else if (x % 4 == 2 && y % 4 == 1)
            {
                return CenterBottomRight;
            }
            else if (x % 4 == 3 && y % 4 == 1)
            {
                return RightBottom;
            }
            else if (x % 4 == 0 && y % 4 == 0)
            {
                return CornerBottomLeft;
            }
            else if (x % 4 == 1 && y % 4 == 0)
            {
                return BottomLeft;
            }
            else if (x % 4 == 2 && y % 4 == 0)
            {
                return BottomRight;
            }
            return CornerBottomRight;
        }

        private TmapSprite Repeating3X3Pattern(
            TmapSprite Center,
            TmapSprite TopLeft,
            TmapSprite Top,
            TmapSprite TopRight,
            TmapSprite Right,
            TmapSprite BottomRight,
            TmapSprite Bottom,
            TmapSprite BottomLeft,
            TmapSprite Left)
        {
            if (x % 3 == 0 && y % 3 == 0)
            {
                return BottomLeft;
            } else if (x % 3 == 0 && y % 3 == 1)
            {
                return Left;
            } else if (x % 3 == 0 && y % 3 == 2)
            {
                return TopLeft;
            } else if (x % 3 == 1 && y % 3 == 0)
            {
                return Bottom;
            } else if (x % 3 == 1 && y % 3 == 1)
            {
                return Center;
            } else if (x % 3 == 1 && y % 3 == 2)
            {
                return Top;
            } else if (x % 3 == 2 && y % 3 == 0)
            {
                return BottomRight;
            } else if (x % 3 == 2 && y % 3 == 1)
            {
                return Right;
            }
            return TopRight;
        }

        private TmapSprite Repeating2X2Pattern(TmapSprite TopRight,TmapSprite TopLeft,TmapSprite BottomRight,TmapSprite BottomLeft){
            if(x % 2 == 1 && y % 2 == 1){
                return BottomRight;
            } else if(x % 2 == 0 && y % 2 == 1){
                return BottomLeft;
            } else if(x % 2 == 1 && y % 2 == 0){
                return TopRight;
            }
            return TopLeft;
        }

        private void Repeating2X2(TmapSprite TopRight,TmapSprite TopLeft,TmapSprite BottomRight,TmapSprite BottomLeft){
            if(x % 2 == 1 && y % 2 == 1){
                grid.GetGridObject(x, y).tmapSprite = BottomLeft;
            } else if(x % 2 == 0 && y % 2 == 1){
                grid.GetGridObject(x, y).tmapSprite = BottomRight;
            } else if(x % 2 == 1 && y % 2 == 0){
                grid.GetGridObject(x, y).tmapSprite = TopLeft;
            }
            grid.GetGridObject(x, y).tmapSprite = TopRight;
        }

        private void ThickWallTileRules(TmapSprite Outer,TmapSprite Middle,TmapSprite Inner){
            if(!grid.GetGridObject(x - 1, y).SameTile(Outer, Middle, Inner)){
                grid.GetGridObject(x, y).tmapSprite = Outer;
            } else if(!grid.GetGridObject(x - 2, y).SameTile(Outer, Middle, Inner)){
                grid.GetGridObject(x, y).tmapSprite = Middle;
            } else if(!grid.GetGridObject(x + 1, y).SameTile(Outer, Middle, Inner)){
                grid.GetGridObject(x, y).tmapSprite = Middle;
            } else if(!grid.GetGridObject(x + 2, y).SameTile(Outer, Middle, Inner)){
                grid.GetGridObject(x, y).tmapSprite = Outer;
            } else {
                grid.GetGridObject(x, y).tmapSprite = Inner;
            }

            
        }

        private bool IsGround()
        {
            return (tmapSprite == TmapSprite.Ground || 
                tmapSprite == TmapSprite.GroundT || 
                tmapSprite == TmapSprite.GroundB || 
                tmapSprite == TmapSprite.GroundR || 
                tmapSprite == TmapSprite.GroundL || 
                tmapSprite == TmapSprite.GroundTR || 
                tmapSprite == TmapSprite.GroundBR || 
                tmapSprite == TmapSprite.GroundTL ||
                tmapSprite == TmapSprite.GroundBL ||
                tmapSprite == TmapSprite.GroundTB ||
                tmapSprite == TmapSprite.GroundRL);
        }

        public bool IsTower()
        {
            return (tmapSprite == TmapSprite.Tower ||
                tmapSprite == TmapSprite.TowerTR ||
                tmapSprite == TmapSprite.TowerTL ||
                tmapSprite == TmapSprite.TowerL ||
                tmapSprite == TmapSprite.TowerR ||
                tmapSprite == TmapSprite.TowerT);
        }

        public bool IsPipe()
        {
            return (tmapSprite == TmapSprite.Pipe ||
                tmapSprite == TmapSprite.PipeTR ||
                tmapSprite == TmapSprite.PipeTL ||
                tmapSprite == TmapSprite.PipeT ||
                tmapSprite == TmapSprite.PipeB);
        }

        public bool IsDirt()
        {
            return (
            tmapSprite == TmapSprite.DirtCTR ||
            tmapSprite == TmapSprite.DirtCTL ||
            tmapSprite == TmapSprite.DirtCBR ||
            tmapSprite == TmapSprite.DirtCBL ||
            tmapSprite == TmapSprite.DirtOTR ||
            tmapSprite == TmapSprite.DirtOTL ||
            tmapSprite == TmapSprite.DirtOBR ||
            tmapSprite == TmapSprite.DirtOBL ||
            tmapSprite == TmapSprite.DirtRT ||
            tmapSprite == TmapSprite.DirtRB ||
            tmapSprite == TmapSprite.DirtLT ||
            tmapSprite == TmapSprite.DirtLB ||
            tmapSprite == TmapSprite.DirtBL ||
            tmapSprite == TmapSprite.DirtBR ||
            tmapSprite == TmapSprite.DirtTL ||
            tmapSprite == TmapSprite.DirtTR);
        }

        public bool IsSand()
        {
            return (tmapSprite == TmapSprite.Sand ||
                tmapSprite == TmapSprite.SandT ||
                tmapSprite == TmapSprite.SandR ||
                tmapSprite == TmapSprite.SandL ||
                tmapSprite == TmapSprite.SandB ||
                tmapSprite == TmapSprite.SandTL ||
                tmapSprite == TmapSprite.SandTR ||
                tmapSprite == TmapSprite.SandBL ||
                tmapSprite == TmapSprite.SandBR);
        }

        public bool IsCyber()
        {
            return (tmapSprite == TmapSprite.CyberABL ||
                tmapSprite == TmapSprite.CyberABR ||
                tmapSprite == TmapSprite.CyberATL ||
                tmapSprite == TmapSprite.CyberATR ||
                tmapSprite == TmapSprite.CyberBBL ||
                tmapSprite == TmapSprite.CyberBBR ||
                tmapSprite == TmapSprite.CyberBTL ||
                tmapSprite == TmapSprite.CyberBTR);
        }

        public bool SameTile(TmapSprite Center, TmapSprite Top, TmapSprite Bottom, TmapSprite Left, TmapSprite Right, TmapSprite TopRight, TmapSprite TopLeft, TmapSprite BottomRight, TmapSprite BottomLeft)
        {
            return (
            tmapSprite == Center ||
            tmapSprite == Top ||
            tmapSprite == Bottom ||
            tmapSprite == Left ||
            tmapSprite == Right ||
            tmapSprite == TopRight ||
            tmapSprite == TopLeft ||
            tmapSprite == BottomRight ||
            tmapSprite == BottomLeft);
        }

        public bool SameTile(TmapSprite CenterTopRight,TmapSprite CenterTopLeft,TmapSprite CenterBottomRight,TmapSprite CenterBottomLeft,TmapSprite CornerTopRight,TmapSprite CornerTopLeft,TmapSprite CornerBottomRight,TmapSprite CornerBottomLeft,TmapSprite RightTop,TmapSprite RightBottom, TmapSprite LeftTop,TmapSprite LeftBottom,TmapSprite TopRight,TmapSprite TopLeft,TmapSprite BottomRight,TmapSprite BottomLeft)
        {
            return (
            tmapSprite == CenterTopRight ||
            tmapSprite == CenterTopLeft ||
            tmapSprite == CenterBottomRight ||
            tmapSprite == CenterBottomLeft ||
            tmapSprite == CornerTopRight ||
            tmapSprite == CornerTopLeft ||
            tmapSprite == CornerBottomRight ||
            tmapSprite == CornerBottomLeft ||
            tmapSprite == RightTop ||
            tmapSprite == RightBottom ||
            tmapSprite == LeftTop ||
            tmapSprite == LeftBottom ||
            tmapSprite == TopRight ||
            tmapSprite == TopLeft ||
            tmapSprite == BottomRight ||
            tmapSprite == BottomLeft);
        }

        public bool SameTile(TmapSprite Center, TmapSprite Top, TmapSprite Left, TmapSprite Right, TmapSprite TopRight, TmapSprite TopLeft)
        {
            return (
            tmapSprite == Center ||
            tmapSprite == Top ||
            tmapSprite == Left ||
            tmapSprite == Right ||
            tmapSprite == TopRight ||
            tmapSprite == TopLeft);
        }

        public bool SameTile(TmapSprite Center, TmapSprite RightCap, TmapSprite LeftCap)
        {
            return (
            tmapSprite == Center ||
            tmapSprite == RightCap ||
            tmapSprite == LeftCap);
        }

        public bool SameTile(TmapSprite Center, TmapSprite RightCap, TmapSprite LeftCap, TmapSprite Center2)
        {
            return (
            tmapSprite == Center ||
            tmapSprite == RightCap ||
            tmapSprite == LeftCap ||
            tmapSprite == Center2);
        }

        public bool BlacklistedCollisionTiles()
        {
            return (
            tmapSprite == TmapSprite.SmallEnergy ||
            tmapSprite == TmapSprite.RideroidG ||
            tmapSprite == TmapSprite.None ||
            tmapSprite == TmapSprite.Ladder ||
            tmapSprite == TmapSprite.Seahorse ||
            tmapSprite == TmapSprite.SeahorseL ||
            tmapSprite == TmapSprite.SeahorseL2 ||
            tmapSprite == TmapSprite.Fencing ||
            tmapSprite == TmapSprite.FencingL ||
            tmapSprite == TmapSprite.FencingR ||
            tmapSprite == TmapSprite.GuardRail ||
            tmapSprite == TmapSprite.GuardRailL ||
            tmapSprite == TmapSprite.GuardRailR ||
            tmapSprite == TmapSprite.BuffaloPipe ||
            tmapSprite == TmapSprite.BuffaloPipe2 ||
            tmapSprite == TmapSprite.BuffaloPipeTop ||
            tmapSprite == TmapSprite.ScaffHandle ||
            tmapSprite == TmapSprite.Handlebar ||
            tmapSprite == TmapSprite.playerStart ||
            tmapSprite == TmapSprite.winPoint ||
            tmapSprite == TmapSprite.Null||
            tmapSprite == TmapSprite.FlameMammothBoss||
            tmapSprite == TmapSprite.GroundSlopeL1 ||
            tmapSprite == TmapSprite.GroundSlopeL2 ||
            tmapSprite == TmapSprite.GroundSlopeR1 ||
            tmapSprite == TmapSprite.GroundSlopeR2);
        }

        public bool BlacklistedRenderTiles()
        {
            return (
            tmapSprite == TmapSprite.SmallEnergy ||
            tmapSprite == TmapSprite.RideroidG ||
            tmapSprite == TmapSprite.None ||
            tmapSprite == TmapSprite.playerStart ||
            tmapSprite == TmapSprite.winPoint ||
            tmapSprite == TmapSprite.Null||
            tmapSprite == TmapSprite.FlameMammothBoss);
        }

        public bool IsEnemy()
        {
            return (
            tmapSprite == TmapSprite.Scriver ||
            tmapSprite == TmapSprite.RideroidG||
            tmapSprite == TmapSprite.Ladder||
            tmapSprite == TmapSprite.FlameMammothBoss||
            tmapSprite == TmapSprite.GroundSlopeL1||
            tmapSprite == TmapSprite.GroundSlopeR1);
        }
    }
}
