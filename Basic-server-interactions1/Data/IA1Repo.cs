using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using A1.Models;
using A1.Dtos;

namespace A1.Data
{
    public interface IA1Repo
    {
        IEnumerable<Product> AllItems();
        IEnumerable<Product> GetItems(string name);
        Comment WriteComment(Comment comment);
        IEnumerable<CommentDto> GetComments();
    }
}
