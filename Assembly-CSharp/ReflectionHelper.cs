using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MonoMod.Utils;

namespace Modding
{
    /// <summary>
    ///     A class to aid in reflection while caching it.
    /// </summary>
    public static class ReflectionHelper
    {
        private static readonly ConcurrentDictionary<Type, ConcurrentDictionary<string, FieldInfo>> Fields = new();

        private static readonly ConcurrentDictionary<FieldInfo, Delegate> Getters = new();

        private static readonly ConcurrentDictionary<FieldInfo, Delegate> Setters = new();

        private static bool _preloaded;

        /// <summary>
        ///     Caches all fields on a type to frontload cost of reflection
        /// </summary>
        /// <typeparam name="T">The type to cache</typeparam>
        private static void CacheFields<T>()
        {
            Type t = typeof(T);

            if (!Fields.TryGetValue(t, out ConcurrentDictionary<string, FieldInfo> tFields))
            {
                tFields = new ConcurrentDictionary<string, FieldInfo>();
            }

            const BindingFlags privStatic = BindingFlags.NonPublic | BindingFlags.Static;
            const BindingFlags all = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static;

            // Not gonna redesign this class to avoid reflection, this shouldn't be called during gameplay anyway
            MethodInfo getGetter = typeof(ReflectionHelper).GetMethod(nameof(GetGetter), privStatic);
            MethodInfo getSetter = typeof(ReflectionHelper).GetMethod(nameof(GetSetter), privStatic);

            Parallel.ForEach
            (
                t.GetFields(all),
                field =>
                {
                    tFields[field.Name] = field;

                    if (!field.IsLiteral)
                    {
                        getGetter?.MakeGenericMethod(t, field.FieldType).Invoke(null, new object[] { field });
                    }

                    if (!field.IsLiteral && !field.IsInitOnly)
                    {
                        getSetter?.MakeGenericMethod(t, field.FieldType).Invoke(null, new object[] { field });
                    }
                }
            );
        }

        /// <summary>
        ///     Gets a field on a type
        /// </summary>
        /// <param name="t">Type</param>
        /// <param name="field">Field name</param>
        /// <param name="instance"></param>
        /// <returns>FieldInfo for field or null if field does not exist.</returns>
        public static FieldInfo GetFieldInfo(Type t, string field, bool instance = true)
        {
            if (!Fields.TryGetValue(t, out ConcurrentDictionary<string, FieldInfo> typeFields))
            {
                Fields[t] = typeFields = new ConcurrentDictionary<string, FieldInfo>();
            }

            if (typeFields.TryGetValue(field, out FieldInfo fi))
            {
                return fi;
            }

            fi = t.GetField
            (
                field,
                BindingFlags.NonPublic | BindingFlags.Public | (instance ? BindingFlags.Instance : BindingFlags.Static)
            );

            if (fi != null)
            {
                typeFields.TryAdd(field, fi);
            }

            return fi;
        }

        internal static void PreloadCommonTypes()
        {
            if (_preloaded)
                return;

            var watch = new Stopwatch();
            watch.Start();

            Parallel.Invoke
            (
                CacheFields<PlayerData>,
                CacheFields<HeroController>,
                CacheFields<HeroControllerStates>,
                CacheFields<GameManager>,
                CacheFields<UIManager>
            );

            watch.Stop();

            Logger.APILogger.Log($"Preloaded reflection in {watch.ElapsedMilliseconds}ms");

            _preloaded = true;
        }

        /// <summary>
        ///     Gets delegate getting field on type
        /// </summary>
        /// <param name="fi">FieldInfo for field.</param>
        /// <returns>Function which gets value of field</returns>
        private static Delegate GetGetter<TType, TField>(FieldInfo fi)
        {
            if (Getters.TryGetValue(fi, out Delegate d))
            {
                return d;
            }

            if (fi.IsLiteral)
            {
                throw new ArgumentException("Field cannot be const", nameof(fi));
            }

            d = fi.IsStatic
                ? CreateGetStaticFieldDelegate<TField>(fi)
                : CreateGetFieldDelegate<TType, TField>(fi);

            Getters[fi] = d;

            return d;
        }

