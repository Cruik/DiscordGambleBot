using System;

namespace DotA2.Gambling.Model
{
    public class Season
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsEvaluated { get; set; }
        public int  WinnerAccountId { get; set; }
    }
}