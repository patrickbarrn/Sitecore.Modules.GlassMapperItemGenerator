using Sitecore.Data;
using Sitecore.Data.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sitecore.Modules.GlassMapperItemGenerator.Extensions
{
    public static class ItemExtensions
    {
        public static List<Item> GetTemplateSubItems(this Item item)
        {
            var templateItem = (TemplateItem)Context.ContentDatabase.GetItem(TemplateIDs.Template);

            return item.Axes.GetDescendants()
                         .Where(x => x.Template.ID.Equals(templateItem.ID))
                         .Select(x => x)
                         .OrderBy(x => x.Name)
                         .ToList();
        }
    }
}
