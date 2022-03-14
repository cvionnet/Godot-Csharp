using Godot;

namespace Nucleus.AI
{
    public partial class Nucleus_Steering
    {
        private const float STEERING_DEFAULT_MASS = 2.0f;
        private const float STEERING_DEFAULT_MAXSPEED = 400.0f;
        private const float STEERING_CLOSE_DISTANCE = 3.0f;      // to stop the character to move if he is closed to the target (or it will have a kind of Parkinson movement)
        private const float STEERING_DEFAULT_FLEE = 200.0f;      // to run away

        /// <summary>
        /// Calculate a velocity to move a character towards a destination (Follow)
        /// </summary>
        /// <param name="pCharacterProperties">a Character object with all properties</param>
        /// <param name="pPosition">the actual global position of the character</param>
        /// <param name="pStopRadius">the circle radius around the target where the character stops</param>
        /// <returns>A vector2 to represent the destination velocity or a Vector2(0,0) if character is close to the target</returns>
        public Vector2 Steering_Seek(CCharacter pCharacterProperties, Vector2 pPosition, float pStopRadius = STEERING_CLOSE_DISTANCE)
        {
//            return Steering_Seek(pCharacterProperties.Velocity, pPosition, pCharacterProperties.Steering.TargetGlobalPosition, pCharacterProperties.Steering.Speed,
//                            pCharacterProperties.Steering.Slow_Radius, pCharacterProperties.Steering.Mass, pStopRadius);
            return Steering_Seek(pCharacterProperties.Velocity, pPosition, TargetGlobalPosition, pCharacterProperties.MaxSpeed.x, Slow_Radius, Mass, pStopRadius);
        }

        /// <summary>
        /// Calculate a velocity to move a character towards a destination (Follow)
        /// </summary>
        /// <param name="pVelocity">the actual velocity of the character</param>
        /// <param name="pPosition">the actual global position of the character</param>
        /// <param name="pTargetPosition">the destination of the character</param>
        /// <param name="pMaxSpeed">the maximum speed the character can reach</param>
        /// <param name="pSlowRadius">(0.0f = no slow down) the circle radius around the target where the character starts to slow down</param>
        /// <param name="pMass">to slow down the character</param>
        /// <param name="pStopRadius">the circle radius around the target where the character stops</param>
        /// <returns>A vector2 to represent the destination velocity or a Vector2(0,0) if character is close to the target</returns>
        public Vector2 Steering_Seek(Vector2 pVelocity, Vector2 pPosition, Vector2 pTargetPosition,
                                            float pMaxSpeed = STEERING_DEFAULT_MAXSPEED, float pSlowRadius = 0.0f, float pMass = STEERING_DEFAULT_MASS, float pStopRadius = STEERING_CLOSE_DISTANCE)
        {
            // Check if we have enough distance between the character and the target
            if (pPosition.DistanceTo(pTargetPosition) <= pStopRadius)
                return Nucleus_Utils.VECTOR_0;

            // STEP 1 : use the formula to get the shortest path possible to the target
            Vector2 desire_velocity = (pTargetPosition - pPosition).Normalized() * pMaxSpeed;

            // STEP 2 : slow down the character when he is closed to the target
            if (pSlowRadius != 0.0f)
            {
                float to_target = pPosition.DistanceTo(pTargetPosition);

                // Reduce velocity if in the slow circle around the target
                if (to_target <= pSlowRadius)
                    desire_velocity *= ((to_target / pSlowRadius) * 0.8f) + 0.2f;       // 0.8f + 0.2f : used to not slow down too much the character
            }

            // STEP 3 : apply the steering formula (use mass to slow down character's movement)
            return _CalculateSteering(desire_velocity, pVelocity, pMass);
        }

