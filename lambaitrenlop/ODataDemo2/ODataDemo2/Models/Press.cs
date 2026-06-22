namespace ODataDemo2.Models;

public class Press
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public Category Category { get; set; }
}
