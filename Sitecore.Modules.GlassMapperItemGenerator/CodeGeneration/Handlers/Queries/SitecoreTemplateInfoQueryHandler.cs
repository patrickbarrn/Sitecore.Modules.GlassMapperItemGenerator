using System;
using System.Collections.Generic;
using Sitecore.Modules.GlassMapperItemGenerator.CodeGeneration.Contracts.Quries;
using Sitecore.Modules.GlassMapperItemGenerator.CodeGeneration.Models;
using System.Linq;
using Sitecore.Modules.GlassMapperItemGenerator.Extensions;

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
                    ShortDescription = query.TemplateItem.InnerItem.Fields["__Short Description"].Value,
                    LongDescription = query.TemplateItem.InnerItem.Fields["__Long Description"].Value,

                    BaseClassNamespace = query.BaseGlassNamespace,
                    ClassName = query.TemplateItem.Name.AsClassName(),
                    InterfaceName = query.TemplateItem.Name.AsInterfaceName(),
                    Namespace = GetNamespace(query.TemplateItem.InnerItem.Paths.ParentPath, query.BaseNamespace),
                    FilePathFolder =
                        CombineClassData(query.TemplateItem.InnerItem.Paths.ParentPath, query.BaseFilePath, "\\"),
                    IsInterfaceTemplate = query.TemplateItem.Name.IsInterfaceWord(),
                    BaseTemplateNamespaces = GetBaseNamespaces(query.TemplateItem, query.BaseNamespace, ", "),
                };

            // all fields that this template has (this includes any inherited fields from other template items)
            foreach (
                var templateField in
                    query.TemplateItem.Fields.Where(
                        t =>
                        !t.Name.StartsWith("__") && !t.InnerItem.Paths.Path.StartsWith("/sitecore/templates/System"))
                         .ToList()
                         .Select(fieldInfo => new SitecoreField
                             {
                                 Id = fieldInfo.ID.ToString(),
                                 Name = fieldInfo.Name,
                                 Type = fieldInfo.Type,
                                 ShortDescription = fieldInfo.InnerItem.Fields["__Short Description"].Value,
                                 LongDescription = fieldInfo.InnerItem.Fields["__Long Description"].Value,

                                 PropertyName = fieldInfo.Name.AsPropertyName(),
                                 ReturnType = GetFieldReturnType(fieldInfo.Type),
                                 ShouldInferType = ShouldInferType(fieldInfo.Type),
                             }))
            {
                templateInfo.Fields.Add(templateField);
            }

            // fields only specific to this template
            foreach (
                var templateField in
                    query.TemplateItem.OwnFields.Where(t => !t.Name.StartsWith("__"))
                         .ToList()
                         .Select(fieldInfo => new SitecoreField
                             {
                                 Id = fieldInfo.ID.ToString(),
                                 Name = fieldInfo.Name,
                                 Type = fieldInfo.Type,
                                 ShortDescription = fieldInfo.InnerItem.Fields["__Short Description"].Value,
                                 LongDescription = fieldInfo.InnerItem.Fields["__Long Description"].Value,

                                 PropertyName = fieldInfo.Name.AsPropertyName(),
                                 ReturnType = GetFieldReturnType(fieldInfo.Type),
                                 ShouldInferType = ShouldInferType(fieldInfo.Type),
                             }))
            {
                templateInfo.OwnFields.Add(templateField);
            }

            return templateInfo;
        }

        private static string GetBaseNamespaces(Data.Items.TemplateItem template, string baseNamespace, string preString)
        {
            var namespaces = (from baseTemplate in template.BaseTemplates
                where !baseTemplate.InnerItem.Paths.Path.Contains("/sitecore/templates/System")
                let parentPathNamespace = GetNamespace(baseTemplate.InnerItem.Paths.ParentPath, baseNamespace)
                let interfaceName = baseTemplate.InnerItem.Name.AsInterfaceName()
                select parentPathNamespace + "." + interfaceName).ToList();

            return namespaces.Any() ? preString + string.Join(", ", namespaces) : string.Empty;
        }

        private static string GetNamespace(string templatePath, string basePath)
        {
            // strip the base folder path to get the relative path of the template
            var path = templatePath.Replace("/sitecore/templates/", string.Empty);

            if (path.StartsWith("User Defined"))
            {
                var index = path.IndexOf("/", StringComparison.InvariantCultureIgnoreCase);
                if (index >= 0)
                    path = path.Substring(index + 1);
            }

            var namespaceSegments = new List<string> {basePath, path.Replace("/", ".")};

            return namespaceSegments.AsNamespace();
        }

        private static string CombineClassData(string templatePath, string basePath, string seperator)
        {
            // strip the base folder path to get the relative path of the template
            var path = templatePath.Replace("/sitecore/templates/", string.Empty);

            if (path.StartsWith("User Defined"))
            {
                var index = path.IndexOf("/", StringComparison.InvariantCultureIgnoreCase);
                if (index >= 0)
                    path = path.Substring(index + 1);
            }

            if (!basePath.EndsWith(seperator)) basePath += seperator;
            var namespaceSegments = path.Split('/').ToList();

            return basePath + namespaceSegments.AsNamespace().Replace(".", seperator) + seperator;
        }

        private static bool ShouldInferType(string type)
        {
            switch (type.ToLower())
            {
                case "treelist with search":
                case "treelist":
                case "treelistex":
                case "treelist descriptive":
                case "checklist":
                case "multilist with search":
                case "multilist":
                case "grouped droplink":
                case "droplink":
                case "lookup":
                case "droptree":
                case "reference":
                case "tree":
                    return true;
                default:
                    return false;
            }
        }

        private static string GetFieldReturnType(string type)
        {
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
                    return "IEnumerable<IGlassBase>";

                case "grouped droplink":
                case "droplink":
                case "lookup":
                case "droptree":
                case "reference":
                case "tree":
                    return "IGlassBase";

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
