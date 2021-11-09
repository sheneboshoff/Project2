using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Project2.Models
{
    public class Photo
    {
        public int PhotoId { get; set; }
        public string UserId { get; set; }

        [DisplayName("Name")]
        public string Photo_Name { get; set; }

        [DisplayName("Format")]
        public string Photo_Format { get; set; }

        [DisplayName("Geolocation")]
        public string Photo_Geolocation { get; set; }

        [DisplayName("Tags")]
        public string Photo_Tags { get; set; }

        [DisplayName("Capture Date")]
        public string Photo_CaptureDate { get; set; }
    }
}
