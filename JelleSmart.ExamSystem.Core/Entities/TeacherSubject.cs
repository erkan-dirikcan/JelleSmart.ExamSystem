namespace JelleSmart.ExamSystem.Core.Entities
{
    public class TeacherSubject
    {
        public string? TeacherProfileId { get; set; }
        public TeacherProfile? TeacherProfile { get; set; } = null!;

        public string? SubjectId { get; set; }
        public Subject? Subject { get; set; } = null!;
    }
}
