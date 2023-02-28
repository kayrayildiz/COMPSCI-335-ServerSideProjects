using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using quiz.Models;

namespace quiz.Data
{
    public interface IQuizRepo
    {
        bool ValidLogin(string userName, string password);
        bool ValidAdminLogin(string userName, string password);
        string Register(User user);
        IEnumerable<Item> GetAllItems();
        Item GetItem(int id);
        Item AddItem(Item item);
        IEnumerable<Item> OrderItems(IEnumerable<Item> items);
        string CloseAuction(string userName, int id);
        string UploadImage(IFormFile file, string UserName);
    }
}
