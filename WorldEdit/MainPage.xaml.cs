using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Provider;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Документацию по шаблону элемента "Пустая страница" см. по адресу https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x419

namespace WorldEdit
{
    /// <summary>
    /// Пустая страница, которую можно использовать саму по себе или для перехода внутри фрейма.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }
        VMV vMV = new VMV();
        public async void ttt(IRandomAccessStream randAccStream1)
        {
           await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => {
                editor.TextDocument.LoadFromStream(TextSetOptions.CheckTextLimit, randAccStream1);
            });
            
        }
        private async void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            // Open a text file.
            Windows.Storage.Pickers.FileOpenPicker open =
                new Windows.Storage.Pickers.FileOpenPicker();
            open.SuggestedStartLocation =
                Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;
            open.FileTypeFilter.Add(".rtf");
            open.FileTypeFilter.Add(".txt");
            open.FileTypeFilter.Add(".dat");
            //open.FileTypeFilter.Add(".doc");

            Windows.Storage.StorageFile file = await open.PickSingleFileAsync();

            if (file != null)
            {
                vMV.File = file;
           
                try
                {
                    editor.Document.SetText(TextSetOptions.None, string.Empty);
                    if (file.FileType == ".rtf" || file.FileType == ".dat")
                    {
                        
                       // using (Windows.Storage.Streams.IRandomAccessStream randAccStream =
                        // await file.OpenAsync(Windows.Storage.FileAccessMode.Read))
                         //  {
                            Windows.Storage.Streams.IInputStream windowsRuntimeStream =
    await file.OpenAsync(Windows.Storage.FileAccessMode.Read);
                            // Load the file into the Document property of the RichEditBox.
                           // Stream managedStream = windowsRuntimeStream.AsStreamForRead(bufferSize: 100920);
                    
                  
                  

                        editor.Document.LoadFromStream(Windows.UI.Text.TextSetOptions.FormatRtf, windowsRuntimeStream.AsStreamForRead(bufferSize: 100920).AsRandomAccessStream());
                  

                        
                        //  }
                    }
                    else
                    {
                     
                   
                        // editor.Document.BatchDisplayUpdates();
                        editor.Document.SetText(Windows.UI.Text.TextSetOptions.UnicodeBidi, await FileIO.ReadTextAsync(file));
              
                        // editor.Document.ApplyDisplayUpdates();
                        /*  var buffer = await Windows.Storage.FileIO.ReadBufferAsync(file);
                          using (var dataReader = Windows.Storage.Streams.DataReader.FromBuffer(buffer))
                          {
                              MessageDialog messageDialog = new MessageDialog("hhh");
                             await messageDialog.ShowAsync();
                              //string text = dataReader.ReadString(buffer.Length);
                              editor.Document.SetText(Windows.UI.Text.TextSetOptions.None, dataReader.ReadString(buffer.Length));
                          }
                          */
                    }
                   

                }
                catch (Exception)
                {
                    ContentDialog errorDialog = new ContentDialog()
                    {
                        Title = "Ошибка открытия файла",
                        Content = "Sorry, I couldn't open the file.",
                        PrimaryButtonText = "Okей"
                    };

                    await errorDialog.ShowAsync();
                }
            }
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            Windows.Storage.Pickers.FileSavePicker savePicker = new Windows.Storage.Pickers.FileSavePicker();
            savePicker.SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary;

            // Dropdown of file types the user can save the file as
            savePicker.FileTypeChoices.Add("Rich Text", new List<string>() { ".rtf" });
            savePicker.FileTypeChoices.Add("Текстовый документ", new List<string>() { ".txt" });
            savePicker.FileTypeChoices.Add("DAT", new List<string>() { ".dat" });
            // Default file name if the user does not type one in or select a file to replace
            if (vMV.File != null)
            {


                savePicker.SuggestedFileName = vMV.File.DisplayName;
            }
            else
            {
                savePicker.SuggestedFileName = "NewDoc";
            }

            Windows.Storage.StorageFile file = await savePicker.PickSaveFileAsync();
            if (file != null)
            {
                 CachedFileManager.DeferUpdates(file);
                if(file.FileType==".rtf"|| file.FileType == ".dat")
                {
                    using (Windows.Storage.Streams.IRandomAccessStream randAccStream =
                 await file.OpenAsync(Windows.Storage.FileAccessMode.ReadWrite))
                    {
                        editor.Document.SaveToStream(Windows.UI.Text.TextGetOptions.FormatRtf, randAccStream);

                    }

                }
                // write to file
             else
                {
                    
                        editor.Document.GetText(TextGetOptions.None, out string h1);

                        await Windows.Storage.FileIO.WriteTextAsync(file, h1);
                    
                }
             

                // Let Windows know that we're finished changing the file so the 
                // other app can update the remote version of the file.
                FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(file);
                if (status != FileUpdateStatus.Complete)
                {
                    Windows.UI.Popups.MessageDialog errorBox =
                        new Windows.UI.Popups.MessageDialog("File " + file.Name + " couldn't be saved.");
                    await errorBox.ShowAsync();
                }

            }
        }
        private void UnderlineButton_Click(object sender, RoutedEventArgs e)
        {
            
            Windows.UI.Text.ITextSelection selectedText = editor.Document.Selection;
            if (selectedText != null)
            {
                Windows.UI.Text.ITextCharacterFormat charFormatting = selectedText.CharacterFormat;
                if (charFormatting.Underline == Windows.UI.Text.UnderlineType.None)
                {
                    charFormatting.Underline = Windows.UI.Text.UnderlineType.Single;
                }
                else
                {
                    charFormatting.Underline = Windows.UI.Text.UnderlineType.None;
                }
                selectedText.CharacterFormat = charFormatting;
            }
        }
        private void BoldButton_Click(object sender, RoutedEventArgs e)
        {
            editor.Document.Selection.CharacterFormat.Bold = FormatEffect.Toggle;
        }

        private void ItalicButton_Click(object sender, RoutedEventArgs e)
        {
            editor.Document.Selection.CharacterFormat.Italic = FormatEffect.Toggle;
        }

        private void FindBoxHighlightMatches()
        {
            FindBoxRemoveHighlights();

            Color highlightBackgroundColor = (Color)App.Current.Resources["SystemColorHighlightColor"];
            Color highlightForegroundColor = (Color)App.Current.Resources["SystemColorHighlightTextColor"];

            string textToFind = findBox.Text;
            if (textToFind != null)
            {
                ITextRange searchRange = editor.Document.GetRange(0, 0);
                while (searchRange.FindText(textToFind, TextConstants.MaxUnitCount, FindOptions.None) > 0)
                {
                    searchRange.CharacterFormat.BackgroundColor = highlightBackgroundColor;
                    searchRange.CharacterFormat.ForegroundColor = highlightForegroundColor;
                }
            }
        }

        private void FindBoxRemoveHighlights()
        {
            ITextRange documentRange = editor.Document.GetRange(0, TextConstants.MaxUnitCount);
            SolidColorBrush defaultBackground = editor.Background as SolidColorBrush;
            SolidColorBrush defaultForeground = editor.Foreground as SolidColorBrush;

            documentRange.CharacterFormat.BackgroundColor = defaultBackground.Color;
            documentRange.CharacterFormat.ForegroundColor = defaultForeground.Color;
        }
        private void Editor_GotFocus(object sender, RoutedEventArgs e)
        {
            // reset colors to correct defaults for Focused state
            ITextRange documentRange = editor.Document.GetRange(0, TextConstants.MaxUnitCount);
            SolidColorBrush background = (SolidColorBrush)App.Current.Resources["TextControlBackgroundFocused"];
            SolidColorBrush foreground = (SolidColorBrush)App.Current.Resources["TextControlForegroundFocused"];

            if (background != null && foreground != null)
            {
                documentRange.CharacterFormat.BackgroundColor = background.Color;
                documentRange.CharacterFormat.ForegroundColor = foreground.Color;
            }
        }

      

       

        private async void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            if (vMV.File != null)
            {


                CachedFileManager.DeferUpdates(vMV.File);
                if (vMV.File.FileType == ".rtf")
                {
                    using (Windows.Storage.Streams.IRandomAccessStream randAccStream =
                 await vMV.File.OpenAsync(Windows.Storage.FileAccessMode.ReadWrite))
                    {
                        editor.Document.SaveToStream(Windows.UI.Text.TextGetOptions.FormatRtf, randAccStream);

                    }

                }
                // write to file
                else
                {

                    editor.Document.GetText(TextGetOptions.None, out string h1);

                    await Windows.Storage.FileIO.WriteTextAsync(vMV.File, h1);

                }


                // Let Windows know that we're finished changing the file so the 
                // other app can update the remote version of the file.
                FileUpdateStatus status = await CachedFileManager.CompleteUpdatesAsync(vMV.File);
                if (status != FileUpdateStatus.Complete)
                {
                    Windows.UI.Popups.MessageDialog errorBox =
                        new Windows.UI.Popups.MessageDialog("File " + vMV.File.Name + " couldn't be saved.");
                    await errorBox.ShowAsync();
                }
            }
            else
            {
                SaveButton_Click(null, null);
            }

        }

        private void MenuFlyoutItem_Click_1(object sender, RoutedEventArgs e)
        {
            Application.Current.Exit();
        }
    }
}
