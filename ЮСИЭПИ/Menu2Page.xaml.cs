using System;
using System.Xml;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using ЮСИЭПИ.Common;

namespace ЮСИЭПИ
{
    public sealed partial class Menu2Page : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        public Menu2Page()
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

            // Код для статуса загрузки.
            await Windows.UI.ViewManagement.StatusBar.GetForCurrentView().ShowAsync();
            var progInd = Windows.UI.ViewManagement.StatusBar.GetForCurrentView().ProgressIndicator;
            progInd.Text = "Подождите, идёт загрузка меню";
            await progInd.ShowAsync();

            this.navigationHelper.OnNavigatedTo(e);
            Menu2Title.Text = HelperClass.Parent2Text;

            if (!HelperClass.CheckConnection())
            {
                var dialog = new MessageDialog("Соединение с интернетом не установлено.");
                dialog.Title = "Проблема с подключением к интернету.";
                await dialog.ShowAsync();
                Frame.Navigate(typeof(PageError));
            }
            else
            {

                string Adress = string.Format("http://www.sakhiepi.ru/mobile/winphone/menu_2.aspx?parent={0}", HelperClass.Parent2Id);
                XmlReader xmlreader = await HelperClass.GetXmlReader(Adress);

                //отображение
                try
                {
                    xmlreader.ReadStartElement("main");

                    int k = 0;
                    do
                    {
                        ListViewItem lvi = new ListViewItem();
                        lvi.Margin = new Thickness(0, 0, 0, 15);
                        lvi.FontSize = 34;
                        lvi.Content = xmlreader.GetAttribute(2);
                        lvi.Tag = xmlreader.GetAttribute(0);

                        Menu2ListView.Items.Add(lvi);
                        k++;
                    }
                    while (xmlreader.ReadToNextSibling("menu"));
                }
                catch (Exception)
                {
                }
            }

            await progInd.HideAsync();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        public string FindActiveNumber(string text)
        {
            foreach (ListViewItem lvi in Menu2ListView.Items)
            {
                if (lvi.Content.ToString().ToLower() == text.ToLower())
                {
                    return lvi.Tag.ToString();
                }
            }
            return "";
        }

        private void Menu2ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            string tag = FindActiveNumber(e.ClickedItem.ToString());
            HelperClass.Parent3Id = tag;
            HelperClass.Parent3Text = e.ClickedItem.ToString();
            switch (tag)
            {
                case "62":
                    HelperClass.PageId = "90";
                    Frame.Navigate(typeof(PageContent));
                    return;

                case "70":
                    HelperClass.PageId = "69";
                    Frame.Navigate(typeof(PageContent));
                    return;

                case "71":
                    HelperClass.PageId = "70";
                    Frame.Navigate(typeof(PageContent));
                    return;
            }
            Frame.Navigate(typeof(Menu3Page));
        }

        private void Menu2ListView_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
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
