using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MultiTool;
using SPADemoCRUD.Models;
using SPADemoCRUD.Models.view;

namespace SPADemoCRUD.Controllers
{
    [Authorize(Policy = "AccessMinLevelManager")]
    public class FilesController : Controller
    {
        protected readonly AppConfig conf;
        protected readonly AppDataBaseContext DbContext;
        private FileInfo[] getFtpFiles() => new DirectoryInfo(conf.UploadsPatch).GetFiles();

        public FilesController(AppDataBaseContext db_context, IOptions<AppConfig> options)
        {
            DbContext = db_context;
            conf = options.Value;
            DirectoryInfo uploadsDirInfo = new DirectoryInfo(conf.UploadsPatch);
            if (!uploadsDirInfo.Exists)
            {
                Directory.CreateDirectory(uploadsDirInfo.FullName);
            }
            uploadsDirInfo = new DirectoryInfo(Path.Combine(conf.UploadsPatch, "thumbs"));
            if (!uploadsDirInfo.Exists)
            {
                Directory.CreateDirectory(uploadsDirInfo.FullName);
            }
            uploadsDirInfo = new DirectoryInfo(Path.Combine(conf.UploadsPatch, "storage"));
            if (!uploadsDirInfo.Exists)
            {
                Directory.CreateDirectory(uploadsDirInfo.FullName);
            }
        }

        // GET: /files/ftp?PageNum=1&PageSise=10
        [HttpGet]
        public ActionResult<IEnumerable<object>> Ftp(PaginationParameters pagingParameters)
        {
            List<FileInfo> Files = getFtpFiles().OrderBy(c => c.Name).ToList();
            pagingParameters.Init(ref Files);
            HttpContext.Response.Cookies.Append("rowsCount", pagingParameters.CountAllElements.ToString());
            return Files.Select(x => new { x.Name, Size = glob_tools.SizeDataAsString(x.Length) }).ToArray();
        }

        // GET: /files/storage?PageNum=1&PageSise=10
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> Storage(PaginationParameters pagingParameters)
        {
            pagingParameters.Init(DbContext.FilesStorage.Count());
            IQueryable<FileStorageModel> files = DbContext.FilesStorage.OrderBy(x => x.Id);
            if (pagingParameters.PageNum > 1)
                files = files.Skip(pagingParameters.Skip);

            HttpContext.Response.Cookies.Append("rowsCount", pagingParameters.CountAllElements.ToString());
            return await files.Take(pagingParameters.PageSize).ToListAsync();
        }

        //[ResponseCache(Location = ResponseCacheLocation.Any, Duration = 300)]
        //[Authorize(Policy = "AccessMinLevelManager")]
        //public IActionResult GetFileThumb(string file_id, TypesSource type_source)
        //{
        //    //FileGalleryItem galleryItem = new FileGalleryItem() { TypeSource = type_source };
        //    Image myImage;
        //    switch (type_source)
        //    {
        //        case TypesSource.Storage:
        //            int file_id_as_int = 0;
        //            if (!int.TryParse(file_id, out file_id_as_int))
        //            {
        //                return new NotFoundResult();
        //            }
        //            StorageFileGalleryModel f_store = db.StorageFilesGalleries.Find(file_id_as_int);
        //            if (f_store is null)
        //                return new NotFoundResult();
        //            else
        //            {
        //                if (glob_tools.IsImageFile(f_store.Filename))
        //                {
        //                    myImage = Image.FromStream(new MemoryStream(f_store.Bytes));
        //                    return new FileContentResult(UpdateImage(myImage), f_store.MimeType);
        //                }
        //                else
        //                    return new FileContentResult(f_store.Bytes, f_store.MimeType);
        //            }
        //        case TypesSource.Ftp:
        //            DirectoryInfo d = new DirectoryInfo("Uploads");
        //            string full_file_name = Path.Combine(d.FullName, file_id);

        //            if (!System.IO.File.Exists(full_file_name))
        //                return new NotFoundResult();

        //            if (glob_tools.IsImageFile(full_file_name))
        //            {
        //                myImage = Image.FromFile(full_file_name);
        //                return new FileContentResult(UpdateImage(myImage), "image/jpeg");
        //            }
        //            else
        //                return new PhysicalFileResult(full_file_name, MimeTypeMap.GetMimeType(Path.GetExtension(full_file_name)));
        //        default:
        //            return new UnsupportedMediaTypeResult();
        //    }
        //}

        protected byte[] UpdateImage(Image myImage)
        {
            int h_thumb = myImage.Height;
            int w_thumb = myImage.Width;
            MemoryStream stream = new MemoryStream();

            if (h_thumb > conf.ThumbMaxHeight || w_thumb > conf.ThumbMaxWidth)
            {
                if (h_thumb > conf.ThumbMaxHeight)
                {
                    double dif_val = (double)conf.ThumbMaxHeight / (double)h_thumb;
                    h_thumb = (int)(h_thumb * dif_val);
                    w_thumb = (int)(w_thumb * dif_val);
                }

                if (w_thumb > conf.ThumbMaxWidth)
                {
                    double dif_val = (double)conf.ThumbMaxWidth / (double)w_thumb;
                    w_thumb = (int)(w_thumb * dif_val);
                    h_thumb = (int)(h_thumb * dif_val);
                }

                Image thumb_image = ResizeImage(myImage, w_thumb, h_thumb);

                thumb_image.Save(stream, ImageFormat.Jpeg);

            }
            else
            {
                myImage.Save(stream, ImageFormat.Jpeg);
            }
            return stream.ToArray();
        }

        public static Bitmap ResizeImage(Image image, int width, int height)
        {
            Rectangle destRect = new Rectangle(0, 0, width, height);
            Bitmap destImage = new Bitmap(width, height);

            destImage.SetResolution(96.0F, 96.0F);

            using (Graphics graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (ImageAttributes wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }
    }
}
