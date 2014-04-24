using System;
using Sitecore.Modules.GlassMapperItemGenerator.CodeGeneration.Contracts.Quries;
using Sitecore.Modules.GlassMapperItemGenerator.CodeGeneration.Models;
using System.Linq;

namespace Sitecore.Modules.GlassMapperItemGenerator.CodeGeneration.Handlers.Queries
{
    internal class SitecoreTemplateInfoQueryHandler : IQueryHandler<SitecoreTemplateInfoQuery, SitecoreTemplate>
    {
        public SitecoreTemplate Handle(SitecoreTemplateInfoQuery query)
        {
            var templateInfo = new SitecoreTemplate
                {
                    Id = query.TemplateItem.ID.ToString(),
                    Name = query.TemplateItem.Name,
                    Path = query.TemplateItem.InnerItem.Paths.Path,
                    
                    BaseClassNamespace = query.BaseGlassNamespace,
                    ClassName = RemoveIllegalCharacters(query.TemplateItem.Name),
                    InterfaceName = "I" + RemoveIllegalCharacters(query.TemplateItem.Name),
                    Namespace = CombineClassData(query.TemplateItem.InnerItem.Paths.ParentPath, query.BaseNamespace, ".", false),
                    FilePathFolder = CombineClassData(query.TemplateItem.InnerItem.Paths.ParentPath, query.BaseFilePath, "\\", true)
                };

            foreach (
                var templateField in
                    query.TemplateItem.Fields.Where(t => !t.Name.StartsWith("__"))
                         .ToList()
                         .Select(fieldInfo => new SitecoreField
                             {
                                 Id = fieldInfo.ID.ToString(),
                                 Name = fieldInfo.Name,
                                 Type = fieldInfo.Type,
                                 PropertyName = RemoveIllegalCharacters(fieldInfo.Name),
                                 ReturnType = GetFieldReturnType(fieldInfo.Type)
                             }))
            {
                templateInfo.Fields.Add(templateField);
            }

            return templateInfo;
        }

        private static string RemoveIllegalCharacters(string value)
        {
            return value.Replace(" ", string.Empty).Replace("-", string.Empty);
        }

        private static string CombineClassData(string templatePath, string basePath, string seperator, bool endWithSepeartor)
        {
            // strip the base folder path to get the relative path of the template
            var path = templatePath.Replace("/sitecore/templates/", string.Empty);

            // if not at the template root remove the first slash
            var index = path.IndexOf("/", StringComparison.InvariantCultureIgnoreCase);
            if (index >= 0)
                path = path.Substring(index + 1);

            if (endWithSepeartor && !path.EndsWith(seperator)) path += seperator;
            if (!basePath.EndsWith(seperator)) basePath += seperator;

            path = path.Replace("/", seperator);
            path = RemoveIllegalCharacters(path);

            return basePath + path;
        }

        private static string GetFieldReturnType(string type)
        {
            var generic = string.Empty; // TODO: can we leverage a "standard value" field to store a generic key=value list?
            switch (type.ToLower())
            {
                case "tristate":
                    return "TriState";
                case "checkbox":
                    return "bool";

                case "date":
                case "datetime":
                    return "DateTime";

                case "number":
                    return "float";

                case "integer":
                    return "int";

                case "treelist with search":
                case "treelist":
                case "treelistex":
                case "treelist descriptive":
                case "checklist":
                case "multilist with search":
                case "multilist":
                    return string.Format("IEnumerable<{0}>", string.IsNullOrEmpty(generic) ? "Guid" : generic);

                case "grouped droplink":
                case "droplink":
                case "lookup":
                case "droptree":
                case "reference":
                case "tree":
                    return "Guid";

                case "file":
                    return "File";

                case "image":
                    return "Image";

                case "general link":
                case "general link with search":
                    return "Link";

                case "password":
                case "icon":
                case "rich text":
                case "html":
                case "single-line text":
                case "multi-line text":
                case "frame":
                case "text":
                case "memo":
                case "droplist":
                case "grouped droplist":
                case "valuelookup":
                    return "string";
                case "attachment":
                case "word document":
                    return "System.IO.Stream";
                case "name lookup value list":
                case "name value list":
                    return "System.Collections.Specialized.NameValueCollection";
                default:
                    return "object /* UNKNOWN */";
            }
        }
    }
}
