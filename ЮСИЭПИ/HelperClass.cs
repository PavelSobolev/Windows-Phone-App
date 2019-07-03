using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Xml;
using Windows.Networking.Connectivity;

namespace ЮСИЭПИ
{
    public static class HelperClass
    {
        /// <summary>
        /// Индентификтор родительского уровня для второй страницы меню.
        /// </summary>
        public static string Parent2Id;

        /// <summary>
        /// Текст заголовка для второй страницы меню.
        /// </summary>
        public static string Parent2Text;

        /// <summary>
        /// Индентификтор родительского уровня для третьей страницы меню.
        /// </summary>
        public static string Parent3Id;

        /// <summary>
        /// Текст заголовка для третьей страницы меню.
        /// </summary>
        public static string Parent3Text;

        /// <summary>
        /// Индентификтор родительского уровня для четвертой страницы меню.
        /// </summary>
        public static string Parent4Id;

        /// <summary>
        /// Текст заголовка для четвертой страницы меню.
        /// </summary>
        public static string Parent4Text;

        /// <summary>
        /// Индетификатор отображаемой страницы в браузере.
        /// </summary>
        public static string PageId;

        /// <summary>
        /// Номер года, выбранного в архиве новостей.
        /// </summary>
        public static string NewsYear;

        /// <summary>
        /// Номер месяца, выбранного в архиве новостей.
        /// </summary>
        public static string NewsMonth;

        /// <summary>
        /// Название месяца, выбранного в архиве новостей.
        /// </summary>
        public static string NewsMonthName;

        /// <summary>
        /// Индентификатор выбранного факультета для формаирования списка групп.
        /// </summary>
        public static string IdFacultet;

        /// <summary>
        /// Индентификатор выбранного факультета для формамирования списка расписания.
        /// </summary>
        public static string IdGrupp;

        /// <summary>
        /// Индентификатор выбранной группы для формирования списка.
        /// </summary>
        public static string NameGroup;

        /// <summary>
        /// Индетнификатор выбранного факультета для формирования расписания преподавателя.
        /// </summary>
        public static string IdPrepodavatel;

        /// <summary>
        /// Индентификатор выбранного преподавателя.
        /// </summary>
        public static string PrepodName;

        /// <summary>
        /// Страница названия факультета.
        /// </summary>
        public static string NameFacultet;

        /// <summary>
        /// Подсказка.
        /// </summary>
        /// <param name="XMLPath"></param>
        /// <returns></returns>

        public async static Task<XmlReader> GetXmlReader(string XMLPath)
        {
            WebRequest request = WebRequest.Create(XMLPath);
            request.Credentials = CredentialCache.DefaultCredentials;
            WebResponse response = await request.GetResponseAsync();
            Stream dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();

            StringReader Sr = new StringReader(responseFromServer);
            XmlReader xmlreader = XmlReader.Create(Sr);
            return xmlreader;
        }

        public static bool CheckConnection()
        {
            // Проверка соединения с сетью интернет.
            try
            {
                ConnectionProfile CP = NetworkInformation.GetInternetConnectionProfile();
                if (CP.GetNetworkConnectivityLevel() == NetworkConnectivityLevel.None)
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
    }
}
