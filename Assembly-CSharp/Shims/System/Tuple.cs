using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace System
{
    /// <summary>
    /// Helper so we can call some tuple methods recursively without knowing the underlying types.
    /// </summary>
    internal interface ITupleInternal : ITuple
    {
        string ToString(StringBuilder sb);
        int GetHashCode(IEqualityComparer comparer);
    }

    /// <summary>
    /// Provides static methods for creating tuple objects. To browse the .NET Framework source code for this type, see the Reference Source.
    /// </summary>
    public static class Tuple
    {
        /// <summary>Creates a new struct 1-tuple, or singleton.</summary>
        /// <typeparam name="T1">The type of the first component of the tuple.</typeparam>
        /// <param name="item1">The value of the first component of the tuple.</param>
        /// <returns>A 1-tuple (singleton) whose value is (item1).</returns>
        public static Tuple<T1> Create<T1>(T1 item1) =>
            new Tuple<T1>(item1);

        /// <summary>Creates a new struct 2-tuple, or pair.</summary>
        /// <typeparam name="T1">The type of the first component of the tuple.</typeparam>
        /// <typeparam name="T2">The type of the second component of the tuple.</typeparam>
        /// <param name="item1">The value of the first component of the tuple.</param>
        /// <param name="item2">The value of the second component of the tuple.</param>
        /// <returns>A 2-tuple (pair) whose value is (item1, item2).</returns>
        public static Tuple<T1, T2> Create<T1, T2>(T1 item1, T2 item2) =>
            new Tuple<T1, T2>(item1, item2);

        /// <summary>Creates a new struct 3-tuple, or triple.</summary>
        /// <typeparam name="T1">The type of the first component of the tuple.</typeparam>
        /// <typeparam name="T2">The type of the second component of the tuple.</typeparam>
        /// <typeparam name="T3">The type of the third component of the tuple.</typeparam>
        /// <param name="item1">The value of the first component of the tuple.</param>
        /// <param name="item2">The value of the second component of the tuple.</param>
        /// <param name="item3">The value of the third component of the tuple.</param>
        /// <returns>A 3-tuple (triple) whose value is (item1, item2, item3).</returns>
        public static Tuple<T1, T2, T3> Create<T1, T2, T3>(T1 item1, T2 item2, T3 item3) =>
            new Tuple<T1, T2, T3>(item1, item2, item3);

        /// <summary>Creates a new struct 4-tuple, or quadruple.</summary>
        /// <typeparam name="T1">The type of the first component of the tuple.</typeparam>
        /// <typeparam name="T2">The type of the second component of the tuple.</typeparam>
        /// <typeparam name="T3">The type of the third component of the tuple.</typeparam>
        /// <typeparam name="T4">The type of the fourth component of the tuple.</typeparam>
        /// <param name="item1">The value of the first component of the tuple.</param>
        /// <param name="item2">The value of the second component of the tuple.</param>
        /// <param name="item3">The value of the third component of the tuple.</param>
        /// <param name="item4">The value of the fourth component of the tuple.</param>
        /// <returns>A 4-tuple (quadruple) whose value is (item1, item2, item3, item4).</returns>
        public static Tuple<T1, T2, T3, T4> Create<T1, T2, T3, T4>(T1 item1, T2 item2, T3 item3, T4 item4) =>
            new Tuple<T1, T2, T3, T4>(item1, item2, item3, item4);

        /// <summary>Creates a new struct 5-tuple, or quintuple.</summary>
        /// <typeparam name="T1">The type of the first component of the tuple.</typeparam>
        /// <typeparam name="T2">The type of the second component of the tuple.</typeparam>
        /// <typeparam name="T3">The type of the third component of the tuple.</typeparam>
        /// <typeparam name="T4">The type of the fourth component of the tuple.</typeparam>
        /// <typeparam name="T5">The type of the fifth component of the tuple.</typeparam>
        /// <param name="item1">The value of the first component of the tuple.</param>
        /// <param name="item2">The value of the second component of the tuple.</param>
        /// <param name="item3">The value of the third component of the tuple.</param>
        /// <param name="item4">The value of the fourth component of the tuple.</param>
        /// <param name="item5">The value of the fifth component of the tuple.</param>
        /// <returns>A 5-tuple (quintuple) whose value is (item1, item2, item3, item4, item5).</returns>
        public static Tuple<T1, T2, T3, T4, T5> Create<T1, T2, T3, T4, T5>(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5) =>
            new Tuple<T1, T2, T3, T4, T5>(item1, item2, item3, item4, item5);

        // From System.Web.Util.HashCodeCombiner
        internal static int CombineHashCodes(int h1, int h2)
        {
            return (((h1 << 5) + h1) ^ h2);
        }

        internal static int CombineHashCodes(int h1, int h2, int h3)
        {
            return CombineHashCodes(CombineHashCodes(h1, h2), h3);
        }

        internal static int CombineHashCodes(int h1, int h2, int h3, int h4)
        {
            return CombineHashCodes(CombineHashCodes(h1, h2), CombineHashCodes(h3, h4));
        }

        internal static int CombineHashCodes(int h1, int h2, int h3, int h4, int h5)
        {
            return CombineHashCodes(CombineHashCodes(h1, h2, h3, h4), h5);
        }

        internal static int CombineHashCodes(int h1, int h2, int h3, int h4, int h5, int h6)
        {
            return CombineHashCodes(CombineHashCodes(h1, h2, h3, h4), CombineHashCodes(h5, h6));
        }

        internal static int CombineHashCodes(int h1, int h2, int h3, int h4, int h5, int h6, int h7)
        {
            return CombineHashCodes(CombineHashCodes(h1, h2, h3, h4), CombineHashCodes(h5, h6, h7));
        }

        internal static int CombineHashCodes(int h1, int h2, int h3, int h4, int h5, int h6, int h7, int h8)
        {
            return CombineHashCodes(CombineHashCodes(h1, h2, h3, h4), CombineHashCodes(h5, h6, h7, h8));
        }
    }

    /// <summary>Represents a 1-tuple, or singleton, as a value type.</summary>
    /// <typeparam name="T1">The type of the tuple's only component.</typeparam>
    [Serializable]
    public class Tuple<T1> : IComparable, ITupleInternal, ITuple
    {

        private readonly T1 m_Item1;

        /// <summary>
        /// The current <see cref="Tuple{T1}"/> instance's first component.
        /// </summary>
        public T1 Item1 { get { return m_Item1; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="Tuple{T1}"/> value type.
        /// </summary>
        /// <param name="item1">The value of the tuple's first component.</param>
        public Tuple(T1 item1)
        {
            m_Item1 = item1;
        }

        /// <summary>
        /// Returns a value that indicates whether the current <see cref="Tuple{T1}"/> instance is equal to a specified object.
        /// </summary>
        /// <param name="obj">The object to compare with this instance.</param>
        /// <returns><see langword="true"/> if the current instance is equal to the specified object; otherwise, <see langword="false"/>.</returns>
        /// <remarks>
        /// The <paramref name="obj"/> parameter is considered to be equal to the current instance under the following conditions:
        /// <list type="bullet">
        ///     <item><description>It is a <see cref="Tuple{T1}"/> value type.</description></item>
        ///     <item><description>Its components are of the same types as those of the current instance.</description></item>
        ///     <item><description>Its components are equal to those of the current instance. Equality is determined by the default object equality comparer for each component.</description></item>
        /// </list>
        /// </remarks>
        public override bool Equals(object obj)
        {
            return obj is Tuple<T1> && Equals((Tuple<T1>)obj);
        }

        /// <summary>
        /// Returns a value that indicates whether the current <see cref="Tuple{T1}"/>
        /// instance is equal to a specified <see cref="Tuple{T1}"/>.
        /// </summary>
        /// <param name="other">The tuple to compare with this instance.</param>
        /// <returns><see langword="true"/> if the current instance is equal to the specified tuple; otherwise, <see langword="false"/>.</returns>
        /// <remarks>
        /// The <paramref name="other"/> parameter is considered to be equal to the current instance if each of its field
        /// is equal to that of the current instance, using the default comparer for that field's type.
        /// </remarks>
        public bool Equals(Tuple<T1> other)
        {
            return EqualityComparer<T1>.Default.Equals(Item1, other.Item1);
        }

        int IComparable.CompareTo(object other)
        {
            if (other == null) return 1;

            if (!(other is Tuple<T1>))
            {
                throw new ArgumentException();
            }

            var objTuple = (Tuple<T1>)other;

            return Comparer<T1>.Default.Compare(Item1, objTuple.Item1);
        }

        /// <summary>Compares this instance to a specified instance and returns an indication of their relative values.</summary>
        /// <param name="other">An instance to compare.</param>
        /// <returns>
        /// A signed number indicating the relative values of this instance and <paramref name="other"/>.
        /// Returns less than zero if this instance is less than <paramref name="other"/>, zero if this
        /// instance is equal to <paramref name="other"/>, and greater than zero if this instance is greater 
        /// than <paramref name="other"/>.
        /// </returns>
        public int CompareTo(Tuple<T1> other)
        {
            return Comparer<T1>.Default.Compare(Item1, other.Item1);
        }

        /// <summary>
        /// Returns the hash code for the current <see cref="Tuple{T1}"/> instance.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            return Item1?.GetHashCode() ?? 0;
        }

        int ITupleInternal.GetHashCode(IEqualityComparer comparer)
        {
            return comparer.GetHashCode(Item1);
        }

        /// <summary>
        /// Returns a string that represents the value of this <see cref="Tuple{T1}"/> instance.
        /// </summary>
        /// <returns>The string representation of this <see cref="Tuple{T1}"/> instance.</returns>
        /// <remarks>
        /// The string returned by this method takes the form <c>(Item1)</c>,
        /// where <c>Item1</c> represents the value of <see cref="Item1"/>. If the field is <see langword="null"/>,
        /// it is represented as <see cref="string.Empty"/>.
        /// </remarks>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("(");
            return ((ITupleInternal)this).ToString(sb);
        }

        string ITupleInternal.ToString(StringBuilder sb)
        {
            sb.Append(m_Item1);
            sb.Append(")");
            return sb.ToString();
        }

        /// <summary>
        /// The number of positions in this data structure.
        /// </summary>
        int ITuple.Length => 1;

        /// <summary>
        /// Get the element at position <param name="index"/>.
        /// </summary>
        object ITuple.this[int index]
        {
            get
            {
                if (index != 0)
                {
                    throw new IndexOutOfRangeException();
                }
                return Item1;
            }
        }
    }

    /// <summary>
    /// Represents a 2-tuple, or pair, as a value type.
    /// </summary>
    /// <typeparam name="T1">The type of the tuple's first component.</typeparam>
    /// <typeparam name="T2">The type of the tuple's second component.</typeparam>
    [Serializable]
    public class Tuple<T1, T2> : IComparable, ITupleInternal, ITuple
    {
        private readonly T1 m_Item1;
        private readonly T2 m_Item2;

        /// <summary>
        /// The current <see cref="Tuple{T1, T2}"/> instance's first component.
        /// </summary>
        public T1 Item1 { get { return m_Item1; } }

        /// <summary>
        /// The current <see cref="Tuple{T1, T2}"/> instance's first component.
        /// </summary>
        public T2 Item2 { get { return m_Item2; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="Tuple{T1, T2}"/> value type.
        /// </summary>
        /// <param name="item1">The value of the tuple's first component.</param>
        /// <param name="item2">The value of the tuple's second component.</param>
        public Tuple(T1 item1, T2 item2)
        {
            m_Item1 = item1;
            m_Item2 = item2;
        }

        /// <summary>
        /// Returns a value that indicates whether the current <see cref="Tuple{T1, T2}"/> instance is equal to a specified object.
        /// </summary>
        /// <param name="obj">The object to compare with this instance.</param>
        /// <returns><see langword="true"/> if the current instance is equal to the specified object; otherwise, <see langword="false"/>.</returns>
        ///
        /// <remarks>
        /// The <paramref name="obj"/> parameter is considered to be equal to the current instance under the following conditions:
        /// <list type="bullet">
        ///     <item><description>It is a <see cref="Tuple{T1, T2}"/> value type.</description></item>
        ///     <item><description>Its components are of the same types as those of the current instance.</description></item>
        ///     <item><description>Its components are equal to those of the current instance. Equality is determined by the default object equality comparer for each component.</description></item>
        /// </list>
        /// </remarks>
        public override bool Equals(object obj)
        {
            return obj is Tuple<T1, T2> && Equals((Tuple<T1, T2>)obj);
        }

        /// <summary>
        /// Returns a value that indicates whether the current <see cref="Tuple{T1, T2}"/> instance is equal to a specified <see cref="Tuple{T1, T2}"/>.
        /// </summary>
        /// <param name="other">The tuple to compare with this instance.</param>
        /// <returns><see langword="true"/> if the current instance is equal to the specified tuple; otherwise, <see langword="false"/>.</returns>
        /// <remarks>
        /// The <paramref name="other"/> parameter is considered to be equal to the current instance if each of its fields
        /// are equal to that of the current instance, using the default comparer for that field's type.
        /// </remarks>
        public bool Equals(Tuple<T1, T2> other)
        {
            return EqualityComparer<T1>.Default.Equals(Item1, other.Item1)
                && EqualityComparer<T2>.Default.Equals(Item2, other.Item2);
        }

        int IComparable.CompareTo(object other)
        {
            if (other == null) return 1;

            if (!(other is Tuple<T1, T2>))
            {
                throw new ArgumentException();
            }

            return CompareTo((Tuple<T1, T2>)other);
        }

        /// <summary>Compares this instance to a specified instance and returns an indication of their relative values.</summary>
        /// <param name="other">An instance to compare.</param>
        /// <returns>
        /// A signed number indicating the relative values of this instance and <paramref name="other"/>.
        /// Returns less than zero if this instance is less than <paramref name="other"/>, zero if this
        /// instance is equal to <paramref name="other"/>, and greater than zero if this instance is greater 
        /// than <paramref name="other"/>.
        /// </returns>
        public int CompareTo(Tuple<T1, T2> other)
        {
            int c = Comparer<T1>.Default.Compare(Item1, other.Item1);
            if (c != 0) return c;

            return Comparer<T2>.Default.Compare(Item2, other.Item2);
        }

        /// <summary>
        /// Returns the hash code for the current <see cref="Tuple{T1, T2}"/> instance.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            return Tuple.CombineHashCodes(Item1?.GetHashCode() ?? 0,
                                               Item2?.GetHashCode() ?? 0);
        }

        int ITupleInternal.GetHashCode(IEqualityComparer comparer)
        {
            return Tuple.CombineHashCodes(comparer.GetHashCode(Item1), 
                                               comparer.GetHashCode(Item2));
        }

        /// <summary>
        /// Returns a string that represents the value of this <see cref="Tuple{T1, T2}"/> instance.
        /// </summary>
        /// <returns>The string representation of this <see cref="Tuple{T1, T2}"/> instance.</returns>
        /// <remarks>
        /// The string returned by this method takes the form <c>(Item1, Item2)</c>,
        /// where <c>Item1</c> and <c>Item2</c> represent the values of the <see cref="Item1"/>
        /// and <see cref="Item2"/> fields. If either field value is <see langword="null"/>,
        /// it is represented as <see cref="string.Empty"/>.
        /// </remarks>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("(");
            return ((ITupleInternal)this).ToString(sb);
        }

        string ITupleInternal.ToString(StringBuilder sb)
        {
            sb.Append(m_Item1);
            sb.Append(", ");
            sb.Append(m_Item2);
            sb.Append(")");
            return sb.ToString();
        }

        /// <summary>
        /// The number of positions in this data structure.
        /// </summary>
        int ITuple.Length => 2;

        /// <summary>
        /// Get the element at position <param name="index"/>.
        /// </summary>
        object ITuple.this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return Item1;
                    case 1:
                        return Item2;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }
    }

    /// <summary>
    /// Represents a 3-tuple, or triple, as a value type.
    /// </summary>
    /// <typeparam name="T1">The type of the tuple's first component.</typeparam>
    /// <typeparam name="T2">The type of the tuple's second component.</typeparam>
    /// <typeparam name="T3">The type of the tuple's third component.</typeparam>
    [Serializable]
    public class Tuple<T1, T2, T3> : IComparable, ITupleInternal, ITuple
    {

        private readonly T1 m_Item1;
        private readonly T2 m_Item2;
        private readonly T3 m_Item3;

        /// <summary>
        /// The current <see cref="Tuple{T1, T2, T3}"/> instance's first component.
        /// </summary>
        public T1 Item1 { get { return m_Item1; } }
        /// <summary>
        /// The current <see cref="Tuple{T1, T2, T3}"/> instance's second component.
        /// </summary>
        public T2 Item2 { get { return m_Item2; } }
        /// <summary>
        /// The current <see cref="Tuple{T1, T2, T3}"/> instance's third component.
        /// </summary>
        public T3 Item3 { get { return m_Item3; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="Tuple{T1, T2, T3}"/> value type.
        /// </summary>
        /// <param name="item1">The value of the tuple's first component.</param>
        /// <param name="item2">The value of the tuple's second component.</param>
        /// <param name="item3">The value of the tuple's third component.</param>
        public Tuple(T1 item1, T2 item2, T3 item3)
        {
            m_Item1 = item1;
            m_Item2 = item2;
            m_Item3 = item3;
        }

        /// <summary>
        /// Returns a value that indicates whether the current <see cref="Tuple{T1, T2, T3}"/> instance is equal to a specified object.
        /// </summary>
        /// <param name="obj">The object to compare with this instance.</param>
        /// <returns><see langword="true"/> if the current instance is equal to the specified object; otherwise, <see langword="false"/>.</returns>
        /// <remarks>
        /// The <paramref name="obj"/> parameter is considered to be equal to the current instance under the following conditions:
        /// <list type="bullet">
        ///     <item><description>It is a <see cref="Tuple{T1, T2, T3}"/> value type.</description></item>
        ///     <item><description>Its components are of the same types as those of the current instance.</description></item>
        ///     <item><description>Its components are equal to those of the current instance. Equality is determined by the default object equality comparer for each component.</description></item>
        /// </list>
        /// </remarks>
        public override bool Equals(object obj)
        {
            return obj is Tuple<T1, T2, T3> && Equals((Tuple<T1, T2, T3>)obj);
        }

        /// <summary>
        /// Returns a value that indicates whether the current <see cref="Tuple{T1, T2, T3}"/>
        /// instance is equal to a specified <see cref="Tuple{T1, T2, T3}"/>.
        /// </summary>
        /// <param name="other">The tuple to compare with this instance.</param>
        /// <returns><see langword="true"/> if the current instance is equal to the specified tuple; otherwise, <see langword="false"/>.</returns>
        /// <remarks>
        /// The <paramref name="other"/> parameter is considered to be equal to the current instance if each of its fields
        /// are equal to that of the current instance, using the default comparer for that field's type.
        /// </remarks>
        public bool Equals(Tuple<T1, T2, T3> other)
        {
            return EqualityComparer<T1>.Default.Equals(Item1, other.Item1)
                && EqualityComparer<T2>.Default.Equals(Item2, other.Item2)
                && EqualityComparer<T3>.Default.Equals(Item3, other.Item3);
        }

        int IComparable.CompareTo(object other)
        {
            if (other == null) return 1;

            if (!(other is Tuple<T1, T2, T3>))
            {
                throw new ArgumentException();
            }

            return CompareTo((Tuple<T1, T2, T3>)other);
        }

        /// <summary>Compares this instance to a specified instance and returns an indication of their relative values.</summary>
        /// <param name="other">An instance to compare.</param>
        /// <returns>
        /// A signed number indicating the relative values of this instance and <paramref name="other"/>.
        /// Returns less than zero if this instance is less than <paramref name="other"/>, zero if this
        /// instance is equal to <paramref name="other"/>, and greater than zero if this instance is greater 
        /// than <paramref name="other"/>.
        /// </returns>
        public int CompareTo(Tuple<T1, T2, T3> other)
        {
            int c = Comparer<T1>.Default.Compare(Item1, other.Item1);
            if (c != 0) return c;

            c = Comparer<T2>.Default.Compare(Item2, other.Item2);
            if (c != 0) return c;

            return Comparer<T3>.Default.Compare(Item3, other.Item3);
        }

        /// <summary>
        /// Returns the hash code for the current <see cref="Tuple{T1, T2, T3}"/> instance.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            return Tuple.CombineHashCodes(Item1?.GetHashCode() ?? 0,
                                               Item2?.GetHashCode() ?? 0,
                                               Item3?.GetHashCode() ?? 0);
        }

        int ITupleInternal.GetHashCode(IEqualityComparer comparer)
        {
            return Tuple.CombineHashCodes(comparer.GetHashCode(Item1),
                                               comparer.GetHashCode(Item2),
                                               comparer.GetHashCode(Item3));
        }

        /// <summary>
        /// Returns a string that represents the value of this <see cref="Tuple{T1, T2, T3}"/> instance.
        /// </summary>
        /// <returns>The string representation of this <see cref="Tuple{T1, T2, T3}"/> instance.</returns>
        /// <remarks>
        /// The string returned by this method takes the form <c>(Item1, Item2, Item3)</c>.
        /// If any field value is <see langword="null"/>, it is represented as <see cref="string.Empty"/>.
        /// </remarks>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("(");
            return ((ITupleInternal)this).ToString(sb);
        }

        string ITupleInternal.ToString(StringBuilder sb)
        {
            sb.Append(m_Item1);
            sb.Append(", ");
            sb.Append(m_Item2);
            sb.Append(", ");
            sb.Append(m_Item3);
            sb.Append(")");
            return sb.ToString();
        }

        /// <summary>
        /// The number of positions in this data structure.
        /// </summary>
        int ITuple.Length => 3;

        /// <summary>
        /// Get the element at position <param name="index"/>.
        /// </summary>
        object ITuple.this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return Item1;
                    case 1:
                        return Item2;
                    case 2:
                        return Item3;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }
    }

    /// <summary>
    /// Represents a 4-tuple, or quadruple, as a value type.
    /// </summary>
    /// <typeparam name="T1">The type of the tuple's first component.</typeparam>
    /// <typeparam name="T2">The type of the tuple's second component.</typeparam>
    /// <typeparam name="T3">The type of the tuple's third component.</typeparam>
    /// <typeparam name="T4">The type of the tuple's fourth component.</typeparam>
    [Serializable]
    public class Tuple<T1, T2, T3, T4> : IComparable, ITupleInternal, ITuple
    {

        private readonly T1 m_Item1;
        private readonly T2 m_Item2;
        private readonly T3 m_Item3;
        private readonly T4 m_Item4;

        /// <summary>
        /// The current <see cref="Tuple{T1, T2, T3, T4}"/> instance's first component.
        /// </summary>
        public T1 Item1 { get { return m_Item1; } }
        /// <summary>
        /// The current <see cref="Tuple{T1, T2, T3, T4}"/> instance's second component.
        /// </summary>
        public T2 Item2 { get { return m_Item2; } }
        /// <summary>
        /// The current <see cref="Tuple{T1, T2, T3, T4}"/> instance's third component.
        /// </summary>
        public T3 Item3 { get { return m_Item3; } }
        /// <summary>
        /// The current <see cref="Tuple{T1, T2, T3, T4}"/> instance's fourth component.
        /// </summary>
        public T4 Item4 { get { return m_Item4; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="Tuple{T1, T2, T3, T4}"/> value type.
        /// </summary>
        /// <param name="item1">The value of the tuple's first component.</param>
        /// <param name="item2">The value of the tuple's second component.</param>
        /// <param name="item3">The value of the tuple's third component.</param>
        /// <param name="item4">The value of the tuple's fourth component.</param>
        public Tuple(T1 item1, T2 item2, T3 item3, T4 item4)
        {
            m_Item1 = item1;
            m_Item2 = item2;
            m_Item3 = item3;
            m_Item4 = item4;
        }

        /// <summary>
        /// Returns a value that indicates whether the current <see cref="Tuple{T1, T2, T3, T4}"/> instance is equal to a specified object.
        /// </summary>
        /// <param name="obj">The object to compare with this instance.</param>
        /// <returns><see langword="true"/> if the current instance is equal to the specified object; otherwise, <see langword="false"/>.</returns>
        /// <remarks>
        /// The <paramref name="obj"/> parameter is considered to be equal to the current instance under the following conditions:
        /// <list type="bullet">
        ///     <item><description>It is a <see cref="Tuple{T1, T2, T3, T4}"/> value type.</description></item>
        ///     <item><description>Its components are of the same types as those of the current instance.</description></item>
        ///     <item><description>Its components are equal to those of the current instance. Equality is determined by the default object equality comparer for each component.</description></item>
        /// </list>
        /// </remarks>
        public override bool Equals(object obj)
        {
            return obj is Tuple<T1, T2, T3, T4> && Equals((Tuple<T1, T2, T3, T4>)obj);
        }

        /// <summary>
        /// Returns a value that indicates whether the current <see cref="Tuple{T1, T2, T3, T4}"/>
        /// instance is equal to a specified <see cref="Tuple{T1, T2, T3, T4}"/>.
        /// </summary>
        /// <param name="other">The tuple to compare with this instance.</param>
        /// <returns><see langword="true"/> if the current instance is equal to the specified tuple; otherwise, <see langword="false"/>.</returns>
        /// <remarks>
        /// The <paramref name="other"/> parameter is considered to be equal to the current instance if each of its fields
        /// are equal to that of the current instance, using the default comparer for that field's type.
        /// </remarks>
        public bool Equals(Tuple<T1, T2, T3, T4> other)
        {
            return EqualityComparer<T1>.Default.Equals(m_Item1, other.Item1)
                && EqualityComparer<T2>.Default.Equals(m_Item2, other.Item2)
                && EqualityComparer<T3>.Default.Equals(m_Item3, other.Item3)
                && EqualityComparer<T4>.Default.Equals(m_Item4, other.Item4);
        }

        int IComparable.CompareTo(object other)
        {
            if (other == null) return 1;

            if (!(other is Tuple<T1, T2, T3, T4>))
            {
                throw new ArgumentException();
            }

            return CompareTo((Tuple<T1, T2, T3, T4>)other);
        }

        /// <summary>Compares this instance to a specified instance and returns an indication of their relative values.</summary>
        /// <param name="other">An instance to compare.</param>
        /// <returns>
        /// A signed number indicating the relative values of this instance and <paramref name="other"/>.
        /// Returns less than zero if this instance is less than <paramref name="other"/>, zero if this
        /// instance is equal to <paramref name="other"/>, and greater than zero if this instance is greater 
        /// than <paramref name="other"/>.
        /// </returns>
        public int CompareTo(Tuple<T1, T2, T3, T4> other)
        {
            int c = Comparer<T1>.Default.Compare(m_Item1, other.Item1);
            if (c != 0) return c;

            c = Comparer<T2>.Default.Compare(m_Item2, other.Item2);
            if (c != 0) return c;

            c = Comparer<T3>.Default.Compare(m_Item3, other.Item3);
            if (c != 0) return c;

            return Comparer<T4>.Default.Compare(m_Item4, other.Item4);
        }

        /// <summary>
        /// Returns the hash code for the current <see cref="Tuple{T1, T2, T3, T4}"/> instance.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            return Tuple.CombineHashCodes(m_Item1?.GetHashCode() ?? 0,
                                               m_Item2?.GetHashCode() ?? 0,
                                               m_Item3?.GetHashCode() ?? 0,
                                               m_Item4?.GetHashCode() ?? 0);
        }

        int ITupleInternal.GetHashCode(IEqualityComparer comparer)
        {
            return Tuple.CombineHashCodes(comparer.GetHashCode(m_Item1), 
                                               comparer.GetHashCode(m_Item2),
                                               comparer.GetHashCode(m_Item3),
                                               comparer.GetHashCode(m_Item4));
        }

        /// <summary>
        /// Returns a string that represents the value of this <see cref="Tuple{T1, T2, T3, T4}"/> instance.
        /// </summary>
        /// <returns>The string representation of this <see cref="Tuple{T1, T2, T3, T4}"/> instance.</returns>
        /// <remarks>
        /// The string returned by this method takes the form <c>(Item1, Item2, Item3, Item4)</c>.
        /// If any field value is <see langword="null"/>, it is represented as <see cref="string.Empty"/>.
        /// </remarks>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("(");
            return ((ITupleInternal)this).ToString(sb);
        }

        string ITupleInternal.ToString(StringBuilder sb)
        {
            sb.Append(m_Item1);
            sb.Append(", ");
            sb.Append(m_Item2);
            sb.Append(", ");
            sb.Append(m_Item3);
            sb.Append(", ");
            sb.Append(m_Item4);
            sb.Append(")");
            return sb.ToString();
        }

        /// <summary>
        /// The number of positions in this data structure.
        /// </summary>
        int ITuple.Length => 4;

        /// <summary>
        /// Get the element at position <param name="index"/>.
        /// </summary>
        object ITuple.this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return Item1;
                    case 1:
                        return Item2;
                    case 2:
                        return Item3;
                    case 3:
                        return Item4;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }
    }

    /// <summary>
    /// Represents a 5-tuple, or quintuple, as a value type.
    /// </summary>
    /// <typeparam name="T1">The type of the tuple's first component.</typeparam>
    /// <typeparam name="T2">The type of the tuple's second component.</typeparam>
    /// <typeparam name="T3">The type of the tuple's third component.</typeparam>
    /// <typeparam name="T4">The type of the tuple's fourth component.</typeparam>
    /// <typeparam name="T5">The type of the tuple's fifth component.</typeparam>
    [Serializable]
    public class Tuple<T1, T2, T3, T4, T5> : IComparable, ITupleInternal, ITuple
    {

        private readonly T1 m_Item1;
        private readonly T2 m_Item2;
        private readonly T3 m_Item3;
        private readonly T4 m_Item4;
        private readonly T5 m_Item5;

        /// <summary>
        /// The current <see cref="Tuple{T1, T2, T3, T4, T5}"/> instance's first component.
        /// </summary>
        public T1 Item1 { get { return m_Item1; } }
        /// <summary>
        /// The current <see cref="Tuple{T1, T2, T3, T4, T5}"/> instance's second component.
        /// </summary>
        public T2 Item2 { get { return m_Item2; } }
        /// <summary>
        /// The current <see cref="Tuple{T1, T2, T3, T4, T5}"/> instance's third component.
        /// </summary>
        public T3 Item3 { get { return m_Item3; } }
        /// <summary>
        /// The current <see cref="Tuple{T1, T2, T3, T4, T5}"/> instance's fourth component.
        /// </summary>
        public T4 Item4 { get { return m_Item4; } }
        /// <summary>
        /// The current <see cref="Tuple{T1, T2, T3, T4, T5}"/> instance's fifth component.
        /// </summary>
        public T5 Item5 { get { return m_Item5; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="Tuple{T1, T2, T3, T4, T5}"/> value type.
        /// </summary>
        /// <param name="item1">The value of the tuple's first component.</param>
        /// <param name="item2">The value of the tuple's second component.</param>
        /// <param name="item3">The value of the tuple's third component.</param>
        /// <param name="item4">The value of the tuple's fourth component.</param>
        /// <param name="item5">The value of the tuple's fifth component.</param>
        public Tuple(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5)
        {
            m_Item1 = item1;
            m_Item2 = item2;
            m_Item3 = item3;
            m_Item4 = item4;
            m_Item5 = item5;
        }

        /// <summary>
        /// Returns a value that indicates whether the current <see cref="Tuple{T1, T2, T3, T4, T5}"/> instance is equal to a specified object.
        /// </summary>
        /// <param name="obj">The object to compare with this instance.</param>
        /// <returns><see langword="true"/> if the current instance is equal to the specified object; otherwise, <see langword="false"/>.</returns>
        /// <remarks>
        /// The <paramref name="obj"/> parameter is considered to be equal to the current instance under the following conditions:
        /// <list type="bullet">
        ///     <item><description>It is a <see cref="Tuple{T1, T2, T3, T4, T5}"/> value type.</description></item>
        ///     <item><description>Its components are of the same types as those of the current instance.</description></item>
        ///     <item><description>Its components are equal to those of the current instance. Equality is determined by the default object equality comparer for each component.</description></item>
        /// </list>
        /// </remarks>
        public override bool Equals(object obj)
        {
            return obj is Tuple<T1, T2, T3, T4, T5> && Equals((Tuple<T1, T2, T3, T4, T5>)obj);
        }

        /// <summary>
        /// Returns a value that indicates whether the current <see cref="Tuple{T1, T2, T3, T4, T5}"/>
        /// instance is equal to a specified <see cref="Tuple{T1, T2, T3, T4, T5}"/>.
        /// </summary>
        /// <param name="other">The tuple to compare with this instance.</param>
        /// <returns><see langword="true"/> if the current instance is equal to the specified tuple; otherwise, <see langword="false"/>.</returns>
        /// <remarks>
        /// The <paramref name="other"/> parameter is considered to be equal to the current instance if each of its fields
        /// are equal to that of the current instance, using the default comparer for that field's type.
        /// </remarks>
        public bool Equals(Tuple<T1, T2, T3, T4, T5> other)
        {
            return EqualityComparer<T1>.Default.Equals(m_Item1, other.Item1)
                && EqualityComparer<T2>.Default.Equals(m_Item2, other.Item2)
                && EqualityComparer<T3>.Default.Equals(m_Item3, other.Item3)
                && EqualityComparer<T4>.Default.Equals(m_Item4, other.Item4)
                && EqualityComparer<T5>.Default.Equals(m_Item5, other.Item5);
        }

        int IComparable.CompareTo(object other)
        {
            if (other == null) return 1;

            if (!(other is Tuple<T1, T2, T3, T4, T5>))
            {
                throw new ArgumentException();
            }

            return CompareTo((Tuple<T1, T2, T3, T4, T5>)other);
        }

        /// <summary>Compares this instance to a specified instance and returns an indication of their relative values.</summary>
        /// <param name="other">An instance to compare.</param>
        /// <returns>
        /// A signed number indicating the relative values of this instance and <paramref name="other"/>.
        /// Returns less than zero if this instance is less than <paramref name="other"/>, zero if this
        /// instance is equal to <paramref name="other"/>, and greater than zero if this instance is greater 
        /// than <paramref name="other"/>.
        /// </returns>
        public int CompareTo(Tuple<T1, T2, T3, T4, T5> other)
        {
            int c = Comparer<T1>.Default.Compare(m_Item1, other.Item1);
            if (c != 0) return c;

            c = Comparer<T2>.Default.Compare(m_Item2, other.Item2);
            if (c != 0) return c;

            c = Comparer<T3>.Default.Compare(m_Item3, other.Item3);
            if (c != 0) return c;

            c = Comparer<T4>.Default.Compare(m_Item4, other.Item4);
            if (c != 0) return c;

            return Comparer<T5>.Default.Compare(m_Item5, other.Item5);
        }

        /// <summary>
        /// Returns the hash code for the current <see cref="Tuple{T1, T2, T3, T4, T5}"/> instance.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            return Tuple.CombineHashCodes(m_Item1?.GetHashCode() ?? 0,
                                               m_Item2?.GetHashCode() ?? 0,
                                               m_Item3?.GetHashCode() ?? 0,
                                               m_Item4?.GetHashCode() ?? 0,
                                               m_Item5?.GetHashCode() ?? 0);
        }

        Int32 ITupleInternal.GetHashCode(IEqualityComparer comparer)
        {
            return Tuple.CombineHashCodes(comparer.GetHashCode(m_Item1),
                                               comparer.GetHashCode(m_Item2),
                                               comparer.GetHashCode(m_Item3),
                                               comparer.GetHashCode(m_Item4),
                                               comparer.GetHashCode(m_Item5));
        }

        /// <summary>
        /// Returns a string that represents the value of this <see cref="Tuple{T1, T2, T3, T4, T5}"/> instance.
        /// </summary>
        /// <returns>The string representation of this <see cref="Tuple{T1, T2, T3, T4, T5}"/> instance.</returns>
        /// <remarks>
        /// The string returned by this method takes the form <c>(Item1, Item2, Item3, Item4, Item5)</c>.
        /// If any field value is <see langword="null"/>, it is represented as <see cref="string.Empty"/>.
        /// </remarks>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("(");
            return ((ITupleInternal)this).ToString(sb);
        }

        string ITupleInternal.ToString(StringBuilder sb)
        {
            sb.Append(m_Item1);
            sb.Append(", ");
            sb.Append(m_Item2);
            sb.Append(", ");
            sb.Append(m_Item3);
            sb.Append(", ");
            sb.Append(m_Item4);
            sb.Append(", ");
            sb.Append(m_Item5);
            sb.Append(")");
            return sb.ToString();
        }

        /// <summary>
        /// The number of positions in this data structure.
        /// </summary>
        int ITuple.Length => 5;

        /// <summary>
        /// Get the element at position <param name="index"/>.
        /// </summary>
        object ITuple.this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return Item1;
                    case 1:
                        return Item2;
                    case 2:
                        return Item3;
                    case 3:
                        return Item4;
                    case 4:
                        return Item5;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }
    }

    /// <summary>
    /// Represents a 6-tuple, or sextuple, as a value type.
    /// </summary>
    /// <typeparam name="T1">The type of the tuple's first component.</typeparam>
    /// <typeparam name="T2">The type of the tuple's second component.</typeparam>
    /// <typeparam name="T3">The type of the tuple's third component.</typeparam>
    /// <typeparam name="T4">The type of the tuple's fourth component.</typeparam>
    /// <typeparam name="T5">The type of the tuple's fifth component.</typeparam>
    /// <typeparam name="T6">The type of the tuple's sixth component.</typeparam>
    [Serializable]
    public class Tuple<T1, T2, T3, T4, T5, T6> : IComparable, ITupleInternal, ITuple
    {

        private readonly T1 m_Item1;
        private readonly T2 m_Item2;
        private readonly T3 m_Item3;
        private readonly T4 m_Item4;
        private readonly T5 m_Item5;
        private readonly T6 m_Item6;

        /// <summary>
        /// The current <see cref="Tuple{T1, T2, T3, T4, T5, T6}"/> instance's first component.
        /// </summary>
        public T1 Item1 { get { return m_Item1; } }
        /// <summary>
        /// The current <see cref="Tuple{T1, T2, T3, T4, T5, T6}"/> instance's second component.
        /// </summary>
        public T2 Item2 { get { return m_Item2; } }
        /// <summary>
        /// The current <see cref="Tuple{T1, T2, T3, T4, T5, T6}"/> instance's third component.
        /// </summary>
        public T3 Item3 { get { return m_Item3; } }
        /// <summary>
        /// The current <see cref="Tuple{T1, T2, T3, T4, T5, T6}"/> instance's fourth component.
        /// </summary>
        public T4 Item4 { get { return m_Item4; } }
        /// <summary>
        /// The current <see cref="Tuple{T1, T2, T3, T4, T5, T6}"/> instance's fifth component.
        /// </summary>
        public T5 Item5 { get { return m_Item5; } }
        /// <summary>
        /// The current <see cref="Tuple{T1, T2, T3, T4, T5, T6}"/> instance's fifth component.
        /// </summary>
        public T6 Item6 { get { return m_Item6; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="Tuple{T1, T2, T3, T4, T5, T6}"/> value type.
        /// </summary>
        /// <param name="item1">The value of the tuple's first component.</param>
        /// <param name="item2">The value of the tuple's second component.</param>
        /// <param name="item3">The value of the tuple's third component.</param>
        /// <param name="item4">The value of the tuple's fourth component.</param>
        /// <param name="item5">The value of the tuple's fifth component.</param>
        /// <param name="item6">The value of the tuple's sixth component.</param>
        public Tuple(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6)
        {
            m_Item1 = item1;
            m_Item2 = item2;
            m_Item3 = item3;
            m_Item4 = item4;
            m_Item5 = item5;
            m_Item6 = item6;
        }

        /// <summary>
        /// Returns a value that indicates whether the current <see cref="Tuple{T1, T2, T3, T4, T5, T6}"/> instance is equal to a specified object.
        /// </summary>
        /// <param name="obj">The object to compare with this instance.</param>
        /// <returns><see langword="true"/> if the current instance is equal to the specified object; otherwise, <see langword="false"/>.</returns>
        /// <remarks>
        /// The <paramref name="obj"/> parameter is considered to be equal to the current instance under the following conditions:
        /// <list type="bullet">
        ///     <item><description>It is a <see cref="Tuple{T1, T2, T3, T4, T5, T6}"/> value type.</description></item>
        ///     <item><description>Its components are of the same types as those of the current instance.</description></item>
        ///     <item><description>Its components are equal to those of the current instance. Equality is determined by the default object equality comparer for each component.</description></item>
        /// </list>
        /// </remarks>
        public override bool Equals(object obj)
        {
            return obj is Tuple<T1, T2, T3, T4, T5, T6> && Equals((Tuple<T1, T2, T3, T4, T5, T6>)obj);
        }

        /// <summary>
        /// Returns a value that indicates whether the current <see cref="Tuple{T1, T2, T3, T4, T5, T6}"/>
        /// instance is equal to a specified <see cref="Tuple{T1, T2, T3, T4, T5, T6}"/>.
        /// </summary>
        /// <param name="other">The tuple to compare with this instance.</param>
        /// <returns><see langword="true"/> if the current instance is equal to the specified tuple; otherwise, <see langword="false"/>.</returns>
        /// <remarks>
        /// The <paramref name="other"/> parameter is considered to be equal to the current instance if each of its fields
        /// are equal to that of the current instance, using the default comparer for that field's type.
        /// </remarks>
        public bool Equals(Tuple<T1, T2, T3, T4, T5, T6> other)
        {
            return EqualityComparer<T1>.Default.Equals(m_Item1, other.Item1)
                && EqualityComparer<T2>.Default.Equals(m_Item2, other.Item2)
                && EqualityComparer<T3>.Default.Equals(m_Item3, other.Item3)
                && EqualityComparer<T4>.Default.Equals(m_Item4, other.Item4)
                && EqualityComparer<T5>.Default.Equals(m_Item5, other.Item5)
                && EqualityComparer<T6>.Default.Equals(m_Item6, other.Item6);
        }

        int IComparable.CompareTo(object other)
        {
            if (other == null) return 1;

            if (!(other is Tuple<T1, T2, T3, T4, T5, T6>))
            {
                throw new ArgumentException();
            }

            return CompareTo((Tuple<T1, T2, T3, T4, T5, T6>)other);
        }

        /// <summary>Compares this instance to a specified instance and returns an indication of their relative values.</summary>
        /// <param name="other">An instance to compare.</param>
        /// <returns>
        /// A signed number indicating the relative values of this instance and <paramref name="other"/>.
        /// Returns less than zero if this instance is less than <paramref name="other"/>, zero if this
        /// instance is equal to <paramref name="other"/>, and greater than zero if this instance is greater 
        /// than <paramref name="other"/>.
        /// </returns>
        public int CompareTo(Tuple<T1, T2, T3, T4, T5, T6> other)
        {
            int c = Comparer<T1>.Default.Compare(m_Item1, other.Item1);
            if (c != 0) return c;

            c = Comparer<T2>.Default.Compare(m_Item2, other.Item2);
            if (c != 0) return c;

            c = Comparer<T3>.Default.Compare(m_Item3, other.Item3);
            if (c != 0) return c;

            c = Comparer<T4>.Default.Compare(m_Item4, other.Item4);
            if (c != 0) return

            c = Comparer<T5>.Default.Compare(m_Item5, other.Item5);
            if (c != 0) return c;

            return Comparer<T6>.Default.Compare(m_Item6, other.Item6);
        }

        /// <summary>
        /// Returns the hash code for the current <see cref="Tuple{T1, T2, T3, T4, T5, T6}"/> instance.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            return Tuple.CombineHashCodes(m_Item1?.GetHashCode() ?? 0,
                                               m_Item2?.GetHashCode() ?? 0,
                                               m_Item3?.GetHashCode() ?? 0,
                                               m_Item4?.GetHashCode() ?? 0,
                                               m_Item5?.GetHashCode() ?? 0,
                                               m_Item6?.GetHashCode() ?? 0);
        }

        Int32 ITupleInternal.GetHashCode(IEqualityComparer comparer)
        {
            return Tuple.CombineHashCodes(comparer.GetHashCode(m_Item1),
                                               comparer.GetHashCode(m_Item2),
                                               comparer.GetHashCode(m_Item3),
                                               comparer.GetHashCode(m_Item4),
                                               comparer.GetHashCode(m_Item5),
                                               comparer.GetHashCode(m_Item6));
        }

        /// <summary>
        /// Returns a string that represents the value of this <see cref="Tuple{T1, T2, T3, T4, T5, T6}"/> instance.
        /// </summary>
        /// <returns>The string representation of this <see cref="Tuple{T1, T2, T3, T4, T5, T6}"/> instance.</returns>
        /// <remarks>
        /// The string returned by this method takes the form <c>(Item1, Item2, Item3, Item4, Item5)</c>.
        /// If any field value is <see langword="null"/>, it is represented as <see cref="string.Empty"/>.
        /// </remarks>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("(");
            return ((ITupleInternal)this).ToString(sb);
        }

        string ITupleInternal.ToString(StringBuilder sb)
        {
            sb.Append(m_Item1);
            sb.Append(", ");
            sb.Append(m_Item2);
            sb.Append(", ");
            sb.Append(m_Item3);
            sb.Append(", ");
            sb.Append(m_Item4);
            sb.Append(", ");
            sb.Append(m_Item5);
            sb.Append(", ");
            sb.Append(m_Item6);
            sb.Append(")");
            return sb.ToString();
        }

        /// <summary>
        /// The number of positions in this data structure.
        /// </summary>
        int ITuple.Length => 6;

        /// <summary>
        /// Get the element at position <param name="index"/>.
        /// </summary>
        object ITuple.this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:
                        return Item1;
                    case 1:
                        return Item2;
                    case 2:
                        return Item3;
                    case 3:
                        return Item4;
                    case 4:
                        return Item5;
                    case 5:
                        return Item6;
                    default:
                        throw new IndexOutOfRangeException();
                }
            }
        }
    }

    //[Serializable]
    //public class Tuple<T1, T2, T3, T4, T5, T6, T7> : IComparable, ITupleInternal, ITuple
    //{

    //    private readonly T1 m_Item1;
    //    private readonly T2 m_Item2;
    //    private readonly T3 m_Item3;
    //    private readonly T4 m_Item4;
    //    private readonly T5 m_Item5;
    //    private readonly T6 m_Item6;
    //    private readonly T7 m_Item7;

    //    public T1 Item1 { get { return m_Item1; } }
    //    public T2 Item2 { get { return m_Item2; } }
    //    public T3 Item3 { get { return m_Item3; } }
    //    public T4 Item4 { get { return m_Item4; } }
    //    public T5 Item5 { get { return m_Item5; } }
    //    public T6 Item6 { get { return m_Item6; } }
    //    public T7 Item7 { get { return m_Item7; } }

    //    public Tuple(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7)
    //    {
    //        m_Item1 = item1;
    //        m_Item2 = item2;
    //        m_Item3 = item3;
    //        m_Item4 = item4;
    //        m_Item5 = item5;
    //        m_Item6 = item6;
    //        m_Item7 = item7;
    //    }

    //    public override Boolean Equals(Object obj)
    //    {
    //        return ((IStructuralEquatable)this).Equals(obj, EqualityComparer<Object>.Default); ;
    //    }

    //    Boolean IStructuralEquatable.Equals(Object other, IEqualityComparer comparer)
    //    {
    //        if (other == null) return false;

    //        Tuple<T1, T2, T3, T4, T5, T6, T7> objTuple = other as Tuple<T1, T2, T3, T4, T5, T6, T7>;

    //        if (objTuple == null)
    //        {
    //            return false;
    //        }

    //        return comparer.Equals(m_Item1, objTuple.m_Item1) && comparer.Equals(m_Item2, objTuple.m_Item2) && comparer.Equals(m_Item3, objTuple.m_Item3) && comparer.Equals(m_Item4, objTuple.m_Item4) && comparer.Equals(m_Item5, objTuple.m_Item5) && comparer.Equals(m_Item6, objTuple.m_Item6) && comparer.Equals(m_Item7, objTuple.m_Item7);
    //    }

    //    Int32 IComparable.CompareTo(Object obj)
    //    {
    //        return ((IStructuralComparable)this).CompareTo(obj, Comparer<Object>.Default);
    //    }

    //    Int32 IStructuralComparable.CompareTo(Object other, IComparer comparer)
    //    {
    //        if (other == null) return 1;

    //        Tuple<T1, T2, T3, T4, T5, T6, T7> objTuple = other as Tuple<T1, T2, T3, T4, T5, T6, T7>;

    //        if (objTuple == null)
    //        {
    //            throw new ArgumentException(Environment.GetResourceString("ArgumentException_TupleIncorrectType", this.GetType().ToString()), "other");
    //        }

    //        int c = 0;

    //        c = comparer.Compare(m_Item1, objTuple.m_Item1);

    //        if (c != 0) return c;

    //        c = comparer.Compare(m_Item2, objTuple.m_Item2);

    //        if (c != 0) return c;

    //        c = comparer.Compare(m_Item3, objTuple.m_Item3);

    //        if (c != 0) return c;

    //        c = comparer.Compare(m_Item4, objTuple.m_Item4);

    //        if (c != 0) return c;

    //        c = comparer.Compare(m_Item5, objTuple.m_Item5);

    //        if (c != 0) return c;

    //        c = comparer.Compare(m_Item6, objTuple.m_Item6);

    //        if (c != 0) return c;

    //        return comparer.Compare(m_Item7, objTuple.m_Item7);
    //    }

    //    public override int GetHashCode()
    //    {
    //        return ((IStructuralEquatable)this).GetHashCode(EqualityComparer<Object>.Default);
    //    }

    //    Int32 IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
    //    {
    //        return Tuple.CombineHashCodes(comparer.GetHashCode(m_Item1), comparer.GetHashCode(m_Item2), comparer.GetHashCode(m_Item3), comparer.GetHashCode(m_Item4), comparer.GetHashCode(m_Item5), comparer.GetHashCode(m_Item6), comparer.GetHashCode(m_Item7));
    //    }

    //    Int32 ITupleInternal.GetHashCode(IEqualityComparer comparer)
    //    {
    //        return ((IStructuralEquatable)this).GetHashCode(comparer);
    //    }
    //    public override string ToString()
    //    {
    //        StringBuilder sb = new StringBuilder();
    //        sb.Append("(");
    //        return ((ITupleInternal)this).ToString(sb);
    //    }

    //    string ITupleInternal.ToString(StringBuilder sb)
    //    {
    //        sb.Append(m_Item1);
    //        sb.Append(", ");
    //        sb.Append(m_Item2);
    //        sb.Append(", ");
    //        sb.Append(m_Item3);
    //        sb.Append(", ");
    //        sb.Append(m_Item4);
    //        sb.Append(", ");
    //        sb.Append(m_Item5);
    //        sb.Append(", ");
    //        sb.Append(m_Item6);
    //        sb.Append(", ");
    //        sb.Append(m_Item7);
    //        sb.Append(")");
    //        return sb.ToString();
    //    }

    //    /// <summary>
    //    /// The number of positions in this data structure.
    //    /// </summary>
    //    int ITuple.Length => 7;

    //    /// <summary>
    //    /// Get the element at position <param name="index"/>.
    //    /// </summary>
    //    object ITuple.this[int index]
    //    {
    //        get
    //        {
    //            switch (index)
    //            {
    //                case 0:
    //                    return Item1;
    //                case 1:
    //                    return Item2;
    //                case 2:
    //                    return Item3;
    //                case 3:
    //                    return Item4;
    //                case 4:
    //                    return Item5;
    //                case 5:
    //                    return Item6;
    //                case 6:
    //                    return Item7;
    //                default:
    //                    throw new IndexOutOfRangeException();
    //            }
    //        }
    //    }
    //}

    //[Serializable]
    //public class Tuple<T1, T2, T3, T4, T5, T6, T7, TRest> : IComparable, ITupleInternal, ITuple
    //{

    //    private readonly T1 m_Item1;
    //    private readonly T2 m_Item2;
    //    private readonly T3 m_Item3;
    //    private readonly T4 m_Item4;
    //    private readonly T5 m_Item5;
    //    private readonly T6 m_Item6;
    //    private readonly T7 m_Item7;
    //    private readonly TRest m_Rest;

    //    public T1 Item1 { get { return m_Item1; } }
    //    public T2 Item2 { get { return m_Item2; } }
    //    public T3 Item3 { get { return m_Item3; } }
    //    public T4 Item4 { get { return m_Item4; } }
    //    public T5 Item5 { get { return m_Item5; } }
    //    public T6 Item6 { get { return m_Item6; } }
    //    public T7 Item7 { get { return m_Item7; } }
    //    public TRest Rest { get { return m_Rest; } }

    //    public Tuple(T1 item1, T2 item2, T3 item3, T4 item4, T5 item5, T6 item6, T7 item7, TRest rest)
    //    {
    //        if (!(rest is ITupleInternal))
    //        {
    //            throw new ArgumentException(Environment.GetResourceString("ArgumentException_TupleLastArgumentNotATuple"));
    //        }

    //        m_Item1 = item1;
    //        m_Item2 = item2;
    //        m_Item3 = item3;
    //        m_Item4 = item4;
    //        m_Item5 = item5;
    //        m_Item6 = item6;
    //        m_Item7 = item7;
    //        m_Rest = rest;
    //    }

    //    public override Boolean Equals(Object obj)
    //    {
    //        return ((IStructuralEquatable)this).Equals(obj, EqualityComparer<Object>.Default); ;
    //    }

    //    Boolean IStructuralEquatable.Equals(Object other, IEqualityComparer comparer)
    //    {
    //        if (other == null) return false;

    //        Tuple<T1, T2, T3, T4, T5, T6, T7, TRest> objTuple = other as Tuple<T1, T2, T3, T4, T5, T6, T7, TRest>;

    //        if (objTuple == null)
    //        {
    //            return false;
    //        }

    //        return comparer.Equals(m_Item1, objTuple.m_Item1) && comparer.Equals(m_Item2, objTuple.m_Item2) && comparer.Equals(m_Item3, objTuple.m_Item3) && comparer.Equals(m_Item4, objTuple.m_Item4) && comparer.Equals(m_Item5, objTuple.m_Item5) && comparer.Equals(m_Item6, objTuple.m_Item6) && comparer.Equals(m_Item7, objTuple.m_Item7) && comparer.Equals(m_Rest, objTuple.m_Rest);
    //    }

    //    Int32 IComparable.CompareTo(Object obj)
    //    {
    //        return ((IStructuralComparable)this).CompareTo(obj, Comparer<Object>.Default);
    //    }

    //    Int32 IStructuralComparable.CompareTo(Object other, IComparer comparer)
    //    {
    //        if (other == null) return 1;

    //        Tuple<T1, T2, T3, T4, T5, T6, T7, TRest> objTuple = other as Tuple<T1, T2, T3, T4, T5, T6, T7, TRest>;

    //        if (objTuple == null)
    //        {
    //            throw new ArgumentException(Environment.GetResourceString("ArgumentException_TupleIncorrectType", this.GetType().ToString()), "other");
    //        }

    //        int c = 0;

    //        c = comparer.Compare(m_Item1, objTuple.m_Item1);

    //        if (c != 0) return c;

    //        c = comparer.Compare(m_Item2, objTuple.m_Item2);

    //        if (c != 0) return c;

    //        c = comparer.Compare(m_Item3, objTuple.m_Item3);

    //        if (c != 0) return c;

    //        c = comparer.Compare(m_Item4, objTuple.m_Item4);

    //        if (c != 0) return c;

    //        c = comparer.Compare(m_Item5, objTuple.m_Item5);

    //        if (c != 0) return c;

    //        c = comparer.Compare(m_Item6, objTuple.m_Item6);

    //        if (c != 0) return c;

    //        c = comparer.Compare(m_Item7, objTuple.m_Item7);

    //        if (c != 0) return c;

    //        return comparer.Compare(m_Rest, objTuple.m_Rest);
    //    }

    //    public override int GetHashCode()
    //    {
    //        return ((IStructuralEquatable)this).GetHashCode(EqualityComparer<Object>.Default);
    //    }

    //    Int32 IStructuralEquatable.GetHashCode(IEqualityComparer comparer)
    //    {
    //        // We want to have a limited hash in this case.  We'll use the last 8 elements of the tuple
    //        ITupleInternal t = (ITupleInternal)m_Rest;
    //        if (t.Length >= 8) { return t.GetHashCode(comparer); }

    //        // In this case, the rest memeber has less than 8 elements so we need to combine some our elements with the elements in rest
    //        int k = 8 - t.Length;
    //        switch (k)
    //        {
    //            case 1:
    //                return Tuple.CombineHashCodes(comparer.GetHashCode(m_Item7), t.GetHashCode(comparer));
    //            case 2:
    //                return Tuple.CombineHashCodes(comparer.GetHashCode(m_Item6), comparer.GetHashCode(m_Item7), t.GetHashCode(comparer));
    //            case 3:
    //                return Tuple.CombineHashCodes(comparer.GetHashCode(m_Item5), comparer.GetHashCode(m_Item6), comparer.GetHashCode(m_Item7), t.GetHashCode(comparer));
    //            case 4:
    //                return Tuple.CombineHashCodes(comparer.GetHashCode(m_Item4), comparer.GetHashCode(m_Item5), comparer.GetHashCode(m_Item6), comparer.GetHashCode(m_Item7), t.GetHashCode(comparer));
    //            case 5:
    //                return Tuple.CombineHashCodes(comparer.GetHashCode(m_Item3), comparer.GetHashCode(m_Item4), comparer.GetHashCode(m_Item5), comparer.GetHashCode(m_Item6), comparer.GetHashCode(m_Item7), t.GetHashCode(comparer));
    //            case 6:
    //                return Tuple.CombineHashCodes(comparer.GetHashCode(m_Item2), comparer.GetHashCode(m_Item3), comparer.GetHashCode(m_Item4), comparer.GetHashCode(m_Item5), comparer.GetHashCode(m_Item6), comparer.GetHashCode(m_Item7), t.GetHashCode(comparer));
    //            case 7:
    //                return Tuple.CombineHashCodes(comparer.GetHashCode(m_Item1), comparer.GetHashCode(m_Item2), comparer.GetHashCode(m_Item3), comparer.GetHashCode(m_Item4), comparer.GetHashCode(m_Item5), comparer.GetHashCode(m_Item6), comparer.GetHashCode(m_Item7), t.GetHashCode(comparer));
    //        }
    //        Contract.Assert(false, "Missed all cases for computing Tuple hash code");
    //        return -1;
    //    }

    //    Int32 ITupleInternal.GetHashCode(IEqualityComparer comparer)
    //    {
    //        return ((IStructuralEquatable)this).GetHashCode(comparer);
    //    }
    //    public override string ToString()
    //    {
    //        StringBuilder sb = new StringBuilder();
    //        sb.Append("(");
    //        return ((ITupleInternal)this).ToString(sb);
    //    }

    //    string ITupleInternal.ToString(StringBuilder sb)
    //    {
    //        sb.Append(m_Item1);
    //        sb.Append(", ");
    //        sb.Append(m_Item2);
    //        sb.Append(", ");
    //        sb.Append(m_Item3);
    //        sb.Append(", ");
    //        sb.Append(m_Item4);
    //        sb.Append(", ");
    //        sb.Append(m_Item5);
    //        sb.Append(", ");
    //        sb.Append(m_Item6);
    //        sb.Append(", ");
    //        sb.Append(m_Item7);
    //        sb.Append(", ");
    //        return ((ITupleInternal)m_Rest).ToString(sb);
    //    }

    //    /// <summary>
    //    /// The number of positions in this data structure.
    //    /// </summary>
    //    int ITuple.Length
    //    {
    //        get
    //        {
    //            return 7 + ((ITupleInternal)Rest).Length;
    //        }
    //    }

    //    /// <summary>
    //    /// Get the element at position <param name="index"/>.
    //    /// </summary>
    //    object ITuple.this[int index]
    //    {
    //        get
    //        {
    //            switch (index)
    //            {
    //                case 0:
    //                    return Item1;
    //                case 1:
    //                    return Item2;
    //                case 2:
    //                    return Item3;
    //                case 3:
    //                    return Item4;
    //                case 4:
    //                    return Item5;
    //                case 5:
    //                    return Item6;
    //                case 6:
    //                    return Item7;
    //            }

    //            return ((ITupleInternal)Rest)[index - 7];
    //        }
    //    }
    //}
}