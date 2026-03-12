using JelleSmart.ExamSystem.Core.Entities;
using JelleSmart.ExamSystem.WebUI.Models;

namespace JelleSmart.ExamSystem.WebUI.Models
{
    public static class MappingExtensions
    {
        // Subject mappings
        public static SubjectViewModel ToViewModel(this Subject entity)
        {
            return new SubjectViewModel
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                IconClass = entity.IconClass
            };
        }

        public static Subject ToEntity(this SubjectViewModel viewModel)
        {
            return new Subject
            {
                Id = viewModel.Id,
                Name = viewModel.Name,
                Description = viewModel.Description,
                IconClass = viewModel.IconClass
            };
        }

        public static void UpdateEntity(this SubjectViewModel viewModel, Subject entity)
        {
            entity.Name = viewModel.Name;
            entity.Description = viewModel.Description;
            entity.IconClass = viewModel.IconClass;
        }

        // Grade mappings
        public static GradeViewModel ToViewModel(this Grade entity)
        {
            return new GradeViewModel
            {
                Id = entity.Id,
                Name = entity.Name,
                Level = entity.Level
            };
        }

        public static Grade ToEntity(this GradeViewModel viewModel)
        {
            return new Grade
            {
                Id = viewModel.Id,
                Name = viewModel.Name,
                Level = viewModel.Level
            };
        }

        public static void UpdateEntity(this GradeViewModel viewModel, Grade entity)
        {
            entity.Name = viewModel.Name;
            entity.Level = viewModel.Level;
        }

        // Unit mappings
        public static UnitViewModel ToViewModel(this Unit entity)
        {
            return new UnitViewModel
            {
                Id = entity.Id,
                Name = entity.Name,
                SubjectId = entity.SubjectId,
                GradeId = entity.GradeId,
                Description = entity.Description,
                SubjectName = entity.Subject?.Name,
                GradeName = entity.Grade?.Name
            };
        }

        public static Unit ToEntity(this UnitViewModel viewModel)
        {
            return new Unit
            {
                Id = viewModel.Id,
                Name = viewModel.Name,
                SubjectId = viewModel.SubjectId,
                GradeId = viewModel.GradeId,
                Description = viewModel.Description
            };
        }

        public static void UpdateEntity(this UnitViewModel viewModel, Unit entity)
        {
            entity.Name = viewModel.Name;
            entity.SubjectId = viewModel.SubjectId;
            entity.GradeId = viewModel.GradeId;
            entity.Description = viewModel.Description;
        }

        // Topic mappings
        public static TopicViewModel ToViewModel(this Topic entity)
        {
            return new TopicViewModel
            {
                Id = entity.Id,
                Name = entity.Name,
                UnitId = entity.UnitId,
                Code = entity.Code,
                Description = entity.Description,
                UnitName = entity.Unit?.Name,
                SubjectName = entity.Unit?.Subject?.Name
            };
        }

        public static Topic ToEntity(this TopicViewModel viewModel)
        {
            return new Topic
            {
                Id = viewModel.Id,
                Name = viewModel.Name,
                UnitId = viewModel.UnitId,
                Code = viewModel.Code,
                Description = viewModel.Description
            };
        }

        public static void UpdateEntity(this TopicViewModel viewModel, Topic entity)
        {
            entity.Name = viewModel.Name;
            entity.UnitId = viewModel.UnitId;
            entity.Code = viewModel.Code;
            entity.Description = viewModel.Description;
        }
    }
}
