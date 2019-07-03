﻿using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Xml;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using ЮСИЭПИ.Common;

namespace ЮСИЭПИ
{
    /// <summary>
    /// page shows shcedule of studying goup of students
    /// </summary>
    public sealed partial class RaspGroupPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        public TextBlock[] Premds;
        public TextBlock[] Details;
        public int Y = DateTime.Now.Year;
        public int M = DateTime.Now.Month;
        public int D = DateTime.Now.Day;
        public string FileName = "RaspSetting.txt";

        public RaspGroupPage()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;

            Premds = new TextBlock[] { Pr1, Pr2, Pr3, Pr4, Pr5, Pr6 };
            Details = new TextBlock[] { GrAud1, GrAud2, GrAud3, GrAud4, GrAud5, GrAud6 };

            DateTime T = new DateTime(Y, M, D);
            TxtDate.Text = HelperClass.NameGroup + "\n" + T.ToString("dd MMMM yyyy", new CultureInfo("ru-RU")) + "\n" + T.ToString("dddd", new CultureInfo("ru-RU"));

            RaspDatePicker.MinYear = new DateTimeOffset(new DateTime(2013, 9, 1));
            RaspDatePicker.MaxYear = new DateTimeOffset(new DateTime(DateTime.Now.Year + 1, 8, 31));
            ReadSettings();
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
            this.navigationHelper.OnNavigatedTo(e);

            await Windows.UI.ViewManagement.StatusBar.GetForCurrentView().ShowAsync();
            var progInd = Windows.UI.ViewManagement.StatusBar.GetForCurrentView().ProgressIndicator;
            progInd.Text = "Подождите, идёт загрузка расписания";
            await progInd.ShowAsync();

            if (!HelperClass.CheckConnection())
            {
                var dialog = new MessageDialog("Соединение с интернетом не установлено.");
                dialog.Title = "Проблема с подключением к интернету.";
                await dialog.ShowAsync();
                Frame.Navigate(typeof(PageError));
                for (int i = 0; i < 6; i++)
                {
                    Premds[i].Text = string.Empty;
                    Details[i].Text = string.Empty;
                }
                return;
            }
            else
            {
                await FillRasp();
            }

