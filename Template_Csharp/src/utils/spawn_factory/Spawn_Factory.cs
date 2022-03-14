using System;
using Godot;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nucleus;
using System.Reflection;

/// <summary>
/// A class to spawn new instance of a scene
/// How to use it :
///     1. ⚠️ the scene that will be instancied must have a script attached
///     2. in the scene where to spawn :
///         1. add the Spawn_Factory scene as a node and rename it (eg : Spawn_Enemies)
///         2. in the parent code, in the "_Ready()" method, put the following code inside
///
///         3.1 only one scene ?
///             _spawnFactory.Load_NewScene("res://src/MyScene.tscn");
///             type instance = _spawnFactory.Add_Instance<type>(this/null, MyPosition);
///             OR
///             type instance = _spawnFactory.Add_Instance<type>(this/null, null);       // to not specify a position
///
///         3.2. more than one scene ?
///             _spawnFactory.Load_NewScene("res://src/MyScene.tscn");
///             _spawnFactory.Load_NewScene("res://src/MyScene2.tscn");
///             FOR (_spawnFactory.ListScenes.Count)
///                 type instance = _spawnFactory.Add_Instance<type>(this/null, MyPosition, 0);
///
///             ℹ️ another option : using multiple Spawn_Factory scenes
///                 _spawnFactory1.Load_NewScene("res://src/MyScene.tscn");
///                 _spawnFactory2.Load_NewScene("res://src/MyScene2.tscn");
///
///         3.3. to create an instance using a Spawn_Timing object :
///             set Initialize_Game as async           private async void Initialize_Game()
///                 type instance = await _spawnBlocks.Add_Instance_With_Delay<type>(this/null, MyPosition, new Spawn_Timing(true, true, true, 1.0f, 2.0f));
///
///         3.4. to create multiple instances (eg : 4) using a Spawn_Timing object :
///             set Initialize_Game as async           private async void Initialize_Game()
///                 type instance = await _spawnBlocks.Add_Multiple_Instances_With_Delay<type>(this/null, MyPosition, new Spawn_Timing(true, true, true, 1.0f, 2.0f), 4);
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
    /// <returns>True if no errors occurs</returns>
    public bool Load_NewScene(string pPath)
    {
        if (pPath != "")
        {
            try
            {
                PackedScene scene = ResourceLoader.Load(pPath) as PackedScene;
                if (scene != null)
                {
                    ListScenes.Add(scene);
                    return true;
                }
                else
                {
                    throw new System.NullReferenceException();
                }
            }
            catch (System.Exception ex)
            {
                Nucleus_Utils.Error($"Error while loading Path = {pPath}", ex, GetType().Name, MethodBase.GetCurrentMethod().Name);
                return false;
            }
        }
        else
        {
            Nucleus_Utils.Error("Path is empty", new CustomAttributeFormatException(), GetType().Name, MethodBase.GetCurrentMethod().Name);
        }

        return false;
    }

    /// <summary>
    /// Spawn a single instance of the index of the PackedScene's list   (scene must ihnerits from Node2D)
    /// </summary>
    /// <param name="pDestinationNode_Deferred">The node where to add the instance (usually THIS to use CallDeferred / NULL = do not use CallDeferred)</param>
    /// <param name="pGlobalPosition">Where to display the scene spawned (NULL = do not use set a position)</param>
    /// <param name="pIndexSceneToDisplay">The index of the PackedScene to display (0 by default) </param>
    /// <param name="pGroupName">Name of the group the instance will belong ("" by default)</param>
    /// <returns>An instance of the scene</returns>
    public T Add_Instance<T>(Node pDestinationNode_Deferred, Vector2? pGlobalPosition, int pIndexSceneToDisplay=0, string pGroupName="") where T:Node2D
    {
        if(ListScenes == null || ListScenes.Count == 0 || ListScenes[pIndexSceneToDisplay] == null)
            return null;

        PackedScene scene = ListScenes[pIndexSceneToDisplay];

        // Instance
        T instance;
        if (scene.Instance().GetType() == typeof(T))
        {
            instance = (T)scene.Instance();
            if (pDestinationNode_Deferred == null)
                AddChild(instance);
            else
                pDestinationNode_Deferred.CallDeferred("add_child", instance);

            if (pGlobalPosition != null) instance.GlobalPosition = (Vector2)pGlobalPosition;
            if(pGroupName != "") instance.AddToGroup(pGroupName);
        }
        else
        {
            Nucleus_Utils.Error($"TODO: ERROR - Class {scene.Instance().GetType()} is not defined in 'Add_Instance'", new NullReferenceException(), GetType().Name, MethodBase.GetCurrentMethod().Name);
            instance = null;
        }

        return instance;
    }

    /// <summary>
    /// (Use CallDeferred) Spawn a single instance of the index of the PackedScene's list with a delay   (scene must ihnerits from Node2D)
    /// </summary>
    /// <param name="pDestinationNode_Deferred">The node where to add the instance (usually THIS to use CallDeferred / NULL = do not use CallDeferred)</param>
    /// <param name="pGlobalPosition">Where to display the scene spawned (NULL = do not use set a position)</param>
    /// <param name="pTiming">A Spawn_Timing object to define timing options</param>
    /// <param name="pIndexSceneToDisplay">The index of the PackedScene to display (0 by default) </param>
    /// <param name="pGroupName">Name of the group the instance will belong ("" by default)</param>
    /// <returns>An instance of the scene</returns>
    public async Task<T> Add_Instance_With_Delay<T>(Node pDestinationNode_Deferred, Vector2? pGlobalPosition, Spawn_Timing pTiming, int pIndexSceneToDisplay=0, string pGroupName="") where T:Node2D
    {
        if(ListScenes == null || ListScenes.Count == 0 || ListScenes[pIndexSceneToDisplay] == null)
            return null;

        // Set the timing options and wait
        float spawn_time = pTiming.GetTiming();
        if(pTiming.IsTimed)
            await ToSignal(GetTree().CreateTimer(spawn_time), "timeout");

        // Create and return the instance
        return Add_Instance<T>(pDestinationNode_Deferred, pGlobalPosition, pIndexSceneToDisplay, pGroupName);
    }

    /// <summary>
    /// (Use CallDeferred) Spawn multiple instances from PackedScene's list with a delay   (scene must ihnerits from Node2D)
    /// </summary>
    /// <param name="pDestinationNode_Deferred">The node where to add the instance (usually THIS to use CallDeferred / NULL = do not use CallDeferred)</param>
    /// <param name="pGlobalPosition">Where to display the scene spawned (NULL = do not use set a position)</param>
    /// <param name="pTiming">A Spawn_Timing object to define timing options</param>
    /// <param name="pSpawnNumber">How many instance to create</param>
    /// <param name="pRandomInstance">If true, get a random scene from the scenes loaded in Load_NewScene() method. Else, load scene in the same order</param>
    /// <param name="pGroupName">Name of the group the instance will belong</param>
    /// <returns>An instance of the scene</returns>
    public async Task<T> Add_Multiple_Instances_With_Delay<T>(Node pDestinationNode_Deferred, Vector2? pGlobalPosition, Spawn_Timing pTiming, int pSpawnNumber=1, bool pRandomInstance=false, string pGroupName="") where T:Node2D
    {
        if(ListScenes == null || ListScenes.Count == 0 || pSpawnNumber < 1)
            return null;

        // Create the instance
        T instance = null;
        int sceneId = 0;

        // For instances to create
        for (int i = 0; i < pSpawnNumber; i++)
        {
            // Create the delayed instance
            instance = await Add_Instance_With_Delay<T>(pDestinationNode_Deferred, pGlobalPosition, pTiming, sceneId, pGroupName);

            // Select a random scene if needed or load the next scene in the list
            if (pRandomInstance && ListScenes.Count > 1)
                sceneId = Nucleus_Maths.Rnd.RandiRange(0, ListScenes.Count-1);
            else if (ListScenes.Count > 1)
                sceneId = (sceneId < ListScenes.Count-1) ? sceneId+1 : 0;
        }

        return instance;
    }

#endregion
}
