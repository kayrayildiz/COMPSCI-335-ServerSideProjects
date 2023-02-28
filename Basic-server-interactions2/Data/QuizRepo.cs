using Microsoft.EntityFrameworkCore.ChangeTracking;
using quiz.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace quiz.Data
{
    public class QuizRepo : IQuizRepo
    {
        private readonly QuizDBContext _dbContext;

        public QuizRepo(QuizDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Authentication
        public bool ValidLogin(string userName, string password)
        {
            User u = _dbContext.Users.FirstOrDefault(e => e.UserName == userName && e.Password == password);
            if (u == null)
                return false;
            else
                return true;
        }

        public bool ValidAdminLogin(string userName, string password)
        {
            Admin a = _dbContext.Admins.FirstOrDefault(e => e.UserName == userName && e.Password == password);
            if (a == null)
                return false;
            else
                return true;
        }

        // Endpoint methods
        public string Register(User user)
        {
            User u = _dbContext.Users.FirstOrDefault(e => e.UserName == user.UserName);

            if (u == null)
            {
                _dbContext.Users.Add(user);
                _dbContext.SaveChanges();
                return "User successfully registered.";
            }
            else

                return "Username not available.";
        }

        public IEnumerable<Item> GetAllItems()
        {
            return _dbContext.Items.ToList();
        }

        public Item GetItem(int id)
        {
            return _dbContext.Items.FirstOrDefault(i => i.Id == id);
        }

        public Item AddItem(Item item)
        {
            _dbContext.Items.Add(item);
            _dbContext.SaveChanges();
            return item;
        }

        public IEnumerable<Item> OrderItems(IEnumerable<Item> items)
        {
            return items.OrderBy(e => e.Id);
        }

        public string CloseAuction(string userName, int id)
        {
            if (userName == "admin")
            {
                // user is an admin
                Item i = _dbContext.Items.FirstOrDefault(e => e.Id == id);
                if (i != null)
                {
                    _dbContext.Items.Remove(i);
                    _dbContext.SaveChanges();
                    return "Auction closed.";
                }
                return "Auction does not exist.";
            }
            else 
            {
                // user is normal
                Item i = _dbContext.Items.FirstOrDefault(e => e.Id == id);
                if (i != null)
                {
                    if (i.Owner == userName)
                    {
                        _dbContext.Items.Remove(i);
                        _dbContext.SaveChanges();
                        return "Auction closed.";
                    }
                    else
                    {
                        return "You are not the owner of the auction.";
                    }
                }
                return "Auction does not exist.";
            }
        }

        public string UploadImage(IFormFile file, string userName)
        {
            string path = Directory.GetCurrentDirectory();
            string route = Path.Combine(path, "Photos");

            var extension = Path.GetExtension(file.FileName);
            var fileName = Path.GetFileNameWithoutExtension(file.FileName);

            Item item = _dbContext.Items.FirstOrDefault(i => i.Id.ToString() == fileName);

            if (item != null)
            {
                Console.WriteLine("Item was not empty");
                if (item.Owner == userName)
                {
                    string fileRoute = Path.Combine(route, fileName);

                    using (FileStream fileStream = File.Create(fileRoute))
                    {
                        file.OpenReadStream().CopyToAsync(fileStream);
                    }

                    Console.WriteLine("success");
                    return "Image uploaded successfully.";
                }
                else
                    return "You do not own the item.";
            }
            return null;
            
            
        }
    }
}
