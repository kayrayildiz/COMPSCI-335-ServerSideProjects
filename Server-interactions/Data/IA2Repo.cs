using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using A2.Models;
using A2.Dtos;

namespace A2.Data
{
    public interface IA2Repo
    {
        bool ValidLogin(string userName, string password);
        string Register(User user);
        GameRecordOut PairMe(string userName);
        GameRecord GetGameRecord(string gameId);
        string TheirMove(string userName, GameRecord gr);
        string MyMove(string userName, string move, GameRecord gr);
        string QuitGame(string userName, GameRecord gr);
    }
}
