using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.Syndication;

namespace RssViewer.Utilities
{
    public static class HtmlBuilder
    {
        public static string ConstructItemsHtml(SyndicationFeed feed)
        {
            StringBuilder result = new StringBuilder(
                    @"<!DOCTYPE HTML PUBLIC ""-//W3C//DTD HTML 4.0 Transitional//EN"">" +
                         "<html>" +
                           @"<meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8"">" +
                            "<head></head>" +
                            @"<body style=""font-family:Calibri;"">");                       
            foreach (var item in feed.Items)
            {
                result.Append(@"<h6 style=""text-align:right;"">" + item.PublishDate.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss") + "</h5>");
                result.Append(@"<h3 style=""text-align:center;"">" + item.Title.Text + "</h3>");
                result.Append(item.Summary.Text);
                result.Append("<hr/>");
            }
            result.Append(  "</body>");
            result.Append("</html>");
            return result.ToString();
        }
    }
}