        /// <summary>
        /// Calculate a velocity to move a character away from a destination (Run away)
        /// </summary>
        /// <param name="pVelocity">the actual velocity of the character</param>
        /// <param name="pPosition">the actual global position of the character</param>
        /// <param name="pTargetPosition">the destination of the character</param>
        /// <param name="pMaxSpeed">the maximum speed the character can reach</param>
        /// <param name="pFleeRadius">the circle radius where the character stop to flee</param>
        /// <param name="pMass">to slow down the character</param>
        /// <returns>A vector2 to represent the destination velocity or a Vector2(0,0) if character is away to the target</returns>
        public Vector2 Steering_Flee(Vector2 pVelocity, Vector2 pPosition, Vector2 pTargetPosition,
                                            float pMaxSpeed = STEERING_DEFAULT_MAXSPEED, float pFleeRadius = STEERING_DEFAULT_FLEE, float pMass = STEERING_DEFAULT_MASS)
        {
            // If the target is outside the radius, do nothing
            if (pPosition.DistanceTo(pTargetPosition) >= pFleeRadius)
                return Nucleus_Utils.VECTOR_0;

            // Use the formula to get the shortest path possible to run away from the target, then apply steering
            Vector2 desire_velocity = (pPosition - pTargetPosition).Normalized() * pMaxSpeed;
            return _CalculateSteering(desire_velocity, pVelocity, pMass);
        }

        /// <summary>
        /// Calculate the steering
        /// </summary>
        /// <param name="pDesiredVelocity">Calculated from the Steering Behaviour formula (seek, flee ...)</param>
        /// <param name="pVelocity">the actual velocity of the character</param>
        /// <param name="pMass">to slow down the character</param>
        /// <returns>A vector2 to represent the steering velocity</returns>
        private Vector2 _CalculateSteering(Vector2 pDesiredVelocity, Vector2 pVelocity, float pMass)
        {
            Vector2 steering = (pDesiredVelocity - pVelocity) / pMass;
            return pVelocity + steering;
        }

        /// <summary>
        /// Calculate the velocity to keep distance behind the leader
        /// </summary>
        /// <param name="pLeaderPosition">the position of the node to follow</param>
        /// <param name="pFollowerPosition">the position of the follower</param>
        /// <param name="pFollowOffset">the distance to keep between the leader and follower node</param>
        /// <returns>A vector2 to represent the position to follow with the distance to keep</returns>
        public Vector2 Steering_CalculateDistanceBetweenFollowers(Vector2 pLeaderPosition, Vector2 pFollowerPosition, float pFollowOffset)
        {
            // Get the vector direction to the leader (behind the leader)
            Vector2 direction = (pFollowerPosition - pLeaderPosition).Normalized();
            Vector2 velocity = pFollowerPosition - (direction * pFollowOffset);

            // To avoid the follower to be too close to the leader (or it will have a kind of Parkinson movement)
            if (pLeaderPosition.DistanceTo(pFollowerPosition) <= pFollowOffset)
                velocity = pFollowerPosition;

            return velocity;
        }

        /* OLD Steering_CalculateFlee (2020-01-19) - METHOD FROM GDSCRIPT

        /// /// <summary>
        /// Calculate a velocity to flee from a destination
        /// </summary>
        /// <param name="pPosition">the actual global position of the character</param>
        /// <param name="pTargetPosition">the destination of the character</param>
        /// <param name="pFleeRadius">the circle radius where the character starts to flee</param>
        /// <returns>A vector2 to represent the destination velocity</returns>
        public Vector2 Steering_CalculateFlee(Vector2 pPosition, Vector2 pTargetPosition, float pFleeRadius = STEERING_DEFAULT_FLEE)
        {
            // If the target is outside the radius, do nothing
            if (pPosition.DistanceTo(pTargetPosition) > pFleeRadius)
                return pPosition;

            Vector2 flee_global_position = pTargetPosition - (pTargetPosition - pPosition).Normalized();
            Vector2 target_position = pPosition + (pPosition - flee_global_position).Normalized() * pFleeRadius;

            return target_position;
        }

        */
    }
}