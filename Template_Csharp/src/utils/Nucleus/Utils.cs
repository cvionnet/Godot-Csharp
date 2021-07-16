using System;
using Godot;
using Serilog;

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
        public static StateMachine_Template StateMachine_Template { get; set; }
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
            Finalize_Serilog();
        }

    #endregion

    #region METHODS - LOG

        /// <summary>
        /// Initialize the Serilog Logger
        /// </summary>
        private static void Initialize_Serilog()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
#if GODOT_WINDOWS || GODOT_X11 || GODOT_OSX || GODOT_ANDROID || GODOT_IOS || GODOT_SERVER
//! TODO test it on Linux / OSX / Mobile
            // Only use Serilog on Windows  (not working with HTML5)
                .WriteTo.Console(outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception} {Properties:j}")
#endif
                .CreateLogger();
        }

        /// <summary>
        /// Finalize the Serilog Logger
        /// </summary>
        public static void Finalize_Serilog() => Log.CloseAndFlush();

        /// <summary>
        /// Print system information at startup
        /// </summary>
        private static void Initialize_Log_System()
        {
            // Godot OS options : https://docs.godotengine.org/en/stable/classes/class_os.html
            Info($"Game Name : { ProjectSettings.GetSetting("application/config/name") } ({_gameShortName})", false);
            Info($"Debug Build Internal : { DEBUG_MODE.ToString().ToUpper() } / Godot : { OS.IsDebugBuild().ToString().ToUpper() }", false);
            Info($"Id Unique : { _uniqueId }", false);
            Info($"Time : { OS.GetTime(true) } UTC / { OS.GetTime(false) } Local", false);

            Info($"System : { OS.GetName() }", false);
            Info($"CPU Number of cores : { OS.GetProcessorCount()/2 } / Memory : { (OS.GetStaticMemoryUsage() / 1024).ToString() } Go", false);
#if GODOT_WINDOWS || GODOT_X11 || GODOT_OSX || GODOT_ANDROID || GODOT_IOS || GODOT_SERVER
            Info($"Power Type : { OS.GetPowerState() } / Left : { OS.GetPowerSecondsLeft() }", false);
#endif

            Info($"Video Driver : { OS.GetCurrentVideoDriver() } / Screen size { OS.GetScreenSize() } / Game screen size { OS.GetRealWindowSize() }", true);

            Info($"Mobile Model : { OS.GetModelName() }", false);
        }

        /// <summary>
        /// Display an information message using Serilog
        /// ⚠️ ONLY if DEBUG_MODE = true
        /// </summary>
        /// <param name="pMessage">text to display</param>
        /// <param name="pPrintToGodotConsole">True = display message on Godot Editor Output window</param>
        public static void Info(string pMessage, bool pPrintToGodotConsole = true)
        {
            if (DEBUG_MODE)
            {
#if GODOT_WINDOWS || GODOT_X11 || GODOT_OSX || GODOT_ANDROID || GODOT_IOS || GODOT_SERVER
                if (pPrintToGodotConsole) GD.Print($"[INF]{pMessage}");
                Log.Information($"[{_gameShortName}][{_uniqueId}]{pMessage}");
#else
                GD.Print($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} [INF][{_gameShortName}][{_uniqueId}]{pMessage}");
#endif
            }
        }

        /// <summary>
        /// Display a debug message using Serilog
        /// </summary>
        /// <param name="pMessage">text to display</param>
        /// <param name="pPrintToGodotConsole">True = display message on Godot Editor Output window</param>
        public static void Debug(string pMessage, bool pPrintToGodotConsole = true)
        {
#if GODOT_WINDOWS || GODOT_X11 || GODOT_OSX || GODOT_ANDROID || GODOT_IOS || GODOT_SERVER
            if (pPrintToGodotConsole) GD.Print($"[DBG]{pMessage}");
            Log.Debug($"[{_gameShortName}]{pMessage}");
#else
            GD.Print($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} [DBG][{_gameShortName}][{_uniqueId}]{pMessage}");
#endif
        }

        /// <summary>
        /// Display a debug message using Serilog
        /// </summary>
        /// <param name="pMessage">text to display</param>
        /// <param name="pException">the exception raised</param>
        /// <param name="pPrintToGodotConsole">True = display message on Godot Editor Output window</param>
        public static void Debug(string pMessage, Exception pException, bool pPrintToGodotConsole = true)
        {
#if GODOT_WINDOWS || GODOT_X11 || GODOT_OSX || GODOT_ANDROID || GODOT_IOS || GODOT_SERVER
            if (pPrintToGodotConsole) GD.Print($"[DBG]{pMessage} - {pException}");
            Log.Debug(pException, $"[{_gameShortName}]{pMessage}");
#else
            GD.Print($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} [DBG][{_gameShortName}][{_uniqueId}]{pMessage} - {pException}");
#endif
        }

        /// <summary>
        /// Display an error message using Serilog
        /// </summary>
        /// <param name="pMessage">text to display</param>
        /// <param name="pException">the exception raised</param>
        /// <param name="pPrintToGodotConsole">True = display message on Godot Editor Output window</param>
        public static void Error(string pMessage, Exception pException, bool pPrintToGodotConsole = true)
        {
#if GODOT_WINDOWS || GODOT_X11 || GODOT_OSX || GODOT_ANDROID || GODOT_IOS || GODOT_SERVER
            if (pPrintToGodotConsole) GD.PrintErr($"[ERR]{pMessage} - {pException}");
            Log.Error(pException, $"[{_gameShortName}]{pMessage}");
#else
            GD.Print($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} [ERR][{_gameShortName}][{_uniqueId}]{pMessage} - {pException}");
#endif
        }

        /// <summary>
        /// Display a fatal error message using Serilog
        /// </summary>
        /// <param name="pMessage">text to display</param>
        /// <param name="pException">the exception raised</param>
        /// <param name="pPrintToGodotConsole">True = display message on Godot Editor Output window</param>
        public static void Fatal(string pMessage, Exception pException, bool pPrintToGodotConsole = true)
        {
#if GODOT_WINDOWS || GODOT_X11 || GODOT_OSX || GODOT_ANDROID || GODOT_IOS || GODOT_SERVER
            if (pPrintToGodotConsole) GD.PrintErr($"[FTL]{pMessage} - {pException}");
            Log.Fatal(pException, $"[{_gameShortName}]{pMessage}");
#else
            GD.Print($"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")} [FTL][{_gameShortName}][{_uniqueId}]{pMessage} - {pException}");
#endif
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