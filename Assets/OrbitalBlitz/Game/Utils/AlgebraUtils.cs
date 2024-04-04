using OpenCover.Framework.Model;
using UnityEngine;

public class AlgebraUtils {
    public static float angleFromObjectToObject(GameObject from, GameObject to) {
        var direction = to.transform.position - from.transform.position;
        direction.y = 0;

        var forward = from.transform.forward;
        forward.y = 0;
        
        return Vector3.Angle(forward, direction);
    }
    
    /// <summary>
    /// -180 is 0 and 180 is 1
    /// </summary>
    public static float normalizeAngle(float angle) {
        return (angle + 180) / 360;
    }
}