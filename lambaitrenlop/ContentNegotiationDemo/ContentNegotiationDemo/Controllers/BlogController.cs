using Microsoft.AspNetCore.Mvc;
using ContentNegotiationDemo.Models;
using System.Collections.Generic;

namespace ContentNegotiationDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            var blogPosts = new List<BlogPost>();
            blogPosts.Add(new BlogPost
            {
                Title = "Content negotiation in .NET Core",
                MetaDescription = "Content negotiation is one of those quality-of-life improvements you can add to your REST API to make it more user friendly and flexible.",
                Published = true
            });

            var blogs = new List<Blog>();
            var blog = new Blog
            {
                Name = "Code Maze",
                Description = "A practical programmers resource",
                BlogPosts = blogPosts
            };
            blogs.Add(blog);

            return Ok(blogs);
        }
    }
}
