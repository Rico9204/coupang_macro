using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using Microsoft.Win32;

namespace 쿠팡_상품_방문
{
    public class ChromeDriverDownloader
    {
        private readonly string _downloadPath;

        public ChromeDriverDownloader(string downloadPath)
        {
            _downloadPath = downloadPath;
        }

        public bool Download()
        {
            var chromeVersion = GetChromeVersion();
            var driverVersion = GetChromeDriverVersion(chromeVersion);

            if (driverVersion == null)
            {
                Console.WriteLine("Could not find a matching chromedriver version.");
                return false;
            }

            var driverUrl = $"https://chromedriver.storage.googleapis.com/{driverVersion}/chromedriver_win32.zip";
            var driverPath = Path.Combine(_downloadPath, "chromedriver.exe");

            DownloadChromeDriver(driverUrl, driverPath);

            Console.WriteLine("Chromedriver downloaded to: " + driverPath);
            return true;
        }

        public string GetChromeVersion()
        {
            string version = null;
            string keyName = @"SOFTWARE\Google\Chrome\BLBeacon";
            using (var key = Registry.CurrentUser.OpenSubKey(keyName))
            {
                if (key != null)
                {
                    var value = key.GetValue("version");
                    if (value != null)
                    {
                        version = value.ToString();
                    }
                }
            }
            return version;
        }

        private string GetChromeDriverVersion(string chromeVersion)
        {
            Match majorVersionMatch = Regex.Match(chromeVersion, @"^\d+");
            int majorVersion = int.Parse(majorVersionMatch.Value);

            var wc = new WebClient();
            var releasesUrl = "https://chromedriver.storage.googleapis.com/LATEST_RELEASE_" + majorVersion;

            try
            {
                return wc.DownloadString(releasesUrl);
            }
            catch
            {
                return null;
            }
        }

        private void DownloadChromeDriver(string url, string path)
        {
            using (var wc = new WebClient())
            {

                var driverPath = Path.Combine(_downloadPath, "LICENSE.chromedriver");
                var zipPath = Path.Combine(_downloadPath, "chromedriver.zip");
                var exePath = Path.Combine(_downloadPath, "chromedriver.exe");
                var FullPath = Path.GetFullPath(exePath);
                var LatestVersion = GetChromeVersion(); //드라이버의 최신버전

                if (File.Exists(exePath) == false)  //디렉토리에 .exe파일이 존재하지 않을 경우
                {
                    wc.DownloadFile(url, zipPath);
                    ZipFile.ExtractToDirectory(zipPath, _downloadPath);
                    File.Delete(zipPath);
                    File.Delete(driverPath);
                    File.Move(FullPath, path);
                }
                else    //디렉토리에 .exe파일이 존재 할 경우
                {
                    var exeVersionInfo = FileVersionInfo.GetVersionInfo(FullPath).ToString();    //현재 디렉토리에 존재하는 .exe 파일 버전
                    if (exeVersionInfo.Equals(LatestVersion) == false)  //최신버전과 현재 다운되어있는 파일의 버전이 다를 경우
                    {
                        File.Delete(FullPath);   //기존에있는 exe파일 삭제 후
                        this.Download();    //다시 다운로드
                    }
                }
            }
        }
    }
}

