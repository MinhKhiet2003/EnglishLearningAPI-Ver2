namespace EnglishLearningAPI.DTO
{
    public class VocabReview
    {
        public int Id { get; set; }
        public int Status { get; set; }

        // Constructor không tham số
        public VocabReview() { }

        // Constructor có tham số
        public VocabReview(int id, int status)
        {
            Id = id;
            Status = status;
        }
    }
}
