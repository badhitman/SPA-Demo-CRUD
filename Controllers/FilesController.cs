////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MultiTool;
using SPADemoCRUD.Models;
using SPADemoCRUD.Services;

namespace SPADemoCRUD.Controllers
{
    [Authorize(Policy = "AccessMinLevelManager")]
    public class FilesController : Controller
    {
        #region props
        protected readonly AppConfig conf;
        protected readonly AppDataBaseContext DbContext;
        private readonly ILogger<FilesController> _logger;

        /// <summary>
        /// зарезервированное имя папки для хранения превьюшек
        /// </summary>
        private readonly string thumbSubFolderName = "thumbs";

        /// <summary>
        /// зарезервированное имя папки для хранения файлов, информация о которых записаных в базу (т.е. требует контроля целостности/согласованности со строками в бд)
        /// </summary>
        private readonly string storageSubFolderName = "storage";

        /// <summary>
        /// Корневая папка для работы с файлами
        /// </summary>
        private string UploadsPath
        {
            get
            {
                DirectoryInfo d = new DirectoryInfo(conf.UploadsPath);
                if (!d.Exists)
                {
                    Directory.CreateDirectory(d.FullName);
                }
                return d.FullName;
            }
        }
        /// <summary>
        /// Подпапка для хранения превьюшек файлов корневой папки. Имена превьюшек совпадают с именем оригинала. Только расположены во вложеной папке
        /// </summary>
        private string UploadsThumbsPath
        {
            get
            {
                DirectoryInfo d = new DirectoryInfo(Path.Combine(UploadsPath, thumbSubFolderName));
                if (!d.Exists)
                {
                    Directory.CreateDirectory(d.FullName);
                }
                return d.FullName;
            }
        }

        /// <summary>
        /// Подпапка главной папки где храняться файлы, информация о которых записана в бд (т.е. требует контроля целостности/согласованности с БД)
        /// </summary>
        private string StoragePath
        {
            get
            {
                DirectoryInfo d = new DirectoryInfo(Path.Combine(UploadsPath, storageSubFolderName));
                if (!d.Exists)
                {
                    Directory.CreateDirectory(d.FullName);
                }
                return d.FullName;
            }
        }
        /// <summary>
        /// Папка хранения превьюшек для сохранённых/защищённых файлов
        /// </summary>
        private string StorageThumbsPath
        {
            get
            {
                DirectoryInfo d = new DirectoryInfo(Path.Combine(StoragePath, thumbSubFolderName));
                if (!d.Exists)
                {
                    Directory.CreateDirectory(d.FullName);
                }
                return d.FullName;
            }
        }
        #endregion

        public FilesController(AppDataBaseContext db_context, IOptions<AppConfig> options, ILogger<FilesController> logger)
        {
            DbContext = db_context;
            conf = options.Value;
            _logger = logger;
            if (string.IsNullOrWhiteSpace(conf.UploadsPath))
            {
                conf.UploadsPath = "uploads";
            }

            _ = UploadsPath;
            _ = UploadsThumbsPath;
            _ = StoragePath;
            _ = StorageThumbsPath;
        }

        #region private
        /// <summary>
        /// Подобрать свободное имя файлу по преданному примеру. Сначала проверяется запрашиваемое имя и если оно свободно, то возвращается по сути посланое имя.
        /// Если же имя окажется занятым, то подберётся первое свободное имя с постфиксом "запрошиваемоеИмя_ХХ.расширениеФайла", где ХХ это номер в диапазоне 1-100
        /// </summary>
        /// <param name="file_name">Запрашиваемое имя. Если оно свободно, то оно же и возвращается. Если имя занято, то подбирается похожее</param>
        /// <param name="folder_path">Расположение файла (папка)</param>
        /// <returns>Свободное имя файла, которое равно или похоже на запрашиваемое</returns>
        private string PickFileName(string file_name, string folder_path)
        {
            if (string.IsNullOrWhiteSpace(file_name) || string.IsNullOrWhiteSpace(folder_path) || !System.IO.Directory.Exists(folder_path))
            {
                _logger.LogError("Ошибка вызова подбора свободного имени файла: {0}", "string.IsNullOrWhiteSpace(file_name) || string.IsNullOrWhiteSpace(folder_path) || !System.IO.Directory.Exists(folder_path)");
                return null;
            }

            if (!System.IO.File.Exists(Path.Combine(folder_path, file_name)))
            {
                return file_name;
            }

            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file_name);
            string fileExtension = Path.GetExtension(file_name);

