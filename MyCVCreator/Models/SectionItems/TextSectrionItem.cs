namespace MyCVCreator.Models.SectionItems
{
    [TypeDiscriminator("text")]
    public class TextSectionItem : SectionItem
    {
        public string Text { get; set; } = null!;
    }
}
