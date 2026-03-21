using JelleSmart.ExamSystem.Core.Entities;
using JelleSmart.ExamSystem.Core.Entities.Identity;
using JelleSmart.ExamSystem.Core.Enums;
using JelleSmart.ExamSystem.Repository.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace JelleSmart.ExamSystem.Repository.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(AppDbContext context, UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            await SeedRolesAsync(roleManager);
            await SeedAdminUserAsync(userManager, roleManager);
            await SeedHierarchicalDataAsync(context);
        }

        private static async Task SeedRolesAsync(RoleManager<AppRole> roleManager)
        {
            string[] roles = { UserRoles.Admin, UserRoles.Teacher, UserRoles.Student };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    var appRole = new AppRole
                    {
                        Name = role,
                        Description = role switch
                        {
                            UserRoles.Admin => "Tam yetkili yönetici",
                            UserRoles.Teacher => "Ders ve sınav sorumlusu",
                            UserRoles.Student => "Sınav çözen öğrenci",
                            _ => ""
                        }
                    };

                    var result = await roleManager.CreateAsync(appRole);

                    if (!result.Succeeded)
                    {
                        throw new Exception($"Failed to create role {role}: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    }
                }
            }
        }

        private static async Task SeedAdminUserAsync(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            // Check if any user with Admin role exists
            var adminUsers = await userManager.GetUsersInRoleAsync(UserRoles.Admin);

            if (adminUsers.Any())
            {
                // At least one admin exists, no need to create bootstrap admin
                return;
            }

            // No admin exists, create bootstrap admin
            const string adminEmail = "info@jellosmart.com";
            const string adminPassword = "Orko123!";

            var adminUser = new AppUser
            {
                FirstName = "Jello",
                LastName = "Smart",
                Email = adminEmail,
                UserName = adminEmail,
                EmailConfirmed = true,
                IsActive = true
            };

            var result = await userManager.CreateAsync(adminUser, adminPassword);

            if (!result.Succeeded)
            {
                throw new Exception($"Failed to create bootstrap admin user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }

            // Ensure Admin role exists before assigning
            if (!await roleManager.RoleExistsAsync(UserRoles.Admin))
            {
                await roleManager.CreateAsync(new AppRole
                {
                    Name = UserRoles.Admin,
                    Description = "Tam yetkili yönetici"
                });
            }

            await userManager.AddToRoleAsync(adminUser, UserRoles.Admin);
        }

        /// <summary>
        /// Seeds sample hierarchical data for testing purposes
        /// </summary>
        private static async Task SeedHierarchicalDataAsync(AppDbContext context)
        {
            // Check if data already exists
            if (await context.Subjects.AnyAsync())
            {
                Console.WriteLine("Hierarchical data already exists. Skipping seed.");
                return;
            }

            Console.WriteLine("Seeding hierarchical data...");

            // Create Subject: Matematik
            var matematik = new Subject
            {
                Name = "Matematik",
                Code = "MAT",
                Description = "Matematik dersi",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            context.Subjects.Add(matematik);
            await context.SaveChangesAsync();

            // Create Grades
            var sinif1 = new Grade
            {
                Name = "1. Sınıf",
                Order = 1,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            var sinif2 = new Grade
            {
                Name = "2. Sınıf",
                Order = 2,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            context.Grades.AddRange(sinif1, sinif2);
            await context.SaveChangesAsync();

            // Create SubjectGrade associations
            var subjectGrade1 = new SubjectGrade
            {
                SubjectId = matematik.Id,
                GradeId = sinif1.Id
            };
            var subjectGrade2 = new SubjectGrade
            {
                SubjectId = matematik.Id,
                GradeId = sinif2.Id
            };
            context.SubjectGrades.AddRange(subjectGrade1, subjectGrade2);
            await context.SaveChangesAsync();

            // Create Units for 1. Sınıf
            var unit1Sinif1 = new Unit
            {
                Name = "Sayılar",
                GradeId = sinif1.Id,
                Order = 1,
                Description = "Doğal sayılar ve işlemler",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            var unit2Sinif1 = new Unit
            {
                Name = "İşlemler",
                GradeId = sinif1.Id,
                Order = 2,
                Description = "Toplama ve çıkarma",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            context.Units.AddRange(unit1Sinif1, unit2Sinif1);
            await context.SaveChangesAsync();

            // Create Topics for Unit 1 (1. Sınıf)
            var topic1 = new Topic
            {
                Name = "Doğal Sayılar",
                UnitId = unit1Sinif1.Id,
                GradeId = sinif1.Id,
                Order = 1,
                Description = "1-100 arası sayılar",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            var topic2 = new Topic
            {
                Name = "Tam Sayılar",
                UnitId = unit1Sinif1.Id,
                GradeId = sinif1.Id,
                Order = 2,
                Description = "Tam sayılar kavramı",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            context.Topics.AddRange(topic1, topic2);
            await context.SaveChangesAsync();

            // Create Topics for Unit 2 (1. Sınıf)
            var topic3 = new Topic
            {
                Name = "Toplama",
                UnitId = unit2Sinif1.Id,
                GradeId = sinif1.Id,
                Order = 1,
                Description = "Toplama işlemi",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            context.Topics.Add(topic3);
            await context.SaveChangesAsync();

            // Create Units for 2. Sınıf
            var unit1Sinif2 = new Unit
            {
                Name = "Sayılar",
                GradeId = sinif2.Id,
                Order = 1,
                Description = "Doğal sayılar ve işlemler",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            context.Units.Add(unit1Sinif2);
            await context.SaveChangesAsync();

            // Create Topics for Unit 1 (2. Sınıf)
            var topic4 = new Topic
            {
                Name = "Doğal Sayılar",
                UnitId = unit1Sinif2.Id,
                GradeId = sinif2.Id,
                Order = 1,
                Description = "1-1000 arası sayılar",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            context.Topics.Add(topic4);
            await context.SaveChangesAsync();

            // Create sample Questions
            var question1 = new Question
            {
                QuestionText = "5 + 3 işleminin sonucu kaçtır?",
                OptionA = "6",
                OptionB = "7",
                OptionC = "8",
                OptionD = "9",
                CorrectAnswer = "C",
                TopicId = topic3.Id,
                Difficulty = "Easy",
                Points = 10,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            var question2 = new Question
            {
                QuestionText = "10 - 4 işleminin sonucu kaçtır?",
                OptionA = "5",
                OptionB = "6",
                OptionC = "7",
                OptionD = "8",
                CorrectAnswer = "B",
                TopicId = topic3.Id,
                Difficulty = "Easy",
                Points = 10,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            context.Questions.AddRange(question1, question2);
            await context.SaveChangesAsync();

            Console.WriteLine("Hierarchical data seeded successfully.");
            Console.WriteLine($"- Subjects: 1 (Matematik)");
            Console.WriteLine($"- Grades: 2 (1. Sınıf, 2. Sınıf)");
            Console.WriteLine($"- SubjectGrades: 2");
            Console.WriteLine($"- Units: 3");
            Console.WriteLine($"- Topics: 4");
            Console.WriteLine($"- Questions: 2");
        }
    }
}
