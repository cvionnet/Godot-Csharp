using System;
using Godot;

namespace Nucleus
{
    /// <summary>
    /// Class that contains all variables and methods usefull between projects
    /// </summary>
    public static class Nucleus_Utils
    {
    #region VARIABLES

        public static bool DEBUG_MODE { get; } = true;

        public static float ScreenWidth { get; private set; }
        public static float ScreenHeight { get; private set; }

        private const float _zero = 0.0f;

        public static Vector2 VECTOR_0 { get; } = new Vector2(0.0f,0.0f);         // (=Vector2.ZERO in GDScript)
        public static Vector2 VECTOR_1 { get; } = new Vector2(1.0f,1.0f);
        public static Vector2 VECTOR_INF { get; } = new Vector2(1.0f/_zero,1.0f/_zero);    // infinite vector  (=Vector2.INF in GDScript)
        public static Vector2 VECTOR_FLOOR { get; } = new Vector2(0,-1);          // (=Vector2.UP in GDScript) Use it for plateformer

        // References to state machines & StateManager
        public static StateManager State_Manager { get; set; }

        private static string _gameShortName;
        private static string _uniqueId;

    #endregion

    //*-------------------------------------------------------------------------*//

    #region METHODS - GENERIC

        /// <summary>
        /// Call this method in the 1st scene of the game   ( Utils.Initialize_Utils(GetViewport()); )
        /// </summary>
        /// <param name="pGame">The viewport of the scene</param>
        public static void Initialize_Utils(Viewport pGame)
        {
            Initialize_Serilog();

            Nucleus_Maths.Rnd.Randomize();

            _gameShortName = ProjectSettings.GetSetting("application/config/description").ToString();
            _uniqueId = Guid.NewGuid().ToString();      // generate a unique ID to be able to follow logs of the session

            ScreenWidth = pGame.Size.x;
            ScreenHeight = pGame.Size.y;

            Initialize_Log_System();
        }

        /// <summary>
        /// Actions to perform when the game is exited
        /// </summary>
        public static void Finalize_Utils()
        {
            Info("User has quit the game");
        }

    #endregion

    #region METHODS - LOG

        /// <summary>
        /// Initialize the Serilog Logger
        /// </summary>
        private static void Initialize_Serilog()
        {
            // Address to Loki server
            //var credentials = new BasicAuthCredentials("https://logs-prod-us-central1.grafana.net", "71119", "eyJrIjoiOTU3OTA4OWUyZjFkNjNkMzdjNTA3MmE2MmExMzllM2EwMDk1NjkxYSIsIm4iOiJMb2tpQVBJS2V5IiwiaWQiOjUxMzI3N30=");
        }

        /// <summary>
        /// Print system information at startup
        /// </summary>
        private static void Initialize_Log_System()
        {
            // Godot OS options : https://docs.godotengine.org/en/stable/classes/class_os.html
            Info($"Game Name : { ProjectSettings.GetSetting("application/config/name") } ({_gameShortName})");
            Info($"Debug Build Internal : { DEBUG_MODE.ToString().ToUpper() } / Godot : { OS.IsDebugBuild().ToString().ToUpper() }");
            Info($"Id Unique : { _uniqueId }");
            Info($"Time : { OS.GetTime(true) } UTC / { OS.GetTime(false) } Local");

            Info($"System : { OS.GetName() }");
            Info($"CPU Number of cores : { OS.GetProcessorCount()/2 } / Memory : { (OS.GetStaticMemoryUsage() / 1024).ToString() } Go");
#if GODOT_WINDOWS || GODOT_X11 || GODOT_OSX || GODOT_ANDROID || GODOT_IOS || GODOT_SERVER
            Info($"Power Type : { OS.GetPowerState() } / Left : { OS.GetPowerSecondsLeft() }");
#endif

            Info($"Video Driver : { OS.GetCurrentVideoDriver() } / Screen size { OS.GetScreenSize() } / Game screen size { OS.GetRealWindowSize() }");

            Info($"Mobile Model : { OS.GetModelName() }");
            Info("---------------------------------------------------------------------------------------------", pWriteToDBLog: false);
        }