            // end of downloading
            await progInd.HideAsync();
        }

        public async Task<int> FillRasp()
        {
            XmlReader xmlreader =
                await HelperClass.GetXmlReader(string.Format("http://www.sakhiepi.ru/mobile/rasp/gr_rasp.aspx?y={0}&m={1}&d={2}&gr={3}", Y, M, D, HelperClass.IdGrupp));

            for (int i = 0; i < 6; i++)
            {
                Premds[i].Text = string.Empty;
                Details[i].Text = string.Empty;
            }

            // xml parsing
            try
            {
                xmlreader.ReadStartElement("main");
                if (xmlreader.GetAttribute("pname") != "0")
                {
                    do
                    {
                        int nom = Convert.ToInt32(xmlreader.GetAttribute("nomzan"));

                        Premds[nom - 1].Text = xmlreader.GetAttribute("pname");
                        Premds[nom - 1].Tag = "Дисциплина: " + xmlreader.GetAttribute("pfullname") + "\n" + "Вид занятия: " + xmlreader.GetAttribute("vzname") + ".";
                        Premds[nom - 1].Tapped += MainPage_Tapped;

                        if (Convert.ToBoolean(xmlreader.GetAttribute("iscontrol")))
                        {
                            Details[nom - 1].Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 255, 0));
                            Details[nom - 1].Text = xmlreader.GetAttribute("grname") + ", ауд.:" + xmlreader.GetAttribute("audname") + " [" + xmlreader.GetAttribute("vzname") + "]";
                        }
                        else
                        {
                            Details[nom - 1].Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 255, 255));
                            Details[nom - 1].Text = xmlreader.GetAttribute("grname") + ", ауд.:" + xmlreader.GetAttribute("audname");
                        }
                    }
                    while (xmlreader.ReadToNextSibling("zan"));
                }
                else
                {
                    for (int i = 0; i < 6; i++)
                    {
                        Premds[i].Text = "Занятий нет";
                        Details[i].Text = string.Empty;
                    }
                }
            }
            catch (Exception ex)
            {
                for (int i = 0; i < 6; i++)
                {
                    Premds[i].Text = string.Empty;
                    Details[i].Text = string.Empty;
                }
            }
            return 0;
        }

        public void MainPage_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            string txt = (e.OriginalSource as TextBlock).Tag != null ? (e.OriginalSource as TextBlock).Tag.ToString() : "";
            ShowContent(txt);
        }

        private async Task<int> ShowContent(string Txt)
        {
            ContentDialog dialog = new ContentDialog()
            {
                Title = "Описание пункта расписания",
                Content = Txt,
                PrimaryButtonText = "Ok",
                FontSize = 30,
                Foreground = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 255, 255, 0))
            };

            ContentDialogResult result = await dialog.ShowAsync();

            return 0;
        }

        public enum Directions { Back, Forward };

        public int GetSpan(DateTime T, Directions D)
        {
            if (Convert.ToBoolean(ApplicationData.Current.LocalSettings.Values["IgnoreHolydays"]))
            {
                if (D == Directions.Forward)
                {
                    switch (T.DayOfWeek)
                    {
                        case DayOfWeek.Saturday:
                            return 2;
                        case DayOfWeek.Friday:
                            return 3;
                        default: return 1;
                    }
                }
                else
                {
                    switch (T.DayOfWeek)
                    {
                        case DayOfWeek.Monday:
                            return -3;
                        case DayOfWeek.Sunday:
                            return -2;
                        default: return -1;
                    }
                }
            }
            else
            {
                if (D == Directions.Forward)
                    return 1;
                else
                    return -1;
            }
        }

        // onde day back
        private async void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            await Windows.UI.ViewManagement.StatusBar.GetForCurrentView().ShowAsync();
            var progInd = Windows.UI.ViewManagement.StatusBar.GetForCurrentView().ProgressIndicator;
            progInd.Text = "Подождите, идёт загрузка расписания";
            await progInd.ShowAsync();

            for (int i = 0; i < 6; i++)
            {
                Premds[i].Text = "Загрузка ... ";
                Details[i].Text = string.Empty;
            }

            DateTime T = new DateTime(Y, M, D);
            T = T.AddDays(GetSpan(T, Directions.Back));

            Y = T.Year;
            M = T.Month;
            D = T.Day;
            TxtDate.Text = HelperClass.NameGroup + "\n" + T.ToString("dd MMMM yyyy", new CultureInfo("ru-RU")) + "\n" + T.ToString("dddd", new CultureInfo("ru-RU"));

            await FillRasp();

            await progInd.HideAsync();
        }

        // 1 day forward
        private async void AppBarButton_Click_1(object sender, RoutedEventArgs e)
        {
            await Windows.UI.ViewManagement.StatusBar.GetForCurrentView().ShowAsync();
            var progInd = Windows.UI.ViewManagement.StatusBar.GetForCurrentView().ProgressIndicator;
            progInd.Text = "Подождите, идёт загрузка расписания";
            await progInd.ShowAsync();

            for (int i = 0; i < 6; i++)
            {
                Premds[i].Text = "Загрузка ... ";
                Details[i].Text = string.Empty;
            }

            DateTime T = new DateTime(Y, M, D);
            T = T.AddDays(GetSpan(T, Directions.Forward));
            Y = T.Year;
            M = T.Month;
            D = T.Day;
            TxtDate.Text = HelperClass.NameGroup + "\n" + T.ToString("dd MMMM yyyy", new CultureInfo("ru-RU")) + "\n" + T.ToString("dddd", new CultureInfo("ru-RU"));

            await FillRasp();

            await progInd.HideAsync();
        }

        // today
        private async void AppBarButton_Click_2(object sender, RoutedEventArgs e)
        {
            await Windows.UI.ViewManagement.StatusBar.GetForCurrentView().ShowAsync();
            var progInd = Windows.UI.ViewManagement.StatusBar.GetForCurrentView().ProgressIndicator;
            progInd.Text = "Подождите, идёт загрузка расписания";
            await progInd.ShowAsync();

            for (int i = 0; i < 6; i++)
            {
                Premds[i].Text = "Загрузка ... ";
                Details[i].Text = string.Empty;
            }

            DateTime T = DateTime.Now;
            Y = T.Year;
            M = T.Month;
            D = T.Day;
            TxtDate.Text = HelperClass.NameGroup + "\n" + T.ToString("dd MMMM yyyy", new CultureInfo("ru-RU")) + "\n" + T.ToString("dddd", new CultureInfo("ru-RU"));

            await FillRasp();

            await progInd.HideAsync();
        }

        // на выбранную дату
        private async void RaspDatePicker_DatePicked(DatePickerFlyout sender, DatePickedEventArgs args)
        {
            await Windows.UI.ViewManagement.StatusBar.GetForCurrentView().ShowAsync();
            var progInd = Windows.UI.ViewManagement.StatusBar.GetForCurrentView().ProgressIndicator;
            progInd.Text = "Подождите, идёт загрузка расписания";
            await progInd.ShowAsync();

            for (int i = 0; i < 6; i++)
            {
                Premds[i].Text = "Загрузка ... ";
                Details[i].Text = string.Empty;
            }

            DateTime T = args.NewDate.Date;
            Y = T.Year;
            M = T.Month;
            D = T.Day;
            TxtDate.Text = HelperClass.NameGroup + "\n" + T.ToString("dd MMMM yyyy", new CultureInfo("ru-RU")) + "\n" + T.ToString("dddd", new CultureInfo("ru-RU"));

            await FillRasp();

            await progInd.HideAsync();
        }

        // сохранить настройку
        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            ApplicationData.Current.LocalSettings.Values["IgnoreHolydays"] = IgnoreHolydaysToggle.IsOn;

            if (ApplicationData.Current.LocalSettings.Values["GrName"].ToString() != "0")
            {
                ApplicationData.Current.LocalSettings.Values["GrId"] = HelperClass.IdGrupp;
                ApplicationData.Current.LocalSettings.Values["GrName"] = HelperClass.NameGroup;
            }

            SettingsBtn.Flyout.Hide();
        }

        // получить настройку
        public void ReadSettings()
        {
            ApplicationDataContainer ADC = ApplicationData.Current.LocalSettings;
            if (ADC.Values.Keys.Contains("IgnoreHolydays"))
            {
                IgnoreHolydaysToggle.IsOn = Convert.ToBoolean(ADC.Values["IgnoreHolydays"]);
            }
            else
            {
                ADC.Values["IgnoreHolydays"] = true;
            }

            SelestGrupp.Text = "Выбрано: " + HelperClass.NameGroup.Substring(0,1).ToLower() + HelperClass.NameGroup.Substring(1) + ".";

            if (ADC.Values.Keys.Contains("GrName"))
            {
                if (ADC.Values["GrName"].ToString() == "0")
                    DefaultGrupp.Text = "Группа по умолчанию: не задана.";
                else
                {
                    string GrName = ADC.Values["GrName"].ToString();
                    DefaultGrupp.Text = "Выбрано по умолчанию: " + GrName.Substring(0, 1).ToLower() + GrName.Substring(1) + ".";
                }
            }
            else
            {
                DefaultGrupp.Text = "Группа по умолчанию: не задана.";
            }

        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            ApplicationData.Current.LocalSettings.Values["GrId"] = HelperClass.IdGrupp;
            ApplicationData.Current.LocalSettings.Values["GrName"] = HelperClass.NameGroup;
            DefaultGrupp.Text = "Выбрано по умолчанию: " + HelperClass.NameGroup.Substring(0, 1).ToLower() + HelperClass.NameGroup.Substring(1) + ".";
        }

        // group reset
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            ApplicationData.Current.LocalSettings.Values["GrName"] = "0";
            DefaultGrupp.Text = "Группа по умолчанию: не задана.";
        }

        private void SettingsBtn2_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(MainPage));
        }
    }
}
