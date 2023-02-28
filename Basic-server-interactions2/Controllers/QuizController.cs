using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using quiz.Data;
using quiz.Models;
using quiz.Dtos;
using System.Security.Claims;
using System.Net.Mime;
// Helpful IDs: 4718598, 21626395, 25173739,34685231, 35641539, 35648762

namespace quiz.Controllers
{
    [Route("api")]
    [ApiController]

    public class A1Controller : Controller
    {
        private readonly IQuizRepo _repository;

        public A1Controller(IQuizRepo repository)
        {
            _repository = repository;
        }

        // Endpoint 1 | User registration
        [HttpPost("Register")]
        public ActionResult<string> Register(User user)
        {
            User u = new User
            {
                UserName = user.UserName,
                Password = user.Password,
            };

            return _repository.Register(u);
        }

        // Endpoint 2 | List auction items
        [HttpGet("ListItems")]
        public IEnumerable<Item> ListItems()
        {
            return _repository.GetAllItems();
        }

        // Endpoint 3 | Get the photo of an item
        [HttpGet("GetItemPhoto/{id}")]
        public ActionResult GetItemPhoto(int id)
        {
            string path = Directory.GetCurrentDirectory();
            string imgDir = Path.Combine(path, "Photos");
            string fileName1 = Path.Combine(imgDir, id.ToString() + ".png");
            string fileName2 = Path.Combine(imgDir, id.ToString() + ".jpeg");
            string fileName3 = Path.Combine(imgDir, id.ToString() + ".gif");

            string respHeader = "";
            string fileName = "";

            if (System.IO.File.Exists(fileName1))
            {
                respHeader = "image/png";
                fileName = fileName1;
            }

            else if (System.IO.File.Exists(fileName2))
            {
                respHeader = "image/jpeg";
                fileName = fileName2;
            }

            else if (System.IO.File.Exists(fileName3))
            {
                respHeader = "image/gif";
                fileName = fileName3;
            }

            else
            {
                respHeader = "application/pdf";
                fileName = Path.Combine(imgDir, "logo.pdf");
            }

            return PhysicalFile(fileName, respHeader); // test with 4718598.jpeg also
        }

        // Endpoint 4 | Get an item
        [HttpGet("GetItem/{id}")]
        public ActionResult<Item> GetItem(int id)
        {
            Item item = _repository.GetItem(id);
            if (item == null)
            {
                return StatusCode(204, null);
            }
            else
            {
                return Ok(item);
            }
        }

        // Endpoint 5 | Add a new auction item
        [Authorize(AuthenticationSchemes = "QuizAuthentication")]
        [Authorize(Policy = "UserOnly")]
        [HttpPost("AddItem")]
        public ActionResult<Item> AddItem(ItemInput ii)
        {
            ClaimsIdentity ci = HttpContext.User.Identities.FirstOrDefault();
            Claim c = ci.FindFirst("userName");
            string userName = c.Value;

            if (ii.Description == null || ii.Title == null)
            {
                return StatusCode(400);
            }

            else
            {
                Item newItem;

                if (ii.StartBid == null)
                {
                    newItem = new Item { Owner = userName, Title = ii.Title, Description = ii.Description, StartBid = 0, State = "active" };
                    _repository.AddItem(newItem);
                }
                else 
                {
                    newItem = new Item { Owner = userName, Title = ii.Title, Description = ii.Description, StartBid = (float)ii.StartBid, State = "active" };
                    _repository.AddItem(newItem);
                }

                return CreatedAtAction(nameof(GetItem), new { id = newItem.Id }, newItem);
            }
        }

        // Endpoint 6 | List auction item for administrator
        [Authorize(AuthenticationSchemes = "QuizAuthentication")]
        [Authorize(Policy = "AdminOnly")]
        [HttpGet("ListItemsAdmin")]
        public IEnumerable<Item> ListItemsAdmin()
        {
            IEnumerable<Item> items = _repository.GetAllItems();
            IEnumerable<Item> orderedItems = _repository.OrderItems(items);
            return orderedItems;
        }

        // Endpoint 7 | Close an auction
        [Authorize(AuthenticationSchemes = "QuizAuthentication")]
        [Authorize(Policy = "UserOrAdmin")]
        [HttpGet("CloseAuction/{id}")]
        public string CloseAuction(int id)
        {
            ClaimsIdentity ci = HttpContext.User.Identities.FirstOrDefault();
            Claim c = ci.FindFirst("admin");

            if (c != null)
            {
                string userName = c.Value;
                Console.WriteLine("admin");
                return _repository.CloseAuction("admin", id);
            }

            else
            {
                c = ci.FindFirst("userName");
                string userName = c.Value;
                Console.WriteLine("user");
                return _repository.CloseAuction(userName, id);
            }
        }

        // Endpoint 8 | Upload the photo of an item
        [Authorize(AuthenticationSchemes = "QuizAuthentication")]
        [Authorize(Policy = "UserOnly")] // allows admins where it shouldnt
        [HttpPost("UploadImage")]
        public ActionResult<string> UploadImage(IFormFile file)
        {
            ClaimsIdentity ci = HttpContext.User.Identities.FirstOrDefault();
            Claim c = ci.FindFirst("userName");
            string userName = c.Value;
            string result = _repository.UploadImage(file, userName);
            if (result == null)
            {
                return StatusCode(500, null);
            }
            return Ok(result);
        }
    }
}


