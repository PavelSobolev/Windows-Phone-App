using System;
using System.Xml;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace ЮСИЭПИ
{

    public sealed partial class MainPage : Page
    {
        public TextBlock[] AllButtonTitles;
        public TextBlock[] AllButtonDescriptions;
        public ListViewItem[] AllListItems;
        public StackPanel[] Panels;
        public StackPanel[] NewsDetails;
        public TextBlock[] NewsTitle;
        public TextBlock[] NewsDate;
        public TextBlock[] NewsText;
        public TextBlock[] NameFaculty;
        public TextBlock[] TextActual;
        public Image[] ImageActual;
        public StackPanel[] StackPanelActual;
        public ListViewItem[] ListFaculty;
        public StackPanel[] Faculty;
        public ListViewItem[] ActualListItem;

        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;

            AllButtonTitles = new TextBlock[] { T1Title, T2Title, T3Title, T4Title, T5Title, T6Title, T7Title, T8Title, T9Title, T10Title };
            AllButtonDescriptions = new TextBlock[] { T1Description, T2Description, T3Description, T4Description, T5Description, T6Description, T7Description, T8Description, T9Description, T10Description };
            Panels = new StackPanel[] { S1, S2, S3, S4, S5, S6, S7, S8, S9, S10 };
            AllListItems = new ListViewItem[] { ListItem1, ListItem2, ListItem3, ListItem4, ListItem5, ListItem6, ListItem7, ListItem8, ListItem9, ListItem10 };
            NewsDetails = new StackPanel[] { News1, News2, News3, News4 };
            NewsTitle = new TextBlock[] { NewsTitle1, NewsTitle2, NewsTitle3, NewsTitle4 };
            NewsDate = new TextBlock[] { NewsDate1, NewsDate2, NewsDate3, NewsDate4 };
            NewsText = new TextBlock[] { NewsText1, NewsText2, NewsText3, NewsText4 };
            NameFaculty = new TextBlock[] { NameFaculty1, NameFaculty2, NameFaculty3, NameFaculty4 };
            ListFaculty = new ListViewItem[] { ListFaculty1, ListFaculty2, ListFaculty3, ListFaculty4 };
            Faculty = new StackPanel[] { Faculty1, Faculty2, Faculty3, Faculty4 };
            StackPanelActual = new StackPanel[] { A1, A2, A3, A4, A5, A6 };
            ImageActual = new Image[] { ImageActual1, ImageActual2, ImageActual3, ImageActual4, ImageActual5, ImageActual6 };
            TextActual = new TextBlock[] { TextActual1, TextActual2, TextActual3, TextActual4, TextActual5, TextActual6 };
            ActualListItem = new ListViewItem[] { Actual1, Actual2, Actual3, Actual4, Actual5, Actual6 };
            
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {

            await Windows.UI.ViewManagement.StatusBar.GetForCurrentView().ShowAsync();
            var progInd = Windows.UI.ViewManagement.StatusBar.GetForCurrentView().ProgressIndicator;
            progInd.Text = "Подождите, идёт загрузка меню";
            await progInd.ShowAsync();

            bool Problems = false;

            if (ApplicationData.Current.LocalSettings.Values.Keys.Contains("GrName"))
            {
                if (ApplicationData.Current.LocalSettings.Values["GrName"].ToString() != "0")
                {
                    MyRasp.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    MyRaspText.Text = "Моё расписание\n" + ApplicationData.Current.LocalSettings.Values["GrName"].ToString();
                }
                else
                {
                    MyRasp.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                }
            }

            if (ApplicationData.Current.LocalSettings.Values.Keys.Contains("PrepName"))
            {
                if (ApplicationData.Current.LocalSettings.Values["PrepName"].ToString() != "0")
                {
                    MyRaspPrepod.Visibility = Windows.UI.Xaml.Visibility.Visible;
                    MyRaspPrepodText.Text = "Расписание преподавателя\n" + ApplicationData.Current.LocalSettings.Values["PrepName"].ToString();
                }
                else
                {
                    MyRaspPrepod.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                }
            }


            if (!HelperClass.CheckConnection())
            {
                var dialog = new MessageDialog("Соединение с интернетом не установлено.");
                dialog.Title = "Проблема с подключением к интернету.";
                await dialog.ShowAsync();
                Frame.Navigate(typeof(PageError));
            }
            else
            {
                XmlReader xmlreader = null;
                    //await HelperClass.GetXmlReader("http://www.sakhie pi.ru/mobile/winphone/menu_1.aspx");

                try
                {
                    xmlreader = 
                    await HelperClass.GetXmlReader("http://www.sakhiepi.ru/mobile/winphone/menu_1.aspx");

                    xmlreader.ReadStartElement("main");
                    int k = 0;
                    do
                    {
                        AllButtonTitles[k].Text = xmlreader.GetAttribute(2);
                        AllButtonDescriptions[k].Text = xmlreader.GetAttribute(7);

                        Panels[k].Tag = xmlreader.GetAttribute(0);
                        AllListItems[k].Visibility = Windows.UI.Xaml.Visibility.Visible;
                        k++;
                    }
                    while (xmlreader.ReadToNextSibling("menu"));

                    // Конец вывода кнопок главной страницы -------------------

                    // Вывод списка новостей -----------------------------------

                    string Adress = string.Format("http://www.sakhiepi.ru/mobile/winphone/news_list.aspx");
                    xmlreader = await HelperClass.GetXmlReader(Adress);

                    try
                    {
                        xmlreader.ReadStartElement("main");

                        k = 0;
                        do
                        {
                            NewsTitle[k].Text = xmlreader.GetAttribute("ru_short_name");
                            NewsDate[k].Text = xmlreader.GetAttribute("pubdate");
                            NewsText[k].Text = xmlreader.GetAttribute("ru_pred_text");
                            NewsDetails[k].Tag = xmlreader.GetAttribute("page_id");
                            NewsDetails[k].Visibility = Windows.UI.Xaml.Visibility.Visible;
                            k++;
                        }
                        while (xmlreader.ReadToNextSibling("news"));
                    }
                    catch (Exception)
                    {
                        Problems = true;
                    }
                    

                    // Вывод списка блока актуально -----------------------------------                    
                    xmlreader = await HelperClass.GetXmlReader(string.Format("http://www.sakhiepi.ru/mobile/winphone/actual_list.aspx"));

                    //отображение
                    try
                    {
                        xmlreader.ReadStartElement("main");

                        k = 0;
                        do
                        {
                            ImageActual[k].Source = new BitmapImage(new Uri("http://www.sakhiepi.ru/" + xmlreader.GetAttribute("pictfile")));
                            StackPanelActual[k].Tag = xmlreader.GetAttribute("page_id");
                            TextActual[k].Text = xmlreader.GetAttribute("ru_short_name");

                            ActualListItem[k].Visibility = Windows.UI.Xaml.Visibility.Visible;
                            k++;
                        }
                        while (xmlreader.ReadToNextSibling("news"));

                        // Сделать кнопку невидимой

                        ArchiveButton.Visibility = Windows.UI.Xaml.Visibility.Visible;

                        // Список событий календаря ------------------------------------------------------
                        CalendarList.Items.Clear();
                        CalendarTitle.Text = DateTime.Now.ToString("MMMM, yyyy");//, new Culture("ru-RU"));

                        Adress = string.Format("http://www.sakhiepi.ru/mobile/winphone/read_events.aspx?y={0}&m={1}",
                            DateTime.Now.Year, DateTime.Now.Month);
                        xmlreader = await HelperClass.GetXmlReader(Adress);
                        //отображение
                        try
                        {
                            xmlreader.ReadStartElement("main");

                            k = 0;
                            do
                            {
                                ListViewItem Lvi = new ListViewItem();
                                Lvi.Padding = new Thickness(0, 0, 0, 0);
                                Lvi.Margin = new Thickness(0, 0, 0, 15);

                                StackPanel Panel = new StackPanel();
                                Panel.Margin = new Thickness(0, 0, 0, 9.5);
                                Panel.Tag = xmlreader.GetAttribute("id");

                                TextBlock tb1 = new TextBlock();
                                tb1.Padding = new Thickness(10, 0, 0, 0);
                                tb1.FontWeight = FontWeights.Bold;
                                tb1.FontSize = 18;
                                tb1.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(0xff, 0, 0xff, 0xe8));
                                tb1.Text = xmlreader.GetAttribute("Kogda");

                                TextBlock tb2 = new TextBlock();
                                tb2.Padding = new Thickness(10, 0, 0, 0);
                                tb2.FontWeight = FontWeights.Normal;
                                tb2.FontSize = 25;
                                tb2.TextWrapping = TextWrapping.Wrap;
                                tb2.Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(0xff, 0xff, 0xff, 0xff));
                                tb2.Text = xmlreader.GetAttribute("Sobitie");

                                Panel.Children.Add(tb1);
                                Panel.Children.Add(tb2);

                                Lvi.Content = Panel;

                                CalendarList.Items.Add(Lvi);

                                k++;
                            }
                            while (xmlreader.ReadToNextSibling("calendar_sobitiy"));
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
                            tb1.Text = "На выбранный месяц событий пока нет";

                            Panel.Children.Add(tb1);

                            Lvi.Content = Panel;

                            CalendarList.Items.Add(Lvi);
                        }
                    }
                    catch (Exception)
                    {
                        Problems = true;
                    }
                }
                catch (Exception)
                {
                    Problems = true;
                }
            }

            // Конец выполнения статуса загрузки (окончание).
            await progInd.HideAsync();

            if(Problems)
            {
                var dialog = new MessageDialog("Соединение с интернетом установлено, но сетевой ресурс недоступен.");
                dialog.Title = "Проблема с подключением к интернет-ресурсу.";
                await dialog.ShowAsync();
                Frame.Navigate(typeof(PageErrorR));
            }

        }

        public int FindActiveNumber(string tag)
        {
            int i = 0;
            foreach (StackPanel lvi in Panels)
            {
                if (lvi.Tag.ToString() == tag)
                {
                    return i;
                }
                i++;
            }
            return 0;
        }

        private void ListItem1_PointerReleased(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
        }

        private void ListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            StackPanel Sp = (StackPanel)e.ClickedItem;

            string tag = Sp.Tag.ToString();
            HelperClass.Parent2Id = tag;
            HelperClass.Parent2Text = AllButtonTitles[FindActiveNumber(tag)].Text;

            switch (tag)
            {
                case "1":
                    HelperClass.Parent3Id = "11";
                    HelperClass.Parent3Text = "Главное меню";
                    Frame.Navigate(typeof(Menu3Page));
                    return;

                case "37":
                    HelperClass.Parent3Id = "37";
                    HelperClass.Parent3Text = "Библиотека СахГТИ";
                    Frame.Navigate(typeof(Menu3Page));
                    return;
            }

            Frame.Navigate(typeof(Menu2Page));
        }

        private void AllNews_ItemClick(object sender, ItemClickEventArgs e)
        {
            string tag = (e.ClickedItem as StackPanel).Tag.ToString();
            HelperClass.PageId = tag;

            if (tag != "10000")
            {
                Frame.Navigate(typeof(PageContent));
            }
        }

        private void ArchiveButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Frame.Navigate(typeof(NewsArchiveYear));
        }

        /// <summary>
        /// переход на отображение подробностей выбранного блока Акутально
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Actual_ItemClick(object sender, ItemClickEventArgs e)
        {
            string tag = (e.ClickedItem as StackPanel).Tag.ToString();
            HelperClass.PageId = tag;
            Frame.Navigate(typeof(PageContent));
        }

        private void Raspisanie_ItemClick(object sender, ItemClickEventArgs e)
        {
            if ((e.ClickedItem as StackPanel).Tag.ToString()=="MR")
            {
                HelperClass.IdGrupp = ApplicationData.Current.LocalSettings.Values["GrId"].ToString();
                HelperClass.NameGroup = ApplicationData.Current.LocalSettings.Values["GrName"].ToString();
                Frame.Navigate(typeof(RaspGroupPage));
                return;
            }

            if ((e.ClickedItem as StackPanel).Tag.ToString() == "MRP")
            {
                HelperClass.IdPrepodavatel = ApplicationData.Current.LocalSettings.Values["PrId"].ToString();
                HelperClass.PrepodName = ApplicationData.Current.LocalSettings.Values["PrepName"].ToString();
                Frame.Navigate(typeof(RaspPrepodPage));
                return;
            }

            HelperClass.IdFacultet = (e.ClickedItem as StackPanel).Tag.ToString();
            HelperClass.NameFacultet = ((e.ClickedItem as StackPanel).Children[1] as TextBlock).Text;

            if (int.Parse(HelperClass.IdFacultet) > 5)
            {
                Frame.Navigate(typeof(GroupList));
                return;
            }

            if (int.Parse(HelperClass.IdFacultet) == 0)
            {
                Frame.Navigate(typeof(PrepodList));
                return;
            }

            switch (HelperClass.IdFacultet)
            {
                case "4":
                    HelperClass.PageId = "794"; break;
                case "5":
                    HelperClass.PageId = "793"; break;
            }
            Frame.Navigate(typeof(PageContent));
        }
    }
}
