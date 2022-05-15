using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace JiraCloudBackupViewer
{
    public class SearchIssue
    {
        public string ProjectKey => Issue?.Attribute("projectKey")?.Value;
        public string Number => Issue?.Attribute("number")?.Value;
        public string IssueNr => $"{ProjectKey}-{Number}";
        public string Summary => Issue?.Attribute("summary")?.Value;
        public string Description => Issue?.Descendants("description")?.FirstOrDefault()?.Value;
        public DateTime Created => ((DateTime)Issue?.Attribute("created"));

        public XElement Issue { get; set; }

        public List<SearchAction> Actions { get; set; } = new List<SearchAction>();

        public List<SearchFileAttachment> FileAttachments { get; set; } = new List<SearchFileAttachment>();

        public bool IsMatch(string keyword)
        {
            return Description?.Contains(keyword, StringComparison.InvariantCultureIgnoreCase) ?? false
                || Summary.Contains(keyword, StringComparison.InvariantCultureIgnoreCase)
                || Actions.Any(a => a.Body?.Contains(keyword, StringComparison.InvariantCultureIgnoreCase) ?? false);
        }
    }

    public class SearchAction
    {
        public string Type => Action?.Attribute("type")?.Value;
        public DateTime Created => ((DateTime)Action?.Attribute("created"));
        public string Author => Action?.Attribute("author")?.Value;

        public string Body
        {
            get
            {
                return Action?.Descendants("body")?.FirstOrDefault()?.Value ?? Action.Attribute("body")?.Value;
            }
        }

        public XElement Action { get; set; }
    }

    public class SearchFileAttachment
    {
        public string Id => FileAttachment?.Attribute("id")?.Value;
        public DateTime Created => ((DateTime)FileAttachment?.Attribute("created"));
        public string Author => FileAttachment?.Attribute("author")?.Value;
        public string Mimetype => FileAttachment?.Attribute("mimetype")?.Value;
        public string Thumbnailable => FileAttachment?.Attribute("thumbnailable")?.Value;
        public string Filename => FileAttachment?.Attribute("filename")?.Value;

        public XElement FileAttachment { get; set; }
    }
}
