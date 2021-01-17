using Godot;

/// <summary>
/// Class that contains all variables and methods usefull between projects
/// </summary>
public static class Utils
{
#region VARIABLES

    public static Vector2 VECTOR_0 = new Vector2(0.0f,0.0f);          // (=Vector2.ZERO in GDScript)
    public static Vector2 VECTOR_1 = new Vector2(1.0f,1.0f);
    public static Vector2 VECTOR_FLOOR = new Vector2(0,-1);     // (=Vector2.UP in GDScript) Use it for plateformer

    public static StateMachine StateMachine_Node { get; set; }

#endregion

//*-------------------------------------------------------------------------*//

#region METHODS - NODES

    /// <summary>
    /// Reccurcive method to get the 1st Node in a Group named pGroupName (Editor > Node window > Groups)
    /// </summary>
    /// <param name="pNode">The node to check</param>
    /// <param name="pGroupName">the name of the Group</param>
    /// <returns>If found, the node in the Group; if not found, null</returns>
    public static Node FindNode_BasedOnGroup(Node pNode, string pGroupName)
    {
        // Reccurcive on the parent node (until all nodes read or the node found)
        if(pNode != null && !pNode.IsInGroup(pGroupName))
            return FindNode_BasedOnGroup(pNode.GetParent(), pGroupName);

        return pNode;
    }

#endregion

#region METHODS - PHYSICS

/// <summary>
/// Get the direction of a character according of the inputs
/// Platformer : only focus on the x axis  (left/right)
/// </summary>
/// <param name="pMove_Left">the name of the action used in the Input Map</param>
/// <param name="pMove_Right">the name of the action used in the Input Map</param>
/// <param name="pNormalize">True to return a normalized vector</param>
/// <returns>A vector2 to represent the direction of the character</returns>
public static Vector2 GetDirection_Platformer(string pMove_Left, string pMove_Right, bool pNormalize)
{
    Vector2 new_velocity;

    // Get the direction
    new_velocity = new Vector2(
        Input.GetActionStrength(pMove_Right) - Input.GetActionStrength(pMove_Left),
        1.0f
    );

    // Normalize the vector (to not have a character that move quicker with an angled direction)
    new_velocity = (pNormalize) ? new_velocity.Normalized() : new_velocity;

    return new_velocity;
}

/// <summary>
/// Calculate the velocity of a character
/// </summary>
/// <param name="pOld_Velocity">the actual velocity of the character</param>
/// <param name="pMax_Speed">to limit the new velocity</param>
/// <param name="pAcceleration">the acceleration of the character</param>
/// <param name="pDecceleration">the decceleration of the character</param>
/// <param name="pDirection">the direction of the character</param>
/// <param name="delta">delta time</param>
/// <param name="pMaxFallSpeed">the maximum speed the character can reach when he is falling</param>
/// <returns>A vector2 to represent the new velocity</returns>
public static Vector2 CalculateVelocity(Vector2 pOld_Velocity, Vector2 pMax_Speed, Vector2 pAcceleration, Vector2 pDecceleration, Vector2 pDirection, float delta, float pMaxFallSpeed = 1500.0f)
{
    Vector2 new_velocity = pOld_Velocity;

    // Calculate the new velocity for y axis (acceleration only)
    new_velocity.y += pDirection.y * pAcceleration.y * delta;


    // Calculate the new velocity for x axis (acceleration and decceleration)
    // If the character has a direction (it means that the player is pressing keys), we calculate the velocity (same as y axis)
    if (pDirection.x != 0.0f)
    {
        new_velocity.x += pDirection.x * pAcceleration.x * delta;
    }
    else if (pDirection.x == 0.0f && Mathf.Abs(new_velocity.x) > 0.2f)
    {
        // Read the direction from velocity (can be positive or negative)
        int direction = Mathf.Sign(new_velocity.x);

        new_velocity.x -= direction * pDecceleration.x * delta;

        // To avoid a bug : if the Inertia_Stop is a large value, the result vector will be greater than 0.2f and in the opposite direction (the player moves by himself)
         if (Mathf.Sign(new_velocity.x) != Mathf.Sign(pOld_Velocity.x))
            new_velocity.x = 0.0f;
    }


    // Limit the velocity on both axis to the max speed
    new_velocity.x = Mathf.Clamp(new_velocity.x, -pMax_Speed.x, pMax_Speed.x);
    new_velocity.y = Mathf.Clamp(new_velocity.y, -pMax_Speed.y, pMaxFallSpeed);

    return new_velocity;
}

/// <summary>
/// Calculate the jump velocity of a character
/// </summary>
/// <param name="pVelocity">the actual velocity of the character</param>
/// <param name="pMax_Speed">to limit the new velocity</param>
/// <param name="pImpulse">a value to represent the high of the jump</param>
/// <returns></returns>
public static Vector2 CalculateJumpVelocity(Vector2 pVelocity, Vector2 pMax_Speed, float pImpulse)
{
    return CalculateVelocity(pVelocity, pMax_Speed, new Vector2(0.0f, pImpulse), VECTOR_0, VECTOR_FLOOR, 1.0f);
}

#endregion

}