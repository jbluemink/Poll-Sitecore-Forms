using System;
using System.Collections.Generic;

namespace RadioPollResult.Models
{
    public class RadioPollResultModel
    {
        public string Title { get; set; }
        public string Text { get; set; }
        public Dictionary<string, int> Result { get; set; } = new Dictionary<string, int>();

    }
}