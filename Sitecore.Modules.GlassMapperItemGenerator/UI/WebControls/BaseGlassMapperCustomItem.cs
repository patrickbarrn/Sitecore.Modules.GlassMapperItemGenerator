using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sitecore.Data.Items;
using Sitecore.Web.UI.Pages;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Web.UI.Sheer;
using Sitecore.Web;

namespace Sitecore.Modules.GlassMapperItemGenerator.UI.WebControls
{
    public abstract class BaseGlassMapperCustomItem : DialogForm
    {
        protected Edit ItemNamespace;
        protected Edit ItemFilePath;
        
        protected string ContentId
        {
            get
            {
                var value = WebUtil.GetQueryString("id");
                return !string.IsNullOrEmpty(value) ? value : string.Empty;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            var isLoadSuccessful = LoadTemplates();

            if (isLoadSuccessful)
            {
                if (!Context.ClientPage.IsEvent)
                {
                    var defaultNamespace = Configuration.Settings.GetSetting("Sitecore.Modules.GlassMapperItemGenerator.DefaultNamespace");
                    var defaultFilePath = Configuration.Settings.GetSetting("Sitecore.Modules.GlassMapperItemGenerator.DefaultFilePath");

                    ItemNamespace.Value = !string.IsNullOrWhiteSpace(defaultNamespace) ? defaultNamespace : string.Empty;
                    ItemFilePath.Value = !string.IsNullOrWhiteSpace(defaultFilePath) ? defaultFilePath : string.Empty;
                }
            }
            else
            {
                SheerResponse.Alert("You must select a template to continue.", new string[0]);
            }

            base.OnLoad(e);
        }

        protected override void OnOK(object sender, EventArgs args)
        {
            GenerateGlassBaseItems();
            var message = GenerateCodeFiles();

            SheerResponse.Alert(message, new string[0]);
            base.OnOK(sender, args);
        }

        public abstract bool LoadTemplates();
        public abstract string GenerateCodeFiles();

        protected static string GetFilePathForFolders(Item item)
        {
            if (item.Parent.Paths.Path.Equals("/sitecore/templates")) return string.Empty;
            return GetFilePathForFolders(item.Parent) + "\\" + item.Name;
        }

        protected static void GenerateGlassBaseItems()
        {
            // TODO: generate /GlassBase/IGlassBase.cs, & /GlassBase/GlassBase.cs
        }
    }
}
