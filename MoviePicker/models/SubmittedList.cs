namespace MoviePicker.Models;

public class MovieEntry
{
    public string Title { get; set; } = "";
    public string ImgUrl { get; set; } = ""; //Default to empty

    public int Rank { get; set; }   //Numerical ranking each user can set
}

public class SubmittedList
{
    public string ID { get; set; } = Guid.NewGuid().ToString(); //Ensures each list has a unique id
    public string UsersName { get; set; } = "";
    public List<MovieEntry> MovieList { get; set; } = new List<MovieEntry>();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}