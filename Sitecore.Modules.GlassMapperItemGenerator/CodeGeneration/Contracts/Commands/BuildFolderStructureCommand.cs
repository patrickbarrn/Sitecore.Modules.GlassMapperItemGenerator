
namespace Sitecore.Modules.GlassMapperItemGenerator.CodeGeneration.Contracts.Commands
{
    public class BuildFolderStructureCommand
    {
        public string FilePathRoot { get; set; }
        public string BaseNamespace { get; set; }
        public string ClassNamespace { get; set; }
    }
}
