namespace MyCVCreator.Models
{
    public class Container
    {
        public string Title { get; set; } = null!;
        public IEnumerable<Section> Sections { get; set; } = null!;
    }
}
