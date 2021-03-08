using Godot;
using System.Collections.Generic;

/// <summary>
/// A class to define timing options to apply to spawn an instance
/// </summary>
public class Spawn_Timing
{
    public bool IsTimed { get; private set; }
    public bool IsRandomTime { get; private set; }
    public bool IsRandomTimePerSpawn { get; private set; }
    public float MinTime { get; private set; }
    public float MaxTime { get; private set; }

    /// <summary>
    /// Constructor
    /// </summary>
    /// <param name="pIsTimed"></param>
    /// <param name="pIsRandomTime">Use a random spawn timing ?</param>
    /// <param name="pIsRandomTimePerSpawn">Set a random spawn timing on each instance ?</param>
    /// <param name="pMinTime">Minimum time to wait before creating a new instance</param>
    /// <param name="pMaxTime">Maximum time to wait before creating a new instance (used if pIsRandomTime or pIsRandomTimePerSpawn=true)</param>
    public Spawn_Timing(bool pIsTimed=false, bool pIsRandomTime=false, bool pIsRandomTimePerSpawn=false, float pMinTime=0.0f, float pMaxTime=1.0f)
    {
        IsTimed = pIsTimed;
        IsRandomTime = pIsRandomTime;
        IsRandomTimePerSpawn = pIsRandomTimePerSpawn;
        MinTime = pMinTime;
        MaxTime = pMaxTime;
    }

    /// <summary>
    /// Return the amount of time to wait
    ///     - if IsTimed = false                        => return 0.0f
    ///     - if IsTimed = true                         => return MinTime
    ///     - if IsTimed = true and IsRandomTime = true => return a random value between MinTime and MaxTime
    /// </summary>
    /// <returns>A float to represent the amount of time</returns>
    public float GetTiming()
    {
        float spawn_time = 0.0f;

        if(IsTimed)
        {
            spawn_time = MinTime;

            if (IsRandomTime)
                spawn_time = Utils.Rnd.RandfRange(MinTime, MaxTime);
        }

        return spawn_time;
    }
}

//*-------------------------------------------------------------------------*//
//*-------------------------------------------------------------------------*//

/// <summary>
/// A class to spawn new instance of a scene
/// How to use it :
///     - ⚠️ the scene that will be instancied must have a script attached
///     - in the main scene, add a Spawn_Factory scene
///     - to load existing scene to instanciate, use the Load_NewScene() method
///         - 👉 to spawn different objects  (from the main)
///             option 1 : load more than 1 scene using Load_NewScene() method
///                             mySpawnFactory.Load_NewScene("res://src/spawn_factory/spawn_objects/SpawnObject.tscn");
///                             mySpawnFactory.Load_NewScene("res://src/spawn_factory/spawn_objects/SpawnObject2.tscn");
///             option 2 : add more than 1 Spawn_Factory scene in the main scene
///                             mySpawnFactory1.Load_NewScene("res://src/spawn_factory/spawn_objects/SpawnObject.tscn");
///                             mySpawnFactory2.Load_NewScene("res://src/spawn_factory/spawn_objects/SpawnObject2.tscn");
///     - ⚠️ edit/add the Add_Instance() method to replace "SpawnObject" by the scene instance(s) to create
///     - to create an instance, use the Add_Instance() method
///         - quick instance :   mySpawnFactory.Add_Instance(0, _position.GlobalPosition);
///         - using a Spawn_Timing object (the default empty constructor define an object that use no timing options to spawn instance)
///             mySpawnFactory.Add_Instance(GlobalPosition, new Spawn_Timing(true, true, true, 0.2f, 1.0f), 5, true, "enemies");
/// </summary>
public class Spawn_Factory : Position2D
{
#region HEADER

    public List<PackedScene> ListScenes { get; private set; } = new List<PackedScene>();

#endregion

//*-------------------------------------------------------------------------*//

#region GODOT METHODS
#endregion

//*-------------------------------------------------------------------------*//

#region SIGNAL CALLBACKS
#endregion

//*-------------------------------------------------------------------------*//

#region USER METHODS

    /// <summary>
    /// Load an existing scene in the Spawn_Factory  (can be used to spawn random instance of different scene)
    /// </summary>
    /// <param name="pPath">the path to the scene (eg : ""res://src/Enemy.tscn")</param>
    public void Load_NewScene(string pPath)
    {
        PackedScene scene = (PackedScene)ResourceLoader.Load(pPath);
        ListScenes.Add(scene);
    }

    /// <summary>
    /// Spawn a single intance of the index of the PackedScene's list
    /// </summary>
    /// <param name="pIndexSceneToDisplay">The index of the PackedScene to add</param>
    /// <param name="pGlobalPosition">Where to display the scene spawned</param>
    /// <param name="pGroupName">Name of the group the instance will belong</param>
//👉 TODO: "SpawnObject" : replace this by the scene you want to instance
    public SpawnObject Add_Instance(int pIndexSceneToDisplay, Vector2 pGlobalPosition, string pGroupName="")
    {
        if(ListScenes == null || ListScenes.Count == 0 || ListScenes[pIndexSceneToDisplay] == null)
            return;

        PackedScene scene = ListScenes[pIndexSceneToDisplay];

        // Instance
        //👉 "SpawnObject" : replace this by the scene you want to instance
        SpawnObject instance;
        if (scene.Instance().GetType() == typeof(SpawnObject))
        {
            instance = (SpawnObject)scene.Instance();
            AddChild(instance);

            instance.GlobalPosition = pGlobalPosition;
            if(pGroupName != "") instance.AddToGroup(pGroupName);
        }
        else
        {
            GD.Print("TODO: ERROR - Class " + scene.Instance().GetType() + " is not defined in 'Add_Instance'");
            instance = null;
        }

        return instance;
    }

    /// <summary>
    /// Spawn a new intance of the PackedScene
    /// </summary>
    /// <param name="pGlobalPosition">Where to display the scene spawned</param>
    /// <param name="pTiming">A Spawn_Timing object to define timing options</param>
    /// <param name="pSpawnNumber">How many instance to create</param>
    /// <param name="pRandomInstance">If true, get a random scene from the scenes loaded in Load_NewScene() method. Else, load scene in the same order</param>
    /// <param name="pGroupName">Name of the group the instance will belong</param>
    async public void Add_Instance(Vector2 pGlobalPosition, Spawn_Timing pTiming, int pSpawnNumber=1, bool pRandomInstance=false, string pGroupName="")
    {
        if(ListScenes == null || ListScenes.Count == 0)
            return;

        int scene_id = 0;

        // Set the timing options
        float spawn_time = pTiming.GetTiming();

        // Create the instance
        for (int i = 0; i < pSpawnNumber; i++)
        {
            // Timing : apply if exists
            if(pTiming.IsTimed)
                await ToSignal(GetTree().CreateTimer(spawn_time), "timeout");

            Add_Instance(scene_id, pGlobalPosition, pGroupName);

            // Timing : get a new random timing for the next instance if needed
            if (pTiming.IsRandomTimePerSpawn)
                spawn_time = Utils.Rnd.RandfRange(pTiming.MinTime, pTiming.MaxTime);

            // Select a random scene if needed  or  load the next scene in the list
            if (pRandomInstance && ListScenes.Count > 1)
                scene_id = Utils.Rnd.RandiRange(0, ListScenes.Count-1);
            else if (ListScenes.Count > 1)
                scene_id = (scene_id < ListScenes.Count-1) ? scene_id+1 : 0;
        }
    }

#endregion
}