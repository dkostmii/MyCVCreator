using MyCVCreator.Models.SectionItems;
using System.Net;

using static MyCVCreator.Models.SectionItems.TypeDiscriminatorAttribute;

namespace MyCVCreator.Models
{
    public class ModelSanitizer<T> where T : class
    {
        public T Sanitized { get; }

        private static string EscapeValue(string value)
        {
            return WebUtility.HtmlEncode(value);
        }

        public ModelSanitizer(T model)
        {
            if (typeof(T).IsAssignableTo(typeof(CV)))
            {
                var bulletTd = GetTypeDiscriminator(typeof(BulletSectionItem));
                var labeledTd = GetTypeDiscriminator(typeof(LabeledSectionItem));
                var textTd = GetTypeDiscriminator(typeof(TextSectionItem));

                if (string.IsNullOrEmpty(bulletTd) || string.IsNullOrEmpty(labeledTd) || string.IsNullOrEmpty(textTd))
                    throw new Exception("Some of type discriminators are undefined.");

                CV cv = (model as CV)!;

                var sanitizedCV = new CV
                {
                    FirstName = EscapeValue(cv.FirstName),
                    LastName = EscapeValue(cv.LastName),
                    Position = EscapeValue(cv.Position),
                    Containers = cv.Containers.Select(c =>
                    {
                        return new Container()
                        {
                            Title = EscapeValue(c.Title),
                            Sections = c.Sections.Select(s =>
                            {
                                return new Section()
                                {
                                    Title = EscapeValue(s.Title),
                                    Items = s.Items.Select<SectionItem, SectionItem>(si =>
                                    {
                                        var siType = EscapeValue(si.Type);

                                        if (siType.Equals(bulletTd))
                                        {
                                            var bSi = (si as BulletSectionItem)!;

                                            return new BulletSectionItem()
                                            {
                                                Type = siType,
                                                List = bSi.List.Select(li =>
                                                {
                                                    return new ListItem
                                                    {
                                                        Text = EscapeValue(li.Text)
                                                    };
                                                }),
                                            };
                                        }
                                        else if (siType.Equals(labeledTd))
                                        {
                                            var lSi = (si as LabeledSectionItem)!;

                                            return new LabeledSectionItem
                                            {
                                                Label = EscapeValue(lSi.Label),
                                                Text = EscapeValue(lSi.Text),
                                                Type = siType
                                            };
                                        }
                                        else if (siType.Equals(textTd))
                                        {
                                            var tSi = (si as TextSectionItem)!;

                                            return new TextSectionItem
                                            {
                                                Text = EscapeValue(tSi.Text),
                                                Type = siType
                                            };
                                        }

                                        throw new Exception("Invalid sectionItem type: " + si.Type);
                                    }),
                                };
                            }),
                        };
                    }),
                };

                Sanitized = (sanitizedCV as T)!;
            }
            else
            {
                throw new TypeInitializationException(nameof(ModelSanitizer<T>),
                    new Exception("Cannot sanitize model of unknown type."));
            }
        }
    }
}