        /// <summary>
        /// Display an information message
        /// ⚠️ ONLY if DEBUG_MODE = true
        /// </summary>
        /// <param name="pMessage">Text to display</param>
        /// <param name="pClassName">Name of the class  (GetType().Name)</param>
        /// <param name="pMethodName">Name of the method  (MethodBase.GetCurrentMethod().Name)</param>
        /// <param name="pWriteToDBLog">True = write the message in the database log system</param>
        public static void Info(string pMessage, string pClassName = "", string pMethodName = "", bool pWriteToDBLog = true)
        {
            if (DEBUG_MODE)
            {
                if (pClassName != "") pClassName = string.Concat("[C:", pClassName, "]");
                if (pMethodName != "") pMethodName = string.Concat("[M:", pMethodName, "]");
                GD.Print($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} [INF][{_gameShortName}][{_uniqueId}]{pClassName}{pMethodName}{pMessage}");
                //if (pWriteToDBLog) TODO write to Loki server
            }
        }

        /// <summary>
        /// Display a debug message
        /// </summary>
        /// <param name="pMessage">Text to display</param>
        /// <param name="pClassName">Name of the class  (GetType().Name)</param>
        /// <param name="pMethodName">Name of the method  (MethodBase.GetCurrentMethod().Name)</param>
        /// <param name="pWriteToDBLog">True = write the message in the database log system</param>
        public static void Debug(string pMessage, string pClassName = "", string pMethodName = "", bool pWriteToDBLog = true)
        {
            if (pClassName != "") pClassName = string.Concat("[C:", pClassName, "]");
            if (pMethodName != "") pMethodName = string.Concat("[M:", pMethodName, "]");
            GD.Print($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} [DBG][{_gameShortName}][{_uniqueId}]{pClassName}{pMethodName}{pMessage}");
            //if (pWriteToDBLog) TODO write to Loki server
        }

        /// <summary>
        /// Display a debug message using Serilog
        /// </summary>
        /// <param name="pMessage">Text to display</param>
        /// <param name="pException">The exception raised</param>
        /// <param name="pClassName">Name of the class  (GetType().Name)</param>
        /// <param name="pMethodName">Name of the method  (MethodBase.GetCurrentMethod().Name)</param>
        /// <param name="pWriteToDBLog">True = write the message in the database log system</param>
        public static void Debug(string pMessage, Exception pException, string pClassName = "", string pMethodName = "", bool pWriteToDBLog = true)
        {
            if (pClassName != "") pClassName = string.Concat("[C:", pClassName, "]");
            if (pMethodName != "") pMethodName = string.Concat("[M:", pMethodName, "]");
            GD.Print($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} [DBG][{_gameShortName}][{_uniqueId}]{pClassName}{pMethodName}{pMessage} - {pException}");
            //if (pWriteToDBLog) TODO write to Loki server
        }

        /// <summary>
        /// Display an error message using Serilog
        /// </summary>
        /// <param name="pMessage">Text to display</param>
        /// <param name="pException">The exception raised</param>
        /// <param name="pClassName">Name of the class  (GetType().Name)</param>
        /// <param name="pMethodName">Name of the method  (MethodBase.GetCurrentMethod().Name)</param>
        /// <param name="pWriteToDBLog">True = write the message in the database log system</param>
        public static void Error(string pMessage, Exception pException, string pClassName = "", string pMethodName = "", bool pWriteToDBLog = true)
        {
            if (pClassName != "") pClassName = string.Concat("[C:", pClassName, "]");
            if (pMethodName != "") pMethodName = string.Concat("[M:", pMethodName, "]");
            GD.Print($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} [ERR][{_gameShortName}][{_uniqueId}]{pClassName}{pMethodName}{pMessage} - {pException}");
            //if (pWriteToDBLog) TODO write to Loki server
        }

        /// <summary>
        /// Display a fatal error message using Serilog
        /// </summary>
        /// <param name="pMessage">Text to display</param>
        /// <param name="pException">The exception raised</param>
        /// <param name="pClassName">Name of the class  (GetType().Name)</param>
        /// <param name="pMethodName">Name of the method  (MethodBase.GetCurrentMethod().Name)</param>
        /// <param name="pWriteToDBLog">True = write the message in the database log system</param>
        public static void Fatal(string pMessage, Exception pException, string pClassName = "", string pMethodName = "", bool pWriteToDBLog = true)
        {
            if (pClassName != "") pClassName = string.Concat("[C:", pClassName, "]");
            if (pMethodName != "") pMethodName = string.Concat("[M:", pMethodName, "]");
            GD.Print($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} [FTL][{_gameShortName}][{_uniqueId}]{pClassName}{pMethodName}{pMessage} - {pException}");
            //if (pWriteToDBLog) TODO write to Loki server
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