using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;

namespace CheckBook.App.Helpers
{
    public class FileStorageHelper
    {

        public static string FileStoragePath
        {
            get
            {
                var path = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, "Images");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                return path;
            }
        }

        public static string ImageUrlPrefix => "/Images/";

        /// <summary>
        /// Stores the file and return its URL for the client.
        /// </summary>
        public static string StoreFile(Stream stream, string fileName)
        {
            var name = Guid.NewGuid() + Path.GetExtension(fileName);

            using (var fs = new FileStream(Path.Combine(FileStoragePath, name), FileMode.Create, FileAccess.Write))
            {
                stream.CopyTo(fs);
            }

            return ImageUrlPrefix + name;
        }

    }
}