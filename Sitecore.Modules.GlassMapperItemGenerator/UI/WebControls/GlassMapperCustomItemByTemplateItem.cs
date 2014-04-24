using Sitecore.Data.Items;

namespace Sitecore.Modules.GlassMapperItemGenerator.UI.WebControls
{
    internal class GlassMapperCustomItemByTemplateItem : BaseGlassMapperCustomItem
    {
        protected TemplateItem TemplateItem { get; set; }

        public override bool LoadTemplates()
        {
            TemplateItem = Context.ContentDatabase.GetItem(ContentId);

            return TemplateItem != null;
        }

        public override string GenerateCodeFiles()
        {
            base.GenerateTemplateClassAndInterface(TemplateItem);

            return "[Put Custom Message here]";
        }
    }
}