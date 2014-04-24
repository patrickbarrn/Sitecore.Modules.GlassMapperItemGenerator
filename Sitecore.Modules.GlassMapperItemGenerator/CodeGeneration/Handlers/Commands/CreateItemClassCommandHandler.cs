using System.IO;
using NVelocity;
using NVelocity.App;
using Sitecore.Modules.GlassMapperItemGenerator.CodeGeneration.Contracts.Commands;

namespace Sitecore.Modules.GlassMapperItemGenerator.CodeGeneration.Handlers.Commands
{
    public class CreateItemClassCommandHandler: ICommandHandler<CreateItemClassCommand>
    {
        public void Handle(CreateItemClassCommand command)
        {
            if (command.SkipIfFileAlreadyExists && File.Exists(command.FilePath)) return;

            var baseContext = new VelocityContext();

            baseContext.Put("SitecoreTemplate", command.SitecoreTemplate);

            TextReader reader = new StreamReader(command.WebAppBasePath + command.ModuleTemplatePath + command.TemplateName);
            var template = reader.ReadToEnd();

            using (var streamWriter = new StreamWriter(command.FilePath))
            {
                Velocity.Init();
                streamWriter.Write(Text.NVelocity.VelocityHelper.Evaluate(baseContext, template,
                                                                          "GlassMapperItemGenerator.CreateItemClassCommandHandler"));
            }
        }
    }
}
