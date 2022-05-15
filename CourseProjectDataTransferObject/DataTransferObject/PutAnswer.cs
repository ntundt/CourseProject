namespace CourseProjectDataTransferObject
{
    public class PutAnswer
    {
        public int? SelectedOption { get; set; }
        public int[]? SelectedOptions { get; set; }
        public string? Answer { get; set; } = "";
    }
}
