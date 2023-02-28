using Microsoft.EntityFrameworkCore.ChangeTracking;
using A2.Models;
using A2.Dtos;
using A2.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace A2.Data
{
    public class A2Repo : IA2Repo
    {
        private readonly A2DBContext _dbContext;

        public A2Repo(A2DBContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Helpful methods
        protected GameRecordOut createGameRecordOut(GameRecord gr)
        {
            GameRecordOut gro = new GameRecordOut
            {
                GameId = gr.GameId,
                State = gr.State,
                Player1 = gr.Player1,
                Player2 = gr.Player2,
                LastMovePlayer1 = gr.LastMovePlayer1,
                LastMovePlayer2 = gr.LastMovePlayer2
            };
            return gro;
        }

        public GameRecord GetGameRecord(string gameId)
        {
            GameRecord gr = _dbContext.GameRecords.FirstOrDefault(e => e.GameId == gameId);
            if (gr != null)
            {
                return gr;
            }
            else
                return null;
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

        public GameRecordOut PairMe(string userName)
        {
            GameRecord gr = _dbContext.GameRecords.FirstOrDefault(g => g.State == "wait");

            if (gr != null && gr.Player1 != userName)                           // player cannot play against themself
            {
                gr.State = "progress";
                gr.Player2 = userName;
                _dbContext.SaveChanges();

                GameRecordOut gro = createGameRecordOut(gr);
                return gro;
            }

            else                                                                // if no "wait" game exists, or the game in "progress" has Player1 == userName
            {
                GameRecord newGr = new GameRecord
                {
                    GameId = System.Guid.NewGuid().ToString(),
                    State = "wait",
                    Player1 = userName,
                    Player2 = null,
                    LastMovePlayer1 = null,
                    LastMovePlayer2 = null
                };

                _dbContext.GameRecords.Add(newGr);
                GameRecordOut gro = createGameRecordOut(newGr);
                _dbContext.SaveChanges();

                return gro;
            }
        }

        public string TheirMove(string userName, GameRecord gr)
        {
            if (gr.State == "wait")
            {
                return "You do not have an opponent yet.";
            }

            else if (gr.Player1 == userName)
            {
                string lastMove = gr.LastMovePlayer2;
                if (lastMove == null)
                {
                    return "Your opponent has not moved yet.";
                }
                return lastMove;
            }

            else if (gr.Player2 == userName)
            {
                string lastMove = gr.LastMovePlayer1;
                if (lastMove == null)
                {
                    return "Your opponent has not moved yet.";
                }
                return lastMove;
            }
            return "not your game id";
        }

        public string MyMove(string userName, string move, GameRecord gr)
        {
            if (gr.State == "wait")
            {
                return "You do not have an opponent yet.";
            }

            else if (gr.Player1 == userName)
            {
                if (gr.LastMovePlayer1 == null) 
                {
                    gr.LastMovePlayer1 = move;
                    gr.LastMovePlayer2 = null;
                    _dbContext.SaveChanges();
                    return "move registered";
                }
                return "It is not your turn.";
            }

            else if (gr.Player2 == userName)
            {
                if (gr.LastMovePlayer2 == null)
                {
                    gr.LastMovePlayer2 = move;
                    gr.LastMovePlayer1 = null;
                    _dbContext.SaveChanges();
                    return "move registered";
                }
                return "It is not your turn.";
            }
            return "not your game id";
        }

        public string QuitGame(string userName, GameRecord gr)
        {
            if (gr.Player1 == userName || gr.Player2 == userName)
            {
                _dbContext.GameRecords.Remove(gr);
                _dbContext.SaveChanges();
                return "game over";
            }
            else if (gr.Player1 != userName && gr.Player2 != userName)
            {
                return "not your game id";
            }
            return "You have not started a game.";
        }
    }
}