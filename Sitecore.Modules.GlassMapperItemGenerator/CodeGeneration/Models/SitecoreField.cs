
namespace Sitecore.Modules.GlassMapperItemGenerator.CodeGeneration.Models
{
    public class SitecoreField
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }

        public string PropertyName { get; set; }
        public string ReturnType { get; set; }
        public bool ShouldInferType { get; set; }
    }
}
