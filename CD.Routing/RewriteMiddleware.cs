﻿using System;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Net.Http.Headers;

namespace RewriteRules
{
    public class MethodRules
    {
        #region snippet_RedirectXmlFileRequests
        public static void RedirectXmlFileRequests(RewriteContext context)
        {
            var request = context.HttpContext.Request;

            // Because the client is redirecting back to the same app, stop 
            // processing if the request has already been redirected.
            if (request.Path.StartsWithSegments(new PathString("/xmlfiles")))
            {
                return;
            }

            if (request.Path.Value.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
            {
                var response = context.HttpContext.Response;
                response.StatusCode = StatusCodes.Status301MovedPermanently;
                context.Result = RuleResult.EndResponse;
                response.Headers[HeaderNames.Location] =
                    "/xmlfiles" + request.Path + request.QueryString;
            }
        }
        #endregion

        #region snippet_RewriteTextFileRequests
        public static void RewriteTextFileRequests(RewriteContext context)
        {
            var request = context.HttpContext.Request;

            if (request.Path.Value.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
            {
                context.Result = RuleResult.SkipRemainingRules;
                request.Path = "/file.txt";
            }
        }
        #endregion
    }

    #region snippet_RedirectImageRequests
    public class RedirectImageRequests : IRule
    {
        private readonly string _extension;
        private readonly PathString _newPath;

        public RedirectImageRequests(string extension, string newPath)
        {
            if (string.IsNullOrEmpty(extension))
            {
                throw new ArgumentException(nameof(extension));
            }

            if (!Regex.IsMatch(extension, @"^\.(png|jpg|gif)$"))
            {
                throw new ArgumentException("Invalid extension", nameof(extension));
            }

            if (!Regex.IsMatch(newPath, @"(/[A-Za-z0-9]+)+?"))
            {
                throw new ArgumentException("Invalid path", nameof(newPath));
            }

            _extension = extension;
            _newPath = new PathString(newPath);
        }

        public void ApplyRule(RewriteContext context)
        {
            var request = context.HttpContext.Request;

            // Because we're redirecting back to the same app, stop 
            // processing if the request has already been redirected
            if (request.Path.StartsWithSegments(new PathString(_newPath)))
            {
                return;
            }

            if (request.Path.Value.EndsWith(_extension, StringComparison.OrdinalIgnoreCase))
            {
                var response = context.HttpContext.Response;
                response.StatusCode = StatusCodes.Status301MovedPermanently;
                context.Result = RuleResult.EndResponse;
                response.Headers[HeaderNames.Location] =
                    _newPath + request.Path + request.QueryString;
            }
        }
    }
    #endregion

    public static class RewriteMiddlewareExtension
    {
        public static void UseRewriteMiddleware(this IApplicationBuilder app)
        {
            using (StreamReader apacheModRewriteStreamReader =
                File.OpenText("ApacheModRewrite.txt"))
            {
                var options = new RewriteOptions()
                    .AddApacheModRewrite(apacheModRewriteStreamReader);

                app.UseRewriter(options);
            }
        }
    }
}
