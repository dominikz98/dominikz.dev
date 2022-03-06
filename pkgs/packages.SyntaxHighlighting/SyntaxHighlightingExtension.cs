using Markdig;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using packages.ColorCode;
using System;

namespace packages.SyntaxHighlighting
{
    public class SyntaxHighlightingExtension : IMarkdownExtension
    {
        private readonly IStyleSheet _customCss;

        public SyntaxHighlightingExtension(IStyleSheet customCss = null)
        {
            _customCss = customCss;
        }

        public void Setup(MarkdownPipelineBuilder pipeline) { }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
            if (renderer == null)
            {
                throw new ArgumentNullException(nameof(renderer));
            }

            if (renderer as TextRendererBase<HtmlRenderer> == null)
            {
                return;
            }

            var originalCodeBlockRenderer = (renderer as TextRendererBase<HtmlRenderer>).ObjectRenderers.FindExact<CodeBlockRenderer>();
            if (originalCodeBlockRenderer != null)
            {
                (renderer as TextRendererBase<HtmlRenderer>).ObjectRenderers.Remove(originalCodeBlockRenderer);
            }
            (renderer as TextRendererBase<HtmlRenderer>).ObjectRenderers.AddIfNotAlready(new SyntaxHighlightingCodeBlockRenderer(originalCodeBlockRenderer, _customCss));
        }
    }
}
