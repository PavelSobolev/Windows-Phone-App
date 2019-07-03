using System;
using System.Collections.Generic;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using ЮСИЭПИ.Common;

namespace ЮСИЭПИ
{

    public sealed partial class NewsArchiveMon : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        public NewsArchiveMon()
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

            ArchiveTitle.Text = HelperClass.NewsYear + " - Архив новостей";

            int God = DateTime.Now.Year;
            if (God < 2012 || God > 2020) God = 2015;

            int Month = DateTime.Now.Month;

            for (int i = 0; i < Menu2ListView.Items.Count; i++)
            {
                int NumberMonth = int.Parse(((ListViewItem)Menu2ListView.Items[i]).Tag.ToString());
                if (NumberMonth > Month && God.ToString() == HelperClass.NewsYear) ((ListViewItem)Menu2ListView.Items[i]).Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                else
                    ((ListViewItem)Menu2ListView.Items[i]).Visibility = Windows.UI.Xaml.Visibility.Visible;
            }

            await progInd.HideAsync();

        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void Menu2ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            Dictionary<string, string> MonthNumber = new Dictionary<string, string>();
            MonthNumber["январь"] = "1";
            MonthNumber["февраль"] = "2";
            MonthNumber["март"] = "3";
            MonthNumber["апрель"] = "4";
            MonthNumber["май"] = "5";
            MonthNumber["июнь"] = "6";
            MonthNumber["июль"] = "7";
            MonthNumber["август"] = "8";
            MonthNumber["сентябрь"] = "9";
            MonthNumber["октябрь"] = "10";
            MonthNumber["ноябрь"] = "11";
            MonthNumber["декабрь"] = "12";

            HelperClass.NewsMonth = MonthNumber[e.ClickedItem.ToString().ToLower()];
            HelperClass.NewsMonthName = e.ClickedItem.ToString().ToLower();
            Frame.Navigate(typeof(ArchiveNewsList));
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
