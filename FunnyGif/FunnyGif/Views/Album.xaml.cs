using Microsoft.Toolkit.Uwp;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Http;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace FunnyGif.Views
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class Album : Page
    {
        IncrementalLoadingCollection<PhotosSource, Photo> photos { get; set; }

        public Album()
        {
            photos = new IncrementalLoadingCollection<PhotosSource, Photo>(10);
            this.InitializeComponent();
        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            photos.RefreshAsync();
        }
    }

    public class Photo
    {
        public String Cover { get; set; }

        public String Title { get; set; }

        public List<String> Urls { get; set; }
    }

    public class PhotosSource : IIncrementalSource<Photo>
    {
        private readonly List<Photo> photos;

        public PhotosSource()
        {
            photos = new List<Photo>();
        }


        public async Task<IEnumerable<Photo>> GetPagedItemsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default(CancellationToken))
        {
            var photos = new List<Photo>();

            var httpClient = new HttpClient();
            var requestStr = "http://funnygif.lihulab.net/news/photos?page=" + (pageIndex + 1);
            var requestUri = new Uri(requestStr);

            try
            {
                //var httpClient = new HttpClient();
                var httpResponse = await httpClient.GetAsync(requestUri);

                if (httpResponse.StatusCode == HttpStatusCode.NotFound)
                {
                    return photos;
                }

                //httpResponse.EnsureSuccessStatusCode();
                var bytes = await httpResponse.Content.ReadAsBufferAsync();
                var properEncodedString = Encoding.UTF8.GetString(bytes.ToArray());

                JObject res = JObject.Parse(properEncodedString);
                var results = res["results"].Children().ToList();
                var serverResults = new List<ServerResult>();
                foreach (var result in results)
                {
                    ServerResult serverResult = result.ToObject<ServerResult>();
                    serverResults.Add(serverResult);
                }

                foreach (var result in serverResults)
                {
                    var item = new Photo();
                    item.Cover = result.imglink;
                    item.Title = result.title;
                    var imgurls = result.content.Split(' ');
                    item.Urls = new List<string>();
                    foreach (var url in imgurls)
                    {
                        if (url.Length > 10)
                            item.Urls.Add(url);
                    }
                    photos.Add(item);
                }

            }
            catch (Exception ex)
            {
                var dialog = new MessageDialog("发生网络错误。若重复出现，联系作者修复。");
                await dialog.ShowAsync();
            }

            return photos;
        }

        public class ServerResult
        {
            public string title { get; set; }
            public string imglink { get; set; }
            public string content { get; set; }
        }
    }
}
