using System;
using System.Collections.Generic;

namespace Bot.Model
{
    public class UserMessageInfo
    {
        public UserMessageInfo()
        {
            Words = new Dictionary<string, int>(StringComparer.CurrentCultureIgnoreCase);
        }

        public int MessageCount { get; set; }
        public Dictionary<string,int> Words { get; set; }
    }
}