namespace MyCVCreator.Models.SectionItems
{
    [TypeDiscriminator("labeled")]
    public class LabeledSectionItem : SectionItem
    {
        public string Label { get; set; } = null!;
        public string Text { get; set; } = null!;
    }
}
