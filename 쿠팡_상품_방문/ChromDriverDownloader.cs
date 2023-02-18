using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text.RegularExpressions;
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
                var zipPath = Path.Combine(_downloadPath, "chromedriver.zip");
                var driverPath = Path.Combine(_downloadPath, "LICENSE.chromedriver");
                wc.DownloadFile(url, zipPath);
                ZipFile.ExtractToDirectory(zipPath, _downloadPath);
                File.Delete(zipPath);
                File.Delete(driverPath);

                var exePath = Path.Combine(_downloadPath, "chromedriver.exe");
                File.Move(exePath, path);
            }
        }
    }
}