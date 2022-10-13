using MyCVCreator.Models.SectionItems;

namespace MyCVCreator.Models
{
    public class Section
    {
        public string Title { get; set; } = null!;
        public IEnumerable<SectionItem> Items { get; set; } = null!;
    }
}
