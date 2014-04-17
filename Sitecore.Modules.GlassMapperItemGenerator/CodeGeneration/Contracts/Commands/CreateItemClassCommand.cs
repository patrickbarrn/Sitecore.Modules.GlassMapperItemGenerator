using Sitecore.Modules.GlassMapperItemGenerator.CodeGeneration.Models;

namespace Sitecore.Modules.GlassMapperItemGenerator.CodeGeneration.Contracts.Commands
{
    public class CreateItemClassCommand
    {
        public SitecoreTemplate SitecoreTemplate { get; set; }

        public bool GenerateClassItem { get; set; }
        public bool GenerateInterfaceItem { get; set; }
    }
}
