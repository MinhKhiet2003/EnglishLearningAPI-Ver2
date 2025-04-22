using System.Collections.Generic;

namespace EnglishLearningAPI.DTO
{
    public class VocabSelected
    {
        public List<int> Id { get; set; }

        // Constructor không tham số
        public VocabSelected()
        {
            Id = new List<int>();
        }

        // Constructor có tham số
        public VocabSelected(List<int> id)
        {
            Id = id;
        }
    }
}
