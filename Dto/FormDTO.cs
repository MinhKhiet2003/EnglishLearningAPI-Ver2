namespace EnglishLearningAPI.DTO
{
    public class FormDTO
    {
        public int FormType { get; set; }
        public string Content { get; set; }
        public int Status { get; set; }

        // Constructor không tham số
        public FormDTO() { }

        // Constructor với tham số
        public FormDTO(int formType, string content, int status)
        {
            FormType = formType;
            Content = content;
            Status = status;
        }
    }
}
