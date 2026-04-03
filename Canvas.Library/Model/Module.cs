/*
Module
    - Id
    - Content (list of strings)
*/

using System.Collections.Generic;

namespace Canvas.Library.Model
{
    public class Module
    {
        public int Id { get; set; }
        public string ModuleName { get; set; }
        public List<string>? Content; // retained for CLI
        public List<ModuleContent>? ModuleContents { get; set; }

        public override string ToString() => $"{Id}. {ModuleName}";
    }

    public class ModuleContent
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Body { get; set; }
    }

    public class PageContent : ModuleContent { }

    public class FileContent : ModuleContent
    {
        public string FilePath { get; set; }
        public string MimeType { get; set; }
    }

    public class AssignmentContent : ModuleContent
    {
        public int AssignmentId { get; set; }
    }
}