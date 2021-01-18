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
    /// <returns>A vector2 to represent the jump velocity</returns>
    public static Vector2 CalculateJumpVelocity(Vector2 pVelocity, Vector2 pMax_Speed, float pImpulse)
    {
        return CalculateVelocity(pVelocity, pMax_Speed, new Vector2(0.0f, pImpulse), VECTOR_0, VECTOR_FLOOR, 1.0f);
    }

#endregion

#region METHODS - AI / STEERING
    public const float STEERING_DEFAULT_MASS = 2.0f;
    public const float STEERING_DEFAULT_MAX_SPEED = 400.0f;
    public const float STEERING_DISTANCE_DESACTIVATE = 3.0f;  // to stop the character to move if he is closed to the target (or it will have a kind of Parkinson movement)
    public const float STEERING_DEFAULT_FLEE = 200.0f;      // to run away

    public static Node2D LeaderToFollow;    // to save the leader node to follow

    /// <summary>
    /// Calculate a velocity to move a character towards a destination (use steering behaviour to adjust every frame)
    /// </summary>
    /// <param name="pVelocity">the actual velocity of the character</param>
    /// <param name="pGlobalPosition">the actual global position of the character</param>
    /// <param name="pTargetPosition">the destination of the character</param>
    /// <param name="pMaxSpeed">the maximum speed the character can reach</param>
    /// <param name="pSlowRadius">(0.0f = no slow down) the circle radius around the target where the character starts to slow down</param>
    /// <returns>A vector2 to represent the destination velocity or a Vector2(0,0) if there is not enough distance between the character and the target</returns>
    public static Vector2 Steering_Follow(Vector2 pVelocity, Vector2 pGlobalPosition, Vector2 pTargetPosition, float pMaxSpeed = STEERING_DEFAULT_MAX_SPEED, float pSlowRadius = 0.0f, float pMass = STEERING_DEFAULT_MASS)
    {
        // Check if we have enough distance between the character and the target
        if (pGlobalPosition.DistanceTo(pTargetPosition) <= STEERING_DISTANCE_DESACTIVATE)
            return VECTOR_0;


        // Calculate the maximum velocity the character can move towards the target
        // Get a velocity vector between the target and the character position ...
        Vector2 desire_velocity = (pTargetPosition - pGlobalPosition).Normalized();
        // ... moving as fast as he can
        desire_velocity *= pMaxSpeed;


        // Slow down the character when he is closed to the target
        if (pSlowRadius != 0.0f)
        {
            float to_target = pGlobalPosition.DistanceTo(pTargetPosition);

            // Reduce velocity if in the slow circle around the target
            // 0.8f + 0.2f : used to not slow down too much the character
            if (to_target <= pSlowRadius)
                desire_velocity *= ((to_target / pSlowRadius) * 0.8f) + 0.2f;
        }


        // Calculate the steering vector : (maximum character's velocity - current velocity) / mass (to slow down character's movement)
        Vector2 steering = (desire_velocity - pVelocity) / pMass;

        return pVelocity + steering;
    }

    /// <summary>
    /// Calculate a velocity to flee from a destination
    /// </summary>
    /// <param name="pGlobalPosition">the actual global position of the character</param>
    /// <param name="pTargetPosition">the destination of the character</param>
    /// <param name="pFleeRadius">the circle radius where the character starts to flee</param>
    /// <returns>A vector2 to represent the destination velocity</returns>
    public static Vector2 Steering_CalculateFlee(Vector2 pGlobalPosition, Vector2 pTargetPosition, float pFleeRadius = STEERING_DEFAULT_FLEE)
    {
        // If the target is outside the radius, do nothing
        if (pGlobalPosition.DistanceTo(pTargetPosition) > pFleeRadius)
            return pGlobalPosition;

        Vector2 flee_global_position = pTargetPosition - (pTargetPosition - pGlobalPosition).Normalized();
	    Vector2 target_position = pGlobalPosition + (pGlobalPosition - flee_global_position).Normalized() * pFleeRadius;

        return target_position;
    }

    /// <summary>
    /// Calculate the velocity to keep distance behind the leader
    /// </summary>
    /// <param name="pLeaderPosition">the position of the node to follow</param>
    /// <param name="pFollowerPosition">the position of the follower</param>
    /// <param name="pFollowOffset">the distance to keep between the leader and follower node</param>
    /// <returns>A vector2 to represent the position to follow with the distance to keep</returns>
    public static Vector2 Steering_CalculateDistanceBetweenFollowers(Vector2 pLeaderPosition, Vector2 pFollowerPosition, float pFollowOffset)
    {
        // Get the vector direction to the leader (behind the leader)
        Vector2 direction = (pFollowerPosition - pLeaderPosition).Normalized();
        Vector2 velocity = pFollowerPosition - (direction * pFollowOffset);

        // To avoid the follower to be too close to the leader (or it will have a kind of Parkinson movement)
        if (pLeaderPosition.DistanceTo(pFollowerPosition) <= pFollowOffset)
            velocity = pFollowerPosition;

        return velocity;
    }

#endregion

}