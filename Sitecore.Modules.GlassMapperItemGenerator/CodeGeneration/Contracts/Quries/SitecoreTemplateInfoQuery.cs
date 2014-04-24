using Sitecore.Data.Items;
using Sitecore.Modules.GlassMapperItemGenerator.CodeGeneration.Models;

namespace Sitecore.Modules.GlassMapperItemGenerator.CodeGeneration.Contracts.Quries
{
    public class SitecoreTemplateInfoQuery : IQuery<SitecoreTemplate>
    {
        public string BaseNamespace { get; set; }
        public string BaseGlassNamespace { get; set; }
        public string BaseFilePath { get; set; }
        public TemplateItem TemplateItem { get; set; }
    }
}
