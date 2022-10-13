namespace MyCVCreator.Models
{
    public class CV
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Position { get; set; } = null!;
        public IEnumerable<Container> Containers { get; set; } = null!;
    }
}
