using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Data.Items;

namespace Sitecore.Modules.GlassMapperItemGenerator.UI.WebControls
{
    class GlassMapperCustomItemByTemplateItem : BaseGlassMapperCustomItem
    {
        protected TemplateItem TemplateItem { get; set; }

        public override bool LoadTemplates()
        {
            TemplateItem = Context.ContentDatabase.GetItem(ContentId);

            return TemplateItem != null;
        }
        
        public override string GenerateCodeFiles()
        {
            // TODO: 
            return "[Put Custom Message here]"; 
        }

        
    }
}