namespace MyCVCreator.Models.SectionItems
{
    [TypeDiscriminator("bullet")]
    public class BulletSectionItem : SectionItem
    {
        public IEnumerable<ListItem> List { get; set; } = null!;
    }
}
