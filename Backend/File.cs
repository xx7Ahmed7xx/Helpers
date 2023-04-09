using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AAM.Helpers.Common
{
    /// <summary>
    /// The Files class is designed to work with ASP.NET Core environment. Provides helpful functionalities to backend developers uploading/downloading files from their server.
    /// Can also work with other environments.
    /// </summary>
    public class File
    {
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

        
    }
}
