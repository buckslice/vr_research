using UnityEngine;
using UnityEngine.Networking;

public class NUtils {

    public static void LogNetworkError(byte error) {
        if (error != (byte)NetworkError.Ok) {
            NetworkError nerror = (NetworkError)error;
            Debug.Log("Error: " + nerror.ToString());
        }
    }
}
