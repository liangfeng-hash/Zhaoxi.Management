using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System.IO;
using System.Net;
using Zhaoxi.Manage.BusinessInterface;
using Zhaoxi.Manage.BusinessService;
using Zhaoxi.Manage.Common;
using Zhaoxi.Manage.Common.EnumEntity;
using Zhaoxi.Manage.Common.ModelDTO;
using Zhaoxi.Manage.MentApi.Utility.Filters;
using Zhaoxi.Manage.MentApi.Utility.InitDatabaseExt;
using Zhaoxi.Manage.MentApi.Utility.SwaggerExt;
using Zhaoxi.Manage.Models.Entity;



namespace Zhaoxi.Manage.MentApi.Controllers.SystemApi
{
    /// <summary>
    /// 文件上传操作
    /// </summary>
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = false, GroupName = nameof(ApiVersions.V1))]
    [Route("api/[controller]")]
    public class FileController : ControllerBase
    {
        private readonly ILogger<FileController> _logger;


        /// <summary>
        /// 【构造函数】
        /// </summary>
        /// <param name="logger"></param>
        public FileController(ILogger<FileController> logger)
        {
            _logger = logger;
        }


        /// <summary>
        /// 文件上传Api
        /// </summary>
        /// <param name="file"></param>
        [HttpPost]
        public JsonResult UploadFiles([FromForm] IFormFile file)
        {
            string suffix = string.Empty;
            #region 获取文件后缀 
            string filename = file.FileName.Trim();
            int index = filename.LastIndexOf(".");
            if (index > 0 && index < filename.Length - 1)
            {
                suffix = filename.Substring(index + 1);
            }
            #endregion

            #region 重新命名保存文件的名字 
            string saveDirectory = $"FileUpload\\{DateTime.Now.ToString("yyyy-MM-dd")}";
            string allSavePath = $"{Directory.GetCurrentDirectory()}\\{saveDirectory}";
            if (Directory.Exists(allSavePath) == false)
            {
                Directory.CreateDirectory(allSavePath);
            }

            //保存的新文件名
            string newFileName = $"{DateTime.Now.ToString("yyyyMMddHHmmss")}_{Guid.NewGuid().ToString()}.{suffix}";

            //保存的文件名
            string allSaveFilePath = $"{allSavePath}\\{newFileName}";
            #endregion
            try
            {
                using (var stream = System.IO.File.Create(allSaveFilePath))
                {
                    file.CopyToAsync(stream);
                }

                return new JsonResult(new ApiDataResult<string>()
                {
                    Success = true,
                    Message = "文件上传成功",
                    Data = $"{saveDirectory}\\{newFileName}"
                });
            }
            catch (Exception)
            {

                return new JsonResult(new ApiDataResult<string>()
                {
                    Success = false,
                    Message = "文件上传失败了"
                });
            }
        }



        //https://blog.csdn.net/liulv_yan/article/details/121148484



        private static bool StatusUpLoad(string filename, string path, string ftppath)
        {
            bool bol = false;
            string url = "172.28.35.31";
            string _newpath = "";
            FileInfo fileInf = new FileInfo(path);
            _newpath = ftppath + "/" + filename;
            if (_newpath != "")
            {
                url = _newpath + "/" + fileInf.Name;
            }
            else
            {
                new Exception();
            }
            FtpWebRequest reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(url));
            reqFTP.Credentials = new NetworkCredential("Administrator", "Ab1988828");
            reqFTP.KeepAlive = false;
            reqFTP.Method = WebRequestMethods.Ftp.UploadFile;
            reqFTP.UseBinary = true;
            reqFTP.ContentLength = fileInf.Length;
            int buffLength = 2048;
            byte[] buff = new byte[buffLength];
            int contentLen;
            var fs = fileInf.OpenRead();
            try
            {
                var strm = reqFTP.GetRequestStream();
                contentLen = fs.Read(buff, 0, buffLength);
                while (contentLen != 0)
                {
                    strm.Write(buff, 0, contentLen);
                    contentLen = fs.Read(buff, 0, buffLength);
                }
                strm.Close();
                fs.Close();
                bol = true;
            }
            catch (Exception ex)
            {
                bol = false;
            }
            return bol;
        }
    }
}