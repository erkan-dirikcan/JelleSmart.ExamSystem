namespace JelleSmart.ExamSystem.Core.Entities
{
    public class TeacherSubject
    {
        public int TeacherProfileId { get; set; }
        public TeacherProfile TeacherProfile { get; set; } = null!;

        public int SubjectId { get; set; }
        public Subject Subject { get; set; } = null!;
    }
}
