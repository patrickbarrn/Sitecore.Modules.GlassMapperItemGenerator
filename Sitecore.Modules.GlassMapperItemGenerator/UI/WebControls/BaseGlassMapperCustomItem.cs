using System;
using Sitecore.Data.Items;
using Sitecore.Modules.GlassMapperItemGenerator.CodeGeneration.Contracts.Quries;
using Sitecore.Modules.GlassMapperItemGenerator.CodeGeneration.Handlers.Commands;
using Sitecore.Modules.GlassMapperItemGenerator.CodeGeneration.Handlers.Queries;
using Sitecore.Modules.GlassMapperItemGenerator.CodeGeneration.Models;
using Sitecore.Modules.GlassMapperItemGenerator.CodeGeneration.Contracts.Commands;
using Sitecore.Modules.GlassMapperItemGenerator.CodeGeneration;
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

        private readonly ICommandHandler<BuildFolderStructureCommand> _buildFolderStructureHandler;
        private readonly ICommandHandler<CreateItemClassCommand> _createItemClassHandler;
        private readonly IQueryHandler<SitecoreTemplateInfoQuery, SitecoreTemplate> _sitecoreTemplateInfoHandler; 

        protected BaseGlassMapperCustomItem()
        {
            _buildFolderStructureHandler = new BuildFolderStructureCommandHandler();
            _createItemClassHandler = new CreateItemClassCommandHandler();
            _sitecoreTemplateInfoHandler = new SitecoreTemplateInfoQueryHandler();
        }

        protected string ContentId
        {
            get
            {
                var value = WebUtil.GetQueryString("id");
                return !string.IsNullOrEmpty(value) ? value : string.Empty;
            }
        }

        protected string BaseGlassClassNamespace
        {
            get
            {
                return ItemNamespace.Value + "." +
                   Configuration.Settings.GetSetting(
                       "Sitecore.Modules.GlassMapperItemGenerator.GlassMapperClassFolder");
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            var isLoadSuccessful = LoadTemplates();

            if (isLoadSuccessful)
            {
                if (!Context.ClientPage.IsEvent)
                {
                    var defaultNamespace =
                        Configuration.Settings.GetSetting("Sitecore.Modules.GlassMapperItemGenerator.DefaultNamespace");
                    var defaultFilePath =
                        Configuration.Settings.GetSetting("Sitecore.Modules.GlassMapperItemGenerator.DefaultFilePath");

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

        protected virtual void GenerateTemplateClassAndInterface(TemplateItem template)
        {
            var templateInfo = _sitecoreTemplateInfoHandler.Handle(new SitecoreTemplateInfoQuery
                {
                    TemplateItem = template,
                    BaseNamespace = ItemNamespace.Value,
                    BaseGlassNamespace = BaseGlassClassNamespace,
                    BaseFilePath = ItemFilePath.Value
                });

            // Create Directory structure if it doesn't exist
            _buildFolderStructureHandler.Handle(new BuildFolderStructureCommand
            {
                BaseNamespace = ItemNamespace.Value,
                ClassNamespace = templateInfo.Namespace,
                FilePathRoot = ItemFilePath.Value
            });

            // Generate "Designer" partial interface, always overwrite this file
            _createItemClassHandler.Handle(new CreateItemClassCommand
            {
                SitecoreTemplate = templateInfo,
                WebAppBasePath = System.Web.HttpContext.Current.Server.MapPath("/"),
                ModuleTemplatePath = Global.NVelocityTemplateFolder,
                FilePath = templateInfo.FilePathFolder + templateInfo.InterfaceName +".designer.cs",
                TemplateName = "IItemMapper.designer.vm",
                SkipIfFileAlreadyExists = false,
            });

            // Generate "main" partial interface, never overwrite this file
            _createItemClassHandler.Handle(new CreateItemClassCommand
            {
                SitecoreTemplate = templateInfo,
                WebAppBasePath = System.Web.HttpContext.Current.Server.MapPath("/"),
                ModuleTemplatePath = Global.NVelocityTemplateFolder,
                FilePath = templateInfo.FilePathFolder + templateInfo.InterfaceName + ".cs",
                TemplateName = "IItemMapper.vm",
                SkipIfFileAlreadyExists = true,
            });

            if (templateInfo.IsInterfaceTemplate) return; // don't generate the class if it's a interface type template

            // Generate "Designer" partial class, always overwrite this file
            _createItemClassHandler.Handle(new CreateItemClassCommand
            {
                SitecoreTemplate = templateInfo,
                WebAppBasePath = System.Web.HttpContext.Current.Server.MapPath("/"),
                ModuleTemplatePath = Global.NVelocityTemplateFolder,
                FilePath = templateInfo.FilePathFolder + templateInfo.ClassName + ".designer.cs",
                TemplateName = "ItemMapper.designer.vm",
                SkipIfFileAlreadyExists = false,
            });

            // Generate "main" partial class, never overwrite this file
            _createItemClassHandler.Handle(new CreateItemClassCommand
            {
                SitecoreTemplate = templateInfo,
                WebAppBasePath = System.Web.HttpContext.Current.Server.MapPath("/"),
                ModuleTemplatePath = Global.NVelocityTemplateFolder,
                FilePath = templateInfo.FilePathFolder + templateInfo.ClassName + ".cs",
                TemplateName = "ItemMapper.vm",
                SkipIfFileAlreadyExists = true,
            });
        }

        public abstract bool LoadTemplates();
        public abstract string GenerateCodeFiles();

        protected static string GetFilePathForFolders(Item item)
        {
            if (item.Parent.Paths.Path.Equals("/sitecore/templates")) return string.Empty;
            return GetFilePathForFolders(item.Parent) + "\\" + item.Name;
        }

        protected void GenerateGlassBaseItems()
        {
            var filePath = ItemFilePath.Value +@"\" + Configuration.Settings.GetSetting(
                "Sitecore.Modules.GlassMapperItemGenerator.GlassMapperClassFolder") +@"\";
            
            var classFilePath = filePath + "GlassBase.cs";
            var classFilePathDesigner = filePath + "GlassBase.designer.cs";
            var interfaceFilePath = filePath + "IGlassBase.cs";
            var interfaceFilePathDesigner = filePath + "IGlassBase.designer.cs";

            var glassMapperTempalate = new SitecoreTemplate
            {
                Namespace = BaseGlassClassNamespace
            };

            _buildFolderStructureHandler.Handle(new BuildFolderStructureCommand
                {
                    BaseNamespace = ItemNamespace.Value,
                    ClassNamespace =
                        Configuration.Settings.GetSetting(
                            "Sitecore.Modules.GlassMapperItemGenerator.GlassMapperClassFolder"),
                    FilePathRoot = ItemFilePath.Value
                });

            // create interface item
            _createItemClassHandler.Handle(new CreateItemClassCommand
                {
                    SitecoreTemplate = glassMapperTempalate,
                    WebAppBasePath = System.Web.HttpContext.Current.Server.MapPath("/"),
                    ModuleTemplatePath = Global.NVelocityTemplateFolder,
                    FilePath = interfaceFilePath,
                    TemplateName = "IGlassBase.vm",
                    SkipIfFileAlreadyExists = true,
                });

            _createItemClassHandler.Handle(new CreateItemClassCommand
            {
                SitecoreTemplate = glassMapperTempalate,
                WebAppBasePath = System.Web.HttpContext.Current.Server.MapPath("/"),
                ModuleTemplatePath = Global.NVelocityTemplateFolder,
                FilePath = interfaceFilePathDesigner,
                TemplateName = "IGlassBase.designer.vm",
                SkipIfFileAlreadyExists = false,
            });

            // create class item
            _createItemClassHandler.Handle(new CreateItemClassCommand
            {
                SitecoreTemplate = glassMapperTempalate,
                WebAppBasePath = System.Web.HttpContext.Current.Server.MapPath("/"),
                ModuleTemplatePath = Global.NVelocityTemplateFolder,
                FilePath = classFilePath,
                TemplateName = "GlassBase.vm",
                SkipIfFileAlreadyExists = true,
            });

            _createItemClassHandler.Handle(new CreateItemClassCommand
            {
                SitecoreTemplate = glassMapperTempalate,
                WebAppBasePath = System.Web.HttpContext.Current.Server.MapPath("/"),
                ModuleTemplatePath = Global.NVelocityTemplateFolder,
                FilePath = classFilePathDesigner,
                TemplateName = "GlassBase.designer.vm",
                SkipIfFileAlreadyExists = false,
            });
        }
    }
}
