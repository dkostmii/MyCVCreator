using HtmlAgilityPack;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using MyCVCreator.Models;
using MyCVCreator.Models.SectionItems;

using System.Text;

using static MyCVCreator.Models.SectionItems.TypeDiscriminatorAttribute;

namespace MyCVCreator.Services
{
    public class DocumentService
    {
        private readonly string baseUrl;

        private string StyleUrl => $"{baseUrl}/document/style.css";
        private string ScriptUrl => $"{baseUrl}/document/script.js";

        public DocumentService(IServer server)
        {
            var addrFeature = server.Features.Get<IServerAddressesFeature>();

            if (addrFeature is null)
                throw new Exception("DocumentService failed to start.");

            baseUrl = addrFeature.Addresses.First();

            if (baseUrl[^1] == '/')
                baseUrl = baseUrl[..^1];
        }

        private HtmlDocument CreateDocument()
        {
            var meta = new List<HtmlNode>
            {
                HtmlNode.CreateNode("<meta charset='UTF-8'>"),
                HtmlNode.CreateNode("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">")
            };

            var styleNode = HtmlNode.CreateNode($"<link rel=\"stylesheet\" href=\"{StyleUrl}\"");

            var headElement = HtmlNode.CreateNode("<head></head>");

            headElement.AppendChild(styleNode);
            foreach (var metaTag in meta)
            {
                headElement.AppendChild(metaTag);
            }

            var bodyElement = HtmlNode.CreateNode("<body></body>");

            var doctypeNode = HtmlNode.CreateNode("<!DOCTYPE html>");
            var htmlElement = HtmlNode.CreateNode("<html lang=\"en\"></html>");

            htmlElement.AppendChild(headElement);
            htmlElement.AppendChild(bodyElement);

            var document = new HtmlDocument()
            {
                OptionUseIdAttribute = true,
            };

            document.DocumentNode.AppendChild(doctypeNode);
            document.DocumentNode.AppendChild(htmlElement);

            return document;
        }

        private HtmlNode CreateSectionItem(SectionItem si)
        {
            var bulletTd = GetTypeDiscriminator(typeof(BulletSectionItem));
            var labeledTd = GetTypeDiscriminator(typeof(LabeledSectionItem));
            var textTd = GetTypeDiscriminator(typeof(TextSectionItem));

            if (string.IsNullOrEmpty(bulletTd) || string.IsNullOrEmpty(labeledTd) || string.IsNullOrEmpty(textTd))
                throw new Exception("Some of type discriminators are undefined.");

            var div = HtmlNode.CreateNode("<div></div>");

            if (string.IsNullOrEmpty(si.Type))
                throw new Exception("The type of section item is undefined.");

            if (si.Type.Equals(bulletTd))
            {
                var b = (si as BulletSectionItem)!;
                var ul = HtmlNode.CreateNode("<ul></ul>");

                foreach (var listItem in b.List)
                {
                    if (string.IsNullOrEmpty(listItem.Text))
                        throw new Exception("List item of bullet section item has undefined text.");

                    var li = HtmlNode.CreateNode($"<li>{listItem.Text}</li>");
                    ul.AppendChild(li);
                }
                div.AppendChild(ul);
            }
            else if (si.Type.Equals(textTd))
            {
                var t = (si as TextSectionItem)!;

                if (string.IsNullOrEmpty(t.Text))
                    throw new Exception("The text content of text section item is undefined.");

                var p = HtmlNode.CreateNode($"<p>{t.Text}</p>");
                div.AppendChild(p);
            }
            else if (si.Type.Equals(labeledTd))
            {
                var l = (si as LabeledSectionItem)!;

                if (string.IsNullOrEmpty(l.Label))
                    throw new Exception("The label of labeled section item is undefined.");

                if (string.IsNullOrEmpty(l.Text))
                    throw new Exception("The text of labeled section item is undefined.");

                // lsi - labeled section item
                var lsiContainer = HtmlNode.CreateNode("<div class=\"labeled-item\"></div>");
                var lsiContents = new List<HtmlNode>
                {
                    HtmlNode.CreateNode($"<label>{l.Label}: </label>"),
                    HtmlNode.CreateNode($"<span>{l.Text}</span>"),
                };

                foreach (var el in lsiContents)
                    lsiContainer.AppendChild(el);

                div.AppendChild(lsiContainer);
            }
            else
            {
                throw new Exception("Invalid sectionItem type: " + si.Type);
            }

            return div;
        }

        public string CreateCV(CV cv)
        {
            var doc = CreateDocument();

            var head = doc.DocumentNode.SelectSingleNode("/html/head");

            if (string.IsNullOrEmpty(cv.FirstName))
                throw new Exception("First name is undefined.");

            if (string.IsNullOrEmpty(cv.LastName))
                throw new Exception("Last name is undefined.");

            if (string.IsNullOrEmpty(cv.Position))
                throw new Exception("Position is undefined.");

            var title = HtmlNode.CreateNode($"<title>{cv.FirstName} {cv.LastName} | {cv.Position}</title>");
            head.AppendChild(title);

            var body = doc.DocumentNode.SelectSingleNode("/html/body");

            var header = doc.CreateElement("header");
            var fullName = cv.FirstName + " " + cv.LastName;

            var headerContent = new List<HtmlNode>
            {
                HtmlNode.CreateNode($"<h1>{fullName}</h1>"),
                HtmlNode.CreateNode($"<h3>{cv.Position}</h3>")
            };
            foreach (var c in headerContent)
                header.AppendChild(c);

            body.AppendChild(header);

            var main = doc.CreateElement("main");
            var mainContainer = doc.CreateElement("div");
            mainContainer.AddClass("main-container");

            foreach (var c in cv.Containers)
            {
                var section = doc.CreateElement("section");

                if (string.IsNullOrEmpty(c.Title))
                    throw new Exception("CV container title is undefined.");

                section.SetAttributeValue("id", c.Title.ToLower().Replace(" ", "-"));

                foreach (var s in c.Sections)
                {
                    var article = doc.CreateElement("article");
                    if (string.IsNullOrEmpty(s.Title))
                        throw new Exception("CV section title is undefined.");

                    article.SetAttributeValue("id", s.Title.ToLower().Replace(" ", "-"));

                    var articleHeader = HtmlNode.CreateNode($"<header><h4>{s.Title}</h4></header>");
                    article.AppendChild(articleHeader);

                    foreach (var si in s.Items)
                    {
                        article.AppendChild(CreateSectionItem(si));
                    }

                    section.AppendChild(article);
                }

                mainContainer.AppendChild(section);
            }

            main.AppendChild(mainContainer);

            body.AppendChild(main);

            var scriptNode = HtmlNode.CreateNode($"<script src=\"{ScriptUrl}\" defer></script>");

            body.AppendChild(scriptNode);

            return GetDocumentString(doc);
        }

        private string GetDocumentString(HtmlDocument document)
        {
            using (var stream = new MemoryStream())
            {
                document.Save(stream, Encoding.UTF8);

                stream.Position = 0;

                using (var reader = new StreamReader(stream))
                {
                    var result = reader.ReadToEnd();

                    return result;
                }
            }
        }
    }
}
