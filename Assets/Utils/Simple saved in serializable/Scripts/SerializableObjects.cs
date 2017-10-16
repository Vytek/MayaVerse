using UnityEngine;
using System.Collections;
using System;

namespace SerializableObjects {

    public static class Adapter {
        public static object ToSerializableObject(object obj) {
            if (obj == null) return null;

            if (obj is Color){
                return new SColor((Color)obj);
            }else if(obj is Vector2) {
                return new SVector2((Vector2)obj);
            }else if(obj is Vector3) {
                return new SVector3((Vector3)obj);
            }else if(obj is Vector4) {
                return new SVector4((Vector4)obj);
            }else if(obj is Quaternion) {
                return new SQuaternion((Quaternion)obj);
            }

            return obj;
        }

        public static object FormSerializableObject(object obj) {
            if (obj == null) return null;

            if (obj is SColor) {
                return((SColor)obj).getColor();
            } else if (obj is SVector2) {
                return((SVector2)obj).getVector();
            } else if (obj is SVector3) {
                return ((SVector3)obj).getVector();
            } else if (obj is SVector4) {
                return ((SVector4)obj).getVector4();
            } else if (obj is SQuaternion) {
                return ((SQuaternion)obj).getQuaternion();
            }

            return obj;
        }

    }

    [Serializable]
    public class SColor {
        float r, g, b, a;
        public SColor(Color color) {
            this.r = color.r;
            this.g = color.g;
            this.b = color.b;
            this.a = color.a;
        }

        public Color getColor() {
            return new Color(r, g, b, a);
        }
        public override string ToString() {
            return string.Format("{0}, {1}, {2}, {3}", r, g, b, a);
        }

    }

    [Serializable]
    public class SVector2 {
        float x, y;
        public SVector2(Vector2 vector2) {
            x = vector2.x;
            y = vector2.y;
        }
        public Vector2 getVector() {
            return new Vector2(x, y);
        }
        public override string ToString() {
            return string.Format("{0}, {1}", x, y);
        }
    }

    [Serializable]
    public class SVector3 {
        float x, y, z;
        public SVector3(Vector3 vector3) {
            x = vector3.x;
            y = vector3.y;
            z = vector3.z;
        }
        public SVector3(float x, float y, float z) {
            this.x = x;
            this.y = y;
            this.z = z;
        }
        public Vector3 getVector() {
            return new Vector3(x, y, z);
        }
        public override string ToString() {
            return string.Format("{0}, {1}, {2}", x, y, z);
        }
    }

    [Serializable]
    public class SVector4 {
        float x, y, z, w;
        public SVector4(Vector4 vector4) {
            x = vector4.x;
            y = vector4.y;
            z = vector4.z;
            w = vector4.w;
        }
        public Vector4 getVector4() {
            return new Vector4(x, y, z, w);
        }
        public override string ToString() {
            return string.Format("{0}, {1}, {2}, {3}", x, y, z, w);
        }
    }

    [Serializable]
    public class SQuaternion {
        float x, y, z, w;
        public SQuaternion(Quaternion quaternion) {
            x = quaternion.x;
            y = quaternion.y;
            z = quaternion.z;
            w = quaternion.w;
        }
        public Quaternion getQuaternion() {
            return new Quaternion(x, y, z, w);
        }
        public override string ToString() {
            return string.Format("{0}, {1}, {2}, {3}", x, y, z, w);
        }
    }

}