            int i = 1;
            while (System.IO.File.Exists(Path.Combine(folder_path, file_name)))
            {
                if (i > 100)
                {
                    file_name = fileNameWithoutExtension + "_" + Guid.NewGuid().ToString() + fileExtension;
                }
                else
                {
                    file_name = fileNameWithoutExtension + "_" + i.ToString() + fileExtension;
                    i++;
                }
            }

            return file_name;
        }

        private myFileMetadata FileVerification(string id, string path, bool reconciliationDatabase = false)
        {
            if (string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(path))
            {
                return null;
            }
            string fileExtension = string.Empty;
            myFileMetadata fileMetadata = new myFileMetadata();
            try
            {
                fileMetadata.FileInfo = new FileInfo(Path.Combine(path, id));
                fileExtension = fileMetadata.FileInfo.Extension;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка доступа к файлу: {0}", Path.Combine(path, id));
                return fileMetadata;
            }
            if (!fileMetadata.FileInfo.Exists || id.Length <= fileExtension.Length)
            {
                fileMetadata.FileInfo = null;
            }
            else
            {
                if (glob_tools.IsImageFile(id) && path != UploadsThumbsPath && path != StorageThumbsPath)
                {
                    try
                    {
                        fileMetadata.ThumbFileInfo = new FileInfo(Path.Combine(path, thumbSubFolderName, id));
                        if (!fileMetadata.ThumbFileInfo.Exists)
                        {
                            using (Stream myImageStream = UpdateImage(Image.FromFile(fileMetadata.FileInfo.FullName)))
                            {
                                using (FileStream fileStream = System.IO.File.Create(fileMetadata.ThumbFileInfo.FullName))
                                {
                                    myImageStream.Seek(0, SeekOrigin.Begin);
                                    myImageStream.CopyTo(fileStream);
                                    fileStream.Flush();
                                    fileStream.Close();
                                    fileStream.Dispose();
                                }
                                myImageStream.Flush();
                                myImageStream.Close();
                                myImageStream.Dispose();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Ошибка формирования превьюшки");
                        return fileMetadata;
                    }
                }
            }

            if (reconciliationDatabase)
            {
                id = id.Substring(0, id.Length - fileExtension.Length);
                if (!Regex.IsMatch(id, @"^\d+$"))
                {
                    _logger.LogError("в запросе указан не корректный ключ файла: {0}", id);
                    return fileMetadata;
                }
                int objId = int.Parse(id);
                if (objId < 1)
                {
                    _logger.LogError("в запросе указан не корректный ключ файла: {0}", objId);
                    return fileMetadata;
                }
                fileMetadata.Object = DbContext.FilesStorage.Find(objId);
                if (fileMetadata.Object is null)
                {
                    _logger.LogError("файл не найден по ключу в базе данных: {0}", objId);
                }
            }
            return fileMetadata;
        }

        private Stream UpdateImage(Image myImage)
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
                thumb_image.Dispose();
            }
            else
            {
                myImage.Save(stream, ImageFormat.Jpeg);
                myImage.Dispose();
            }
            return stream;
        }

        private Bitmap ResizeImage(Image image, int width, int height)
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
        #endregion

        #region ftp
        // GET: /files/ftp?PageNum=1&PageSise=10
        [HttpGet]
        public ActionResult<IEnumerable<object>> Ftp(PaginationParameters pagingParameters)
        {
            List<FileInfo> Files = new DirectoryInfo(conf.UploadsPath).GetFiles().Where(x => x.Name.Substring(0, 1) != ".").OrderBy(c => c.Name).ToList();
            pagingParameters.Init(ref Files);
            HttpContext.Response.Cookies.Append("rowsCount", pagingParameters.CountAllElements.ToString());

            return new ObjectResult(new ServerActionResult()
            {
                Success = true,
                Info = "Доступ к начальной папке успешно обработан",
                Status = StylesMessageEnum.success.ToString(),
                Tag = Files.Select(x => new { x.Name, Size = glob_tools.SizeDataAsString(x.Length) }).ToArray()
            });
        }

