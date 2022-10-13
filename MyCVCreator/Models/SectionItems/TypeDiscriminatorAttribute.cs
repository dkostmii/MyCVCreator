namespace MyCVCreator.Models.SectionItems
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TypeDiscriminatorAttribute : Attribute
    {
        private string Value { get; }

        public TypeDiscriminatorAttribute(string typeDiscriminator)
        {
            Value = typeDiscriminator;
        }

        public static string? GetTypeDiscriminator(Type t)
        {
            var attr = GetCustomAttribute(t, typeof(TypeDiscriminatorAttribute));

            if (attr is not null)
            {
                var tdAttr = (TypeDiscriminatorAttribute)attr;
                return tdAttr.Value;
            }

            return null;
        }
    }
}
