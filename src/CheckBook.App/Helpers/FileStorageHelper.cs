using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Hosting;

namespace CheckBook.App.Helpers
{
    public class FileStorageHelper
    {
        private readonly IWebHostEnvironment env;

        public FileStorageHelper(IWebHostEnvironment env)
        {
            this.env = env;
        }

        public string FileStoragePath
        {
            get
            {
                var path = Path.Combine(env.WebRootPath, "Images");
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
        public string StoreFile(Stream stream, string fileName)
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