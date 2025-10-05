using System;
using System.Reflection;

namespace GS.Trade.Web.Charts.Mvc_02.Areas.HelpPage.ModelDescriptions
{
    public interface IModelDocumentationProvider
    {
        string GetDocumentation(MemberInfo member);

        string GetDocumentation(Type type);
    }
}