using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace FontAnZhuang
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("字体安装");
                #region 字体不注册直接添加内存中使用
                System.Drawing.Text.PrivateFontCollection privateFonts = new System.Drawing.Text.PrivateFontCollection();
                privateFonts.AddFontFile($@"{AppDomain.CurrentDomain.BaseDirectory}\CFonts\STSONG.ttf");
                System.Drawing.Font font = new Font(privateFonts.Families[0], 12);
                #endregion

                //注册
                DirectoryInfo folder = new DirectoryInfo($@"{AppDomain.CurrentDomain.BaseDirectory}\CFonts");
                foreach (FileInfo file in folder.GetFiles())
                {
                    Console.WriteLine(file.FullName);
                    Console.WriteLine($"安装字体结果：{FontOperate.InstallFont(file.FullName, file.Name)}");
                }

                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }

    public class FontOperate
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern int WriteProfileString(string lpszSection, string lpszKeyName, string lpszString);

        [DllImport("user32.dll")]
        public static extern int SendMessage(int hWnd,// handle to destination window 
        uint Msg,   // message 
        int wParam, // first message parameter 
        int lParam  // second message parameter 
        );
        [DllImport("gdi32")]
        public static extern int AddFontResource(string lpFileName);


        public static bool InstallFont(string sFontFileName, string sFontName)
        {
            string _sTargetFontPath = string.Format(@"{0}\fonts\{1}", System.Environment.GetEnvironmentVariable("WINDIR"), sFontName);//系统FONT目录
            string _sResourceFontPath = sFontFileName; //string.Format(@"{0}\Font\{1}", System.Windows.Forms.Application.StartupPath, sFontFileName);//需要安装的FONT目录

            int Res;
            const int WM_FONTCHANGE = 0x001D;
            const int HWND_BROADCAST = 0xffff;

            try
            {
                if (!File.Exists(_sTargetFontPath) && File.Exists(_sResourceFontPath))
                {
                    int _nRet;
                    File.Copy(_sResourceFontPath, _sTargetFontPath);
                    _nRet = AddFontResource(_sTargetFontPath);
                    Res = SendMessage(HWND_BROADCAST, WM_FONTCHANGE, 0, 0);
                    sFontName = sFontName.Remove(sFontName.Length - 4, 4);
                    _nRet = WriteProfileString("fonts", sFontName + "(TrueType)", sFontFileName);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            return true;
        }
    }
}
