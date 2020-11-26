using System;
using System.IO;
using System.Web;

namespace SparkPost
{
    public abstract class File
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public string Data { get; set; }

        public static T Create<T>(string filename) where T : File, new()
        {
            var content = System.IO.File.ReadAllBytes(filename);
            return Create<T>(content, Path.GetFileName(filename));            
        }

        public static T Create<T>(string filename, string name) where T : File, new()
        {
            var result = Create<T>(filename);
            result.Name = name;
            return result;
        }

        public static T Create<T>(byte[] content) where T : File, new()
        {
            return Create<T>(content, String.Empty);
        }

        public static T Create<T>(byte[] content, string name) where T : File, new()
        {
            var result = new T();
            if (content != null)
            {
                result.Data = Convert.ToBase64String(content);
                result.Type = MimeMapping.MimeUtility.GetMimeMapping(name);
                result.Name = name;
            };
            return result;
        }

        public static T Create<T>(Stream content) where T : File, new()
        {
            return Create<T>(content, String.Empty);
        }

        public static T Create<T>(Stream content, string name) where T : File, new()
        {
            using (var ms = new MemoryStream())
            {
                content.CopyTo(ms);
                return Create<T>(ms.ToArray(), name);
            }
        }
    }
}