using System.Linq;
using Sitecore.Data.Items;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Modules.GlassMapperItemGenerator.Extensions;

namespace Sitecore.Modules.GlassMapperItemGenerator.UI.WebControls
{
    class GlassMapperCustomItemByTemplateFolder : BaseGlassMapperCustomItem
    {
        protected Checklist TemplateList;

        protected Item TemplateItem { get; set; }

        public override bool LoadTemplates()
        {
            TemplateItem = Context.ContentDatabase.GetItem(ContentId);

            if (TemplateItem != null && !Context.ClientPage.IsEvent)
            {
                FillTemplates();
            }

            return TemplateItem != null;
        }

        private void FillTemplates()
        {
            TemplateList.Controls.Clear();

            var templateFolderItems = TemplateItem.GetTemplateSubItems();
            templateFolderItems.ForEach(x=> TemplateList.Controls.Add(new ChecklistItem
                {
                    ID = Control.GetUniqueID("I"),
                    Value = x.ID.ToString(),
                    Header = x.Name
                }));

            TemplateList.CheckAll();
        }
        
        public override string GenerateCodeFiles()
        {
            foreach (
                var templateItem in
                    TemplateList.Items.Where(template => template.Checked)
                                .Select(template => Context.ContentDatabase.GetItem(template.Value))
                                .Where(templateItem => templateItem != null))
            {
                base.GenerateTemplateClassAndInterface(templateItem);
            }

            return "Your classes and interfaces have been generated"; // TODO: [Put Custom Message here]
        }
    }
}