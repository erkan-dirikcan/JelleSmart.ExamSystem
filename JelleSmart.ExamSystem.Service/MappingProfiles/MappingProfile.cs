using AutoMapper;
using JelleSmart.ExamSystem.Core.Entities;
using JelleSmart.ExamSystem.Core.Entities.Identity;
using JelleSmart.ExamSystem.Core.ViewModels;
using JelleSmart.ExamSystem.Core.DTOs;
using JelleSmart.ExamSystem.Core.RequestModels;

namespace JelleSmart.ExamSystem.Service.MappingProfiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Grade mappings
            CreateMap<Grade, GradeViewModel>();
            CreateMap<GradeViewModel, Grade>();

            // Subject mappings
            CreateMap<Subject, SubjectViewModel>();
            CreateMap<SubjectViewModel, Subject>();

            // Unit mappings
            CreateMap<Unit, UnitViewModel>();
            CreateMap<UnitViewModel, Unit>();

            // Topic mappings
            CreateMap<Topic, TopicViewModel>();
            CreateMap<TopicViewModel, Topic>();

            // Question mappings
            CreateMap<Question, QuestionViewModel>();
            CreateMap<CreateQuestionRequestModel, Question>();
            CreateMap<UpdateQuestionRequestModel, Question>();

            // Choice mappings
            CreateMap<Choice, ChoiceViewModel>();
            CreateMap<CreateChoiceRequestModel, Choice>();
            CreateMap<ChoiceViewModel, Choice>();

            // Exam mappings
            CreateMap<Exam, ExamDto>();
            CreateMap<CreateExamRequestModel, Exam>();
            CreateMap<CreateExamViewModel, CreateExamRequestModel>();

            // ExamQuestion mappings
            CreateMap<ExamQuestion, QuestionInExamViewModel>();

            // StudentExam mappings
            CreateMap<StudentExam, TakeExamViewModel>();

            // User mappings
            CreateMap<AppUser, UserViewModel>();
            CreateMap<RegisterRequestModel, RegisterViewModel>();

            // TeacherProfile mappings
            CreateMap<TeacherProfile, TeacherProfileDto>();
            CreateMap<TeacherProfileDto, TeacherProfile>();
            CreateMap<CreateTeacherProfileDto, TeacherProfile>();

            // StudentProfile mappings
            CreateMap<StudentProfile, StudentProfileDto>();
            CreateMap<StudentProfileDto, StudentProfile>();

            // StudentParent mappings
            CreateMap<StudentParent, ParentDto>();

            // ExamResult mappings
            CreateMap<StudentExam, ExamResultDto>();
        }
    }
}
