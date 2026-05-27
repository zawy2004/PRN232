namespace ContentNegotiationDemo.CustomFormatters
{
    using Microsoft.AspNetCore.Mvc.Formatters;
    using System.Text;
    using System.Threading.Tasks;
    using ContentNegotiationDemo.Models;
    using System;
    using System.Linq;
    using System.Collections.Generic;

    public class CsvOutputFormatter : TextOutputFormatter
    {
        public CsvOutputFormatter()
        {
            SupportedMediaTypes.Add("text/csv");
            SupportedEncodings.Add(Encoding.UTF8);
            SupportedEncodings.Add(Encoding.Unicode);
        }

        protected override bool CanWriteType(Type type)
        {
            if (typeof(Blog).IsAssignableFrom(type) || typeof(IEnumerable<Blog>).IsAssignableFrom(type))
            {
                return true;
            }

            return false;
        }

        public override async Task WriteResponseBodyAsync(OutputFormatterWriteContext context, Encoding selectedEncoding)
        {
            var response = context.HttpContext.Response;
            var buffer = new StringBuilder();

            if (context.Object is IEnumerable<Blog> blogs)
            {
                buffer.AppendLine("Name,Description,BlogPosts");
                foreach (var blog in blogs)
                {
                    FormatCsv(buffer, blog);
                }
            }
            else if (context.Object is Blog blog)
            {
                buffer.AppendLine("Name,Description,BlogPosts");
                FormatCsv(buffer, blog);
            }

            await response.WriteAsync(buffer.ToString(), selectedEncoding);
        }

        private static void FormatCsv(StringBuilder buffer, Blog blog)
        {
            string Escape(string s)
            {
                if (string.IsNullOrEmpty(s)) return string.Empty;
                if (s.Contains(',') || s.Contains('"') || s.Contains('\n'))
                {
                    return '"' + s.Replace("\"", "\"\"") + '"';
                }
                return s;
            }

            var posts = blog.BlogPosts != null && blog.BlogPosts.Any()
                ? string.Join(";", blog.BlogPosts.Select(p => p.Title))
                : string.Empty;

            buffer.AppendLine($"{Escape(blog.Name)},{Escape(blog.Description)},{Escape(posts)}");
        }
    }
}
