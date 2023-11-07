using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using Keys = OpenQA.Selenium.Keys;

namespace 쿠팡_상품_방문
{
    public class Form1 : Form
    {
        public delegate void LiatUpdateDelegate_Log(string Ask);

        private bool work_State = false;

        private Thread work_Thread;

        private ChromeDriver driver;

        private Random rnd = new Random();

        private IContainer components = null;

        private DataGridView 작업데이터;

        private ListView 기록리스트;

        private Button 계정저장버튼;

        private DataGridView 계정데이터;

        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;

        private DataGridViewTextBoxColumn Column5;

        private DataGridViewTextBoxColumn Column6;

        private Button 계정불러오기버튼;

        private Button 저장버튼;

        private Button 불러오기버튼;

        private Button 시작버튼;

        private Button 일시정지버튼;

        private ColumnHeader columnHeader1;

        private ColumnHeader columnHeader2;

        private DataGridViewTextBoxColumn Column1;

        private DataGridViewTextBoxColumn Column2;

        private DataGridViewTextBoxColumn Column3;

        [DllImport("user32.dll")]
        public static extern int FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int FindWindowEx(int parentHandle, int childAfter, string lclassName, string windowTitle);

        [DllImport("User32.dll")]
        public static extern int SendMessage(int hWnd, int uMsg, int wParam, int lParam);

        double lateNum = 1;

        public Form1()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;
        }

