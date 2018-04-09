using System.Web.Mvc;
using NWebsec.Mvc.HttpHeaders.Csp;

namespace DIHMT
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());

            GlobalFilters.Filters.Add(new CspAttribute());

            filters.Add(new CspDefaultSrcAttribute { None = true });
            filters.Add(new CspScriptSrcAttribute { Self = true, CustomSources = "platform.twitter.com cdn.syndication.twitter.com cdn.syndication.twimg.com" });
            filters.Add(new CspStyleSrcAttribute { Self = true, UnsafeInline = true, CustomSources = "fonts.googleapis.com platform.twitter.com ton.twimg.com" });
            filters.Add(new CspImgSrcAttribute { Self = true, CustomSources = "www.giantbomb.com syndication.twitter.com pbs.twimg.com platform.twitter.com ton.twimg.com data:" });
            filters.Add(new CspFontSrcAttribute { CustomSources = "fonts.gstatic.com" });
            filters.Add(new CspFrameSrcAttribute { Self = true, CustomSources = "platform.twitter.com syndication.twitter.com" });
            filters.Add(new CspChildSrcAttribute { Self = true, CustomSources = "platform.twitter.com syndication.twitter.com" });
            filters.Add(new CspConnectSrcAttribute { Self = true });
            filters.Add(new CspReportUriAttribute { ReportUris = "https://mtxz.report-uri.com/r/d/csp/enforce" });
        }
    }
}
