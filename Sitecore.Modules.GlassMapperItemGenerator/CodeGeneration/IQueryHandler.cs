
namespace Sitecore.Modules.GlassMapperItemGenerator.CodeGeneration
{
    public interface IQueryHandler<in TQuery, out TResult> where TQuery : IQuery<TResult>
    {
        TResult Handle(TQuery query);
    }
}
