using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace NewTrendAPI.Controllers
{
    //文件夹中按时间排序最新的文件读取  
    public class FIleLastTimeComparer : IComparer<FileInfo>
    {
        #region IComparer<FileInfo> 成员


        public int Compare(FileInfo x, FileInfo y)
        {
            return x.LastWriteTime.CompareTo(y.LastWriteTime);
        }

        #endregion
    }
    [RoutePrefix("download")]
    public class DownloadController : ApiController
    {
        [Route("get_demo_file")]
        public HttpResponseMessage GetFileFromWebApi()
        {
            try
            {
                FileInfo[] list = new DirectoryInfo(@"D:\我的工作\测试使用\NewTrend\NewTrend\Files\").GetFiles();
                Array.Sort<FileInfo>(list, new FIleLastTimeComparer());
                string fileName = list[list.Length - 1].Name;
                long fileSize = list[list.Length - 1].Length;
                string path = @"D:\我的工作\测试使用\NewTrend\NewTrend\Files\" + fileName;
                var stream = new FileStream(path, FileMode.Open);
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new StreamContent(stream);
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileName = "Wep Api Demo File.zip"
                };
                return response;
            }
            catch
            {
                return new HttpResponseMessage(HttpStatusCode.NoContent);
            }
        }
    }
}
