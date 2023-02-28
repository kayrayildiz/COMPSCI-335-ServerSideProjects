using System;
using A1.Models;
using Microsoft.AspNetCore.Mvc;
using A1.Data;
using A1.Models;
using A1.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace A1.Controllers
{
    [Route("api")]
    [ApiController]

    public class A1Controller : Controller
    {
        private readonly IA1Repo _repository;

        public A1Controller(IA1Repo repository)
        {
            _repository = repository;
        }

        // Q1
        [HttpGet("GetLogo")]
        public ActionResult GetLogo()
        {
            string path = Directory.GetCurrentDirectory();
            string imgDir = Path.Combine(path, "Logos");
            string fileName = Path.Combine(imgDir, "Logo.png"); 
            string respHeader = "image/png";

            return PhysicalFile(fileName, respHeader);
        }

        // Q2
        [HttpGet("GetFavIcon")]
        public ActionResult GetFavIcon()
        {
            string path = Directory.GetCurrentDirectory();
            string imgDir = Path.Combine(path, "Logos");
            string fileName = Path.Combine(imgDir, "Logo-192x192.png");
            string respHeader = "image/png";

            return PhysicalFile(fileName, respHeader);
        }

        // Q3
        [HttpGet("GetVersion")]
        public String GetVersion()
        {
            return "1.0.0";
        }

        // Q4
        [HttpGet("AllItems")]
        public IEnumerable<Product> AllItems()
        {
            IEnumerable<Product> products = _repository.AllItems();
            return products;
        }

        // Q5
        [HttpGet("GetItems/{name}")]
        public IEnumerable<Product> GetItems(string name)
        {
            IEnumerable<Product> products = _repository.GetItems(name.ToLower());
            return products;
        }

        // Q6
        [HttpGet("ItemPhoto/{id}")]
        public ActionResult ItemPhoto(string id)
        {
            string path = Directory.GetCurrentDirectory();
            string imgDir = Path.Combine(path, "ItemsImages");
            string fileName1 = Path.Combine(imgDir, id + ".jpg");
            string fileName2 = Path.Combine(imgDir, id + ".gif");

            string respHeader = "";
            string fileName = "";

            if (System.IO.File.Exists(fileName1))
            {
                respHeader = "image/jpeg";
                fileName = fileName1;
            }

            else if (System.IO.File.Exists(fileName2))
            {
                respHeader = "image/jpeg";
                fileName = fileName2;
            }

            else if (( !(System.IO.File.Exists(fileName1)) ) && (! (System.IO.File.Exists(fileName2)) ))
            {
                respHeader = "image/png";
                fileName = Path.Combine(imgDir, "default.png");
            }
                
            return PhysicalFile(fileName, respHeader);
        }

        // Q7
        [HttpPost("WriteComment")]
        public string WriteComment(CommentDto comment)
        {
            Comment c = new Comment
            {
                UserComment = comment.UserComment,
                Name = comment.Name,
                IP = Request.HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(),
                Time = new DateTime().ToString()
            };

            Comment addedComment = _repository.WriteComment(c);

            CommentDto co = new CommentDto
            {
                UserComment = addedComment.UserComment,
                Name = addedComment.Name
            };

            return co.UserComment;
        }

        // Q8
        [HttpGet("GetComments")]
        public IEnumerable<CommentDto> GetComments()
        {
            IEnumerable<CommentDto> comments = _repository.GetComments();
            return comments;
        }
    }
}