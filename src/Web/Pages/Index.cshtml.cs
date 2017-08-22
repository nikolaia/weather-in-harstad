using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MyLib;

namespace Web.Pages
{
    public class IndexModel : PageModel
    {
        private readonly BloggingContext db;
        public List<Post> Blogposts { get; private set; }

        public IndexModel(BloggingContext db)
        {
            this.db = db;
        }
        public void OnGet()
        {
            var blog = db.Blogs.Include(b => b.Posts).FirstOrDefault();
            Blogposts = blog?.Posts.ToList();
        }
    }
}
