using System;
using System.Text;

namespace DotA2.Gambling.Model
{
    public class Gamble
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime BetTime { get; set; }
        public float Odds { get; set; }
        public Prediction Result { get; set; }
        public bool IsOpen { get; set; }
        public string MatchId { get; set; }
        public bool IsGameFinished { get; set; }
        public bool IsArchived { get; set; }
    }
}
