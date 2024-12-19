using ServiceReceiverMedia.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;
using System;
using System.IO;
namespace ServiceReceiverMedia.Common
{
    public class ImageUploadHelper
    {
        public static bool IsBase64String(string s)
        {
            try
            {
                byte[] bytes = System.Convert.FromBase64String(s);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public static string SaveFile (ImageDetail img_detail, string imgPath_base, string imgPath_url_base, int max_file_size)
        {
            string url = "";
            try
            {
                //Nếu lấy ra được thông tin:
                if (ImageUploadHelper.IsBase64String(img_detail.data_file))
                {
                    //Lấy thông tin thời gian hiện tại
                    DateTime time = DateTime.Now;
                    string year = time.Year.ToString();
                    string month = time.Month.ToString();
                    string day = time.Day.ToString();
                    // string hour = time.Hour.ToString();

                    //Thông tin file và build đường dẫn local:
                    string file_name = Guid.NewGuid() + "." + img_detail.extend;
                    string imgPath_year = @"\" + year + @"\";
                    string imgPath_month = imgPath_year + month + @"\";
                    string imgPath_day = imgPath_month + day + @"\";

                    //Nếu folder trống,tạo mới, nếu file exsist, thêm _[i] vào sau tên file
                    if (!Directory.Exists(imgPath_base))
                    {
                        Directory.CreateDirectory(imgPath_base);
                    }

                    if (!Directory.Exists(imgPath_base + imgPath_year))
                    {
                        Directory.CreateDirectory(imgPath_base + imgPath_year);
                    }

                    if (!Directory.Exists(imgPath_base + imgPath_month))
                    {
                        Directory.CreateDirectory(imgPath_base + imgPath_month);
                    }

                    if (!Directory.Exists(imgPath_base + imgPath_day))
                    {
                        Directory.CreateDirectory(imgPath_base + imgPath_day);
                    }
                    string imgPath_full = (imgPath_base + imgPath_day + file_name);
                    byte[] bytes = System.Convert.FromBase64String(img_detail.data_file);
                    //Kiểm tra nếu file vượt quá max size:
                    if (bytes.Length > max_file_size)
                    {
                        return url;
                    }
                    else
                    {
                        //Ghi byte[] vào file đã tạo:
                        using (var fs = new FileStream(imgPath_full, FileMode.Create, FileAccess.Write))
                        {
                            fs.Write(bytes, 0, bytes.Length);
                        }
                    }
                    //Build đường link local:
                    string urlPath_full = imgPath_url_base + imgPath_day + file_name;
                    url = urlPath_full.Replace(@"\", @"/");
                }
               
            }
            catch (Exception ex)
            {
            }
            return url;
        }
        /// <summary>
        /// Resize image with maximum 1000px width
        /// </summary>
        /// <param name="ImageBase64"></param>
        /// <returns></returns>
        public static string ResizeBase64ImageToWidth(string ImageBase64, out string FileType, int width = 250)
        {
            FileType = null;
            try
            {
                var IsValid = TryGetFromBase64String(ImageBase64, out byte[] ImageByte);
                if (IsValid)
                {
                    using (Image image = Image.Load(ImageByte))
                    {
                        int height = (int) (image.Height * ((double)width / image.Width));
                        image.Mutate(x => x.Resize(width, height));
                        FileType = "jpeg";
                        return image.ToBase64String(JpegFormat.Instance).Split(",")[1];
                    }
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public static bool TryGetFromBase64String(string input, out byte[] output)
        {
            output = null;
            try
            {
                output = Convert.FromBase64String(input);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}
