using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewComponents;

namespace JelleSmart.ExamSystem.WebUI.ViewComponents
{
    public static class ManageNavPages
    {
        // Admin Pages
        public static string Dashboard => "Dashboard";
        public static string Users => "UserManagement";
        public static string Subjects => "Subject";
        public static string Grades => "Grade";
        public static string Units => "Unit";
        public static string Topics => "Topic";

        // Teacher Pages
        public static string Questions => "Teacher.Question";
        public static string Exams => "Teacher.Exam";
        public static string TeacherReports => "Teacher.Report";

        // Student Pages
        public static string StudentExams => "Student.Exam";
        public static string StudentReports => "Student.Report";

        // Auth Pages
        public static string Login => "Account.Login";
        public static string Register => "Account.Register";

        // Navigation Class Methods
        public static string DashboardNavClass(ViewContext viewContext) => PageMainNavClass(viewContext, Dashboard);
        public static string UsersNavClass(ViewContext viewContext) => PageMainNavClass(viewContext, Users);
        public static string SubjectsNavClass(ViewContext viewContext) => PageMainNavClass(viewContext, Subjects);
        public static string GradesNavClass(ViewContext viewContext) => PageMainNavClass(viewContext, Grades);
        public static string UnitsNavClass(ViewContext viewContext) => PageMainNavClass(viewContext, Units);
        public static string TopicsNavClass(ViewContext viewContext) => PageMainNavClass(viewContext, Topics);
        public static string QuestionsNavClass(ViewContext viewContext) => PageMainNavClass(viewContext, Questions);
        public static string ExamsNavClass(ViewContext viewContext) => PageMainNavClass(viewContext, Exams);
        public static string TeacherReportsNavClass(ViewContext viewContext) => PageMainNavClass(viewContext, TeacherReports);
        public static string StudentExamsNavClass(ViewContext viewContext) => PageMainNavClass(viewContext, StudentExams);
        public static string StudentReportsNavClass(ViewContext viewContext) => PageMainNavClass(viewContext, StudentReports);
        public static string LoginNavClass(ViewContext viewContext) => PageMainNavClass(viewContext, Login);
        public static string RegisterNavClass(ViewContext viewContext) => PageMainNavClass(viewContext, Register);

        // Parent Menus
        private static string Teacher => "Teacher";
        private static string Student => "Student";

        public static string TeacherNavClass(ViewContext viewContext) => PageMainToggleNavClass(viewContext, Teacher);
        public static string StudentNavClass(ViewContext viewContext) => PageMainToggleNavClass(viewContext, Student);

        public static string PageMainNavClass(ViewContext viewContext, string pageName)
        {
            var activePage = viewContext.ViewData["ActivePage"] as string;
            return activePage == pageName ? "menu-item active" : "menu-item";
        }

        public static string PageMainToggleNavClass(ViewContext viewContext, string menuName)
        {
            var activeMenu = viewContext.ViewData["MenuToggle"] as string;
            return activeMenu == menuName ? "menu-item active" : "menu-item";
        }

        public static string PageAsideNavClass(ViewContext viewContext, string pageName)
        {
            var activePage = viewContext.ViewData["ActivePage"] as string;
            return activePage == pageName ? "menu-item active" : "menu-item";
        }
    }
}