        /// <summary>
        ///     Gets delegate setting field on type
        /// </summary>
        /// <param name="fi">FieldInfo for field.</param>
        /// <returns>Function which sets field passed as FieldInfo</returns>
        private static Delegate GetSetter<TType, TField>(FieldInfo fi)
        {
            if (Setters.TryGetValue(fi, out Delegate d))
            {
                return d;
            }

            if (fi.IsLiteral || fi.IsInitOnly)
            {
                throw new ArgumentException("Field cannot be readonly or const", nameof(fi));
            }

            d = fi.IsStatic
                ? CreateSetStaticFieldDelegate<TField>(fi)
                : CreateSetFieldDelegate<TType, TField>(fi);

            Setters[fi] = d;

            return d;
        }

        /// <summary>
        ///     Create delegate returning value of static field.
        /// </summary>
        /// <param name="fi">FieldInfo of field</param>
        /// <typeparam name="TField">Field type</typeparam>
        /// <returns>Function returning static field</returns>
        private static Delegate CreateGetStaticFieldDelegate<TField>(FieldInfo fi)
        {
            var dm = new DynamicMethodDefinition
            (
                "FieldAccess" + fi.DeclaringType?.Name + fi.Name,
                typeof(TField),
                new Type[0]
            );

            ILGenerator gen = dm.GetILGenerator();

            gen.Emit(OpCodes.Ldsfld, fi);
            gen.Emit(OpCodes.Ret);

            return dm.Generate().CreateDelegate(typeof(Func<TField>));
        }

        /// <summary>
        ///     Create delegate returning value of field of object
        /// </summary>
        /// <param name="fi"></param>
        /// <typeparam name="TType"></typeparam>
        /// <typeparam name="TField"></typeparam>
        /// <returns>Function which returns value of field of object parameter</returns>
        private static Delegate CreateGetFieldDelegate<TType, TField>(FieldInfo fi)
        {
            var dm = new DynamicMethodDefinition
            (
                "FieldAccess" + fi.DeclaringType?.Name + fi.Name,
                typeof(TField),
                new[] { typeof(TType) }
            );

            ILGenerator gen = dm.GetILGenerator();

            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldfld, fi);
            gen.Emit(OpCodes.Ret);

            return dm.Generate().CreateDelegate(typeof(Func<TType, TField>));
        }

        private static Delegate CreateSetFieldDelegate<TType, TField>(FieldInfo fi)
        {
            var dm = new DynamicMethodDefinition
            (
                "FieldSet" + fi.DeclaringType?.Name + fi.Name,
                typeof(void),
                new[] { typeof(TType), typeof(TField) }
            );

            ILGenerator gen = dm.GetILGenerator();

            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldarg_1);
            gen.Emit(OpCodes.Stfld, fi);
            gen.Emit(OpCodes.Ret);

