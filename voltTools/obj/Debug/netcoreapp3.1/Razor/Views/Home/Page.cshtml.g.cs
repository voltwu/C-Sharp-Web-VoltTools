#pragma checksum "D:\test\C-Sharp-Web-VoltTools\voltTools\Views\Home\Page.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "a6505498c84614ab67cf96790de5275694309233"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Home_Page), @"mvc.1.0.view", @"/Views/Home/Page.cshtml")]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"a6505498c84614ab67cf96790de5275694309233", @"/Views/Home/Page.cshtml")]
    public class Views_Home_Page : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<VoltTools.Models.Views.PageView>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
            WriteLiteral("\r\n");
#nullable restore
#line 3 "D:\test\C-Sharp-Web-VoltTools\voltTools\Views\Home\Page.cshtml"
  
    Layout = "~/Views/Shared/_Layout.cshtml";
    ViewData["Title"] = Model.title;

#line default
#line hidden
#nullable disable
            WriteLiteral("<div id=\"page\">\r\n    <b-container>\r\n        <b-card-body>\r\n            ");
#nullable restore
#line 10 "D:\test\C-Sharp-Web-VoltTools\voltTools\Views\Home\Page.cshtml"
       Write(Html.Raw(Model.contents));

#line default
#line hidden
#nullable disable
            WriteLiteral("\r\n        </b-card-body>\r\n    </b-container>\r\n</div>\r\n<script>\r\n    new Vue({\r\n        el: \"#page\",\r\n        data: {\r\n\r\n        }\r\n    });\r\n</script>");
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<VoltTools.Models.Views.PageView> Html { get; private set; }
    }
}
#pragma warning restore 1591
