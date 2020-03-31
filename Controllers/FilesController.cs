using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MultiTool;
using SPADemoCRUD.Models;
using SPADemoCRUD.Models.view;

namespace SPADemoCRUD.Controllers
{
    //[Route("api/[controller]")]
    //[ApiController]
    //[Authorize(Policy = "AccessMinLevelManager")]
    public class FilesController : Controller
    {
        protected readonly string uploadsPatch;
        protected readonly AppDataBaseContext DbContext;
        private FileInfo[] getFtpFiles() => new DirectoryInfo(uploadsPatch).GetFiles();

        public FilesController(AppDataBaseContext db_context, IOptions<AppConfig> options)
        {
            DbContext = db_context;
            uploadsPatch = options.Value.UploadsPatch;
            DirectoryInfo uploadsDirInfo = new DirectoryInfo(uploadsPatch);
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
            return Files.Select(x => new { x.Name, x.Length }).ToArray();
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
    }
}