            return dm.Generate().CreateDelegate(typeof(Action<TType, TField>));
        }

        private static Delegate CreateSetStaticFieldDelegate<TField>(FieldInfo fi)
        {
            var dm = new DynamicMethodDefinition
            (
                "FieldSet" + fi.DeclaringType?.Name + fi.Name,
                typeof(void),
                new[] { typeof(TField) }
            );

            ILGenerator gen = dm.GetILGenerator();

            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Stsfld, fi);
            gen.Emit(OpCodes.Ret);

            return dm.Generate().CreateDelegate(typeof(Action<TField>));
        }

        /// <summary>
        ///     Get a field on an object using a string. Cast to TCast before returning and if field doesn't exist return default.
        /// </summary>
        /// <param name="obj">Object/Object of type which the field is on</param>
        /// <param name="name">Name of the field</param>
        /// <param name="default">Default return</param>
        /// <typeparam name="TField">Type of field</typeparam>
        /// <typeparam name="TObject">Type of object being passed in</typeparam>
        /// <typeparam name="TCast">Type of return.</typeparam>
        /// <returns>The value of a field on an object/type</returns>
        [PublicAPI]
        public static TCast GetField<TObject, TField, TCast>(TObject obj, string name, TCast @default = default)
        {
            FieldInfo fi = GetFieldInfo(typeof(TObject), name);

            return fi == null
                ? @default
                : (TCast) (object) ((Func<TObject, TField>) GetGetter<TObject, TField>(fi))(obj);
        }

        /// <summary>
        ///     Get a field on an object using a string.
        /// </summary>
        /// <param name="obj">Object/Object of type which the field is on</param>
        /// <param name="name">Name of the field</param>
        /// <typeparam name="TField">Type of field</typeparam>
        /// <typeparam name="TObject">Type of object being passed in</typeparam>
        /// <returns>The value of a field on an object/type</returns>
        [PublicAPI]
        public static TField GetField<TObject, TField>(TObject obj, string name)
        {
            FieldInfo fi = GetFieldInfo(typeof(TObject), name) ?? throw new MissingFieldException();

            return ((Func<TObject, TField>) GetGetter<TObject, TField>(fi))(obj);
        }

        /// <summary>
        ///     Get a static field on an type using a string.
        /// </summary>
        /// <param name="name">Name of the field</param>
        /// <typeparam name="TType">Type which static field resides upon</typeparam>
        /// <typeparam name="TField">Type of field</typeparam>
        /// <returns>The value of a field on an object/type</returns>
        [PublicAPI]
        public static TField GetField<TType, TField>(string name)
        {
            FieldInfo fi = GetFieldInfo(typeof(TType), name, false);

            return fi == null ? default : ((Func<TField>) GetGetter<TType, TField>(fi))();
        }

        /// <summary>
        ///     Set a field on an object using a string.
        /// </summary>
        /// <param name="obj">Object/Object of type which the field is on</param>
        /// <param name="name">Name of the field</param>
        /// <param name="value">Value to set the field to</param>
        /// <typeparam name="TField">Type of field</typeparam>
        /// <typeparam name="TObject">Type of object being passed in</typeparam>
        [PublicAPI]
        public static void SetFieldSafe<TObject, TField>(TObject obj, string name, TField value)
        {
            FieldInfo fi = GetFieldInfo(typeof(TObject), name);

            if (fi == null)
            {
                return;
            }

            ((Action<TObject, TField>) GetSetter<TObject, TField>(fi))(obj, value);
        }

        /// <summary>
        ///     Set a field on an object using a string.
        /// </summary>
        /// <param name="obj">Object/Object of type which the field is on</param>
        /// <param name="name">Name of the field</param>
        /// <param name="value">Value to set the field to</param>
        /// <typeparam name="TField">Type of field</typeparam>
        /// <typeparam name="TObject">Type of object being passed in</typeparam>
        [PublicAPI]
        public static void SetField<TObject, TField>(TObject obj, string name, TField value)
        {
            FieldInfo fi = GetFieldInfo(typeof(TObject), name) ?? throw new MissingFieldException($"Field {name} does not exist!");
            
            ((Action<TObject, TField>) GetSetter<TObject, TField>(fi))(obj, value);
        }

        /// <summary>
        ///     Set a static field on an type using a string.
        /// </summary>
        /// <param name="name">Name of the field</param>
        /// <param name="value">Value to set the field to</param>
        /// <typeparam name="TType">Type which static field resides upon</typeparam>
        /// <typeparam name="TField">Type of field</typeparam>
        [PublicAPI]
        public static void SetField<TType, TField>(string name, TField value)
        {
            ((Action<TField>) GetSetter<TType, TField>(GetFieldInfo(typeof(TType), name, false)))(value);
        }
    }
}