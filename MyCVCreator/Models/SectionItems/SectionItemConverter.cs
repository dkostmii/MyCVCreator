using System.Text.Json;
using System.Text.Json.Serialization;

using static MyCVCreator.Models.SectionItems.TypeDiscriminatorAttribute;

namespace MyCVCreator.Models.SectionItems
{
    public class SectionItemConverter : JsonConverter<SectionItem>
    {
        public override bool CanConvert(Type typeToConvert) =>
            typeof(SectionItem).IsAssignableFrom(typeToConvert);


        private IEnumerable<ListItem>? ReadItems(ref Utf8JsonReader reader)
        {
            bool insideArray;
            bool insideObject = false;

            var items = new List<ListItem>();

            if (reader.TokenType != JsonTokenType.StartArray)
                throw new JsonException("Expected StartArray.");
            else
                insideArray = true;

            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonTokenType.StartArray:
                        if (insideArray)
                            throw new JsonException("Nested arrays while expected single array.");
                        break;

                    case JsonTokenType.StartObject:
                        if (!insideObject)
                            insideObject = true;
                        else
                            throw new JsonException("Nested objects while expected single object.");
                        break;

                    case JsonTokenType.PropertyName:
                        if (!(insideArray && insideObject))
                            throw new JsonException("Expected to be inside array and object.");

                        if (reader.ValueTextEquals("text"))
                        {
                            reader.Read();
                            var text = reader.GetString();
                            var li = new ListItem
                            {
                                Text = text!
                            };
                            items.Add(li);
                        }
                        break;

                    case JsonTokenType.EndObject:
                        if (insideObject)
                            insideObject = false;
                        else
                            throw new JsonException("Expected to be outside a object already.");
                        break;

                    case JsonTokenType.EndArray:
                        if (insideArray)
                            return items;
                        break;
                }
            }

            if (insideArray)
            {
                throw new JsonException("Array is not closed.");
            }

            throw new JsonException("Unable to deserialize JSON array.");
        }

        public override SectionItem? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var insideObject = false;

            string? type = null;
            string? label = null;
            string? text = null;

            IEnumerable<ListItem>? items = null;

            if (reader.TokenType != JsonTokenType.StartObject)
                throw new JsonException("Expected StartObject.");
            else
                insideObject = true;


            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonTokenType.StartObject:
                        if (insideObject)
                            throw new JsonException("Nested objects while expected single object.");
                        break;

                    case JsonTokenType.PropertyName:
                        if (!insideObject)
                            throw new JsonException("Expected to be inside object.");

                        if (reader.ValueTextEquals("type"))
                        {
                            reader.Read();
                            type = reader.GetString();
                        }
                        else if (reader.ValueTextEquals("label"))
                        {
                            reader.Read();
                            label = reader.GetString();
                        }
                        else if (reader.ValueTextEquals("text"))
                        {
                            reader.Read();
                            text = reader.GetString();
                        }
                        else if (reader.ValueTextEquals("list"))
                        {
                            reader.Read();
                            items = ReadItems(ref reader);
                        }

                        break;

                    case JsonTokenType.EndObject:
                        if (insideObject)
                            return AssembleSectionItem(type!, label!, text!, items!);
                        break;
                }
            }

            if (insideObject)
            {
                throw new JsonException("Object is not closed.");
            }

            throw new JsonException("Unable to deserialize JSON object.");
        }

        private SectionItem AssembleSectionItem(string type, string label, string text, IEnumerable<ListItem> list)
        {
            type = type.ToLower();

            var bulletTd = GetTypeDiscriminator(typeof(BulletSectionItem));
            var labeledTd = GetTypeDiscriminator(typeof(LabeledSectionItem));
            var textTd = GetTypeDiscriminator(typeof(TextSectionItem));

            if (string.IsNullOrEmpty(bulletTd) || string.IsNullOrEmpty(labeledTd) || string.IsNullOrEmpty(textTd))
                throw new Exception("Some of type discriminators are undefined.");

            if (type.Equals(textTd))
            {
                return new TextSectionItem
                {
                    Type = type,
                    Text = text
                };
            }
            else if (type.Equals(bulletTd))
            {
                return new BulletSectionItem
                {
                    Type = type,
                    List = list
                };
            }
            else if (type.Equals(labeledTd))
            {
                return new LabeledSectionItem
                {
                    Type = type,
                    Label = label,
                    Text = text
                };
            }
            else
            {
                throw new JsonException("Invalid sectionItem type: " + type);
            }
        }

        public override void Write(Utf8JsonWriter writer, SectionItem value, JsonSerializerOptions options)
        {
            var bulletTd = GetTypeDiscriminator(typeof(BulletSectionItem));
            var labeledTd = GetTypeDiscriminator(typeof(LabeledSectionItem));
            var textTd = GetTypeDiscriminator(typeof(TextSectionItem));

            if (string.IsNullOrEmpty(bulletTd) || string.IsNullOrEmpty(labeledTd) || string.IsNullOrEmpty(textTd))
                throw new Exception("Some of type discriminators are undefined.");

            writer.WriteStartObject();
            writer.WritePropertyName("type");
            writer.WriteStringValue(value.Type);

            if (value.Type.Equals(textTd))
            {
                var textItem = (value as TextSectionItem)!;
                writer.WritePropertyName("text");
                writer.WriteStringValue(textItem.Text);
            }
            else if (value.Type.Equals(bulletTd))
            {
                var bulletItem = (value as BulletSectionItem)!;

                if (bulletItem.List is not null)
                {
                    writer.WritePropertyName("list");
                    writer.WriteStartArray();
                    foreach (var li in bulletItem.List)
                    {
                        writer.WriteStartObject();
                        writer.WritePropertyName("text");
                        writer.WriteStringValue(li.Text);
                        writer.WriteEndObject();
                    }
                    writer.WriteEndArray();
                }
                else
                {
                    writer.WriteNull("list");
                }
            }
            else if (value.Type.Equals(labeledTd))
            {
                var labeledItem = (value as LabeledSectionItem)!;
                writer.WritePropertyName("label");
                writer.WriteStringValue(labeledItem.Label);
                writer.WritePropertyName("text");
                writer.WriteStringValue(labeledItem.Text);
            }
            else
            {
                throw new JsonException("Invalid sectionItem type: " + value.Type);
            }

            writer.WriteEndObject();
        }
    }
}
