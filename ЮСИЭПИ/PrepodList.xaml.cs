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
    // lecturers list
    public sealed partial class PrepodList : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        public PrepodList()
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

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
            PrepodTitle.Text = "Список преподавателей";

            // Код для статуса загрузки.
            await Windows.UI.ViewManagement.StatusBar.GetForCurrentView().ShowAsync();
            var progInd = Windows.UI.ViewManagement.StatusBar.GetForCurrentView().ProgressIndicator;
            progInd.Text = "Подождите, идёт загрузка меню";
            await progInd.ShowAsync();

            // Код для сообщения об ошибке отсутствия интернета
            if (!HelperClass.CheckConnection())
            {
                var dialog = new MessageDialog("Соединение с интернетом не установлено.");
                dialog.Title = "Проблема с подключением к интернету.";
                await dialog.ShowAsync();
                Frame.Navigate(typeof(PageError));
            }
            else
            {
                // Объект для хранения данных с сервера в формате xml.
                XmlReader xmlreader =
                    await HelperClass.GetXmlReader(string.Format("http://www.sakhiepi.ru/mobile/rasp/professor_list.aspx?data=1"));

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
                        Panel.Tag = xmlreader.GetAttribute("id");

                        TextBlock tb2 = new TextBlock();
                        tb2.Padding = new Thickness(10, 0, 0, 0);
                        tb2.FontWeight = FontWeights.Normal;
                        tb2.FontSize = 30;
                        tb2.TextWrapping = TextWrapping.Wrap;
                        tb2.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(0xff, 0xff, 0xff, 0xff));
                        tb2.Text = xmlreader.GetAttribute("name");

                        Panel.Children.Add(tb2);

                        Lvi.Content = Panel;

                        PrepodListView.Items.Add(Lvi);

                        k++;
                    }
                    while (xmlreader.ReadToNextSibling("prof"));
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
                    tb1.FontSize = 30;
                    tb1.TextWrapping = TextWrapping.Wrap;
                    tb1.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(0xff, 0, 0xff, 0xe8));
                    tb1.Text = "Список преподавателей пуст";

                    Panel.Children.Add(tb1);

                    Lvi.Content = Panel;

                    PrepodListView.Items.Add(Lvi);
                }

                await progInd.HideAsync();
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void PrepodListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            HelperClass.IdPrepodavatel = (e.ClickedItem as StackPanel).Tag.ToString();
            HelperClass.PrepodName = ((e.ClickedItem as StackPanel).Children[0] as TextBlock).Text;
            Frame.Navigate(typeof(RaspPrepodPage));
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
