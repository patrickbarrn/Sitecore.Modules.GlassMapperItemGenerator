using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NVelocity;
using NVelocity.App;
using Sitecore.Modules.GlassMapperItemGenerator.CodeGeneration.Contracts.Commands;

namespace Sitecore.Modules.GlassMapperItemGenerator.CodeGeneration.Handlers.Commands
{
    public class CreateItemClassCommandHandler: ICommandHandler<CreateItemClassCommand>
    {
        public void Handle(CreateItemClassCommand command)
        {
            var baseContext = new VelocityContext();

            baseContext.Put("SitecoreTemplate", command.SitecoreTemplate);

            var interfaceFilePath = string.Empty; // TODO: get full path of the class file(s)
            var classFilePath = string.Empty; // TODO: get full path of the class file(s)
            
            using (var streamWriter = new StreamWriter(interfaceFilePath))
            {
                Velocity.Init();
                streamWriter.Write(Text.NVelocity.VelocityHelper.Evaluate(baseContext, "template text", "logname"));
                // TODO: any reporting / logging needed should go here for base interface
            }

            using (var streamWriter = new StreamWriter(classFilePath))
            {
                Velocity.Init();
                streamWriter.Write(Text.NVelocity.VelocityHelper.Evaluate(baseContext, "template text", "logname"));
                // TODO: any reporting / logging needed should go here for base class
            }
        }
    }
}
