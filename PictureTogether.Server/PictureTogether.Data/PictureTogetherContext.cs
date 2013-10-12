using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PictureTogether.Models;

namespace PictureTogether.Data
{
    public class PictureTogetherContext : DbContext
    {
        public PictureTogetherContext()
            : base("PictureTogether")
        {
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Album> Albums { get; set; }

        public DbSet<Picture> Pictures { get; set; }
    }
}
