namespace JelleSmart.ExamSystem.Core.Helpers
{
    public static class ChoiceHelper
    {
        /// <summary>
        /// Generates a label (A, B, C, ...) for a choice based on its index.
        /// </summary>
        public static string GenerateLabel(int index)
        {
            if (index < 0 || index >= 26)
                throw new ArgumentOutOfRangeException(nameof(index), "Index must be between 0 and 25");
            return ((char)('A' + index)).ToString();
        }
    }
}
