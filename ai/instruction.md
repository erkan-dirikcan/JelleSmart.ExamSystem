You are working on a real production-style ASP.NET MVC project.

PROJECT TYPE
- ASP.NET MVC
- Repository Design Pattern
- Multi-project solution

SOLUTION STRUCTURE
- JelleSmart.ExamSystem.Core
- JelleSmart.ExamSystem.Repository
- JelleSmart.ExamSystem.Service
- JelleSmart.ExamSystem.WebUI

ARCHITECTURAL RULES
- Follow the existing repository pattern strictly.
- Do not invent a new architecture.
- Do not introduce Areas under any circumstance.
- Areas are explicitly forbidden in this project.
- Do not re-add Areas later for convenience.
- Keep responsibilities separated according to the current project structure.
- Core contains DTOs, Entities, Enums, Interfaces.
- Repository contains DbContext, Migrations, Repositories.
- Service contains business services.
- WebUI contains Controllers, Models, Views, ViewComponents, wwwroot and UI concerns.

IDENTITY / AUTHORIZATION RULES
- The system uses role-based user management.
- Roles include Admin, Teacher, Student.
- Do not replace or blur role-based authorization with another access model.
- Admin has full system authority and can act like Teacher or Student where needed.
- Teacher manages educational content and exam assignment.
- Student solves assigned exams.
- Keep these responsibilities consistent.

UI / THEME RULES
- The UI theme is Metronic 7.
- Always follow existing Metronic 7 structure and patterns.
- Do not redesign pages arbitrarily.
- Do not modify _Layout, shared partials, or global UI structure unless explicitly requested.
- Prefer adapting existing Metronic page patterns instead of inventing new HTML structure.

JAVASCRIPT RULES
- Never write page-specific JavaScript inside cshtml files.
- Never place script logic directly in Razor views.
- Every page must have its own JavaScript file under wwwroot/js/custom, even if initially empty.
- If a page needs JavaScript, add or update its dedicated JS file only.
- Follow the existing JS module/init style used in the project.

CHANGE MANAGEMENT RULES
- Minimize the scope of changes.
- Do not refactor unrelated files.
- Do not “clean up” unrelated code.
- Do not change other working pages while implementing a feature.
- Before editing, identify exactly which files must change.
- If a change may affect shared code, state the impact first.

COMPLETION RULES
A task is NOT complete unless:
- The requested feature is fully implemented end-to-end within the requested scope.
- Required controller, service, repository, DTO/viewmodel, and view updates are all handled.
- Namespaces, using directives, nullability, method signatures, and dependency wiring are consistent.
- No page-specific JavaScript is left inside cshtml.
- The solution is left in a buildable and coherent state.
- You explicitly list what was changed.

WORKFLOW RULES
For every task, follow this order:
1. Analyze the request
2. Identify affected files
3. Check architecture constraints
4. Make a short implementation plan
5. Implement only the necessary changes
6. Review for broken dependencies, namespaces, role rules, and JS placement
7. Summarize completed work and remaining risks

RESPONSE RULES
- Be strict about existing architecture.
- Do not take shortcuts.
- Do not produce temporary or hacky solutions.
- Do not silently ignore missing pieces.
- If information is missing, ask targeted questions instead of assuming.
- If the request conflicts with project rules, follow project rules.