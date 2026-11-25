// Markup emitted by Markdown.ColorCode 3.0.1 (HtmlFormatterType.Css), verified on built site (2026-11-25).
// Task 7 CSS and Task 8 JS target exactly these shapes.
//
// 1) Language supported by ColorCode (csharp, json, powershell, html, markdown ...):
//      <div class="csharp"><pre>(newline)<span class="tk-kw">public</span> ...(newline)</pre></div>
//    - the wrapper class IS the language id (lowercase); the <pre> has no <code> child.
//    - tokens: <span class="tk-kw|tk-ty|tk-str|tk-num|tk-com|tk-fn">; untokenised text is bare.
//
// 2) Language NOT supported (bash, yaml, ini ...): Markdig default
//      <pre><code class="language-bash">...</code></pre>
//
// 3) Fence without a language:
//      <pre><code>...</code></pre>
//
// Package gotchas: ColorCode's formatter type lives in namespace Markdown.ColorCode
// (not ColorCode), which shadows Markdig.Markdown - always write Markdig.Markdown.ToHtml.

using ColorCode.Common;
using ColorCode.Styling;

namespace VVidman.BlogBuild.Renderers;

public static class BlogCodeStyles
{
    public static StyleDictionary Create()
    {
        var d = new StyleDictionary();
        void Add(string cls, params string[] scopes)
        {
            foreach (var s in scopes)
                d.Add(new Style(s) { ReferenceName = cls });
        }

        Add("tk-kw", ScopeName.Keyword, ScopeName.ControlKeyword, ScopeName.PreprocessorKeyword, ScopeName.PowerShellOperator);
        Add("tk-ty", ScopeName.ClassName, ScopeName.Type, ScopeName.TypeVariable, ScopeName.NameSpace, ScopeName.PowerShellType);
        Add("tk-str", ScopeName.String, ScopeName.StringCSharpVerbatim, ScopeName.StringEscape, ScopeName.HtmlAttributeValue, ScopeName.XmlAttributeValue);
        Add("tk-num", ScopeName.Number);
        Add("tk-com", ScopeName.Comment, ScopeName.XmlDocComment, ScopeName.XmlDocTag, ScopeName.HtmlComment, ScopeName.XmlComment);
        Add("tk-fn", ScopeName.PowerShellVariable, ScopeName.PowerShellParameter, ScopeName.HtmlAttributeName, ScopeName.XmlAttribute, ScopeName.HtmlElementName, ScopeName.XmlName, ScopeName.BuiltinFunction);
        return d;
    }
}
