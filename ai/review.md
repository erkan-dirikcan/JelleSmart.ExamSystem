Now review your own work strictly.

Check for:
1. Did you accidentally create or reintroduce Areas?
2. Did you modify _Layout, shared partials, or global UI unnecessarily?
3. Did you place any page-specific JavaScript inside cshtml?
4. Did you break role-based identity logic?
5. Did you change unrelated files?
6. Are controller, service, repository, DTO/viewmodel and view layers consistent?
7. Are namespaces/usings/dependencies complete?
8. Could any existing page have been broken by this change?

Return:
- issues found
- fixes needed
- final changed file list