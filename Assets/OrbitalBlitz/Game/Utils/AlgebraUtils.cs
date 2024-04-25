using UnityEngine;

public class AlgebraUtils {
    public static float angleFromObjectToObject(GameObject from, GameObject to) {
        var direction = to.transform.position - from.transform.position;
        direction.y = 0;

        var forward = from.transform.forward;
        forward.y = 0;
        
        return Vector3.Angle(forward, direction);
    }
    
    public static float SignedAngleBetween(Vector3 a, Vector3 b, Vector3 normal, bool normalize = false){
        // angle in [0,180]
        float angle = Vector3.Angle(a,b);
        float sign = Mathf.Sign(Vector3.Dot(normal,Vector3.Cross(a,b)));

        // angle in [-179,180]
        float signed_angle = angle * sign;

        return normalize ? normalizeAngle(signed_angle) : signed_angle;
    }
    
    public static float SignedAngleToObject(GameObject from, GameObject to, bool normalize = false){
        var direction = to.transform.position - from.transform.position;
        return SignedAngleBetween(from.transform.forward, direction, from.transform.up, normalize) ;
    }
    
    /// <summary>
    /// -180 is 0 and 180 is 1
    /// </summary>
    public static float normalizeAngle(float angle) {
        var normalizeď = (angle + 180) / 360;
        // Debug.Log($"Normalized angle {angle} to {normalizeď}");
        return normalizeď;
    }
}