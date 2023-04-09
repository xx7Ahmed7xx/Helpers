using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AAM.Helpers.Backend
{
    /// <summary>
    /// The Mapping class is designed to work with ASP.NET Core environment. Provides helpful functionalities to backend developers Mapping Objects.
    /// Can also work with other environments.
    /// </summary>
    public class Mapping
    {
        /// <summary>
        /// Automatically maps the first object's properties' values to the
        /// second object's properties' values. Works on private/public/instance/static properties.
        /// <br />Note: If the Object1's property's value is null, It won't override the value of Object2.
        /// </summary>
        /// <typeparam name="T">The type of the first object.</typeparam>
        /// <typeparam name="U">The type of the second object.</typeparam>
        /// <param name="obj1">The first object which the values will come from.</param>
        /// <param name="obj2">The second object which the values will be set into.</param>
        public static void MapFirstToSecond<T, U>(T obj1, U obj2)
        {
            Type t1 = typeof(T);
            Type t2 = typeof(U);
            var props1 = t1.GetProperties(
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static |
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var props2 = t2.GetProperties(
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static |
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            foreach (var prop2 in props2)
            {
                foreach (var prop1 in props1)
                {
                    if (prop2.Name == prop1.Name && prop1.GetValue(obj1) != null)
                    {
                        t2.InvokeMember(prop1.Name,
                            System.Reflection.BindingFlags.Static |
                            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public
                            | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.SetProperty,
                            Type.DefaultBinder, obj2, new object[] { prop1.GetValue(obj1)! });
                    }
                }
            }
        }
    }
}
