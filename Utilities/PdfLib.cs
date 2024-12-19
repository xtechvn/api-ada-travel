using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web.Http.Routing;
using System.Web.UI;

namespace Utilities
{
    public class PdfLib
    {
        private readonly IHostEnvironment _env;
        public PdfLib(IHostEnvironment env)
        {
            _env = env;
        }


        public void ConvertHTMLToPDFOP2(string HTMLCode, string title, string lead, string PathLink, out bool isError)
        {
            isError = false;
            try
            {
                var FontPath = Path.Combine(_env.ContentRootPath.Replace("API_CORE", "Utilities"), "Content//fonts//");
                //string FontPath =  "Content//fonts//";
                //Render PlaceHolder to temporary stream
                System.IO.StringWriter stringWrite = new StringWriter();
                HtmlTextWriter htmlWrite = new HtmlTextWriter(stringWrite);

                /********************************************************************************/
                //Try adding source strings for each image in content
                string tempPostContent = getImage(HTMLCode);
                /*********************************************************************************/

                var reader = new StringReader(tempPostContent);

                //Create PDF document
                Document doc = new Document(PageSize.A4);

                var parser = new HTMLWorker(doc);

                string sylfaenpath = Path.Combine(FontPath, "TAHOMA.TTF");


                BaseFont sylfaen = BaseFont.CreateFont(sylfaenpath, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);

                Font head = new Font(sylfaen, 20f, Font.NORMAL, BaseColor.BLACK);
                Font normal = new Font(sylfaen, 15f, Font.NORMAL, BaseColor.BLACK);

                PdfWriter.GetInstance(doc, new FileStream(PathLink, FileMode.Create));

                doc.Open();

                //  Chinh Style
                Paragraph p = new Paragraph();
                if (!string.IsNullOrEmpty(title))
                {
                    doc.Add(new Paragraph(title, head));
                    doc.Add(new Paragraph("\n"));
                }

                if (!string.IsNullOrEmpty(lead))
                {
                    doc.Add(new Paragraph(lead, normal));

                    doc.Add(new Paragraph("\n"));
                    doc.Add(new Paragraph("\n"));
                }
                try
                {
                    //Parse Html and dump the result in PDF file
                    var hw = new iTextSharp.text.html.simpleparser.HTMLWorker(doc);

                    FontFactory.Register(Path.Combine(FontPath, "arial.TTF"), "Arial");   // just give a path of arial.ttf 

                    var css = new StyleSheet();
                    css.LoadTagStyle("body", "face", "Arial");
                    css.LoadTagStyle("body", "encoding", "Identity-H");
                    css.LoadTagStyle("body", "size", "12pt");


                    parser.SetStyleSheet(css);

                    parser.Parse(reader);
                    isError = true;
                }
                catch (Exception ex)
                {
                    isError = false;
                    PathLink = ex.ToString();

                    //Display parser errors in PDF.
                    Paragraph paragraph = new Paragraph("Error!" + ex.Message);
                    Chunk text = paragraph.Chunks[0] as Chunk;
                    if (text != null)
                    {
                        text.Font.Color = BaseColor.RED;
                    }
                    doc.Add(paragraph);
                }
                finally
                {
                    isError = true;
                    doc.Close();
                }

            }
            catch (Exception ex)
            {
                isError = false;
                PathLink = string.Empty;
                throw;
            }
        }


        public string getImage(string input)
        {
            if (input == null)
                return string.Empty;
            string tempInput = input;
            string pattern = @"<img(.|\n)+?>";
            string src = string.Empty;
            //var context = System.Web.HttpContext.Current;

            //Change the relative URL's to absolute URL's for an image, if any in the HTML code.
            foreach (Match m in Regex.Matches(input, pattern, RegexOptions.IgnoreCase | RegexOptions.Multiline |

            RegexOptions.RightToLeft))
            {
                if (m.Success)
                {
                    string tempM = m.Value;
                    string pattern1 = " src=[\'|\"](.+?)[\'|\"]";
                    Regex reImg = new Regex(pattern1, RegexOptions.IgnoreCase | RegexOptions.Multiline);
                    Match mImg = reImg.Match(m.Value);

                    //if (mImg.Success)
                    //{
                    //    src = mImg.Value.ToLower().Replace(" src=", "").Replace("\"", "");

                    //    if (src.ToLower().Contains("http://") == false)
                    //    {
                    //        //Insert new URL in img tag
                    //        UrlHelper url = new UrlHelper();

                    //        src = " src=\"" + url.Request.RequestUri.Scheme + "://" +
                    //        url.Request.RequestUri.Authority + src + "\"";
                    //        // context.Request.Url.Authority + src + "\"";
                    //        try
                    //        {
                    //            tempM = tempM.Remove(mImg.Index, mImg.Length);
                    //            tempM = tempM.Insert(mImg.Index, src);

                    //            //insert new url img tag in whole html code
                    //            tempInput = tempInput.Remove(m.Index, m.Length);
                    //            tempInput = tempInput.Insert(m.Index, tempM);
                    //        }
                    //        catch (Exception ex)
                    //        {
                    //            return ex.ToString();
                    //        }
                    //    }
                    //}
                }
            }
            return tempInput;
        }

    }
}
