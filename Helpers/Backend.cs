using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helpers
{
    /// <summary>
    /// The Backend class is designed to work with ASP.NET Core environment. Provides helpful functionalities to backend developers.
    /// Can also work with other environments.
    /// </summary>
    public class Backend
    {
        /// <summary>
        /// Converts a given Image object into it's base 64 string representation.
        /// </summary>
        /// <param name="image">The image object to be converted.</param>
        /// <returns>64 encoded string represtation of the image. (Ex data:image/gif;base64,/9j/4AAQSkZJRgABAgEAYABgAAD..)</returns>
        public static string ConvertImageTo64String(Image image) 
        {
            using (image)
            {
                using (MemoryStream m = new MemoryStream())
                {
                    image.Save(m, image.RawFormat);
                    byte[] imageBytes = m.ToArray();
                    return Convert.ToBase64String(imageBytes);
                }
            }
        }

        /// <summary>
        /// Converts the encoded 64 string Image data into Image object.
        /// </summary>
        /// <param name="imageString">The encoded 64 string data.</param>
        /// <returns>Image object to be used.</returns>
        public static Image Convert64StringToImage(string imageString)
        {
            using (MemoryStream m = new MemoryStream(Convert.FromBase64String(imageString)))
            {
                return Image.FromStream(m);
            }
        }

        /// <summary>
        /// Converts a given Image object into it's byte array representation.
        /// </summary>
        /// <param name="image">The image object to be converted.</param>
        /// <returns>Byte array representation of the image.</returns>
        public static byte[] ConvertImageToBytes(Image image)
        {
            using (image)
            {
                using (MemoryStream m = new MemoryStream())
                {
                    image.Save(m, image.RawFormat);
                    return m.ToArray();
                }
            }
        }

        /// <summary>
        /// Converts the bytes array of the Image into actual Image object.
        /// </summary>
        /// <param name="bytes">The bytes array to be converted.</param>
        /// <returns>Image object to be used.</returns>
        public static Image ConvertBytesToImage(byte[] bytes)
        {
            using (MemoryStream m = new MemoryStream(bytes))
            {
                return Image.FromStream(m);
            }
        }

        /// <summary>
        /// Converts a given IFormFile object into it's byte array representation.
        /// </summary>
        /// <param name="formFile">The FormFile object to be converted.</param>
        /// <returns>The byte array that represent the file.</returns>
        public static byte[] ConvertFileToBytes(IFormFile formFile)
        {
            using (MemoryStream m = new MemoryStream())
            {
                formFile.CopyTo(m);
                return m.ToArray();
            }
        }

        /// <summary>
        /// Converts a given byte array into the equavilent File to be used by the environment. 
        /// The file is automatically set into it's correct type by specific the name and extension of the file.
        /// </summary>
        /// <param name="bytes">The byte array contents of the stored file.</param>
        /// <param name="fileName">File name and extension to be created.</param>
        /// <returns>FileStreamResult object that can be used by the environment.</returns>
        public static FileStreamResult ConvertBytesToFile(byte[] bytes, string fileName)
        {
            return new FileStreamResult(new MemoryStream(bytes), "application/octet-stream")
            {
                FileDownloadName = fileName
            };
        }

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
