using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace A2.Models
{
    public class GameMove
    {
        public string? Move { get; set; }
        public string? GameId { get; set; }
    }
}