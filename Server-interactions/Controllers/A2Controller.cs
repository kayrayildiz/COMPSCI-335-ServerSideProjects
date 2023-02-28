using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

using A2.Models;
using A2.Data;
using A2.Dtos;

namespace A2.Controllers
{
    [Route("api")]
    [ApiController]
    public class CustomersController : Controller
    {
        private readonly IA2Repo _repository;

        public CustomersController(IA2Repo repository)
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
                Address = user.Address,
            };

            return _repository.Register(u);
        }

        // Endpoint 2 | Get version
        [Authorize(AuthenticationSchemes = "A2Authentication")]
        [Authorize(Policy = "UserOnly")]
        [HttpGet("GetVersionA")]
        public ActionResult<string> GetVersionA()
        {
            return Ok("1.0.0 (user)");
        }

        // Endpoint 3 | Purchace an item
        [Authorize(AuthenticationSchemes = "A2Authentication")]
        [Authorize(Policy = "UserOnly")]
        [HttpGet("PurchaseItem/{id}")]
        public Order PurchaseItem(int id)
        {
            ClaimsIdentity ci = HttpContext.User.Identities.FirstOrDefault();
            Claim c = ci.FindFirst("userName");
            string name = c.Value;

            Order newOrder = new Order { userName=name, productId=id };
            return newOrder;
        }

        // Endpoint 4 | Start a game
        [Authorize(AuthenticationSchemes = "A2Authentication")]
        [Authorize(Policy = "UserOnly")]
        [HttpGet("PairMe")]
        public GameRecordOut PairMe()
        {
            ClaimsIdentity ci = HttpContext.User.Identities.FirstOrDefault();
            Claim c = ci.FindFirst("userName");
            string name = c.Value;

            return _repository.PairMe(name);
        }

        // Endpoint 5 | Get opponent's move
        [Authorize(AuthenticationSchemes = "A2Authentication")]
        [Authorize(Policy = "UserOnly")]
        [HttpGet("TheirMove/{gameId}")]
        public string TheirMove(string gameId)
        {
            ClaimsIdentity ci = HttpContext.User.Identities.FirstOrDefault();
            Claim c = ci.FindFirst("userName");
            string name = c.Value;

            GameRecord gr = _repository.GetGameRecord(gameId);
            if (gr != null)
            {
                return _repository.TheirMove(name, gr);
            }
            else
                return "no such gameId";
        }

        // Endpoint 6 | Make a move
        [Authorize(AuthenticationSchemes = "A2Authentication")]
        [Authorize(Policy = "UserOnly")]
        [HttpPost("MyMove")]
        public string MyMove(GameMove gm)
        {
            ClaimsIdentity ci = HttpContext.User.Identities.FirstOrDefault();
            Claim c = ci.FindFirst("userName");
            string name = c.Value;

            GameRecord gr = _repository.GetGameRecord(gm.GameId);
            if (gr != null)
            {
                return _repository.MyMove(name, gm.Move, gr);
            }
            else
                return "no such gameId";
        }

        // Endpoint 7 | Quit a game
        [Authorize(AuthenticationSchemes = "A2Authentication")]
        [Authorize(Policy = "UserOnly")]
        [HttpGet("QuitGame/{gameId}")]
        public string QuitGame(string gameId)
        {
            ClaimsIdentity ci = HttpContext.User.Identities.FirstOrDefault();
            Claim c = ci.FindFirst("userName");
            string name = c.Value;

            GameRecord gr = _repository.GetGameRecord(gameId);
            if (gr != null)
            {
                return _repository.QuitGame(name, gr);
            }
            else
                return "no such gameId";
        }
    }
}
