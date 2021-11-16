using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Project2.Models
{
    public class PhotoViewModel
    {
        [Key]
        public int PhotoId { get; set; }

        [DisplayName("Creator")]
        public string UserId { get; set; }

        [DisplayName("Photo")]
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
