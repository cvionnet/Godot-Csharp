using Godot;
using System.Collections.Generic;
using System.Threading.Tasks;

/// <summary>
/// A class to spawn new instance of a scene
/// How to use it :
///     1. ⚠️ the scene that will be instancied must have a script attached
///     2. in the scene where to spawn :
///         1. add the Spawn_Factory scene as a node  (+ add code to use this node)
///         2. create an empty "private Initialize_Game()" method, call it in the "_Ready()" method and put the following code inside
///
///         3.1 only one scene ?
///             _spawnFactory.Load_NewScene("res://src/MyScene.tscn");
///             type instance = _spawnFactory.Add_Instance<type>(MyPosition);
///
///         3.2. more than one scene ?
///             _spawnFactory.Load_NewScene("res://src/MyScene.tscn");
///             _spawnFactory.Load_NewScene("res://src/MyScene2.tscn");
///             FOR (_spawnFactory.ListScenes.Count)
///                 type instance = _spawnFactory.Add_Instance<type>(MyPosition, 0);
///
///             ℹ️ another option : using multiple Spawn_Factory scenes
///                 _spawnFactory1.Load_NewScene("res://src/MyScene.tscn");
///                 _spawnFactory2.Load_NewScene("res://src/MyScene2.tscn");
///
///         3.3. to create an instance using a Spawn_Timing object :
///             set Initialize_Game as async           private async void Initialize_Game()
///                 type instance = await _spawnBlocks.Add_Instance_With_Delay<type>(MyPosition, new Spawn_Timing(true, true, true, 1.0f, 2.0f));
///
///         3.4. to create multiple instances (eg : 4) using a Spawn_Timing object :
///             set Initialize_Game as async           private async void Initialize_Game()
///                 type instance = await _spawnBlocks.Add_Multiple_Instances_With_Delay<type>(MyPosition, new Spawn_Timing(true, true, true, 1.0f, 2.0f), 4);
/// </summary>
public class Spawn_Factory : Position2D
{
#region HEADER

    public List<PackedScene> ListScenes { get; private set; } = new List<PackedScene>();

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
    /// Spawn a single instance of the index of the PackedScene's list   (scene must ihnerits from Node2D)
    /// </summary>
    /// <param name="pGlobalPosition">Where to display the scene spawned</param>
    /// <param name="pIndexSceneToDisplay">The index of the PackedScene to display (0 by default) </param>
    /// <param name="pGroupName">Name of the group the instance will belong ("" by default)</param>
    /// <returns>An instance of the scene</returns>
    public T Add_Instance<T>(Vector2 pGlobalPosition, int pIndexSceneToDisplay=0, string pGroupName="") where T:Node2D
    {
        if(ListScenes == null || ListScenes.Count == 0 || ListScenes[pIndexSceneToDisplay] == null)
            return null;

        PackedScene scene = ListScenes[pIndexSceneToDisplay];

        // Instance
        T instance;
        if (scene.Instance().GetType() == typeof(T))
        {
            instance = (T)scene.Instance();
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
    /// Spawn a single instance of the index of the PackedScene's list with a delay   (scene must ihnerits from Node2D)
    /// </summary>
    /// <param name="pGlobalPosition">Where to display the scene spawned</param>
    /// <param name="pTiming">A Spawn_Timing object to define timing options</param>
    /// <param name="pIndexSceneToDisplay">The index of the PackedScene to display (0 by default) </param>
    /// <param name="pGroupName">Name of the group the instance will belong ("" by default)</param>
    /// <returns>An instance of the scene</returns>
    public async Task<T> Add_Instance_With_Delay<T>(Vector2 pGlobalPosition, Spawn_Timing pTiming, int pIndexSceneToDisplay=0, string pGroupName="") where T:Node2D
    {
        if(ListScenes == null || ListScenes.Count == 0 || ListScenes[pIndexSceneToDisplay] == null)
            return null;

        // Set the timing options and wait
        float spawn_time = pTiming.GetTiming();
        if(pTiming.IsTimed)
            await ToSignal(GetTree().CreateTimer(spawn_time), "timeout");

        // Create and return the instance
        return Add_Instance<T>(pGlobalPosition, pIndexSceneToDisplay, pGroupName);
    }

    /// <summary>
    /// Spawn multiple instances from PackedScene's list with a delay   (scene must ihnerits from Node2D)
    /// </summary>
    /// <param name="pGlobalPosition">Where to display the scene spawned</param>
    /// <param name="pTiming">A Spawn_Timing object to define timing options</param>
    /// <param name="pSpawnNumber">How many instance to create</param>
    /// <param name="pRandomInstance">If true, get a random scene from the scenes loaded in Load_NewScene() method. Else, load scene in the same order</param>
    /// <param name="pGroupName">Name of the group the instance will belong</param>
    /// <returns>An instance of the scene</returns>
    public async Task<T> Add_Multiple_Instances_With_Delay<T>(Vector2 pGlobalPosition, Spawn_Timing pTiming, int pSpawnNumber=1, bool pRandomInstance=false, string pGroupName="") where T:Node2D
    {
        if(ListScenes == null || ListScenes.Count == 0 || pSpawnNumber < 1)
            return null;

        // Create the instance
        T instance = null;
        int scene_id = 0;

        // For instances to create
        for (int i = 0; i < pSpawnNumber; i++)
        {
            // Create the delayed instance
            instance = await Add_Instance_With_Delay<T>(pGlobalPosition, pTiming, scene_id, pGroupName);

            // Select a random scene if needed or load the next scene in the list
            if (pRandomInstance && ListScenes.Count > 1)
                scene_id = Utils.Rnd.RandiRange(0, ListScenes.Count-1);
            else if (ListScenes.Count > 1)
                scene_id = (scene_id < ListScenes.Count-1) ? scene_id+1 : 0;
        }

        return instance;
    }

#endregion
}

//*-------------------------------------------------------------------------*//
//*-------------------------------------------------------------------------*//

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
    /// Constructor (empty constructor : timing options are disabled)
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
