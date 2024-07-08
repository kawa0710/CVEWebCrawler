using CVEWebCrawler.Models;
using Microsoft.Playwright;
using Sky.Data.Csv;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace CVEWebCrawler
{
    public partial class Form1 : Form
    {
        private string browserPath = string.Empty;

        public Form1()
        {
            InitializeComponent();
        }

        private async void download(string folder, string url)
        {
            var folder_new = Path.Combine(folder, "Downloads");
            //檢查下載資料夾是否存在
            if (!Directory.Exists(folder_new))
            {
                Directory.CreateDirectory(folder_new);
            };

            var fn = Path.GetFileName(new Uri(url).AbsolutePath);
            var path = Path.Combine(folder_new, fn);

            if (!File.Exists(path))
            {
                using var client = new HttpClient();
                using var stream = await client.GetStreamAsync(url);
                using var fileStream = new FileStream(path, FileMode.OpenOrCreate);
                await stream.CopyToAsync(fileStream);
            }
        }

        private async Task<string> GetDownloadHtmlWithPost(string updateID)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage(HttpMethod.Post, "https://www.catalog.update.microsoft.com/DownloadDialog.aspx");
            var collection = new List<KeyValuePair<string, string>>();
            collection.Add(new("updateIDs",
                //"[{\"size\":0,\"languages\":\"\",\"uidInfo\":\"add46e0e-7c9a-4b23-ab2e-164fe3c5841d\",\"updateID\":\"add46e0e-7c9a-4b23-ab2e-164fe3c5841d\"}]"
                $"[{{\"size\":0,\"languages\":\"\",\"uidInfo\":\"{updateID}\",\"updateID\":\"{updateID}\"}}]"
                ));
            var content = new FormUrlEncodedContent(collection);
            request.Content = content;
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        private void disableAllBtn()
        {
            foreach (var button in this.Controls.OfType<Button>())
            {
                button.Enabled = false;
            }
        }

        private void enableAllBtn()
        {
            foreach (var button in this.Controls.OfType<Button>())
            {
                button.Enabled = true;
            }
        }

        private string GetChromeExePathFromRegistry0()
        {
            var path = Microsoft.Win32.Registry.GetValue(
                @"HKEY_CLASSES_ROOT\ChromeHTML\shell\open\command", null, null) as string;
            if (string.IsNullOrEmpty(path)) return "";
            var m = Regex.Match(path, "\"(?<p>.+?)\"");
            if (!m.Success) return "";
            return m.Groups["p"].Value;
        }

        private string GetChromeExePathFromRegistry1()
        {
            var path = Microsoft.Win32.Registry.GetValue(
                @"HKEY_LOCAL_MACHINE\SOFTWARE\Classes\microsoft-edge\shell\open\command", null, null) as string;
            if (string.IsNullOrEmpty(path)) return "";
            var m = Regex.Match(path, "\"(?<p>.+?)\"");
            if (!m.Success) return "";
            return m.Groups["p"].Value;
        }

        private string GetEdgeExePathFromRegistry0()
        {
            var path = Microsoft.Win32.Registry.GetValue(
                @"HKEY_CLASSES_ROOT\microsoft-edge\shell\open\command", null, null) as string;
            if (string.IsNullOrEmpty(path)) return "";
            var m = Regex.Match(path, "\"(?<p>.+?)\"");
            if (!m.Success) return "";
            return m.Groups["p"].Value;
        }

        private string GetEdgeExePathFromRegistry1()
        {
            var path = Microsoft.Win32.Registry.GetValue(
                @"HKEY_LOCAL_MACHINE\SOFTWARE\Classes\microsoft-edge\shell\open\command", null, null) as string;
            if (string.IsNullOrEmpty(path)) return "";
            var m = Regex.Match(path, "\"(?<p>.+?)\"");
            if (!m.Success) return "";
            return m.Groups["p"].Value;
        }

        private (string Browser, string ExePath) detectBrowser()
        {
            var tupleList = new (string Browser, string ExePath)[]
            {
                ("chrome", @"C:\Program Files (x86)\Google\Chrome\Application\chrome.exe"),
                ("chrome", GetChromeExePathFromRegistry0()),
                ("chrome", GetChromeExePathFromRegistry1()),
                ("edge", @"C:\Program Files (x86)\Microsoft\Edge\Application\msedge.exe"),
                ("edge", GetEdgeExePathFromRegistry0()),
                ("edge", GetEdgeExePathFromRegistry1()),
            };

            foreach (var tuple in tupleList)
            {
                if (File.Exists(tuple.ExePath))
                {
                    return tuple;
                }
            }

            return ("", "");
        }

        private async void btnGo_Click(object sender, EventArgs e)
        {
            if ((string.IsNullOrWhiteSpace(browserPath) && string.IsNullOrWhiteSpace(txbBrowser.Text))
                || string.IsNullOrWhiteSpace(txbInput.Text)
                || string.IsNullOrWhiteSpace(txbOutput.Text))
                return;

            if (string.IsNullOrWhiteSpace(browserPath))
                browserPath = txbBrowser.Text;

            var csvPath = txbInput.Text;
            string outputPath = "", msg = "";
            disableAllBtn();

            LoadingForm loadingForm = null;
            //為了保持動畫必須用Task開另一個thread執行
#pragma warning disable CS4014 // 因為未等待此呼叫，所以在完成呼叫之前會繼續執行目前方法
            Task.Run(() =>
            {
                loadingForm = new LoadingForm();
                loadingForm.ShowDialog();
            });
#pragma warning restore CS4014 // 因為未等待此呼叫，所以在完成呼叫之前會繼續執行目前方法

            try
            {
                //檢查CSV是否存在
                if (!File.Exists(csvPath))
                {
                    Console.WriteLine($"Cannot find {csvPath}");
                    Environment.Exit(0);
                }

                var random = RandomNumberGenerator.Create();
                var bytes = new byte[sizeof(int)]; // 4 bytes
                random.GetNonZeroBytes(bytes);
                var rndHexStr = Convert.ToHexString(bytes);

                var fn = $"{Path.GetFileNameWithoutExtension(csvPath)}_Output{DateTime.Now.ToString("yyyyMMddHHmmss")}{rndHexStr}.csv";
                outputPath = Path.Combine(txbOutput.Text, fn);

                //初始化Playwright
                using var playwright = await Playwright.CreateAsync();
                await using var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
                {
#if DEBUG
                    Headless = false,
#endif
                    ExecutablePath = browserPath,
                });
                var page = await browser.NewPageAsync();
                var page2 = await browser.NewPageAsync();

                var dataResolver = new CVEModelResolver();
                var i = 0; //計檔案數

                var cveList = new List<CVEModel>();
                using (var reader = CsvReader<CVEModel>.Create(csvPath, dataResolver))
                {
                    using (var writer = CsvWriter<CVEModel>.Create(outputPath, dataResolver))
                    {
                        bool isFirst = true;
                        foreach (var cve in reader)
                        {
                            if (isFirst)
                            {
                                writer.WriteRow(new string[]
                                {
                                    "SN", "ID", "URL", "KB_ID", "KB_URL",
                                    "IsDownloadable", "DownloadURL"
                                }); //寫入標頭
                                isFirst = false;
                            }
                            cve.SN = ++i;
                            cve.URL = $"https://msrc.microsoft.com/update-guide/vulnerability/{cve.ID}";

                            await page.GotoAsync(cve.URL);
                            await page.WaitForLoadStateAsync(LoadState.NetworkIdle); //等資料全部載完 => 網路Idle

                            #region 不用的---取得瀏覽器及page的height
                            //var sizes = await page.EvaluateAsync<object>(
                            //    @"() => {
                            //        const browserHeight = window.innerHeight;
                            //        const pageHeight = document.body.scrollHeight;
                            //        return { browserHeight, pageHeight };
                            //      }");
                            #endregion

                            bool isDataGridExist = (await page.Locator("div[data-automation-key='product']").AllAsync()).Count() > 0;
                            if (!isDataGridExist)
                            { //找不到資料表格
                                cve.KB_ID = "";
                                cve.KB_URL = "";
                                cve.DownloadURL = "";
                                cve.IsDownloadable = false;
                                cveList.Add(cve);
                                continue;
                            }

                            //如果文字「Windows Server 2019」沒有顯示，讓資料表格捲動到最後一個元件
                            //最多try 5次
                            int j = 0;
                            bool isSvr2019Visible = false;
                            while (!isSvr2019Visible && j < 5)
                            {
                                isSvr2019Visible = await page.GetByText(new Regex("^Windows Server 2019$")).IsVisibleAsync();
                                j++;
                                await page.Locator("div[data-automation-key='product']").Last.ScrollIntoViewIfNeededAsync();
                                await page.WaitForTimeoutAsync(100); //故意暫停等js載入資料
                            }

                            if (!isSvr2019Visible)
                            { //找不到資料表格
                                cve.KB_ID = "";
                                cve.KB_URL = "";
                                cve.DownloadURL = "";
                                cve.IsDownloadable = false;
                                cveList.Add(cve);
                                continue;
                            }

                            //定位到有文字「Windows Server 2019」的div，取相鄰的全部div
                            var items = await page.Locator("div[data-automation-key='product']")
                                .Filter(new LocatorFilterOptions() { HasTextRegex = new Regex("^Windows Server 2019$") }).First
                                .Locator("~ div").AllAsync();

                            if (items.Count() == 0)
                            { //找不到回傳不可下載
                                cve.KB_ID = "";
                                cve.KB_URL = "";
                                cve.DownloadURL = "";
                                cve.IsDownloadable = false;
                                cveList.Add(cve);
                                continue;
                            }

                            var article = await items[3].Locator("a").First.InnerTextAsync(); //找底下第1個a元素
                            cve.KB_ID = article.Substring(0, article.Length - 2);

                            //如果處理過這個KB_ID就略過
                            var q = cveList.Where(x => x.KB_ID == cve.KB_ID);
                            if (q.Any())
                            {
                                var find = q.First();
                                cve.KB_URL = find.KB_URL;
                                cve.DownloadURL = find.DownloadURL;
                                cve.IsDownloadable = find.IsDownloadable;
                                cveList.Add(cve);
                                continue;
                            }

                            //沒有處理過這個KB_ID
                            var articleUrl = await items[3].Locator("a").First
                                .GetAttributeAsync("href");
                            cve.KB_URL = articleUrl;

                            //var downloadItem = items[4].Locator("a").First;
                            //var downloadUrl = await downloadItem.GetAttributeAsync("href");
                            var downloadUrl = $"https://catalog.update.microsoft.com/Search.aspx?q=KB{cve.KB_ID}";
                            cve.DownloadURL = downloadUrl + "%20Windows%20Server%202019"; //查詢參數加入Windows Server 2019

                            //檢查downloadUrl是否有檔案可下載
                            #region 不用的---開新視窗
                            //var popupWindow = await page.RunAndWaitForPopupAsync(async () =>
                            //{
                            //    await downloadItem.ClickAsync();
                            //});
                            //await popupWindow.WaitForLoadStateAsync(LoadState.NetworkIdle); 
                            #endregion
                            await page2.GotoAsync(cve.DownloadURL);
                            await page2.WaitForLoadStateAsync(LoadState.NetworkIdle); //等資料全部載完 => 網路Idle
                            var items2 = await page2.Locator("#ctl00_catalogBody_searchDuration").AllAsync();

                            //如果查詢視窗的有ctl00_catalogBody_searchDuration這個元素代表有找到
                            if (items2.Count == 0)
                            { //沒有下載檔案
                                cve.IsDownloadable = false;
                            }
                            else
                            {
                                //有下載檔案；在指定了查詢參數Windows Server 2019後只有1個結果
                                var updateID = await page2.Locator("input[value='Download']").First.GetAttributeAsync("id");

                                string doc = "";
                                //如果有下載檔案就讀取URL
                                if (updateID != null)
                                    doc = await GetDownloadHtmlWithPost(updateID);

                                if (string.IsNullOrWhiteSpace(doc))
                                    cve.IsDownloadable = false;
                                else
                                {
                                    Match match = Regex.Match(doc, @"https://[^\s']+\.msu");
                                    if (match.Success)
                                    {
                                        cve.DownloadURL = match.Value;
                                        cve.IsDownloadable = true;

                                        if (ckbDownload.Checked)
                                        {
                                            download(txbOutput.Text, cve.DownloadURL);
                                        }
                                    }
                                }
                            }

                            cveList.Add(cve);
                        }

                        writer.WriteRows(cveList);
                    }
                }

                var f = "files";
                if (i == 1) f = "file";

                if (i == 0)
                    msg = "No files have been processed";
                else
                    msg = $"{i} {f} have been processed";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                enableAllBtn();
                loadingForm.DialogResult = DialogResult.OK;

                if (!string.IsNullOrWhiteSpace(msg))
                {
                    this.TopMost = true; //強制MessageBox.Show()跳到視窗最上層
                    MessageBox.Show(msg);
                    this.TopMost = false;
                }
            }
        }

        private void btnInput_Click(object sender, EventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Comma-Separated Values File(*.csv)|*.csv";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                txbInput.Text = openFileDialog.FileName;
                txbOutput.Text = Path.GetDirectoryName(txbInput.Text);
            }
        }

        private void btnOutput_Click(object sender, EventArgs e)
        {
            var folderBrowserDialog1 = new FolderBrowserDialog();
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                txbOutput.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var browser = detectBrowser();
            if (!string.IsNullOrWhiteSpace(browser.Browser))
            {
                browserPath = browser.ExePath;
                lblBrowser.Text = browser.Browser + " (Detected)";
            }
            else
            {
                lblBrowser.Visible = false;
                txbBrowser.Visible = true;
                btnBrowser.Visible = true;
            }

#if DEBUG
            txbInput.Text = @"C:\VSProjects\CVEWebCrawler\cve.csv";
            txbOutput.Text = Path.GetDirectoryName(txbInput.Text);
#endif
        }

        private void btnBrowser_Click(object sender, EventArgs e)
        {
            var openFileDialog = new OpenFileDialog();
            openFileDialog.Title = "Chromium Browser, examples: Chrome, Edge, Opera, Brave, Vivaldi, Arc, ..., etc.";
            openFileDialog.Filter = "Executable File(*.exe)|*.exe";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                txbBrowser.Text = openFileDialog.FileName;
            }
        }
    }
}
