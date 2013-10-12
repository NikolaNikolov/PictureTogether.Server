namespace PictureTogether.Data.Migrations
{
    using PictureTogether.Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    public sealed class Configuration : DbMigrationsConfiguration<PictureTogether.Data.PictureTogetherContext>
    {
        public Configuration()
        {
            this.AutomaticMigrationsEnabled = true;
            this.AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(PictureTogether.Data.PictureTogetherContext context)
        {
            var users = new User[]{
                new User
                {
                    Username = "nikola",
                    AuthCode = "b48630ff92ccdb863262a934a8272a6b586031ba"
                },
                new User
                {
                    Username = "justme",
                    AuthCode = "b48630ff92ccdb863262a934a8272a6b586031ba"
                }
            };

            var albums = new Album[]
            {
                new Album
                {
                    Name = "First",
                    Users = {
                        users[0]
                    }
                },
                new Album
                {
                    Name = "Second",
                    Users = {
                        users[0]
                    }
                },
                new Album
                {
                    Name = "Third",
                    Users = {
                        users[0]
                    }
                },
                new Album
                {
                    Name = "Fourth",
                    Users = {
                        users[0]
                    }
                },
                new Album
                {
                    Name = "Fifth",
                    Users = {
                        users[0]
                    }
                }
            };

            users[0].Albums = albums;
            users[1].Albums = new Album[]
            {
                albums[0],
                albums[1],
                albums[2]
            };

            foreach (var user in users)
            {
                if (context.Users.FirstOrDefault(u => u.Username == user.Username) == null)
                {
                    context.Users.Add(user);
                }
            }

            context.SaveChanges();
        }
    }
}
