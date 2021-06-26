using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Threading.Tasks;
    
namespace ErrorMinimal
{
    
    public class TestDbContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Post> Posts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options) =>
            options.UseSqlite(@"Data Source=test.db");
    }

    public class Blog
    {
        public int BlogId { get; set; }
        public string Name { get; set; }

        public List<Post> Posts { get; } = new List<Post>();
    }

    public class Post
    {
        public int PostId { get; set; }
        public string Content { get; set; }
        public int Number { get; set; }
        
        public int BlogId { get; set; }
        public Blog Blog { get; set; }
    }

    class Dto
    {
       public int? Number { get; set; }
       public Post p { get; set; }
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            using var db = new TestDbContext();

            await db.AddAsync(new Blog {Name = "Test"});
            await db.SaveChangesAsync();

            var blog = db.Blogs.First();
            blog.Posts.Add( new Post { Content = "hello"} );

            await db.SaveChangesAsync();

            // SelectMany gives AmbigiousInvocation
            db.Blogs
                .SelectMany(b => b.Posts.Where(p => p.Content == "hello").DefaultIfEmpty(),
                    (b, p) => new Dto
                    {
                        p = p,
                        Number = p != null ? p.Number : null,
                    });
        }
    }
}