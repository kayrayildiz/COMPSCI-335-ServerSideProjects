using A1.Models;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using A1.Dtos;

namespace A1.Data
{
    public class A1Repo : IA1Repo
    {
        private readonly A1DBContext _dbContext;

        public A1Repo(A1DBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IEnumerable<Product> AllItems()
        {
            IEnumerable<Product> products = _dbContext.Products.ToList();
            return products;
        }

        public IEnumerable<Product> GetItems(string name)
        {
            IEnumerable<Product> products = _dbContext.Products.Where(p => p.Name.ToLower().Contains(name));
            return products;
        }

        public Comment WriteComment(Comment comment)
        {
            EntityEntry<Comment> t = _dbContext.Comments.Add(comment);
            Comment c = t.Entity;
            _dbContext.SaveChanges();
            return c;
        }

        public IEnumerable<CommentDto> GetComments()
        {
            List<CommentDto> comments = _dbContext.Comments.Select(c => new CommentDto { UserComment = c.UserComment, Name = c.Name }).ToList<CommentDto>();

            if (comments.Count >= 5)
            {
                IEnumerable<CommentDto> lastFive = comments.Skip(comments.Count - 5).Take(5).Reverse();
                return lastFive;
            }

            else
            {
                IEnumerable<CommentDto> lastFive = comments.Take(5).Reverse();
                return lastFive;
            }
        }
    }
}
