using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Collections;
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
                case "InkingAlphabtsPenSize":  defaultValue = 10; break;

                case "InkingAlphabtsHighlighterColor": defaultValue = "Yellow"; break;
                case "InkingAlphabtsHighlighterSize": defaultValue = 20; break;

                case "InkingSlatePenColor": defaultValue = "Red"; break;
                case "InkingSlatePenSize": defaultValue = 10; break;

                case "InkingSlateHighlighterColor": defaultValue = "Green"; break;
                case "InkingSlateHighlighterSize": defaultValue = 20; break;


                case "InkingWordsPenColor": defaultValue = "Green"; break;
                case "InkingWordsPenSize": defaultValue = 10; break;

                case "InkingWordsHighlighterColor": defaultValue = "Brown"; break;
                case "InkingWordsHighlighterSize": defaultValue = 20; break;
            }

            return defaultValue;
        }

    }


    public class ColorClass
    {
        public Color Pencolor { get; set; }
        public string Name { set; get; }
    }
}
