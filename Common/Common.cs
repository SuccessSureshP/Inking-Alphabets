using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace InkingAlphabets
{
    public static class Common
    {
        static IPropertySet _localSettings = Windows.Storage.ApplicationData.Current.LocalSettings.Values;
        // The method traverses the visual tree lazily, layer by layer
        // and returns the objects of the desired type        
        public static IEnumerable<T> GetChildrenOfType<T>(this DependencyObject start) where T : class
        {
            var queue = new Queue<DependencyObject>();
            queue.Enqueue(start);

            while (queue.Count > 0)
            {
                var item = queue.Dequeue();

                var realItem = item as T;
                if (realItem != null)
                {
                    yield return realItem;
                }

                int count = VisualTreeHelper.GetChildrenCount(item);
                for (int i = 0; i < count; i++)
                {
                    queue.Enqueue(VisualTreeHelper.GetChild(item, i));
                }
            }
        }
        
        public static void SetLocalSettingValue(string key, object value)
        {
            _localSettings[key] = value;
        }


        public static object GetLocalSettingValue(string key)
        {
            object result;

            if (_localSettings.Keys.Contains(key))
                result = _localSettings[key].ToString();
            else
            {
                _localSettings[key] = GetDefaultValueforLocalSetting(key);
                result = _localSettings[key];
            }
            return result;
        }

        private static object GetDefaultValueforLocalSetting(string key)
        {
            object defaultValue = null; 
            switch(key)
            {
                case "InkingAlphabtsPenColor": defaultValue = "Blue";break;
                case "InkingAlphabtsPenSize":  defaultValue = 25; break;

                case "InkingAlphabtsHighlighterColor": defaultValue = "Orange"; break;
                case "InkingAlphabtsHighlighterSize": defaultValue = 35; break;

                case "InkingSlatePenColor": defaultValue = "Red"; break;
                case "InkingSlatePenSize": defaultValue = 10; break;

                case "InkingSlateHighlighterColor": defaultValue = "Lime"; break;
                case "InkingSlateHighlighterSize": defaultValue = 20; break;


                case "InkingWordsPenColor": defaultValue = "Green"; break;
                case "InkingWordsPenSize": defaultValue = 20; break;

                case "InkingWordsHighlighterColor": defaultValue = "BlueViolet"; break;
                case "InkingWordsHighlighterSize": defaultValue = 30; break;

                case "InkingWord": defaultValue = "అమూల్య"; break;
            }

            return defaultValue;
        }

        public static async Task CopyDefaultSlateBackgroundImage()
        {
            try
            {
                var sourceFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
                var destnationFolder = ApplicationData.Current.LocalFolder;
                await destnationFolder.CreateFolderAsync(App.AssetsFolderName, CreationCollisionOption.ReplaceExisting);
                File.Copy(Path.Combine(sourceFolder.Path, App.AssetsFolderName, App.InkingSlateBackgroundFileName), Path.Combine(destnationFolder.Path, App.AssetsFolderName, App.InkingSlateBackgroundFileName), true);
                _localSettings["SlateBackgroundImageStatus"] = "DefaultImageCopied";
            }
            catch (Exception exp)
            {
                Microsoft.HockeyApp.HockeyClient.Current.TrackEvent($"CopySlateBackgroundImage Failed with Exception:{exp.Message}");
                throw exp;
            }
        }
    }


    public class ColorClass
    {
        public Color Pencolor { get; set; }
        public string Name { set; get; }
    }
}
