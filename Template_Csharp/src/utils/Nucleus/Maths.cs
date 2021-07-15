using Godot;

namespace Nucleus
{
    public static class Nucleus_Maths
    {
    #region VARIABLES

        public static RandomNumberGenerator Rnd = new RandomNumberGenerator();

    #endregion

    //*-------------------------------------------------------------------------*//

    #region METHODS - RANDOM

        /// <summary>
        /// Return a int random value, avoiding 0
        /// </summary>
        /// <param name="min">min int value</param>
        /// <param name="max">max int value</param>
        /// <returns>int random value</returns>
        public static int Rndi_AvoidZero(int min = -1, int max = 1)
        {
            int number;
            do
            {
                number = Rnd.RandiRange(min, max);
            } while (number == 0);
            return number;
        }

        /// <summary>
        /// Return a float random value, avoiding 0.0f
        /// </summary>
        /// <param name="min">min float value</param>
        /// <param name="max">max float value</param>
        /// <returns>float random value</returns>
        public static float Rndf_AvoidZero(float min = -1.0f, float max = 1.0f)
        {
            float number;
            do
            {
                number = Rnd.RandfRange(min, max);
            } while (number == 0.0f);
            return number;
        }

    #endregion

    #region METHODS - GEOMETRY

        /// <summary>
        /// Get the direction vector between 2 objects
        /// </summary>
        /// <param name="pActualPosition">A vector2 representing the 1st object (eg : the player.GlobalPosition)</param>
        /// <param name="pTargetPosition">A vector2 representing the 2nd object (eg : the enemy.GlobalPosition)</param>
        /// <param name="pNormalize">set to True to return a normalized vector</param>
        /// <returns>A Vector2 to represent the direction</returns>
        public static Vector2 GetDirectionBetween_2_Objects(Vector2 pActualPosition, Vector2 pTargetPosition, bool pNormalize = true)
        {
            //Vector2 direction = VECTOR_0;
            Vector2 direction = pNormalize ? (pTargetPosition - pActualPosition).Normalized() : (pTargetPosition - pActualPosition);
            return direction;
        }

        /// <summary>
        /// Get the distance between 2 objects
        /// </summary>
        /// <param name="pActualPosition">A vector2 representing the 1st object (eg : the player.GlobalPosition)</param>
        /// <param name="pTargetPosition">A vector2 representing the 2nd object (eg : the enemy.GlobalPosition)</param>
        /// <returns>A float to represent the distance</returns>
        public static float GetDistanceBetween_2_Objects(Vector2 pActualPosition, Vector2 pTargetPosition)
        {
            return (pTargetPosition - pActualPosition).Length();
        }

        /// <summary>
        /// Get the angle between 2 objects
        /// </summary>
        /// <param name="pActualPosition">A vector2 representing the 1st object (eg : the player.GlobalPosition)</param>
        /// <param name="pTargetPosition">A vector2 representing the 2nd object (eg : the enemy.GlobalPosition)</param>
        /// <returns>A float to represent the angle in Radians</returns>
        public static float GetAngleTo(Vector2 pActualPosition, Vector2 pTargetPosition)
        {
            // Get the destination position, then the angle to the destination position
            return (pTargetPosition - pActualPosition).Angle();
        }

        /// <summary>
        /// Get the centered position of any set of sprites     Eg : get the center position of a grid made from multiple cells
        /// </summary>
        /// <param name="pTotalWidth">The total width of the set</param>
        /// <param name="pTotalHeight">The total height of the set</param>
        /// <param name="pUniqueElementSize">the size of an unique element of the set</param>
        /// <param name="pDestinationPosition">the point where the set will be displayed (typically Utils.VECTOR_0   or  GlobalPosition)</param>
        /// <returns>A vector to represent the centered coordinates</returns>
        public static Vector2 Get_CenteredPosition(float pTotalWidth, float pTotalHeight, float pUniqueElementSize, Vector2 pDestinationPosition)
        {
            // Get the centered position
            float x = pUniqueElementSize - (pUniqueElementSize / 2) - (pTotalWidth / 2);
            float y = pUniqueElementSize - (pTotalHeight / 2);

            // Add the destination point
            return new Vector2(x + pDestinationPosition.x, y + pDestinationPosition.y);
        }

    #endregion

    }
}