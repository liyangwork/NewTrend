using NewTrendAPI.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace NewTrendAPI.Controllers
{
    //文件夹中按时间排序最新的文件读取  
    //public class FIleLastTimeComparer : IComparer<FileInfo>
    //{
    //    #region IComparer<FileInfo> 成员


    //    public int Compare(FileInfo x, FileInfo y)
    //    {
    //        return x.LastWriteTime.CompareTo(y.LastWriteTime);
    //    }

    //    #endregion
    //}
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            string versions = Request["versions"];
            if (!string.IsNullOrEmpty(versions))
            {
                //FileInfo[] list = new DirectoryInfo(@"D:\我的工作\测试使用\NewTrend\NewTrend\Files\").GetFiles();
                string mapPath = Server.MapPath(@"~\Files\" + versions+"\\");
                FileInfo[] list = new DirectoryInfo(mapPath).GetFiles();
                Array.Sort<FileInfo>(list, new FIleLastTimeComparer());
                string fileName = list[list.Length - 1].Name;
                long fileSize = list[list.Length - 1].Length;
                string path = mapPath + fileName;
                ////return File(path, "application/zip-x-compressed", fileName);
                System.IO.FileInfo fileInfo = new System.IO.FileInfo(path);
                if (fileInfo.Exists == true)
                {
                    const long ChunkSize = 1024;//1K 每次读取文件，只读取1K，这样可以缓解服务器的压力
                    byte[] buffer = new byte[ChunkSize];

                    Response.Clear();
                    System.IO.FileStream iStream = System.IO.File.OpenRead(path);
                    long dataLengthToRead = iStream.Length;//获取下载的文件总大小
                    Response.ContentType = "application/octet-stream";
                    Response.AddHeader("Content-Disposition", "attachment; filename=" + HttpUtility.UrlEncode(fileName));
                    CRC3 crc = new CRC3();
                    string crc16 = crc.getCrc16Code("0x010xFE");
                    Response.Write("01FE" + crc16);
                    Response.Flush();
                    Thread.Sleep(1000);
                    //int i = 1;
                    while (dataLengthToRead > 0 && Response.IsClientConnected)
                    {
                        //Response.Write("0x01"+i);
                        int lengthRead = iStream.Read(buffer, 0, Convert.ToInt32(ChunkSize));//读取的大小
                        Response.OutputStream.Write(buffer, 0, lengthRead);
                        //Response.Write(crc.getCrc16Code("0x01"+i));
                        Response.Flush();
                        dataLengthToRead = dataLengthToRead - lengthRead;
                        Thread.Sleep(1000);
                        //i++;
                    }
                    Response.Write("01FF" + crc.getCrc16Code("0x010xFF"));
                    Response.Flush();
                    Response.Close();
                }
            }
            return new EmptyResult();
        }

    }

}
