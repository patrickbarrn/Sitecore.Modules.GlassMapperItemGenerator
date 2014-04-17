using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            templateFolderItems.ForEach(x=> TemplateList.Controls.Add(new ChecklistItem()
                {
                    ID = Control.GetUniqueID("I"),
                    Value = x.ID.ToString(),
                    Header = x.Name
                }));

            TemplateList.CheckAll();
        }
        
        public override string GenerateCodeFiles()
        {
            // TODO: 
            return "[Put Custom Message here]";
        }

        
    }
}