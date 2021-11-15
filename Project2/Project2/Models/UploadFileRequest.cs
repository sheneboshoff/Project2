using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Project2.Models
{
    public class UploadFileRequest
    {
        [DisplayName("Photo Path")]
        public string FilePath { get; set; }

        [DisplayName("Photo Name")]
        public string FileName { get; set; }
    }
}
