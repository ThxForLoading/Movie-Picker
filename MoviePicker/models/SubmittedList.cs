namespace MoviePicker.Models;

public class MovieEntry
{
    public string title { get; set; } = "";
    public string posterUrl { get; set; } = ""; //Default to empty

    public int rank { get; set; }   //Numerical ranking each user can set
}

public class SubmittedList
{
    public Guid ID { get; set; } = Guid.NewGuid(); //Ensures each list has a unique id
    public string username { get; set; } = "";
    public List<MovieEntry> movies { get; set; } = new List<MovieEntry>();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}