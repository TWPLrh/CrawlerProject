using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium;

namespace F74046577_施儒宏_期末專題
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        private FirefoxOptions GetFirefoxOptions()
        {
            string LocalPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + @"\Mozilla\Firefox\Profiles\";
            LocalPath = System.IO.Directory.GetDirectories(LocalPath)[0];

            FirefoxProfile profile = new FirefoxProfile(LocalPath);
            FirefoxOptions Options = new FirefoxOptions();
            Options.Profile = profile;

            return Options;
        }

        public MainWindow() => new Task(() => new CrawBahamut(GetFirefoxOptions(), this)).Start();

        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            FirefoxOptions Cache = GetFirefoxOptions();

            if (checkBox_Baha.IsChecked.Value)
            {
                new Task(() => new CrawBahamut(Cache, this)).Start();
                checkBox_Baha.IsChecked = false;
                checkBox_Baha.IsEnabled = false;
            }

            if (checkBox_Utube.IsChecked.Value)
            {
                new Task(() => new CrawYoutube(Cache, this)).Start();
                checkBox_Utube.IsChecked = false;
                checkBox_Utube.IsEnabled = false;
            }

            if (checkBox_Pixiv.IsChecked.Value)
            {
                new Task(() => new CrawPixiv(Cache, this)).Start();
                checkBox_Pixiv.IsChecked = false;
                checkBox_Pixiv.IsEnabled = false;
            }
        }
    }

    public class CrawBahamut : FirefoxDriver
    {
        public CrawBahamut(FirefoxOptions options, MainWindow Control) : base(options)
        {
            IReadOnlyCollection<IWebElement> webElements;

            Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(10);
            Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            Manage().Timeouts().AsynchronousJavaScript = TimeSpan.FromSeconds(10);

            Actions actions = new Actions(this);

            Navigate().GoToUrl(@"https://www.gamer.com.tw/index2.php?ad=N");

            // 獲取href以及動畫資料
            var elem = FindElementByCssSelector("div.BA-lbox.BA-lbox3");
            var elems = FindElements(By.ClassName("newanime_text"));
            
            foreach (var x in elems)
            {
                x.GetAttribute("href");
                var c = x.Text;
            }
           
            // 獲取各標籤頭條
            elem = FindElementByClassName("BA-ctag1");
            elems = elem.FindElements(By.TagName("a"));

            foreach (var li in elems)
            {
                actions.MoveToElement(li).Perform();

                bool bacj = true;

                while (bacj)
                {
                    try
                    {
                        elem = FindElementByCssSelector("#gnn_head");
                        webElements = elem.FindElements(By.CssSelector("div.BA-cbox.BA-cbox2"));

                        if (li.Text.Equals("電玩瘋"))
                        {
                            var x = elem.FindElement(By.TagName("iframe"));
                            x.GetAttribute("src");
                        }
                        else
                        {
                            foreach (var updiv in webElements)
                            {
                                foreach (var lowdiv in updiv.FindElements(By.TagName("div")))
                                {
                                    var a = lowdiv.FindElements(By.TagName("a"));
                                    for (int i = 0; i < a.Count; i++)
                                    {
                                        if (i % 2 == 0)
                                        {
                                            a[i].GetAttribute("href");
                                            var txt = a[i].Text;
                                        }
                                    }
                                }
                            }
                        }
                        bacj = false;
                    }
                    catch
                    {
                        bacj = true;
                    }
                }
            }
        }
    }

    public class CrawYoutube : FirefoxDriver
    {
        public CrawYoutube(FirefoxOptions options, MainWindow Control) : base(options)
        {
            Navigate().GoToUrl(@"https://www.youtube.com/");
        }
    }
    
    public class CrawPixiv : FirefoxDriver
    {
        public CrawPixiv(FirefoxOptions options, MainWindow Control) : base(options)
        {
            Navigate().GoToUrl(@"https://www.pixiv.net/");
        }
    }
}
