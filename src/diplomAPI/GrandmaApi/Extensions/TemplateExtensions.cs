using Scriban;
using Scriban.Runtime;

namespace GrandmaApi.Extensions;

public static class TemplateExtensions
{
    public static string RenderWithRenamer(this Template template, object payload)
    {
        var context = new TemplateContext
        {
            MemberRenamer = x => x.Name,
        };
            
        var scriptObject = new ScriptObject();
        scriptObject.Import(new {Entities = payload}, renamer: x => x.Name);
        context.PushGlobal(scriptObject);

        return template.Render(context);
    }
}