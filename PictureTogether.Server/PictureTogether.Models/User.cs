using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PictureTogether.Models
{
    public class User
    {
        public User()
        {
        }

        [Key]
        public int Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string AuthCode { get; set; }

        public string SessionKey { get; set; }
    }
}