using System.Collections.Generic;
using UnityEngine;
using RTSEngine.Entities;
using RTSEngine.Game;
using RTSEngine.BuildingExtension;
using RTSEngine.Cameras;
using RTSEngine.Event;
using RTSEngine.Faction;
using RTSEngine.ResourceExtension;
using RTSEngine.Selection;
using RTSEngine.UnitExtension;
using System.Linq;
using System;
using RTSEngine.Terrain;
using RTSEngine.Health;

public class RTSEScenePrepManager : MonoBehaviour, IPreRunGameService
{
    [HideInInspector]
    public int TabID = 0;
    #region RTSE Service calls
    protected IGlobalEventPublisher GlobalEvent { private set; get; }
    protected IGameManager GameMgr { private set; get; }
    protected IBuildingManager BuildingMgr { private set; get; }
    protected IUnitManager UnitMgr { private set; get; }
    protected ISelectionManager SelectionMgr { private set; get; }
    protected IMainCameraController CamController { private set; get; }
    protected IResourceManager ResourceMgr { private set; get; }
    protected ITerrainManager TerrainMgr { private set; get; }
    #endregion

    #region Properties
    [Tooltip("Parent Transform for the Player start position markers")]
    public Transform PlayerPositionsParent = null;
    public List<PlayerStartLocation> PlayerStartLocations = new();
    public List<AvailableFaction> AvailableFactions = new();
    public GameObject TerrainsParent = null;
    [Tooltip("Set the prefab here for the system to add to the map.")]
    public GameObject PlayerStartPointPrefab = null;
    public FactionTypeInfo FactionTypeAdder;
    public int NewPlayerIndex = 0;
    public bool IsPlacingPlayerStart = false;
    #endregion

    public void Init(IGameManager GameMgr)
    {
        this.GameMgr = GameMgr;
        this.GlobalEvent = GameMgr.GetService<IGlobalEventPublisher>();
        this.BuildingMgr = GameMgr.GetService<IBuildingManager>();
        this.UnitMgr = GameMgr.GetService<IUnitManager>();
        this.SelectionMgr = GameMgr.GetService<ISelectionManager>();
        this.ResourceMgr = GameMgr.GetService<IResourceManager>();
        this.CamController = GameMgr.GetService<IMainCameraController>();
        this.TerrainMgr = GameMgr.GetService<ITerrainManager>();

        this.GameMgr.GameStartRunning += HandleGameStartRunning;

    }

    public void HandleGameStartRunning(IGameManager source, EventArgs args)
    {
        
        if (this.AvailableFactions.Count > 0 && source.FactionSlots.Count > 0 && this.PlayerPositionsParent != null && PlayerPositionsParent.childCount == source.FactionCount)
        {
            int index = -1;
            foreach (FactionSlot facSlot in source.FactionSlots)
            {
                index++;
                AvailableFaction thisFactionData = AvailableFactions.Find(AF => AF.FactionType == facSlot.Data.type);
                if (thisFactionData != null)
                {
                    // First we will get the position GO for the player by index
                    if (this.PlayerPositionsParent.GetChild(index).TryGetComponent(out PlayerStartLocation thisPlayersStart))
                    {
                        facSlot.FactionSpawnPosition.Set(thisPlayersStart.transform.position.x, thisPlayersStart.transform.position.y, thisPlayersStart.transform.position.z);
                        // For now we can get the child objects for buildings and units by index so we first run a check on children
                        // We use index 0 for building positions
                        if (thisPlayersStart.BuildingsParent.childCount > 0)
                        {
                            foreach (Transform child in thisPlayersStart.BuildingsParent)
                            {
                                if(child != null)
                                {
                                    // this makes sure that it only fires if there is a marker comp, standard GetComponent is not reliable as will always create an empty instance of that comp.
                                    if (child.gameObject.TryGetComponent(out FactionBuildingMarker bldngMarker))
                                    {
                                        IBuilding spawnMe = thisFactionData.FactionBuildings.ElementAtOrDefault(bldngMarker.buildingIndexToSpawn).Building;
                                        if (spawnMe != null)
                                        {
                                            IBuilding placedBuilding = BuildingMgr.CreatePlacedBuildingLocal(
                                                spawnMe,
                                                child.position,
                                                child.rotation,
                                                new InitBuildingParameters
                                                {
                                                    buildingCenter = spawnMe.BorderComponent,
                                                    factionID = facSlot.ID,
                                                    isBuilt = true,
                                                    setInitialHealth = true,
                                                    initialHealth = thisFactionData.FactionBuildings.ElementAtOrDefault(bldngMarker.buildingIndexToSpawn).StartingHealth,
                                                    playerCommand = false
                                                }
                                            );
                                            if(placedBuilding.InitResources.Count() > 0)
                                                this.ResourceMgr.SetResource(facSlot.ID, placedBuilding.InitResources.ElementAtOrDefault(0));
                                            
                                        }
                                    }
                                }
                            }
                        }
                        // We do the same here for units on index 1
                        if (thisPlayersStart.UnitsParent.childCount > 0)
                        {
                            foreach (Transform child in thisPlayersStart.UnitsParent)
                            {
                                if (child.gameObject.TryGetComponent(out FactionUnitMarker unitMarker))
                                {
                                    // Here we create an instance for this iteration, you can control all it's functions and props before spawning it
                                    IUnit spawnMe = thisFactionData.FactionUnits.ElementAtOrDefault(unitMarker.unitIndexToSpawn).Unit;
                                    if (spawnMe != null)
                                    {
                                        // Spawning just the unit
                                        this.UnitMgr.CreateUnit(
                                            spawnMe,
                                            child.position,
                                            child.rotation,
                                            new InitUnitParameters
                                            {
                                                factionID = facSlot.ID,
                                                playerCommand = false
                                            }
                                        );
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
    public void Disable()
    {
        this.GameMgr.GameStartRunning -= HandleGameStartRunning;
    }
}
[Serializable]
public class AvailableFaction
{
    public FactionTypeInfo FactionType = null;
    public List<FactionBuilding> FactionBuildings = new();
    public List<FactionUnit> FactionUnits = new();
    public AvailableFaction(
        FactionTypeInfo FactionType,
        List<FactionBuilding> FactionBuildings,
        List<FactionUnit> FactionUnits
    )
    {
        this.FactionType = FactionType;
        this.FactionBuildings = FactionBuildings;
        this.FactionUnits = FactionUnits;
    }
}

[Serializable]
public class FactionBuilding
{
    public Building Building = null;
    public int StartingHealth = 0;
    public FactionBuilding(Building Building, int StartingHealth)
    {
        this.Building = Building;
        this.StartingHealth = StartingHealth;
    }
}

[Serializable]
public class FactionUnit
{
    public Unit Unit = null;
    public FactionUnit(Unit Unit)
    {
        this.Unit = Unit;
    }
}
