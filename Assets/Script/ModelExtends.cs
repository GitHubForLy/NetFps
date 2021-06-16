using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pt = ProtobufProto.Model;

namespace Game
{
    public static class ModelExtends
    {
        public static Vector3 ToUnityVector(this Pt.Vector3 vector)
        {
            return new Vector3
            {
                x = vector.X,
                y = vector.Y,
                z = vector.Z
            };
        }

        public static Pt.Vector3 ToMVector(this Vector3 vector)
        {
            return new Pt.Vector3
            {
                X = vector.x,
                Y = vector.y,
                Z = vector.z
            };
        }

        public static string ToRawString(this Vector3 vector3)
        {
            return $"({vector3.x},{vector3.y},{vector3.z})";
        }
        public static string ToRawString(this Vector3 vector3,int dot)
        {
            return string.Format("({0:F" + dot + "},{1:F" + dot + "},{2:F" + dot + "})",vector3.x,vector3.y,vector3.z);
        }

        public static string ToRawString(this Vector2 vector3)
        {
            return $"({vector3.x},{vector3.y})";
        }
        public static string ToRawString(this Vector2 vector3, int dot)
        {
            return string.Format("({0:F" + dot + "},{1:F" + dot + "})", vector3.x, vector3.y);
        }
    }


}
