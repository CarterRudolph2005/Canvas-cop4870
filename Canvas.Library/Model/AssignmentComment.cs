using Canvas.Library.Model;

public class AssignmentComment
{
    public int Id { get; set; }
    public int SubmissionId { get; set; }
    // [JsonIgnore]
    public Submission? Submission { get; set; }
    public int AuthorId { get; set; }
    public string AuthorName { get; set; }
    public string Body { get; set; }
    public DateTime CreatedAt { get; set; }
}