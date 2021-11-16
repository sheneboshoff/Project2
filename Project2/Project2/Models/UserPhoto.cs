using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project2.Models
{
    public class UserPhoto
    {
        public int PhotoId { get; set; }
        public string Creator { get; set; }
        public string SharedWith { get; set; }
    }
}
