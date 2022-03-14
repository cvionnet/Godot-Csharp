using Nucleus;

/// <summary>
/// A class to define timing options to apply to spawn an instance
/// </summary>
public class Spawn_Timing
{
    public bool IsTimed { get; private set; }
    public bool IsRandomTime { get; private set; }
    //public bool IsRandomTimePerSpawn { get; private set; }
    public float MinTime { get; private set; }
    public float MaxTime { get; private set; }

    /// <summary>
    /// Constructor (empty constructor : timing options are disabled)
    /// </summary>
    /// <param name="pIsTimed">Set to true to create a Spawn_Timing</param>
    /// <param name="pIsRandomTime">Use a random spawn timing ?</param>
    /// <param name="pIsRandomTimePerSpawn">Set a random spawn timing on each instance ?</param>
    /// <param name="pMinTime">Minimum time to wait before creating a new instance</param>
    /// <param name="pMaxTime">Maximum time to wait before creating a new instance (used if pIsRandomTime or pIsRandomTimePerSpawn=true)</param>
    public Spawn_Timing(bool pIsTimed=false, bool pIsRandomTime=false, bool pIsRandomTimePerSpawn=false, float pMinTime=0.0f, float pMaxTime=1.0f)
    {
        IsTimed = pIsTimed;
        IsRandomTime = pIsRandomTime;
        //IsRandomTimePerSpawn = pIsRandomTimePerSpawn;
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
        float spawnTime = 0.0f;

        if(IsTimed)
        {
            spawnTime = MinTime;

            if (IsRandomTime)
                spawnTime = Nucleus_Maths.Rnd.RandfRange(MinTime, MaxTime);
        }

        return spawnTime;
    }
}
