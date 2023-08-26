using System;
using UnityEngine.Scripting;

namespace UnityEngine
{
    /// <summary>
    /// Representation of 2D vectors and points using integers.
    /// </summary>
    public struct Vector2Int
    {
        private int m_X;

        private int m_Y;

        private static readonly Vector2Int s_Zero = new Vector2Int(0, 0);

        private static readonly Vector2Int s_One = new Vector2Int(1, 1);

        private static readonly Vector2Int s_Up = new Vector2Int(0, 1);

        private static readonly Vector2Int s_Down = new Vector2Int(0, -1);

        private static readonly Vector2Int s_Left = new Vector2Int(-1, 0);

        private static readonly Vector2Int s_Right = new Vector2Int(1, 0);

        /// <summary>
        /// X component of the vector
        /// </summary>
        public int x
        {
            get
            {
                return m_X;
            }
            set
            {
                m_X = value;
            }
        }
        /// <summary>
        /// Y component of the vector
        /// </summary>
        public int y
        {
            get
            {
                return m_Y;
            }
            set
            {
                m_Y = value;
            }
        }

        /// inherentdoc
        public int this[int index]
        {
            get
            {
                return index switch
                {
                    0 => x,
                    1 => y,
                    _ => throw new IndexOutOfRangeException($"Invalid Vector2Int index addressed: {index}!"),
                };
            }
            set
            {
                switch (index)
                {
                    case 0:
                        x = value;
                        break;
                    case 1:
                        y = value;
                        break;
                    default:
                        throw new IndexOutOfRangeException($"Invalid Vector2Int index addressed: {index}!");
                }
            }
        }

        /// <summary>
        /// Returns the length of this vector (Read Only).
        /// </summary>
        public float magnitude => Mathf.Sqrt(x * x + y * y);

        /// <summary>
        /// Returns the squared length of this vector (Read Only).
        /// </summary>
        public int sqrMagnitude => x * x + y * y;

        /// <summary>
        /// Shorthand for writing Vector2Int (0, 0).
        /// </summary>
        public static Vector2Int zero => s_Zero;

        /// <summary>
        /// Shorthand for writing Vector2Int (1, 1).
        /// </summary>
        public static Vector2Int one => s_One;

        /// <summary>
        /// Shorthand for writing Vector2Int (0, 1).
        /// </summary>
        public static Vector2Int up => s_Up;

        /// <summary>
        /// Shorthand for writing Vector2Int (0, -1).
        /// </summary>
        public static Vector2Int down => s_Down;

        /// <summary>
        /// Shorthand for writing Vector2Int (-1, 0).
        /// </summary>
        public static Vector2Int left => s_Left;

        /// <summary>
        /// Shorthand for writing Vector2Int (1, 0).
        /// </summary>
        public static Vector2Int right => s_Right;

        /// inherentdoc
        public Vector2Int(int x, int y)
        {
            m_X = x;
            m_Y = y;
        }

        /// <summary>
        /// Set x and y components of an existing Vector2Int.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void Set(int x, int y)
        {
            m_X = x;
            m_Y = y;
        }

        /// <summary>
        /// Returns the distance between a and b.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static float Distance(Vector2Int a, Vector2Int b)
        {
            return (a - b).magnitude;
        }
        
        /// <summary>
        /// Returns a vector that is made form the smallest components of two vectors.
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static Vector2Int Min(Vector2Int lhs, Vector2Int rhs)
        {
            return new Vector2Int(Mathf.Min(lhs.x, rhs.x), Mathf.Min(lhs.y, rhs.y));
        }

        /// <summary>
        /// Returns a vector that is made form the largest componets of two vectors.
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static Vector2Int Max(Vector2Int lhs, Vector2Int rhs)
        {
            return new Vector2Int(Mathf.Max(lhs.x, rhs.x), Mathf.Max(lhs.y, rhs.y));
        }

        /// <summary>
        /// Multiplies the two vectors component-wise.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Vector2Int Scale(Vector2Int a, Vector2Int b)
        {
            return new Vector2Int(a.x * b.x, a.y * b.y);
        }

        /// <summary>
        /// Multiplies every component of this vector by the same component of scale.
        /// </summary>
        /// <param name="scale"></param>
        public void Scale(Vector2Int scale)
        {
            x *= scale.x;
            y *= scale.y;
        }

        /// <summary>
        /// Clamps the Vector2Int to the bounds given by min and max
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public void Clamp(Vector2Int min, Vector2Int max)
        {
            x = Math.Max(min.x, x);
            x = Math.Min(max.x, x);
            y = Math.Max(min.y, y);
            y = Math.Min(max.y, y);
        }

        /// inherentdoc
        public static implicit operator Vector2(Vector2Int v)
        {
            return new Vector2(v.x, v.y);
        }

        /// <summary>
        /// Converts a Vector2 to a Vector2Int by doing a Floor on each value.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Vector2Int FloorToInt(Vector2 v)
        {
            return new Vector2Int(Mathf.FloorToInt(v.x), Mathf.FloorToInt(v.y));
        }

        /// <summary>
        /// Converts a Vector2 to a Vector2Int by doing a Ceiling on each value.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Vector2Int CeilToInt(Vector2 v)
        {
            return new Vector2Int(Mathf.CeilToInt(v.x), Mathf.CeilToInt(v.y));
        }
        /// <summary>
        /// Converts a Vector2 to a Vector2Int by doing a Round on each value.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static Vector2Int RoundToInt(Vector2 v)
        {
            return new Vector2Int(Mathf.RoundToInt(v.x), Mathf.RoundToInt(v.y));
        }
        /// inherentdoc 
        public static Vector2Int operator +(Vector2Int a, Vector2Int b)
        {
            return new Vector2Int(a.x + b.x, a.y + b.y);
        }
        /// inherentdoc 
        public static Vector2Int operator -(Vector2Int a, Vector2Int b)
        {
            return new Vector2Int(a.x - b.x, a.y - b.y);
        }
        /// inherentdoc 
        public static Vector2Int operator *(Vector2Int a, Vector2Int b)
        {
            return new Vector2Int(a.x * b.x, a.y * b.y);
        }
        /// inherentdoc 
        public static Vector2Int operator *(Vector2Int a, int b)
        {
            return new Vector2Int(a.x * b, a.y * b);
        }
        /// inherentdoc 
        public static bool operator ==(Vector2Int lhs, Vector2Int rhs)
        {
            return lhs.x == rhs.x && lhs.y == rhs.y;
        }
        /// inherentdoc 
        public static bool operator !=(Vector2Int lhs, Vector2Int rhs)
        {
            return !(lhs == rhs);
        }

        /// <summary>
        /// Returns true if the objects are equal.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public override bool Equals(object other)
        {
            if (!(other is Vector2Int))
            {
                return false;
            }

            Vector2Int vector2Int = (Vector2Int)other;
            return x.Equals(vector2Int.x) && y.Equals(vector2Int.y);
        }

        /// <summary>
        /// Gets the hash code for the Vector2Int.
        /// </summary>
        /// <returns>The hash code of the Vector2Int</returns>

        public override int GetHashCode()
        {
            return x.GetHashCode() ^ (y.GetHashCode() << 2);
        }
        
        /// <summary>
        /// Returns a nicely formatted string for this vector.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("({0}, {1})", new object[2] { x, y });
        }
    }
}