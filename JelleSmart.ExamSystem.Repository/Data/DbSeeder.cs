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
            await SeedHierarchicalDataAsync(context, userManager);
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
                EmailConfirmed = true
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
        private static async Task SeedHierarchicalDataAsync(AppDbContext context, UserManager<AppUser> userManager)
        {
            // Check if data already exists
            if (await context.Subjects.AnyAsync())
            {
                Console.WriteLine("Hierarchical data already exists. Skipping seed.");
                return;
            }

            Console.WriteLine("Seeding hierarchical data...");

            // Get admin user for question creation
            var adminUser = await userManager.Users.FirstOrDefaultAsync(u => u.Email == "info@jellosmart.com");
            if (adminUser == null)
            {
                Console.WriteLine("Warning: Admin user not found. Skipping question creation.");
                return;
            }

            // Create Subject: Matematik
            var matematik = new Subject
            {
                Name = "Matematik",
                Description = "Matematik dersi",
                IconClass = "fas fa-calculator"
            };
            context.Subjects.Add(matematik);
            await context.SaveChangesAsync();

            // Create Grades
            var sinif1 = new Grade
            {
                Name = "1. Sınıf",
                Level = 1
            };
            var sinif2 = new Grade
            {
                Name = "2. Sınıf",
                Level = 2
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
                Description = "Doğal sayılar ve işlemler"
            };
            var unit2Sinif1 = new Unit
            {
                Name = "İşlemler",
                GradeId = sinif1.Id,
                Order = 2,
                Description = "Toplama ve çıkarma"
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
                Code = "M.1.1.1",
                Description = "1-100 arası sayılar"
            };
            var topic2 = new Topic
            {
                Name = "Tam Sayılar",
                UnitId = unit1Sinif1.Id,
                GradeId = sinif1.Id,
                Order = 2,
                Code = "M.1.1.2",
                Description = "Tam sayılar kavramı"
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
                Code = "M.1.2.1",
                Description = "Toplama işlemi"
            };
            context.Topics.Add(topic3);
            await context.SaveChangesAsync();

            // Create Units for 2. Sınıf
            var unit1Sinif2 = new Unit
            {
                Name = "Sayılar",
                GradeId = sinif2.Id,
                Order = 1,
                Description = "Doğal sayılar ve işlemler"
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
                Code = "M.2.1.1",
                Description = "1-1000 arası sayılar"
            };
            context.Topics.Add(topic4);
            await context.SaveChangesAsync();

            // Create sample Questions with Choices
            var question1 = new Question
            {
                Text = "5 + 3 işleminin sonucu kaçtır?",
                TopicId = topic3.Id,
                Difficulty = 1,
                Explanation = "5 + 3 = 8",
                CreatedByUserId = adminUser.Id
            };

            // Choices for question 1 - create as list then assign
            var q1Choices = new List<Choice>
            {
                new Choice { Label = "A", Text = "6", IsCorrect = false },
                new Choice { Label = "B", Text = "7", IsCorrect = false },
                new Choice { Label = "C", Text = "8", IsCorrect = true },
                new Choice { Label = "D", Text = "9", IsCorrect = false }
            };
            foreach (var choice in q1Choices)
            {
                context.Choices.Add(choice);
            }

            var question2 = new Question
            {
                Text = "10 - 4 işleminin sonucu kaçtır?",
                TopicId = topic3.Id,
                Difficulty = 1,
                Explanation = "10 - 4 = 6",
                CreatedByUserId = adminUser.Id
            };

            // Choices for question 2 - create as list then assign
            var q2Choices = new List<Choice>
            {
                new Choice { Label = "A", Text = "5", IsCorrect = false },
                new Choice { Label = "B", Text = "6", IsCorrect = true },
                new Choice { Label = "C", Text = "7", IsCorrect = false },
                new Choice { Label = "D", Text = "8", IsCorrect = false }
            };
            foreach (var choice in q2Choices)
            {
                context.Choices.Add(choice);
            }

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