        [HttpGet]
        [Route("/api/files/infoftp/{id}")]
        public ActionResult InfoFtp(string id)
        {
            myFileMetadata md = FileVerification(id, UploadsPath);
            if (md.FileInfo is null)
            {
                _logger.LogError("Не удалось получить информацию по файлу: {0}", Path.Combine(UploadsPath, id));
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = "Ошибка обработки запроса",
                    Status = StylesMessageEnum.warning.ToString()
                });
            }

            string CreationTime = md.FileInfo.CreationTime.ToString(glob_tools.DateTimeFormat);
            return new ObjectResult(new ServerActionResult()
            {
                Success = true,
                Info = "Доступ к начальной папке успешно обработан",
                Status = StylesMessageEnum.success.ToString(),
                Tag = new { id = md.FileInfo.Name, md.FileInfo.Name, Size = glob_tools.SizeDataAsString(md.FileInfo.Length), CreationTime }
            });
        }

        [HttpGet]
        public ActionResult SrcFtp(bool thumb, string id)
        {
            myFileMetadata md = FileVerification(id, UploadsPath);
            if (md.FileInfo is null)
            {
                _logger.LogError("Ошибка во время получения потока файла FTP папки: {0}", Path.Combine(id, UploadsPath));
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = "Ошибка обработки запроса",
                    Status = StylesMessageEnum.warning.ToString()
                });
            }

            string file_type = MimeTypeMap.GetMimeType(md.FileInfo.Extension);

            if (thumb && !(md.ThumbFileInfo is null))
            {
                return PhysicalFile(md.ThumbFileInfo.FullName, file_type, id);
            }

            return PhysicalFile(md.FileInfo.FullName, file_type, id);
        }

        [HttpGet]
        public ActionResult MoveFileToStorage(string id)
        {
            myFileMetadata md = FileVerification(id, UploadsPath);
            if (md.FileInfo is null)
            {
                _logger.LogError("Не удалось получить информацию по файлу: {0}", Path.Combine(UploadsPath, id));
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = "Ошибка обработки запроса",
                    Status = StylesMessageEnum.warning.ToString()
                });
            }

            FileStorageModel storedFile = UploadFile(md.FileInfo.FullName, md.FileInfo.Name);

            if (storedFile is null)
            {
                _logger.LogError("Не удалось получить информацию по файлу: {0}", md.FileInfo.FullName);
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = "Ошибка обработки запроса",
                    Status = StylesMessageEnum.warning.ToString()
                });
            }

            return new ObjectResult(new ServerActionResult()
            {
                Success = true,
                Info = "Файл сохранён",
                Status = StylesMessageEnum.success.ToString(),
                Tag = new { storedFile, storedFile.Name, Size = glob_tools.SizeDataAsString(storedFile.Length) }
            });
        }

        [HttpDelete]
        [Route("/api/files/deleteftp/{id}")]
        public ActionResult DeleteFtp(string id)
        {
            //string id = ControllerContext.RouteData.Values["id"].ToString();
            myFileMetadata md = FileVerification(id, UploadsPath);
            if (md.FileInfo is null)
            {
                _logger.LogError("Не удалось прочитать файл: {0}", Path.Combine(UploadsPath, id));
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = "Ошибка чтения файла: " + id,
                    Status = StylesMessageEnum.warning.ToString()
                });
            }

            try
            {
                md.FileInfo.Delete();
                if (!(md.ThumbFileInfo is null))
                {
                    md.ThumbFileInfo.Delete();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Не удалось удалить файл(ы): {0}", Path.Combine(UploadsPath, id));
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = "Во время удаления файла произошла ошибка: " + ex.Message,
                    Status = StylesMessageEnum.warning.ToString()
                });
            }
            _logger.LogWarning("Файл удалён: {0}", md.FileInfo.FullName);
            return new ObjectResult(new ServerActionResult()
            {
                Success = true,
                Info = "Файл удалён",
                Status = StylesMessageEnum.success.ToString()
            });
        }

        [HttpPost]
        public ActionResult UploadFileToFtp(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                string file_name = PickFileName(file.FileName, UploadsPath);

                using (FileStream fileStream = new FileStream(Path.Combine(UploadsPath, file_name), FileMode.Create))
                {
                    file.CopyTo(fileStream);
                    fileStream.Flush();
                    fileStream.Close();
                    fileStream.Dispose();
                }

                _ = FileVerification(file_name, UploadsPath);
                return new ObjectResult(new ServerActionResult()
                {
                    Success = true,
                    Info = "Файл загружен: " + file_name,
                    Status = StylesMessageEnum.success.ToString(),
                    Tag = file_name
                });
            }
            return new ObjectResult(new ServerActionResult()
            {
                Success = false,
                Info = "Файл не сохранён",
                Status = StylesMessageEnum.warning.ToString()
            });
        }
        #endregion

        #region storage
        // GET: /files/storage?PageNum=1&PageSise=10
        [HttpGet]
        public ActionResult<IEnumerable<object>> Storage(PaginationParameters pagingParameters)
        {
            IQueryable<FileStorageModel> files = DbContext.FilesStorage.Include(x => x.LogAccessorRow).Where(x => x.LogAccessorRow == null).OrderBy(x => x.Id);
            pagingParameters.Init(files.Count());
            if (pagingParameters.PageNum > 1)
                files = files.Skip(pagingParameters.Skip);

            HttpContext.Response.Cookies.Append("rowsCount", pagingParameters.CountAllElements.ToString());
            //List<FileStorageModel> filesList = files.Take(pagingParameters.PageSize).ToList();
            return new ObjectResult(new ServerActionResult()
            {
                Success = true,
                Info = "Доступ к хранимой папке успешно обработан",
                Status = StylesMessageEnum.success.ToString(),
                Tag = files.Take(pagingParameters.PageSize).ToList().Where(x => System.IO.File.Exists(Path.Combine(StoragePath, x.Id.ToString() + Path.GetExtension(x.Name)))).Select(x => new { x.Id, x.Name, Size = glob_tools.SizeDataAsString(x.Length), x.isDisabled, x.Readonly }).ToArray()
            });
        }

        [HttpGet]
        [Route("/api/files/infostorage/{id}")]
        public ActionResult InfoStorage(string id)
        {
            myFileMetadata md = FileVerification(id, StoragePath, true);
            if (md.FileInfo is null)
            {
                _logger.LogError("Не удалось получить информацию по файлу: {0}", Path.Combine(UploadsPath, id));
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = "Ошибка обработки запроса",
                    Status = StylesMessageEnum.warning.ToString()
                });
            }

            id = id.Substring(0, id.Length - md.FileInfo.Extension.Length);
            int idObject = int.Parse(id);

            FileStorageModel fileStorage = DbContext.FilesStorage.Find(idObject);
            return new ObjectResult(new ServerActionResult()
            {
                Success = true,
                Info = "Доступ к начальной папке успешно обработан",
                Status = StylesMessageEnum.success.ToString(),
                Tag = new { Id = fileStorage.Id.ToString() + md.FileInfo.Extension, fileStorage.Name, Size = glob_tools.SizeDataAsString(fileStorage.Length), fileStorage.isDisabled, fileStorage.Readonly }
            });
        }

        [HttpGet]
        public ActionResult SrcStorage(bool thumb, string id)
        {
            myFileMetadata md = FileVerification(id, StoragePath);
            if (md.FileInfo is null)
            {
                _logger.LogError("Ошибка во время получения потока файла FTP папки: {0}", Path.Combine(id, UploadsPath));
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = "Ошибка обработки запроса",
                    Status = StylesMessageEnum.warning.ToString()
                });
            }

            string file_type = MimeTypeMap.GetMimeType(md.FileInfo.Extension);

            if (glob_tools.IsImageFile(id) && thumb)
            {
                string thumb_path = Path.Combine(StorageThumbsPath, id);
                return PhysicalFile(thumb_path, file_type, id);
            }

            return PhysicalFile(md.FileInfo.FullName, file_type, id);
        }

        [HttpGet]
        public ActionResult MoveFileToFtp(string id)
        {
            myFileMetadata md = FileVerification(id, StoragePath, true);
            if (md.FileInfo is null)
            {
                _logger.LogError("Не удалось получить информацию по файлу: {0}", Path.Combine(UploadsPath, id));
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = "Ошибка обработки запроса",
                    Status = StylesMessageEnum.warning.ToString()
                });
            }

            DbContext.FilesStorage.Remove(md.Object);
            DbContext.SaveChanges();

            string file_name = PickFileName(md.Object.Name, UploadsPath);

            md.FileInfo.MoveTo(Path.Combine(UploadsPath, file_name));
            if (!(md.ThumbFileInfo is null))
            {
                md.ThumbFileInfo.MoveTo(Path.Combine(UploadsThumbsPath, file_name));
            }

            return new ObjectResult(new ServerActionResult()
            {
                Success = true,
                Info = "Файл выгружен в общую папку: " + file_name,
                Status = StylesMessageEnum.success.ToString(),
                Tag = file_name
            });
        }

        [HttpDelete]
        [Route("/api/files/deletestorage/{id}")]
        public ActionResult DeleteStorage(string id)
        {
            //string id = ControllerContext.RouteData.Values["id"].ToString();
            myFileMetadata md = FileVerification(id, StoragePath, true);
            if (md.FileInfo is null)
            {
                if (!(md.Object is null))
                {
                    DbContext.FilesStorage.Remove(md.Object);
                    DbContext.SaveChangesAsync();
                }
                _logger.LogError("Не удалось прочитать файл: {0}", Path.Combine(StoragePath, id));
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = "Ошибка чтения файла: " + id,
                    Status = StylesMessageEnum.warning.ToString()
                });
            }

            DbContext.FilesStorage.Remove(md.Object);
            DbContext.SaveChangesAsync();

            try
            {
                md.FileInfo.Delete();
                if (!(md.ThumbFileInfo is null))
                {
                    md.ThumbFileInfo.Delete();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Не удалось удалить файл(ы): {0}", Path.Combine(StoragePath, id));
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = "Во время удаления файла произошла ошибка: " + ex.Message,
                    Status = StylesMessageEnum.warning.ToString()
                });
            }
            _logger.LogWarning("Файл удалён: {0}", md.FileInfo.FullName);
            return new ObjectResult(new ServerActionResult()
            {
                Success = true,
                Info = "Файл удалён",
                Status = StylesMessageEnum.success.ToString(),
                Tag = md.FileInfo.Name
            });
        }

        /// <summary>
        /// Загрузка файла в хранимую папку
        /// </summary>
        /// <param name="sourceFilePath">Путь к исходному файлу</param>
        /// <param name="shortFileNameWithExtension">Новое имя файла</param>
        /// <param name="isMoveFile">Перемещение или копирование файла</param>
        /// <returns>Id объекта базы данных</returns>
        [NonAction]
        public FileStorageModel UploadFile(string sourceFilePath, string shortFileNameWithExtension, bool isMoveFile = true)
        {
            FileInfo fileInfo = new FileInfo(sourceFilePath);
            if (!fileInfo.Exists)
            {
                return null;
            }
            FileStorageModel newStorageFile = new FileStorageModel() { Name = shortFileNameWithExtension, Information = shortFileNameWithExtension, Length = fileInfo.Length };
            DbContext.FilesStorage.Add(newStorageFile);
            DbContext.SaveChanges();
            try
            {
                if (isMoveFile)
                {
                    fileInfo.MoveTo(Path.Combine(StoragePath, newStorageFile.Id.ToString() + fileInfo.Extension));
                }
                else
                {
                    fileInfo.CopyTo(Path.Combine(StoragePath, newStorageFile.Id.ToString() + fileInfo.Extension));
                }
                return newStorageFile;
            }
            catch (IOException ex)
            {
                _logger.LogError("Ошибка доступа к диску: {0}", ex.Message);
                DbContext.FilesStorage.Remove(newStorageFile);
                DbContext.SaveChanges();

                return null;
            }
        }
        #endregion
    }
}