        private void 계정데이터_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            for (int i = 0; i < 계정데이터.Rows.Count; i++)
            {
                계정데이터.Rows[i].Cells[0].Value = i + 1;
            }
        }

        private void 계정데이터_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            for (int i = 0; i < 계정데이터.Rows.Count; i++)
            {
                계정데이터.Rows[i].Cells[0].Value = i + 1;
            }
        }

        private void 작업데이터_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            for (int i = 0; i < 작업데이터.Rows.Count; i++)
            {
                작업데이터.Rows[i].Cells[0].Value = i + 1;
            }
        }

        private void 작업데이터_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            for (int i = 0; i < 작업데이터.Rows.Count; i++)
            {
                작업데이터.Rows[i].Cells[0].Value = i + 1;
            }
        }

        private void 계정저장버튼_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = "계정.txt";
            saveFileDialog.InitialDirectory = Application.StartupPath;
            saveFileDialog.Filter = "텍스트 파일 (*.txt)|*.txt|모든 파일 (*.*)|*.*";
            if (saveFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            if (File.Exists(saveFileDialog.FileName))
            {
                File.Delete(saveFileDialog.FileName);
            }
            FileStream fileStream = new FileStream(saveFileDialog.FileName, FileMode.Append, FileAccess.Write);
            StreamWriter streamWriter = new StreamWriter(fileStream, Encoding.Default);
            for (int i = 0; i < 계정데이터.Rows.Count - 1; i++)
            {
                if (계정데이터.Rows[i].Cells[1].Value.ToString() != "" && 계정데이터.Rows[i].Cells[2].Value.ToString() != "")
                {
                    streamWriter.WriteLine(계정데이터.Rows[i].Cells[1].Value.ToString() + "\t" + 계정데이터.Rows[i].Cells[2].Value.ToString());
                }
            }
            streamWriter.Close();
            fileStream.Close();
        }

        private void 계정불러오기버튼_Click(object sender, EventArgs e)
        {
            계정데이터.Rows.Clear();
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Application.StartupPath;
            openFileDialog.FileName = "계정.txt";
            openFileDialog.Filter = "텍스트 파일 (*.txt)|*.txt|모든 파일 (*.*)|*.*";
            if (openFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            string[] array = File.ReadAllLines(openFileDialog.FileName, Encoding.Default);
            for (int i = 0; i < array.Count(); i++)
            {
                if (array[i] != "")
                {
                    계정데이터.Rows.Add("", array[i].Split(new string[1] { "\t" }, StringSplitOptions.None)[0], array[i].Split(new string[1] { "\t" }, StringSplitOptions.None)[1]);
                }
            }
        }

        private void 저장버튼_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = "상품정보.txt";
            saveFileDialog.InitialDirectory = Application.StartupPath;
            saveFileDialog.Filter = "텍스트 파일 (*.txt)|*.txt|모든 파일 (*.*)|*.*";
            if (saveFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            if (File.Exists(saveFileDialog.FileName))
            {
                File.Delete(saveFileDialog.FileName);
            }
            FileStream fileStream = new FileStream(saveFileDialog.FileName, FileMode.Append, FileAccess.Write);
            StreamWriter streamWriter = new StreamWriter(fileStream, Encoding.Default);
            for (int i = 0; i < 작업데이터.Rows.Count - 1; i++)
            {
                if (작업데이터.Rows[i].Cells[1].Value.ToString() != "" && 작업데이터.Rows[i].Cells[2].Value.ToString() != "")
                {
                    streamWriter.WriteLine(작업데이터.Rows[i].Cells[1].Value.ToString() + "\t" + 작업데이터.Rows[i].Cells[2].Value.ToString());
                }
            }
            streamWriter.Close();
            fileStream.Close();
        }

        private void 불러오기버튼_Click(object sender, EventArgs e)
        {
            작업데이터.Rows.Clear();
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = Application.StartupPath;
            openFileDialog.FileName = "상품정보.txt";
            openFileDialog.Filter = "텍스트 파일 (*.txt)|*.txt|모든 파일 (*.*)|*.*";
            if (openFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            string[] array = File.ReadAllLines(openFileDialog.FileName, Encoding.Default);
            for (int i = 0; i < array.Count(); i++)
            {
                if (array[i] != "")
                {
                    작업데이터.Rows.Add("", array[i].Split(new string[1] { "\t" }, StringSplitOptions.None)[0], array[i].Split(new string[1] { "\t" }, StringSplitOptions.None)[1]);
                }
            }
        }

        private void Log(string ask)
        {
            ListViewItem listViewItem = new ListViewItem();
            listViewItem.Text = "[ " + DateTime.Now.Hour + "시 " + DateTime.Now.Minute + "분 " + DateTime.Now.Second + "초 ]";
            ListViewItem listViewItem2 = listViewItem;
            listViewItem2.SubItems.Add(ask);
            기록리스트.Items.Add(listViewItem2);
            기록리스트.EnsureVisible(기록리스트.Items.Count - 1);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Hide();
            try
            {
                work_Thread.Abort();
            }
            catch
            {
            }
            try
            {
                driver.Quit();
            }
            catch
            {
            }
            Process.GetCurrentProcess().Kill();
        }

        private void 시작버튼_Click(object sender, EventArgs e)
        {
            시작버튼.Enabled = false;
            일시정지버튼.Enabled = true;
            if (work_State)
            {
                work_Thread.Resume();
                return;
            }
            work_Thread = new Thread(Web_Thread);
            work_Thread.Start();
        }

        private void 일시정지버튼_Click(object sender, EventArgs e)
        {
            시작버튼.Enabled = true;
            일시정지버튼.Enabled = false;
            work_Thread.Suspend();
            work_State = true;
        }

        private void End_Scroll(ChromeDriver Driver)
        {
            DateTime value = DateTime.Now.AddSeconds(3.0);
            int num = Convert.ToInt32(Driver.ExecuteScript("return window.pageYOffset"));
            while (true)
            {
                Driver.ExecuteScript("window.scrollBy(0, " + new Random().Next(1, 10) + ")");
                int num2 = Convert.ToInt32(Driver.ExecuteScript("return window.pageYOffset"));
                if (num == num2)
                {
                    if (DateTime.Now.CompareTo(value) > 0)
                    {
                        break;
                    }
                }
                else
                {
                    num = num2;
                    value = DateTime.Now.AddSeconds(3.0);
                }
            }
        }

        string id = "";
        string pw = "";


        private void Web_Thread()
        {
            LiatUpdateDelegate_Log method = Log;
            //var chromDriverDownloader = new ChromeDriverDownloader("./");
            //chromDriverDownloader.Download();
            Invoke(method, "작업을 시작합니다.");
            while (true)
            {
                for (int i = 0; i < 계정데이터.Rows.Count - 1; i++)
                {
                    var currentAccount = 계정데이터.Rows[i];
                    Invoke(method, i + 1 + "번 계정으로 작업합니다.");
                    Process_Clear();
                    //Invoke(method, "아이피를 변경합니다.");
                    //IP_Change();
                    Thread.Sleep((int)(5000 * lateNum));
                    Invoke(method, "크롬을 생성합니다.");
                    Create_Chrome();
                    try
                    {
                        Invoke(method, "로그인을 시도합니다.");
                        id = currentAccount.Cells[1].Value.ToString();
                        Thread.Sleep(1000);
                        pw = currentAccount.Cells[2].Value.ToString();
                        Thread.Sleep(1000);
                        if (Login(id, pw) == false)
                        {
                            throw new Exception("Login failed");
                        }
                    }
                    catch(Exception e)
                    {                 
                        Invoke(method, e.ToString());
                        continue;
                    }
                    
                    for (int j = 0; j < 작업데이터.Rows.Count - 1; j++)
                    {
                        var currentRow = 작업데이터.Rows[j];
                        Invoke(method, j + 1 + "번 상품을 찾습니다.");
                        try
                        {
                            if (driver.Url != "https://www.coupang.com/")
                            {
                                driver.Navigate().GoToUrl("https://www.coupang.com/");
                                //driver.Manage().Cookies.DeleteAllCookies();
                                //driver.Navigate().Refresh();
                            }
                            if (currentRow.Cells[1].Value.ToString().Contains("검색:"))
                            {
                                driver.FindElement(By.CssSelector("[id='headerSearchKeyword']")).Click();
                                Thread.Sleep((int)(1000 * lateNum));
                                driver.ExecuteScript("arguments[0].value=arguments[1]", driver.FindElement(By.CssSelector("[id='headerSearchKeyword']")), currentRow.Cells[1].Value.ToString().Split(new string[1] { "검색:" }, StringSplitOptions.None)[1]);
                                Thread.Sleep((int)(1000 * lateNum));
                                driver.FindElement(By.CssSelector("[id='headerSearchBtn']")).Click();
                                Thread.Sleep((int)(3000 * lateNum));
                                End_Scroll(driver);
                                for (int pageNum = 0; pageNum < 15; pageNum++) //15페이지까지 물건을 찾고 찜, 장바구니 담기 하는 반복문
                                {
                                    if (driver.FindElements(By.CssSelector($"[id='productList'] [data-product-id='{currentRow.Cells[2].Value.ToString()}']")).Count > 0)	//제품을 쿠팡 화면에서 찾는 구문
                                    {
                                        Invoke(method, "상품을 찾았습니다.");
                                        driver.ExecuteScript("arguments[0].click()", driver.FindElement(By.CssSelector("[id='productList'] [data-product-id='" + currentRow.Cells[2].Value.ToString() + "'] [class='name']")));
                                        Thread.Sleep((int)(3000 * lateNum));
                                        driver.Close();
                                        driver.SwitchTo().Window(driver.WindowHandles.Last());
                                        if (driver.FindElements(By.CssSelector("button[class='prod-favorite-btn ']")).Count > 0)
                                        {
                                            Invoke(method, "찜 클릭");
                                            driver.FindElement(By.CssSelector("button[class='prod-favorite-btn ']")).Click();
                                            Thread.Sleep((int)(5000 * lateNum));	
                                        }
                                        if (driver.FindElements(By.CssSelector("button[class='prod-cart-btn']")).Count > 0)
                                        {
                                            Invoke(method, "장바구니 담기 클릭");
                                            driver.FindElement(By.CssSelector("button[class='prod-cart-btn']")).Click();
                                            Thread.Sleep((int)(5000 * lateNum));
                                        }
                                        if (driver.FindElements(By.CssSelector("#btfTab > ul.tab-titles > li:nth-child(2)")).Count > 0)
                                        {
                                            Invoke(method, "리뷰 클릭");
                                            try {
                                                for (int p = 0; p < 3; p++)
                                                {
                                                    Actions actions = new Actions(driver);
                                                    actions.SendKeys(OpenQA.Selenium.Keys.PageDown).Perform();
                                                    Thread.Sleep((int)(1000 * lateNum));
                                                }
                                                driver.FindElement(By.CssSelector("#btfTab > ul.tab-titles > li:nth-child(2)")).Click();
                                                Thread.Sleep(500);
                                                for (int p = 0; p < 2; p++)
                                                {
                                                    Actions actions = new Actions(driver);
                                                    actions.SendKeys(OpenQA.Selenium.Keys.PageDown).Perform();
                                                    Thread.Sleep(2000);
                                                }
                                                Thread.Sleep(6000);
                                            }
                                            catch(OpenQA.Selenium.ElementClickInterceptedException) {
                                                Invoke(method, "리뷰 클릭 오류");
                                                Thread.Sleep(3000);
                                                break;
                                            }
 
                                        }

                                        break; 
                                    }
                                    //베스트셀러 상품 찾기
                                    if (driver.FindElements(By.CssSelector($"[id='productList'] [class*='search-product best-seller-carousel-item'] [data-product-id='{currentRow.Cells[2].Value.ToString()}']")).Count > 0)	//제품을 쿠팡 화면에서 찾는 구문
                                    {
                                        Invoke(method, "상품을 찾았습니다.");
                                        driver.ExecuteScript("arguments[0].click()", driver.FindElement(By.CssSelector("[id='productList'] [class*='search-product best-seller-carousel-item'] [data-product-id='" + currentRow.Cells[2].Value.ToString() + "'] [class='name']")));
                                        Thread.Sleep((int)(3000 * lateNum));
                                        driver.Close();
                                        driver.SwitchTo().Window(driver.WindowHandles.Last());
                                        if (driver.FindElements(By.CssSelector("button[class='prod-favorite-btn ']")).Count > 0)
                                        {
                                            Invoke(method, "찜 클릭");
                                            driver.FindElement(By.CssSelector("button[class='prod-favorite-btn ']")).Click();
                                            Thread.Sleep((int)(5000 * lateNum));
                                        }
                                        if (driver.FindElements(By.CssSelector("button[class='prod-cart-btn']")).Count > 0)
                                        {
                                            Invoke(method, "장바구니 담기 클릭");
                                            driver.FindElement(By.CssSelector("button[class='prod-cart-btn']")).Click();
                                            Thread.Sleep((int)(5000 * lateNum));
                                        }
                                        // TODO(성환): 장바구니 담기 후 페이지를 휠로 내리고 댓글 2~3번째 페이지까지 누르도록 

                                        if (driver.FindElements(By.CssSelector("#btfTab > ul.tab-titles > li.active")).Count > 0)
                                        {
                                            Invoke(method, "리뷰 클릭");
                                            try
                                            {
                                                for (int p = 0; p < 3; p++)
                                                {
                                                    Actions actions = new Actions(driver);
                                                    actions.SendKeys(OpenQA.Selenium.Keys.PageDown).Perform();
                                                    Thread.Sleep((int)(1000 * lateNum));
                                                }
                                                driver.FindElement(By.CssSelector("#btfTab > ul.tab-titles > li:nth-child(2)")).Click();
                                                Thread.Sleep(500);
                                                for (int p = 0; p < 2; p++)
                                                {
                                                    Actions actions = new Actions(driver);
                                                    actions.SendKeys(OpenQA.Selenium.Keys.PageDown).Perform();
                                                    Thread.Sleep(2000);
                                                }
                                                Thread.Sleep(6000);
                                            }
                                            catch (OpenQA.Selenium.ElementClickInterceptedException)
                                            {
                                                Invoke(method, "리뷰 클릭 오류");
                                                Thread.Sleep(3000);
                                                break;
                                            }

                                        }

                                        break;
                                    }
                                    Invoke(method, "다음 페이지로 이동합니다.");
                                    driver.ExecuteScript("arguments[0].click()", driver.FindElement(By.CssSelector("[class='btn-next']")));
                                    Thread.Sleep((int)(2000 * lateNum));
                                    End_Scroll(driver);
                                }
                            } 
                            else // TODO(성환): 어떤 조건에서 실행시키는지 잘 모르겠음 일단 대충봐서는 등록된 계정이 없는 경우인듯 함
                            {
                                new Actions(driver).MoveToElement(driver.FindElement(By.CssSelector("[class^='category-btn']"))).Perform();
                                if (currentRow.Cells[1].Value.ToString().Split(new string[1] { ";" }, StringSplitOptions.None).Count() == 2)
                                {
                                    new Actions(driver).MoveToElement(driver.FindElement(By.CssSelector("[class='" + currentRow.Cells[1].Value.ToString().Split(new string[1] { ";" }, StringSplitOptions.None)[0] + "']"))).Perform();
                                    driver.FindElement(By.CssSelector("[href='/np/categories/" + currentRow.Cells[1].Value.ToString().Split(new string[1] { ";" }, StringSplitOptions.None)[1] + "']")).Click();
                                }
                                else if (currentRow.Cells[1].Value.ToString().Split(new string[1] { ";" }, StringSplitOptions.None).Count() == 3)
                                {
                                    new Actions(driver).MoveToElement(driver.FindElement(By.CssSelector("[class='" + currentRow.Cells[1].Value.ToString().Split(new string[1] { ";" }, StringSplitOptions.None)[0] + "']"))).Perform();
                                    new Actions(driver).MoveToElement(driver.FindElement(By.CssSelector("[href='/np/categories/" + currentRow.Cells[1].Value.ToString().Split(new string[1] { ";" }, StringSplitOptions.None)[1] + "']"))).Perform();
                                    driver.FindElement(By.CssSelector("[href='/np/categories/" + currentRow.Cells[1].Value.ToString().Split(new string[1] { ";" }, StringSplitOptions.None)[2] + "']")).Click();
                                }
                                else if (currentRow.Cells[1].Value.ToString().Split(new string[1] { ";" }, StringSplitOptions.None).Count() == 4)
                                {
                                    new Actions(driver).MoveToElement(driver.FindElement(By.CssSelector("[class='" + currentRow.Cells[1].Value.ToString().Split(new string[1] { ";" }, StringSplitOptions.None)[0] + "']"))).Perform();
                                    new Actions(driver).MoveToElement(driver.FindElement(By.CssSelector("[href='/np/categories/" + currentRow.Cells[1].Value.ToString().Split(new string[1] { ";" }, StringSplitOptions.None)[1] + "']"))).Perform();
                                    driver.FindElement(By.CssSelector("[href='/np/categories/" + currentRow.Cells[1].Value.ToString().Split(new string[1] { ";" }, StringSplitOptions.None)[2] + "']")).Click();
                                    Thread.Sleep((int)(1000 * lateNum));
                                    driver.FindElement(By.CssSelector("label[for='component" + currentRow.Cells[1].Value.ToString().Split(new string[1] { ";" }, StringSplitOptions.None).Last() + "']")).Click();
                                }
                                Thread.Sleep((int)(1000 * lateNum));
                                driver.ExecuteScript("window.scrollBy(0, 1000)");
                                for (int l = 0; l < 15; l++)
                                {
                                    if (driver.FindElements(By.CssSelector("[id='productList'] [data-product-id='" + currentRow.Cells[2].Value.ToString() + "']")).Count > 0)
                                    {
                                        Invoke(method, "상품을 찾았습니다.");
                                        driver.FindElement(By.CssSelector("[id='productList'] [data-product-id='" + currentRow.Cells[2].Value.ToString() + "']")).Click();
                                        Thread.Sleep((int)(10000 * lateNum));
                                        driver.Close();
                                        driver.SwitchTo().Window(driver.WindowHandles.Last());
                                        if (driver.FindElements(By.CssSelector("button[class='prod-favorite-btn ']")).Count > 0)
                                        {
                                            Invoke(method, "찜 클릭");
                                            driver.FindElement(By.CssSelector("button[class='prod-favorite-btn ']")).Click();
                                            Thread.Sleep((int)(1000 * lateNum));
                                        }
                                        if (driver.FindElements(By.CssSelector("button[class='prod-cart-btn']")).Count > 0)
                                        {
                                            Invoke(method, "장바구니 담기 클릭");
                                            driver.FindElement(By.CssSelector("button[class='prod-cart-btn']")).Click();
                                            Thread.Sleep((int)(1000 * lateNum));
                                        }
                                        if (driver.FindElements(By.CssSelector("#btfTab > ul.tab-titles > li.active")).Count > 0)
                                        {
                                            Invoke(method, "리뷰 클릭");
                                            driver.FindElement(By.CssSelector("#btfTab > ul.tab-titles > li:nth-child(2)")).Click();
                                            Thread.Sleep((int)(3000 * lateNum));

                                            Actions actions = new Actions(driver);
                                            for (int p = 0; p < 2; p++)
                                            {
                                                actions.SendKeys(Keys.PageDown).Build().Perform();
                                                Thread.Sleep((int)(2000 * lateNum));
                                            }

                                            driver.FindElement(By.CssSelector("#btfTab > ul.tab-contents > li.product-review.tab-contents__content > div > div.sdp-review__article.js_reviewArticleContainer > section.js_reviewArticleListContainer > div.sdp-review__article__page.js_reviewArticlePagingContainer > button:nth-child(2)")).Click();
                                            Thread.Sleep((int)(3000 * lateNum));

                                            for (int p = 0; p < 2; p++)
                                            {
                                                actions.SendKeys(Keys.PageDown).Build().Perform();
                                                Thread.Sleep((int)(2000 * lateNum));
                                            }
                                        }
                                        break;
                                    }
                                    for (int m = 0; m < driver.FindElements(By.CssSelector("[class='page-warpper'] a")).Count; m++)
                                    {
                                        try
                                        {
                                            if (driver.FindElements(By.CssSelector("[class='page-warpper'] a"))[m].GetAttribute("class") == "selected")
                                            {
                                                Invoke(method, "다음 페이지로 이동합니다.");
                                                driver.ExecuteScript("arguments[0].click()", driver.FindElements(By.CssSelector("[class='page-warpper'] a"))[m + 1]);
                                                break;
                                            }
                                        }
                                        catch (OpenQA.Selenium.NoSuchWindowException)
                                        {
                                            Invoke(method, "웹 페이지 비정상 종료");
                                            Thread.Sleep(3000);
                                            break;
                                        }
                                    }
                                    Thread.Sleep((int)(2000 * lateNum));
                                }
                            }
                        }
                        catch (OpenQA.Selenium.NoSuchWindowException)
                        {
                            Invoke(method, "웹 페이지 비정상 종료");
                            Thread.Sleep(3000);
                            break;
                            //이떄구나 시발롬
                        }
                        while (driver.WindowHandles.Count != 1)
                        {
                            driver.SwitchTo().Window(driver.WindowHandles[1]);
                            driver.Close();
                            driver.SwitchTo().Window(driver.WindowHandles[0]);
                            Thread.Sleep(3000);
                        }
                    }
                    Invoke(method, "크롬을 종료합니다.");
                    Quit_Chrome();
                    Thread.Sleep((int)(5000 * lateNum));
                }
            }
        }

        private void Process_Clear()
        {
            for (int i = 0; i < 1000; i++)
            {
                int num = 0;
                Process[] processes = Process.GetProcesses();
                for (int j = 0; j < processes.Count(); j++)
                {
                    if (processes[j].ProcessName == "chrome")
                    {
                        try
                        {
                            num++;
                            processes[j].Kill();
                        }
                        catch
                        {
                        }
                    }
                    else if (processes[j].ProcessName == "chromedriver")
                    {
                        try
                        {
                            num++;
                            processes[j].Kill();
                        }
                        catch
                        {
                        }
                    }
                }
                if (num == 0)
                {
                    break;
                }
            }
        }

        private void Create_Chrome()
        {
            LiatUpdateDelegate_Log method = Log;
            try
            {
                ChromeDriverService chromeDriverService = ChromeDriverService.CreateDefaultService();
                chromeDriverService.HideCommandPromptWindow = true;
                ChromeOptions chromeOptions = new ChromeOptions();
                chromeOptions.AddArgument("--window-position=0,0");
                chromeOptions.AddArgument("--window-size=1280,900");
                chromeOptions.AddExcludedArgument("enable-automation");
                chromeOptions.AddArguments("--disable-extensions", "--disable-infobars", "--disable-blink-features=AutomationControlled");
                chromeOptions.AddExcludedArgument("enable-automation");
                //chromeOptions.AddArgument("--incognito");
                chromeOptions.AddArgument("--user-data-dir=C:\\Users\\" + Environment.UserName + "\\AppData\\Local\\Google\\Chrome\\User Data");
                driver = new ChromeDriver(chromeDriverService, chromeOptions);
            }
            catch (Exception ex)
            {
                Invoke(method, ex.ToString());
            }
        }

        private void Quit_Chrome()
        {
            try
            {
                driver.Quit();
            }
            catch
            {
            }
        }

        private bool Login(string id, string pw)
        {
            bool result = false;
            driver.Navigate().GoToUrl("https://coupang.com");
            //Thread.Sleep(1000);
            //driver.Manage().Cookies.DeleteAllCookies();
            Thread.Sleep(1000);
            driver.Navigate().GoToUrl("https://login.coupang.com/login/login.pang");
            Thread.Sleep(1000);
            //driver.Manage().Cookies.DeleteAllCookies();
            //Thread.Sleep(1000);
            if (driver.FindElements(By.CssSelector("[id='logout']")).Count > 0)
            {
                driver.Navigate().GoToUrl("https://login.coupang.com/login/logout.pang");
                Thread.Sleep(1000);
                driver.Navigate().GoToUrl("https://login.coupang.com/login/login.pang");
                Thread.Sleep(1000);
            }
            if (driver.FindElements(By.CssSelector("[id='a.password.active")).Count > 0)
            {
                driver.FindElement(By.CssSelector("button[class='password active']")).Click();
                Thread.Sleep(1000);
            }
            driver.FindElement(By.CssSelector("[id='login-email-input']")).SendKeys(id);
            Thread.Sleep(1000);
            driver.FindElement(By.CssSelector("[id='login-password-input']")).SendKeys(pw);
            Thread.Sleep(3000);
            driver.FindElement(By.CssSelector("[id='login-password-input']")).SendKeys(OpenQA.Selenium.Keys.Enter);
            Thread.Sleep(1000);
            if (driver.Url != "https://www.coupang.com/")
            {
                driver.Navigate().GoToUrl("http://www.coupang.com/");
                Thread.Sleep(5000);
            }
            if (driver.FindElements(By.CssSelector("[id='myCoupang']")).Count > 0)
            {
                result = true;
            }
            return result;
        }

        private void History_Delete()
        {
            LiatUpdateDelegate_Log method = Log;
            try
            {
                Invoke(method, "기록 삭제 페이지로 이동합니다.");
                driver.Navigate().GoToUrl("chrome://settings/clearBrowserData");
                for (int i = 0; i < 100; i++)
                {
                    int num = 0;
                    try
                    {
                        num = Convert.ToInt32(driver.ExecuteScript("return document.querySelector('body > settings-ui').shadowRoot.querySelector('#main').shadowRoot.querySelector('settings-basic-page').shadowRoot.querySelector('#basicPage > settings-section:nth-child(8) > settings-privacy-page').shadowRoot.querySelector('settings-clear-browsing-data-dialog').shadowRoot.querySelectorAll('#clearBrowsingDataConfirm').length;"));
                    }
                    catch
                    {
                    }
                    if (num > 0)
                    {
                        Thread.Sleep(1000);
                        break;
                    }
                }
                Thread.Sleep(1000);
                driver.ExecuteScript("document.querySelector('body > settings-ui').shadowRoot.querySelector('#main').shadowRoot.querySelector('settings-basic-page').shadowRoot.querySelector('#basicPage > settings-section:nth-child(8) > settings-privacy-page').shadowRoot.querySelector('settings-clear-browsing-data-dialog').shadowRoot.querySelector('#clearBrowsingDataConfirm').click();");
                for (int j = 0; j < 100; j++)
                {
                    if (driver.Url == "chrome://settings/")
                    {
                        break;
                    }
                    Thread.Sleep(1000);
                }
                Thread.Sleep(1000);
            }
            catch
            {
            }
            Thread.Sleep(3000);
        }

        private void HaiVPN()
        {
            try
            {
                Process process = Process.GetProcessesByName("CoolipClient")[0];
                int parentHandle = FindWindow(null, process.MainWindowTitle);
                int hWnd = FindWindowEx(parentHandle, 0, "Button", null);
                SendMessage(hWnd, 245, 0, 1);
            }
            catch
            {
            }
        }

        private void IP_Change()
        {
            LiatUpdateDelegate_Log method = Log;
            while (true)
            {
                Invoke(method, "IP 변경을 시도합니다.");
                string text = "";
                string text2 = "";
                try
                {
                    Mobile_Data_ON();
                    text = Get_IP();
                    Mobile_Data_OFF();
                    Thread.Sleep(1000);
                    Mobile_Data_ON();
                    text2 = Get_IP();
                    if (text != "" && text2 != "" && text != text2)
                    {
                        Invoke(method, "이전 아이피 : " + text + " / 현재 아이피 : " + text2);
                        break;
                    }
                }
                catch
                {
                }
                Thread.Sleep(5000);
            }
        }

        private void Mobile_Data_ON()
        {
            Adb_Send("shell svc data enable");
        }

        private void Mobile_Data_OFF()
        {
            Adb_Send("shell svc data disable");
        }

        private string Get_IP()
        {
            string result = "";
            for (int i = 0; i < 10; i++)
            {
                try
                {
                    HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create("http://autopromaker.com/myip.php");
                    httpWebRequest.Method = "GET";
                    httpWebRequest.Timeout = 10000;
                    HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                    string text = new StreamReader(httpWebResponse.GetResponseStream(), Encoding.UTF8).ReadToEnd();
                    if (text != "")
                    {
                        result = text;
                        return result;
                    }
                }
                catch
                {
                }
                Thread.Sleep(1000);
            }
            return result;
        }

        private string Adb_Send(string Args)
        {
            string result = "";
            for (int i = 0; i < 10; Thread.Sleep(1000), i++)
            {
                try
                {
                    Process process = new Process();
                    process.StartInfo.FileName = Application.StartupPath + "\\adb.exe";
                    process.StartInfo.Arguments = Args;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.CreateNoWindow = true;
                    process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    process.Start();
                    result = process.StandardOutput.ReadToEnd();
                    process.Close();
                }
                catch
                {
                    continue;
                }
                break;
            }
            return result;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(쿠팡_상품_방문.Form1));
            this.작업데이터 = new System.Windows.Forms.DataGridView();
            this.기록리스트 = new System.Windows.Forms.ListView();
            this.계정저장버튼 = new System.Windows.Forms.Button();
            this.계정데이터 = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.계정불러오기버튼 = new System.Windows.Forms.Button();
            this.저장버튼 = new System.Windows.Forms.Button();
            this.불러오기버튼 = new System.Windows.Forms.Button();
            this.시작버튼 = new System.Windows.Forms.Button();
            this.일시정지버튼 = new System.Windows.Forms.Button();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)this.작업데이터).BeginInit();
            ((System.ComponentModel.ISupportInitialize)this.계정데이터).BeginInit();
            base.SuspendLayout();
            this.작업데이터.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.작업데이터.Columns.AddRange(this.Column1, this.Column2, this.Column3);
            this.작업데이터.Location = new System.Drawing.Point(330, 12);
            this.작업데이터.Name = "작업데이터";
            this.작업데이터.RowTemplate.Height = 23;
            this.작업데이터.Size = new System.Drawing.Size(516, 150);
            this.작업데이터.TabIndex = 0;
            this.작업데이터.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(작업데이터_RowsAdded);
            this.작업데이터.RowsRemoved += new System.Windows.Forms.DataGridViewRowsRemovedEventHandler(작업데이터_RowsRemoved);
            this.기록리스트.Columns.AddRange(new System.Windows.Forms.ColumnHeader[2] { this.columnHeader1, this.columnHeader2 });
            this.기록리스트.FullRowSelect = true;
            this.기록리스트.GridLines = true;
            this.기록리스트.Location = new System.Drawing.Point(12, 200);
            this.기록리스트.Name = "기록리스트";
            this.기록리스트.Size = new System.Drawing.Size(834, 122);
            this.기록리스트.TabIndex = 1;
            this.기록리스트.UseCompatibleStateImageBehavior = false;
            this.기록리스트.View = System.Windows.Forms.View.Details;
            this.계정저장버튼.Location = new System.Drawing.Point(12, 168);
            this.계정저장버튼.Name = "계정저장버튼";
            this.계정저장버튼.Size = new System.Drawing.Size(152, 26);
            this.계정저장버튼.TabIndex = 2;
            this.계정저장버튼.Text = "저장";
            this.계정저장버튼.UseVisualStyleBackColor = true;
            this.계정저장버튼.Click += new System.EventHandler(계정저장버튼_Click);
            this.계정데이터.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.계정데이터.Columns.AddRange(this.dataGridViewTextBoxColumn1, this.Column5, this.Column6);
            this.계정데이터.Location = new System.Drawing.Point(12, 12);
            this.계정데이터.Name = "계정데이터";
            this.계정데이터.RowTemplate.Height = 23;
            this.계정데이터.Size = new System.Drawing.Size(312, 150);
            this.계정데이터.TabIndex = 3;
            this.계정데이터.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(계정데이터_RowsAdded);
            this.계정데이터.RowsRemoved += new System.Windows.Forms.DataGridViewRowsRemovedEventHandler(계정데이터_RowsRemoved);
            this.dataGridViewTextBoxColumn1.HeaderText = "No";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            this.dataGridViewTextBoxColumn1.Width = 40;
            this.Column5.HeaderText = "아이디";
            this.Column5.Name = "Column5";
            this.Column6.HeaderText = "비밀번호";
            this.Column6.Name = "Column6";
            this.계정불러오기버튼.Location = new System.Drawing.Point(172, 168);
            this.계정불러오기버튼.Name = "계정불러오기버튼";
            this.계정불러오기버튼.Size = new System.Drawing.Size(152, 26);
            this.계정불러오기버튼.TabIndex = 4;
            this.계정불러오기버튼.Text = "불러오기";
            this.계정불러오기버튼.UseVisualStyleBackColor = true;
            this.계정불러오기버튼.Click += new System.EventHandler(계정불러오기버튼_Click);
            this.저장버튼.Location = new System.Drawing.Point(330, 168);
            this.저장버튼.Name = "저장버튼";
            this.저장버튼.Size = new System.Drawing.Size(256, 26);
            this.저장버튼.TabIndex = 5;
            this.저장버튼.Text = "저장";
            this.저장버튼.UseVisualStyleBackColor = true;
            this.저장버튼.Click += new System.EventHandler(저장버튼_Click);
            this.불러오기버튼.Location = new System.Drawing.Point(590, 168);
            this.불러오기버튼.Name = "불러오기버튼";
            this.불러오기버튼.Size = new System.Drawing.Size(256, 26);
            this.불러오기버튼.TabIndex = 6;
            this.불러오기버튼.Text = "불러오기";
            this.불러오기버튼.UseVisualStyleBackColor = true;
            this.불러오기버튼.Click += new System.EventHandler(불러오기버튼_Click);
            this.시작버튼.Location = new System.Drawing.Point(12, 328);
            this.시작버튼.Name = "시작버튼";
            this.시작버튼.Size = new System.Drawing.Size(412, 26);
            this.시작버튼.TabIndex = 7;
            this.시작버튼.Text = "시작";
            this.시작버튼.UseVisualStyleBackColor = true;
            this.시작버튼.Click += new System.EventHandler(시작버튼_Click);
            this.일시정지버튼.Enabled = false;
            this.일시정지버튼.Location = new System.Drawing.Point(434, 328);
            this.일시정지버튼.Name = "일시정지버튼";
            this.일시정지버튼.Size = new System.Drawing.Size(412, 26);
            this.일시정지버튼.TabIndex = 8;
            this.일시정지버튼.Text = "일시정지";
            this.일시정지버튼.UseVisualStyleBackColor = true;
            this.일시정지버튼.Click += new System.EventHandler(일시정지버튼_Click);
            this.columnHeader1.Text = "시간";
            this.columnHeader1.Width = 140;
            this.columnHeader2.Text = "기록";
            this.columnHeader2.Width = 653;
            this.Column1.HeaderText = "No";
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            this.Column1.Width = 40;
            this.Column2.HeaderText = "구분";
            this.Column2.Name = "Column2";
            this.Column2.Width = 300;
            this.Column3.HeaderText = "상품";
            this.Column3.Name = "Column3";
            base.AutoScaleDimensions = new System.Drawing.SizeF(7f, 12f);
            base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            base.ClientSize = new System.Drawing.Size(858, 366);
            base.Controls.Add(this.일시정지버튼);
            base.Controls.Add(this.시작버튼);
            base.Controls.Add(this.불러오기버튼);
            base.Controls.Add(this.저장버튼);
            base.Controls.Add(this.계정불러오기버튼);
            base.Controls.Add(this.계정데이터);
            base.Controls.Add(this.계정저장버튼);
            base.Controls.Add(this.기록리스트);
            base.Controls.Add(this.작업데이터);
            base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            base.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            base.MaximizeBox = false;
            base.MinimizeBox = false;
            base.Name = "Form1";
            base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "쿠팡 상품 방문";
            base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(Form1_FormClosing);
            ((System.ComponentModel.ISupportInitialize)this.작업데이터).EndInit();
            ((System.ComponentModel.ISupportInitialize)this.계정데이터).EndInit();
            base.ResumeLayout(false);
        }
    }
}
