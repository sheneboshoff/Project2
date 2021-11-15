using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Project2.Models
{
    public class BlobInfo
    {
        public Stream Content { get; set; }
        public string ContentType { get; set; }

        public string FileName { get; set; }
        public string FileExtension { get; set; }

        public BlobInfo(Stream content, string contentType)
        {
            this.Content = content;
            this.ContentType = contentType;
        }

        public BlobInfo(string FileName, string FileExtension)
        {
            this.FileName = FileName;
            this.FileExtension = FileExtension;
        }
    }
}
