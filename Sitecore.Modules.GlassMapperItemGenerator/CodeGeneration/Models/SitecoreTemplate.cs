using System.Collections.Generic;

namespace Sitecore.Modules.GlassMapperItemGenerator.CodeGeneration.Models
{
    public class SitecoreTemplate
    {
        public string Id { get; set; }
        public string Path { get; set; }
        public string Name { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        
        public string Namespace { get; set; }
        public string BaseClassNamespace { get; set; }
        public string ClassName { get; set; }
        public string InterfaceName { get; set; }
        public string FilePathFolder { get; set; }
        public bool IsInterfaceTemplate { get; set; }

        public IList<SitecoreField> Fields { get; set; }

        public SitecoreTemplate()
        {
            Fields = new List<SitecoreField>();
        }
    }
}
