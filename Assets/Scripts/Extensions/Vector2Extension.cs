using UnityEngine;
 
 public static class Vector2Extension {
     
     public static Vector2 Rotate(this Vector2 v, float degrees) {
         float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
         float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);
         
         float tx = v.x;
         float ty = v.y;
         v.x = (cos * tx) - (sin * ty);
         v.y = (sin * tx) + (cos * ty);
         return v;
     }

    public static Vector2 FlipX(this Vector2 v) => new Vector2(-v.x,v.y);
    public static Vector2 FlipY(this Vector2 v) => new Vector2(v.x,-v.y);

    public static Vector3 FlipX(this Vector3 v) => new Vector3(-v.x,v.y,v.z);
    public static Vector3 FlipY(this Vector3 v) => new Vector3(v.x,-v.y,v.z);
    public static Vector3 FlipZ(this Vector3 v) => new Vector3(v.x,v.y,-v.z);
 }