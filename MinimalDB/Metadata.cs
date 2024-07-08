namespace MinimalDB;
public class Metadata
{
    public required HashSet<int> UsedIds { get; set; }

    public int MaxId { get; set; }
}