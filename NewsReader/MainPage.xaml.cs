﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace NewsReader
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        ObservableCollection<NewsItem> ListNewsItemCollection = new ObservableCollection<NewsItem>();
        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;
        }

        /// <summary>
        /// Khởi chạy khi mở úng dụng
        /// async bất đồng bộ
        /// gặp await -> đứng lại đợi đến khi hàm thực hiện xong
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: Prepare page for display here.

            // TODO: If your application contains multiple pages, ensure that you are
            // handling the hardware Back button by registering for the
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
            // If you are using the NavigationHelper provided by some templates,
            // this event is handled for you.



            //ObservableCollection giống như list
            //list không có sự kiện tương tác với người dùng, khi đã Binding nếu list thay đổi -> lỗi
            //ObservableCollection hỗ trợ thay đổi list sau khi binding
            var ListCattegory = new ObservableCollection<CattegoryModel>();
            ListCattegory.Add(new CattegoryModel()
            {
                title = "Tin mới nhất",
                links = "http://vnexpress.net/rss/tin-moi-nhat.rss",
                NewsItems = new ObservableCollection<NewsItem>()
            });

            ListCattegory.Add(new CattegoryModel()
            {
                title = "Thế giới",
                links = "http://vnexpress.net/rss/the-gioi.rss",
                NewsItems = new ObservableCollection<NewsItem>()
            });

            ListCattegory.Add(new CattegoryModel()
            {
                title = "Kinh doanh",
                links = "http://vnexpress.net/rss/kinh-doanh.rss",
                NewsItems = new ObservableCollection<NewsItem>()
            });

            ListCattegory.Add(new CattegoryModel()
            {
                title = "Giải trí",
                links = "http://vnexpress.net/rss/giai-tri.rss",
                NewsItems = new ObservableCollection<NewsItem>()
            });

            ListCattegory.Add(new CattegoryModel()
            {
                title = "Pháp luật",
                links = "http://vnexpress.net/rss/phap-luat.rss",
                NewsItems = new ObservableCollection<NewsItem>()
            });

            ListCattegory.Add(new CattegoryModel()
            {
                title = "Giáo dục",
                links = "http://vnexpress.net/rss/giao-duc.rss",
                NewsItems = new ObservableCollection<NewsItem>()
            });

            ListCattegory.Add(new CattegoryModel()
            {
                title = "Sức khỏe",
                links = "http://vnexpress.net/rss/suc-khoe.rss",
                NewsItems = new ObservableCollection<NewsItem>()
            });

            ListCattegory.Add(new CattegoryModel()
            {
                title = "Gia đình",
                links = "http://vnexpress.net/rss/gia-dinh.rss",
                NewsItems = new ObservableCollection<NewsItem>()
            });

            ListCattegory.Add(new CattegoryModel()
            {
                title = "Du lịch",
                links = "http://vnexpress.net/rss/du-lich.rss",
                NewsItems = new ObservableCollection<NewsItem>()
            });

            ListCattegory.Add(new CattegoryModel()
            {
                title = "Khoa học",
                links = "http://vnexpress.net/rss/khoa-hoc.rss",
                NewsItems = new ObservableCollection<NewsItem>()
            });

            ListCattegory.Add(new CattegoryModel()
            {
                title = "Số hóa",
                links = "http://vnexpress.net/rss/so-hoa.rss",
                NewsItems = new ObservableCollection<NewsItem>()
            });

            ListCattegory.Add(new CattegoryModel()
            {
                title = "Xe",
                links = "http://vnexpress.net/rss/oto-xe-may.rss",
                NewsItems = new ObservableCollection<NewsItem>()
            });

            ListCattegory.Add(new CattegoryModel()
            {
                title = "Cộng đồng",
                links = "http://vnexpress.net/rss/cong-dong.rss",
                NewsItems = new ObservableCollection<NewsItem>()
            });


            //set source để Binding cho ControlPovot
            ControlPivot.ItemsSource = ListCattegory;


            //Gọi đến Local Folder chứa các cache file
            var _LocalFolder = ApplicationData.Current.LocalFolder;

            bool acceptOldData = false;

            //Lay du lieu tu RSS
            foreach(var CattegoryModel in ListCattegory)
            {
                //với mỗi category, đọc file xml rss và add các item vào listItem của category đó
                var httpClient = new HttpClient();

                // Ham co Async se doi ham thuc hien xong r ms chay xuong doi
                //tao file cache; mỗi category một file
                var fileCache = await _LocalFolder.CreateFileAsync(CattegoryModel.title, CreationCollisionOption.OpenIfExists);
                
                string strRSS = await Helper.readFile(fileCache);

                var properties = await fileCache.GetBasicPropertiesAsync();
                if (Helper.IsConnectedToInternet())
                {
                    //có kết nối mạng
                    //check xem file cache đã cũ chưa
                    if ((DateTime.Now - properties.DateModified).TotalMinutes > 5 )
                    {
                        //đã cũ, load file mới
                        strRSS = await httpClient.GetStringAsync(CattegoryModel.links);
                        await Helper.writeFile(fileCache, strRSS);  //cache
                    }
                }
                else
                {
                    //không có kết nối mạng
                    //kiem tra co file cahe
                    if (string.IsNullOrEmpty(strRSS))
                    {
                        //thong bao va thoat
                        var msg = new MessageDialog("Không có kết nối mạng, không có dữ liệu cũ, ứng dụng sẽ thoát").ShowAsync();
                        Application.Current.Exit();
                    }
                    else
                    {
                        //thong bao load file cache cu
                        if (!acceptOldData)
                        {
                            bool isAccept = false;
                            var msg = new MessageDialog("Không thể kết nối đến máy chủ, sử dụng dữ liệu cũ ?", "Opp!");
                            msg.Commands.Add(new UICommand("Chấp nhận", command => { isAccept = true; }));
                            msg.Commands.Add(new UICommand("Hủy bỏ", command => { isAccept = false; }));    //hủy bỏ -> thoát
                            msg.Options = MessageDialogOptions.AcceptUserInputAfterDelay;
                            await msg.ShowAsync();
                            if (!isAccept)
                            {
                                Application.Current.Exit();
                            }
                            else
                            {
                                acceptOldData = true;
                            }
                        }
                        
                    }

                }
                


                //chuyen chuoi strRSS sang dinh danh file xml

                var DocXML = XDocument.Parse(strRSS);
                try
                {
                    //Descendants: lấy file xml, đọc các "item", gom các item vào 1 list
                    var ListNews = from item in DocXML.Descendants("item")
                                   //sử dụng Linq
                                   select new NewsItem()
                                   {
                                       Title = item.Element("title").Value,
                                       Description = item.Element("description").Value,
                                       Link = item.Element("link").Value,
                                       Thumb = GetLinkThumb(item.Element("description").Value)
                                   };
                    foreach (var newsitem in ListNews)
                    {
                        CattegoryModel.NewsItems.Add(newsitem);
                    }
                }
                catch (Exception a)
                {

                }
            
            }
            //Load xong du lieu
            LoadScreen.Visibility = Visibility.Collapsed;
        }

        

        public string GetLinkThumb( string StrContent)
        {
            var link = "";
            //Sử dụng regex để lấy chính xác đoạn chuỗi
            var match = Regex.Match(StrContent, "<img.*?src=\"(.*?)\".*?>", RegexOptions.IgnoreCase);
            if(match.Groups.Count > 0)
            {
                link = match.Groups[1].Value;
            }
            return link;
        }

        private async void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }


        //Tất cả Item trong list đều có hàm sự kiện selection change
        //sender đại diện cho đối tượng gây ra sự kiện
        //ở đây đại diện cho listbox mà người dùng nhấn vào 
        private void ControlPivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                //ép kiểu sender thành listbox
                var newsItem = (sender as ListBox).SelectedItem as NewsItem;    //đại diện cho bài báo ng dùng nhấn
                if (newsItem != null)
                {
                    //Truyền đối tượng sang frame khác
                    Frame.Navigate(typeof(ReadingPage), newsItem);
                }
            }
            catch (Exception a) { };
            
        }

        private void TinMoiNhatListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
           
        }

        private void ThoiSuListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }
    }
}
