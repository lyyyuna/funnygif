using Microsoft.Toolkit.Uwp;
using Microsoft.Toolkit.Uwp.UI.Animations;
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
    public sealed partial class Gank : Page
    {
        IncrementalLoadingCollection<GPhotosSource, GPhoto> gphotos { get; set; }
        public Gank()
        {
            gphotos = new IncrementalLoadingCollection<GPhotosSource, GPhoto>(30);
            this.InitializeComponent();
        }

        private async void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            FlipGrid.Visibility = Visibility.Visible;
            await FlipGrid.Fade(1).StartAsync();
        }

        private void FlipView_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var pt = e.GetPosition(imageFlipView);

            if (pt.X < 50 || pt.Y < 50 ||
               imageFlipView.ActualWidth - pt.X < 50 ||
                imageFlipView.ActualHeight - pt.Y < 50) return;

            hideFlipView();
        }

        private async void hideFlipView()
        {
            await FlipGrid.Fade(0).StartAsync();
            FlipGrid.Visibility = Visibility.Collapsed;
        }
    }

    public class GPhoto
    {
        public String Cover { get; set; }

    }

    public class GPhotosSource : IIncrementalSource<GPhoto>
    {
        private readonly List<GPhoto> photos;

        public GPhotosSource()
        {
            photos = new List<GPhoto>();
        }


        public async Task<IEnumerable<GPhoto>> GetPagedItemsAsync(int pageIndex, int pageSize, CancellationToken cancellationToken = default(CancellationToken))
        {
            var photos = new List<GPhoto>();

            var httpClient = new HttpClient();
            var requestStr = "http://gank.io/api/data/%E7%A6%8F%E5%88%A9/" + pageSize + "/" + (pageIndex + 1);
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
                    var item = new GPhoto();
                    item.Cover = result.url;

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
            public string url { get; set; }
        }
    }
}
