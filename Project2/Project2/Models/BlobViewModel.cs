using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Project2.Models
{
    public class BlobViewModel
    {
        public string BlobContainerName { get; set; }
        public string StorageUri { get; set; }
        public string ActualFileName { get; set; }
        public string PrimaryUri { get; set; }
        public string fileExtension { get; set; }

        public string FileNameWithoutExt
        {
            get
            {
                return Path.GetFileNameWithoutExtension(ActualFileName);
            }
        }

        public string FileNameExtOnly
        {
            get
            {
                return Path.GetExtension(ActualFileName).Substring(1);
            }
        }
    }
}
