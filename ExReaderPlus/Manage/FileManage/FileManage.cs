﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using ExReaderPlus.Manage.PassageManager;
using Windows.Storage;
using System.IO;
using System.Diagnostics;
using System.Numerics;
using Windows.ApplicationModel.Core;
using Windows.Storage.Pickers;
using Windows.System;
using Windows.UI;
using Windows.UI.Core;
using ExReaderPlus.Manage.ReaderManager;
using Windows.UI.Notifications;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using ExReaderPlus.View;
using ExReaderPlus.ViewModels;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Text;


namespace ExReaderPlus.FileManage {
   /// <summary>
   /// File
   /// </summary>
   

    public class FileManage
    {
        private EssayPageViewModel viewModel;
        private static FileManage _instence;
        
        public static FileManage Instence {
           get {
                if (_instence == null)
                    _instence = new FileManage();
                return _instence;
            }
        }

        //序列化
        public async void SerializeFile(ReaderManage reader)
        {
            DataContractSerializer serializer = new DataContractSerializer(typeof(ReaderManage));
            var savePicker = new FileSavePicker();
            savePicker.SuggestedStartLocation = PickerLocationId.MusicLibrary;
            // Dropdown of file types the user can save the file as
            savePicker.FileTypeChoices.Add("exReader文件", new List<string>() { ".xread" });
            // Default file name if the user does not type one in or select a file to replace
            savePicker.SuggestedFileName = "xPassage";
            StorageFile file = await savePicker.PickSaveFileAsync();
            if (file != null)
            {
                // Prevent updates to the remote version of the file until
                // we finish making changes and call CompleteUpdatesAsync.
                CachedFileManager.DeferUpdates(file);
                // write to file
                var stream = await file.OpenStreamForWriteAsync();
                Debug.WriteLine("write stream: " + stream.ToString());
                serializer.WriteObject(stream, reader);

                Windows.Storage.Provider.FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(file);
                ShowToastNotification("exReader提示", "成功导出工程文件!");
                if (status == Windows.Storage.Provider.FileUpdateStatus.Complete)
                {
                    //.textBlock.Text = "File " + file.Name + " was saved.";
                }
                else
                {
                    //this.textBlock.Text = "File " + file.Name + " couldn't be saved.";
                }
            }
            else
            {
                //this.textBlock.Text = "Operation cancelled.";
            }
        }

        //反序列化
        public async Task<Passage> DeSerializeFile()
        {

            DataContractSerializer deserializer = new DataContractSerializer(typeof(ReaderManage));
            
            Passage passage = new Passage();
            var picker = new FileOpenPicker();
            picker.ViewMode = PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = PickerLocationId.MusicLibrary;
            picker.FileTypeFilter.Add(".txt");
            
            // TODO:
            //picker.FileTypeFilter.Add(".pdf");

            StorageFile storageFile = await picker.PickSingleFileAsync();
            if (storageFile != null)
            {
                var stream = await storageFile.OpenStreamForReadAsync();

                passage.Content = await FileIO.ReadTextAsync(storageFile);
                passage.HeadName = storageFile.DisplayName;


                return passage;
            }
            else
            {
                return null;
            }

        }

        //显示Toast通知
        private void ShowToastNotification(string title, string stringContent)
        {
            ToastNotifier ToastNotifier = ToastNotificationManager.CreateToastNotifier();
            Windows.Data.Xml.Dom.XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastText02);
            Windows.Data.Xml.Dom.XmlNodeList toastNodeList = toastXml.GetElementsByTagName("text");
            toastNodeList.Item(0).AppendChild(toastXml.CreateTextNode(title));
            toastNodeList.Item(1).AppendChild(toastXml.CreateTextNode(stringContent));
            Windows.Data.Xml.Dom.IXmlNode toastNode = toastXml.SelectSingleNode("/toast");
            Windows.Data.Xml.Dom.XmlElement audio = toastXml.CreateElement("audio");
            audio.SetAttribute("src", "ms-winsoundevent:Notification.SMS");

            ToastNotification toast = new ToastNotification(toastXml);
            toast.ExpirationTime = DateTime.Now.AddSeconds(4);
            ToastNotifier.Show(toast);
        }

        /// <summary>
        /// 用于网页分享功能
        /// </summary>
        public async void ShareData()
        {


            //从文本框获取文章内容
            var uri = new Uri(@"https://sns.qzone.qq.com/cgi-bin/qzshare/cgi_qzshare_onekey?url=http%3A%2F%2Fv.ExreaderPlus.com%2Fvideo%2F3250296&desc=I+find+a+pretty+good+passage+in+ExreaderPlus+you+should+not+miss+it+");

            var success = await Launcher.LaunchUriAsync(uri);
            if (success)
            {
                // 如果你感兴趣，可以在成功启动后在这里执行一些操作。

                ShowToastNotification("ExReaderPlus提示", "分享成功");

            }
            else
            {
                // 如果你感兴趣，可以在这里处理启动失败的一些情况。
                ShowToastNotification("ExReaderPlus提示", "分享失败");
            }

        }

        /// <summary>
        /// win2d对图片写字
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public async Task Win2DTask(string str)
        {
            var pick = new FileOpenPicker();
            pick.FileTypeFilter.Add(".jpg");
            pick.FileTypeFilter.Add(".png");

            var file = await pick.PickSingleFileAsync();
            var duvDbecdgiu =
                await CanvasBitmap.LoadAsync(new CanvasDevice(true), await file.OpenAsync(FileAccessMode.Read));
            var canvasRenderTarget = new CanvasRenderTarget(duvDbecdgiu, duvDbecdgiu.Size);
            
            using (var dc = canvasRenderTarget.CreateDrawingSession())//用后则需撤销
            {

                ///先将图片读取
                dc.DrawImage(duvDbecdgiu);
                ///写图片
                dc.DrawText(str,
                    100,100,1700,50,
                    Colors.Black, new CanvasTextFormat()
                    {
                        FontSize = 50
                    });
            }


            ShowToastNotification("图片已保存成功","请选择图片保存位置");

            var pick1 = new FileSavePicker();
            pick1.FileTypeChoices.Add("image", new List<string>() { ".png" });

            var file1 = await pick1.PickSaveFileAsync();

            await canvasRenderTarget.SaveAsync(await file1.OpenAsync(FileAccessMode.ReadWrite), CanvasBitmapFileFormat.Png);

//            CoreApplicationView newView = CoreApplication.CreateNewView();
//
//            int newViewId = 0;
//
//            await newView.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
//
//            {
//
//                Frame frame = new Frame();
//
//                frame.Navigate(typeof(MainPageViewModel), null);
//
//                Window.Current.Content = frame;
//
//                // You have to activate the window in order to show it later.
//
//                Window.Current.Activate();
//
//
//
//                newViewId = ApplicationView.GetForCurrentView().Id;
//
//            });
//
//            bool viewShown = await ApplicationViewSwitcher.TryShowAsStandaloneAsync(newViewId);
        }


                 

        

    }
}
