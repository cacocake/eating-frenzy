using UnityEngine;


public class Utils : MonoBehaviour {
    public enum HapticType {
        ConsumedObject,
        ActivateVibrationSetting,
        LevelUp
    }

    public static Vector3 GetMovementWithEdgeCollisionCheck(Vector3 currentPosition, Vector3 movement) {
        RaycastHit[] playerMovementHits = new RaycastHit[1];
        if (Physics.RaycastNonAlloc(currentPosition, movement.normalized, playerMovementHits, movement.magnitude, LayerMask.GetMask("Edges")) > 0) {
            movement = Vector3.ProjectOnPlane(movement, playerMovementHits[0].normal);
            // Second ray to see if the projected movement will hit another edge to avoid issues against corners
            if(Physics.RaycastNonAlloc(currentPosition, movement.normalized, playerMovementHits, movement.magnitude, LayerMask.GetMask("Edges")) > 0) {
                movement = Vector3.ProjectOnPlane(movement, playerMovementHits[0].normal);
            }
        }
        return movement;
    }

    public static void ExecuteHapticVibration(HapticType type) {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject currentActivity = null;
        AndroidJavaObject vibrator = null;
        if(unityPlayer != null) {
            currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        }
        
        if(currentActivity != null) {
            vibrator = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");
        }

        if(vibrator == null) {
            Handheld.Vibrate();
        }

        long[] pattern = GetHapticPattern(type);
        vibrator.Call("vibrate", pattern, -1);
#else
        Handheld.Vibrate();
#endif
    }

    private static long[] GetHapticPattern(HapticType type) {
        switch (type) {
            case HapticType.ConsumedObject:
                return new long[] { 0, 20 };
            case HapticType.ActivateVibrationSetting:
                return new long[] { 0, 30, 50, 50 };
            case HapticType.LevelUp:
                return new long[] { 0, 10, 20, 30, 0, 40, 0, 50, 0 };
            default:
                Debug.LogError("Unsupported Haptic Pattern! Using a default vibration.");
                return new long[] { 50 };
        }
    }

}
    
