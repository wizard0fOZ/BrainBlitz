using System;
using System.Collections.Generic;

namespace BrainBlitz
{
    [Serializable]
    public class QuestionData
    {
        public int DatabaseQuestionID { get; set; }
        public string QuestionText { get; set; }
        public Dictionary<string, string> Options { get; set; }
        public string CorrectAnswer { get; set; }
        public int Points { get; set; }

        public QuestionData()
        {
            DatabaseQuestionID = 0;
            Options = new Dictionary<string, string>();
            CorrectAnswer = "A";
            Points = 10;
        }
    }
}