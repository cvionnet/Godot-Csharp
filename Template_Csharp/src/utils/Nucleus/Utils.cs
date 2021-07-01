using Godot;

namespace Nucleus
{
    /// <summary>
    /// Class that contains all variables and methods usefull between projects
    /// </summary>
    public static class Nucleus_Utils
    {
    #region VARIABLES

        public const bool DEBUG_MODE = true;

        public static float ScreenWidth;
        public static float ScreenHeight;

        private const float _zero = 0.0f;

        public static Vector2 VECTOR_0 = new Vector2(0.0f,0.0f);         // (=Vector2.ZERO in GDScript)
        public static Vector2 VECTOR_1 = new Vector2(1.0f,1.0f);
        public static Vector2 VECTOR_INF = new Vector2(1.0f/_zero,1.0f/_zero);    // infinite vector  (=Vector2.INF in GDScript)
        public static Vector2 VECTOR_FLOOR = new Vector2(0,-1);          // (=Vector2.UP in GDScript) Use it for plateformer

        public static RandomNumberGenerator Rnd = new RandomNumberGenerator();

        // References to state machines & StateManager
        public static StateMachine_Template StateMachine_Template { get; set; }
        public static StateManager State_Manager;

    #endregion

    //*-------------------------------------------------------------------------*//

    #region METHODS - GENERIC

        /// <summary>
        /// Call this method in the 1st scene of the game   ( Utils.Initialize_Utils(GetViewport()); )
        /// </summary>
        /// <param name="pGame">The viewport of the scene</param>
        public static void Initialize_Utils(Viewport pGame)
        {
            Rnd.Randomize();

            ScreenWidth = pGame.Size.x;
            ScreenHeight = pGame.Size.y;

            DebugPrint("-> Screen size = " + ScreenWidth + " / " + ScreenHeight);
        }

        /// <summary>
        /// To display a text in the Editor's debug panel ONLY if DEBUG_MODE = true
        /// </summary>
        /// <param name="pText">The text to display</param>
        public static void DebugPrint(string pText)
        {
            if (DEBUG_MODE)
                GD.Print(pText);
        }
    #endregion

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


    }
}