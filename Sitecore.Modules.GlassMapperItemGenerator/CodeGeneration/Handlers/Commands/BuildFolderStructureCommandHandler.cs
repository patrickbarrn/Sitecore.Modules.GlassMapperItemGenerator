using Sitecore.Modules.GlassMapperItemGenerator.CodeGeneration.Contracts.Commands;
using System.IO;
using System.Linq;

namespace Sitecore.Modules.GlassMapperItemGenerator.CodeGeneration.Handlers.Commands
{
    public class BuildFolderStructureCommandHandler: ICommandHandler<BuildFolderStructureCommand>
    {
        public void Handle(BuildFolderStructureCommand command)
        {
            var baseDirectoryExists = Directory.Exists(command.FilePathRoot);
            if(!baseDirectoryExists)
                throw new DirectoryNotFoundException("File Path Root not found:" +command.FilePathRoot);

            var relativeNamespace = command.ClassNamespace.Replace(command.BaseNamespace, string.Empty);

            var directories = relativeNamespace.Split('.');
            var path = command.FilePathRoot;

            foreach (var directory in directories.Where(directory => !string.IsNullOrWhiteSpace(directory)))
            {
                path += @"\" + directory;

                if (Directory.Exists(path)) continue;

                Directory.CreateDirectory(path);
                if(!Directory.Exists(path))
                    throw new FileNotFoundException("Could not create directory:" +path);
            }
        }
    }
}
