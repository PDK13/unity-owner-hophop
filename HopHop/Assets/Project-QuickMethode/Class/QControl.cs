using UnityEngine;
using UnityEngine.SceneManagement;

public class QApplication
{
    public static void SetTimeScale(float TimeScale = 1)
    {
        Time.timeScale = TimeScale;
    }

    public static void SetFrameRateTarget(int FrameRateTarget = 60)
    {
        Application.targetFrameRate = FrameRateTarget;
    }

    public static void SetPhysicSimulation(SimulationMode2D Mode)
    {
        //From Editor Unity Window: Edit/Project Setting/Physic 2D/Simulation Mode.
        //Mode Fixed Update: Physic will be caculated every Fixed Delta Time, after FixedUpdate methode called (By Default of Unity).
        //Mode Update: Physic will caculated every Delta Time, after every Update methode called (Higher Frame Rate, higher correct Physic caculated, but consumed more CPU resources).
        //Mode Script: Unknow?
        Physics2D.simulationMode = Mode;
    }
}

public class QScene
{
    public static void SetSceneChance(string SceneName, LoadSceneMode LoadSceneMode = LoadSceneMode.Single)
    {
        SceneManager.LoadScene(SceneName, LoadSceneMode);
    }

    public static (int Index, string Name) GetSceneCurrent()
    {
        return (GetSceneCurrentBuildIndex(), GetSceneCurrentName());
    }

    public static string GetSceneCurrentName()
    {
        return SceneManager.GetActiveScene().name;
    }

    public static int GetSceneCurrentBuildIndex()
    {
        return SceneManager.GetActiveScene().buildIndex;
    }
}

public class QControl
{
    #region ==================================== Mouse

    public static void SetMouseVisible(bool MouseVisble)
    {
        UnityEngine.Cursor.visible = MouseVisble;
    }

    #endregion

    #region ==================================== Keyboard

    public static string GetKeyboardSimple(KeyCode KeyCode)
    {
        switch (KeyCode)
        {
            case KeyCode.Escape:
                return "Esc";
            case KeyCode.Return:
                return "Enter";
            case KeyCode.Delete:
                return "Del";
            case KeyCode.Backspace:
                return "B-Space";

            case KeyCode.Mouse0:
                return "L-Mouse";
            case KeyCode.Mouse1:
                return "R-Mouse";
            case KeyCode.Mouse2:
                return "M-Mouse";

            case KeyCode.LeftBracket:
                return "[";
            case KeyCode.RightBracket:
                return "]";

            case KeyCode.LeftCurlyBracket:
                return "{";
            case KeyCode.RightCurlyBracket:
                return "}";

            case KeyCode.LeftParen:
                return "(";
            case KeyCode.RightParen:
                return ")";

            case KeyCode.LeftShift:
                return "L-Shift";
            case KeyCode.RightShift:
                return "R-Shift";

            case KeyCode.LeftAlt:
                return "L-Alt";
            case KeyCode.RightAlt:
                return "R-Alt";

            case KeyCode.PageUp:
                return "Page-U";
            case KeyCode.PageDown:
                return "Page-D";
        }

        return KeyCode.ToString();
    }

    #endregion

    #region ==================================== Device

    #region Android - Vibrator

    public static bool VibrateHandle = true;

#if UNITY_ANDROID
#if UNITY_EDITOR
        public static AndroidJavaClass unityPlayer;
        public static AndroidJavaObject currentActivity;
        public static AndroidJavaObject vibrator;
#else
        public static AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        public static AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        public static AndroidJavaObject vibrator = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");
#endif
#endif

    public static void SetDeviceVibrate()
    {
#if UNITY_ANDROID
            if (VibrateHandle)
            {
                Handheld.Vibrate();
            }
            else
            {
                if (Application.platform == RuntimePlatform.Android && !Application.isEditor)
                    vibrator.Call("vibrate");
                else
                    Handheld.Vibrate();
            }
#endif
    }

    public static void SetDeviceVibrate(float TimeMilisecond)
    {
#if UNITY_ANDROID
            if (VibrateHandle)
            {
                Handheld.Vibrate();
            }
            else
            {
                if (Application.platform == RuntimePlatform.Android && !Application.isEditor)
                    vibrator.Call("vibrate", TimeMilisecond);
                else
                    Handheld.Vibrate();
            }
#endif
    }

    public static void SetDeviceVibrate(float[] Pattern, int Repeat)
    {
#if UNITY_ANDROID
            if (VibrateHandle)
            {
                Handheld.Vibrate();
            }
            else
            {
                if (Application.platform == RuntimePlatform.Android && !Application.isEditor)
                    vibrator.Call("vibrate", Pattern, Repeat);
                else
                    Handheld.Vibrate();
            }
#endif
    }

    public static void SetDeviceVibrateCancel()
    {
#if UNITY_ANDROID
            if (VibrateHandle)
            {
                //...
            }
            else
            {
                if (Application.platform == RuntimePlatform.Android && !Application.isEditor)
                    vibrator.Call("cancel");
            }
#endif
    }

    #endregion

    #endregion
}