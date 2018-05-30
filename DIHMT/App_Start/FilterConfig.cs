using System.Web.Mvc;
using NWebsec.Mvc.HttpHeaders;
using NWebsec.Mvc.HttpHeaders.Csp;

namespace DIHMT
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());

            /*
             * Content Security Policy
             */

            filters.Add(new CspAttribute());
            filters.Add(new CspDefaultSrcAttribute { None = true });
            filters.Add(new CspScriptSrcAttribute { Self = true, CustomSources = "https://www.google.com https://www.gstatic.com https://cdnjs.cloudflare.com https://platform.twitter.com https://cdn.syndication.twitter.com https://cdn.syndication.twimg.com" });
            filters.Add(new CspStyleSrcAttribute { Self = true, UnsafeInline = true, CustomSources = "https://fonts.googleapis.com https://platform.twitter.com https://ton.twimg.com" });
            filters.Add(new CspImgSrcAttribute { Self = true, CustomSources = "https://www.giantbomb.com https://www.paypalobjects.com https://syndication.twitter.com https://abs.twimg.com https://pbs.twimg.com https://platform.twitter.com https://ton.twimg.com data:" });
            filters.Add(new CspFontSrcAttribute { CustomSources = "https://fonts.gstatic.com" });
            filters.Add(new CspFrameSrcAttribute { Self = true, CustomSources = "https://www.google.com https://platform.twitter.com https://syndication.twitter.com" });
            filters.Add(new CspChildSrcAttribute { Self = true, CustomSources = "https://www.google.com https://platform.twitter.com https://syndication.twitter.com" });
            filters.Add(new CspConnectSrcAttribute { Self = true });
            filters.Add(new CspFrameAncestorsAttribute { None = true });
            filters.Add(new CspReportUriAttribute { ReportUris = "https://mtxz.report-uri.com/r/d/csp/enforce" });

            /*
             * Other security headers
             */

            filters.Add(new XFrameOptionsAttribute { Policy = XFrameOptionsPolicy.SameOrigin });
            filters.Add(new XXssProtectionAttribute());
            filters.Add(new XContentTypeOptionsAttribute());
        }
    }
}
