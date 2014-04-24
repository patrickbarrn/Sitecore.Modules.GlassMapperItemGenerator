using Sitecore.Modules.GlassMapperItemGenerator.CodeGeneration.Models;

namespace Sitecore.Modules.GlassMapperItemGenerator.CodeGeneration.Contracts.Commands
{
    public class CreateItemClassCommand
    {
        public SitecoreTemplate SitecoreTemplate { get; set; }

        public string WebAppBasePath { get; set; }
        public string ModuleTemplatePath { get; set; }

        public string FilePath { get; set; }
        public string TemplateName { get; set; }
        public bool SkipIfFileAlreadyExists { get; set; }
    }
}
