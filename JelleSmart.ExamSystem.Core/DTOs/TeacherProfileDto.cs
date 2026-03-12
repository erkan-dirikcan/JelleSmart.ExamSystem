namespace JelleSmart.ExamSystem.Core.DTOs
{
    public class TeacherProfileDto
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string? Title { get; set; }
        public string? Department { get; set; }
        public DateOnly? HireDate { get; set; }
        public List<SubjectDto> Subjects { get; set; } = new();
    }

    public class SubjectDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class CreateTeacherProfileDto
    {
        public List<int> SubjectIds { get; set; } = new();
        public string? Title { get; set; }
        public string? Department { get; set; }
    }

    public class UpdateTeacherProfileDto
    {
        public List<int>? SubjectIds { get; set; }
        public string? Title { get; set; }
        public string? Department { get; set; }
    }
}
