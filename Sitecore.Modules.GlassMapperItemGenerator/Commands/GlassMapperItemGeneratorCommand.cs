using Sitecore.Data;
using Sitecore.Diagnostics;
using Sitecore.Shell.Framework.Commands;

namespace Sitecore.Modules.GlassMapperItemGenerator.Commands
{
    public class GlassMapperItemGeneratorCommand : Command
    {
        public override void Execute([NotNull] CommandContext context)
        {
            Assert.IsNotNull(context.Items, "Context.Items");
            if (context.Items.Length != 1) return;

            var item = context.Items[0];

            if (item.TemplateID.ToString().Equals(Global.TemplateId))
            {
                DisplayControl(Global.GenerateByItemControlName, item.ID);
            }
            else if (item.TemplateID.ToString().Equals(Global.TemplateFolderId))
            {
                DisplayControl(Global.GenerateByFolderControlName, item.ID);
            }
            else
            {
                Web.UI.Sheer.SheerResponse.Alert(
                    "You can only run the Item Generator on a Template or Template Folder.", new string[0]);
            }
        }

        private static void DisplayControl(string controlName, ID itemId)
        {
            var controlUrl = UIUtil.GetUri(controlName);
            var url = string.Format("{0}&id={1}", controlUrl, itemId);

            Context.ClientPage.ClientResponse.Broadcast(Context.ClientPage.ClientResponse.ShowModalDialog(url), "Shell");
        }
    }
}