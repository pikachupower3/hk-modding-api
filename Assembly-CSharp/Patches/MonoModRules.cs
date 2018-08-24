﻿using System;
using Mono.Cecil;
using MonoMod.InlineRT;

namespace MonoMod
{
    /// <summary>
    /// Class for controlling some of the adjustments needed for monomod transformations
    /// </summary>
    public static partial class MonoModRules
    {

        static MonoModRules()
        {
            MonoModRule.Modder.ShouldCleanupAttrib = IsUselessAttrib;
        }

        /// <summary>
        /// Supresses MonoMod attributes based on varying rules.  At the very least, we are supressing the "Modding" namespace from getting MonoMod Attributes.
        /// </summary>
        /// <param name="holder"></param>
        /// <param name="attribType"></param>
        /// <returns></returns>
        public static bool IsUselessAttrib(ICustomAttributeProvider holder, TypeReference attribType)
        {
            // If the attribute isn't a MonoMod attribute, it's "useful."
            if (attribType.Namespace.StartsWith("MonoMod") && attribType.Name.StartsWith("MonoMod"))
                return true;

            return false;
            /*
            TypeDefinition type = holder as TypeDefinition ??
                                  (holder as MethodDefinition)?.DeclaringType ??
                                  (holder as FieldDefinition)?.DeclaringType ??
                                  (holder as PropertyDefinition)?.DeclaringType;
            // If the holding type (or the holding method's type) is inside the Modding namespace, return true.
            return type == null || type.FullName.StartsWith("Modding.");*/
        }

        /// <summary>
        /// Returns the get_Instance method for ModHooks.Instance
        /// </summary>
        /// <param name="method">method being worked on</param>
        /// <returns></returns>
        public static MethodDefinition ModHooksInstance(MethodDefinition method)
        {
         //   Console.WriteLine("ModHooksInstance");
            TypeDefinition modHookType = null;

            foreach (TypeDefinition type in method.Module.Types)
            {
                if (type.Name == "ModHooks")
                {
                    modHookType = type;
                    break;
                }

            }

            if (modHookType == null)
            {
                Console.WriteLine("WARNING - Couldn't find ModHooks type");
                return null;
            }

            foreach (PropertyDefinition property in modHookType.Properties)
            {
                if (property.Name == "Instance")
                {
                    return property.GetMethod;
                }
            }
            
            Console.WriteLine("WARNING - Couldn't find ModHooks Instance Property");
            return null;

        }

        /// <summary>
        /// Returns a Method Definition for the ModHooks.Instance.&lt;hookname&gt;
        /// </summary>
        /// <param name="baseMethod">Method being added to</param>
        /// <param name="hookName">Name of method to add</param>
        /// <returns></returns>
        public static MethodDefinition ModHooksHook(MethodDefinition baseMethod, string hookName)
        {
         //   Console.WriteLine("ModHooksHook");
            MethodDefinition instance = ModHooksInstance(baseMethod);
            foreach (MethodDefinition method in instance.DeclaringType.Methods)
            {
                if (method.Name == hookName)
                    return method;
            }

            return null;
        }

        /// <summary>
        /// Returns a Field Definition for the method's DeclaringType to be added to the IL instructions
        /// </summary>
        /// <param name="baseMethod">Method being added to</param>
        /// <param name="fieldName">Name of field to add</param>
        /// <returns></returns>
        public static FieldDefinition GetClassField(MethodDefinition baseMethod, string fieldName)
        {
         //   Console.WriteLine("GetClassField");
            foreach (FieldDefinition field in baseMethod.DeclaringType.Fields)
            {
                if (field.Name == fieldName)
                    return field;
            }
            return null;
        }

        /// <summary>
        /// Returns a Field Definition for the method's DeclaringType to be added to the IL instructions
        /// </summary>
        /// <param name="baseMethod">Method being added to</param>
        /// <param name="typeName"></param>
        /// <param name="fieldName">Name of field to add</param>
        /// <returns></returns>
        public static FieldDefinition GetClassField(MethodDefinition baseMethod, string typeName, string fieldName)
        {
            //   Console.WriteLine("GetClassField");
            foreach (TypeDefinition type in baseMethod.DeclaringType.Module.Types)
            {
                if (type.Name != typeName) continue;
                foreach (FieldDefinition field in baseMethod.DeclaringType.Fields)
                {
                    if (field.Name == fieldName)
                        return field;
                }
            }
            return null;
        }

        /// <summary>
        /// Returns a Method Definition for a sibling method within the same type
        /// </summary>
        /// <param name="baseMethod">Method being added to</param>
        /// <param name="methodName">Method to return</param>
        /// <returns></returns>
        public static MethodDefinition GetMethodDefinition(MethodDefinition baseMethod, string methodName)
        {
        //    Console.WriteLine("GetMethodDefinition");
            foreach (MethodDefinition method in baseMethod.DeclaringType.Methods)
            {
                if (method.Name == methodName)
                    return method;
            }
            return null;
        }

        /// <summary>
        /// Get's a method outside of the current method's class.
        /// </summary>
        /// <param name="baseMethod">Method being added to</param>
        /// <param name="typeName">Type to look for method in</param>
        /// <param name="methodName">Method to get</param>
        /// <returns></returns>
        public static MethodDefinition GetMethodDefinition(MethodDefinition baseMethod, string typeName, string methodName)
        {
        //    Console.WriteLine("GetMethodDefinition");
            foreach (TypeDefinition type in baseMethod.DeclaringType.Module.Types)
            {
                if (type.Name != typeName) continue;

                foreach(MethodDefinition method in type.Methods)
                    if (method.Name == methodName)
                        return method;
            }
            return null;
        }
    }
}
