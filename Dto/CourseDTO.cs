namespace EnglishLearningAPI.DTO
{
    public class CourseDTO
    {
        public string CourseName { get; set; }
        public string Description { get; set; }
        public string CourseTarget { get; set; }

        // Constructor không tham số
        public CourseDTO() { }

        // Constructor với tham số
        public CourseDTO(string courseName, string description, string courseTarget)
        {
            CourseName = courseName;
            Description = description;
            CourseTarget = courseTarget;
        }
    }
}
