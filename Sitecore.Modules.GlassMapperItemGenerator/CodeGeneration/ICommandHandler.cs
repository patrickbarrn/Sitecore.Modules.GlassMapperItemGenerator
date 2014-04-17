
namespace Sitecore.Modules.GlassMapperItemGenerator.CodeGeneration
{
    public interface ICommandHandler<in TCommand>
    {
        void Handle(TCommand command);
    }
}
