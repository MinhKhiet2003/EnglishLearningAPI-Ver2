using System.Collections.Generic;
using EnglishLearningAPI.Models;

namespace EnglishLearningAPI.Response
{
    public class ExamResponse
    {
        public int Type { get; set; }
        public Vocabulary Correct { get; set; }
        public List<Vocabulary> Incorrect { get; set; }

        // Constructor không tham số
        public ExamResponse()
        {
        }

        // Constructor có tham số
        public ExamResponse(int type, Vocabulary correct, List<Vocabulary> incorrect)
        {
            Type = type;
            Correct = correct;
            Incorrect = incorrect;
        }
    }
}
