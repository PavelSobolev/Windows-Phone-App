using System;
using System.Xml;
using Windows.UI.Popups;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using ЮСИЭПИ.Common;

namespace ЮСИЭПИ
{

    public sealed partial class ArchiveNewsList : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        public ArchiveNewsList()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
        }

        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
        }

        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region Регистрация NavigationHelper

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {

            await Windows.UI.ViewManagement.StatusBar.GetForCurrentView().ShowAsync();
            var progInd = Windows.UI.ViewManagement.StatusBar.GetForCurrentView().ProgressIndicator;
            progInd.Text = "Подождите, идёт загрузка меню";
            await progInd.ShowAsync();

            this.navigationHelper.OnNavigatedTo(e);

            if (!HelperClass.CheckConnection())
            {
                var dialog = new MessageDialog("Соединение с интернетом не установлено.");
                dialog.Title = "Проблема с подключением к интернету.";
                await dialog.ShowAsync();
                Frame.Navigate(typeof(PageError));
                return;
            }

            NewsListTitle.Text = HelperClass.NewsYear + ", " + HelperClass.NewsMonthName;

            string Adress = string.Format("http://www.sakhiepi.ru/mobile/winphone/read_news.aspx?y={0}&m={1}&lang=1", 
                HelperClass.NewsYear, HelperClass.NewsMonth);
            XmlReader xmlreader = await HelperClass.GetXmlReader(Adress);

            //отображение
            try
            {
                xmlreader.ReadStartElement("main");

                int k = 0;
                do
                {
                    ListViewItem Lvi = new ListViewItem();
                    Lvi.Padding = new Thickness(0, 0, 0, 0);
                    Lvi.Margin = new Thickness(0, 0, 0, 15);

                    StackPanel Panel = new StackPanel();
                    Panel.Margin = new Thickness(0, 0, 0, 9.5);
                    Panel.Tag = xmlreader.GetAttribute("page_id");

                    TextBlock tb1 = new TextBlock();
                    tb1.Padding = new Thickness(10, 0, 0, 0);
                    tb1.FontWeight = FontWeights.Bold;
                    tb1.FontSize = 18;
                    tb1.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(0xff, 0, 0xff, 0xe8));
                    tb1.Text = xmlreader.GetAttribute("pubdate");

                    TextBlock tb2 = new TextBlock();
                    tb2.Padding = new Thickness(10, 0, 0, 0);
                    tb2.FontWeight = FontWeights.Normal;
                    tb2.FontSize = 25;
                    tb2.TextWrapping = TextWrapping.Wrap;
                    tb2.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(0xff, 0xff, 0xff, 0xff));
                    tb2.Text = xmlreader.GetAttribute("newstxt");

                    Panel.Children.Add(tb1);
                    Panel.Children.Add(tb2);

                    Lvi.Content = Panel;

                    AllNews.Items.Add(Lvi);
                    
                    k++;
                }
                while (xmlreader.ReadToNextSibling("news"));
            }
            catch (Exception)
            {
                ListViewItem Lvi = new ListViewItem();
                Lvi.Padding = new Thickness(0, 0, 0, 0);
                Lvi.Margin = new Thickness(0, 0, 0, 15);

                StackPanel Panel = new StackPanel();
                Panel.Margin = new Thickness(0, 0, 0, 9.5);

                TextBlock tb1 = new TextBlock();
                tb1.Padding = new Thickness(10, 0, 0, 0);
                tb1.FontWeight = FontWeights.Bold;
                tb1.FontSize = 25;
                tb1.TextWrapping = TextWrapping.Wrap;
                tb1.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(0xff, 0, 0xff, 0xe8));
                tb1.Text = "На выбранный месяц новостей нет";

                Panel.Children.Add(tb1);

                Lvi.Content = Panel;

                AllNews.Items.Add(Lvi);
            }

            // Конец выполнения статуса загрузки (окончание).
            await progInd.HideAsync();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void AllNews_ItemClick(object sender, ItemClickEventArgs e)
        {
            HelperClass.PageId = ((StackPanel)e.ClickedItem).Tag.ToString();
            Frame.Navigate(typeof(PageContent));
        }

        private void AppBarButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            string Adress = string.Format("http://www.sakhiepi.ru/winphone.aspx?sp={0}&hd=1", HelperClass.PageId);
        }

        private void AppBarButton_Click_2(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }
    }
}
