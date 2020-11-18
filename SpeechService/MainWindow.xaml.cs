using System;
using System.Windows;
using System.Threading;
using System.Threading.Tasks;

namespace SpeechService
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {


        public MainWindow()
        {
            InitializeComponent();
            LanguageVoice languageSet = LanguageVoice.GetInstance();
            this.DataContext = languageSet;
        }

        #region Events


        private CancellationTokenSource _cts;
        private async void BtnPlay_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _cts = new CancellationTokenSource();
                progressBar1.IsIndeterminate = true;
                btnPlay.IsEnabled = false;
                await TextToSpeech.PlayTheTextAsync();

                progressBar1.IsIndeterminate = false;
            }
            catch (Exception ex)
            {
                this.Dispatcher.Invoke(() =>
                {
                    outputMessage.Text = "Error: " + ex.Message;
                });
            }
            finally
            {
                btnPlay.IsEnabled = true;
            }

        }

        private async void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            await TextToSpeech.SaveTheWavOrMp3();
        }

        private void SliderSpeed_Loaded(object sender, RoutedEventArgs e)
        {
            sliderSpeed.Value = 1;
        }

        private void SliderPitch_Loaded(object sender, RoutedEventArgs e)
        {
            sliderPitch.Value = 0;
        }

        private void ComboVoice_Loaded(object sender, RoutedEventArgs e)
        {
            comboVoice.SelectedIndex = 0;
        }

        private void ComboLanguage_Loaded(object sender, RoutedEventArgs e)
        {
            comboLanguage.SelectedIndex = 0;
        }

        #endregion

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            if (TextToSpeech.synthesizer != null)
            {
                TextToSpeech.synthesizer.StopSpeakingAsync();
                this.Dispatcher.Invoke(() =>
                {
                    outputMessage.Text = "Cancel speak";
                });
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            if (_cts != null)
            {
                _cts.Dispose();
            }
            GC.SuppressFinalize(this);
        }

        private async void btn_SpeechStart_Click(object sender, RoutedEventArgs e)
        {
            btn_SpeechStart.IsEnabled = false;
            try
            {
                var sr = comboInputSource.SelectedValue.ToString();

                if (sr == "Microphone speak")
                {
                    await SpeechToText.ContinuousRecognitionMicrophone();
                }
                else
                {
                    await SpeechToText.ContinuousRecognitionWithFileAsync();
                }
            }catch(Exception ex)
            {
                this.Dispatcher.Invoke(() => { outputMessage.Text = "Error: " + ex.Message; });
            }
            finally
            {
                btn_SpeechStart.IsEnabled = true;
            }
            
        }

        private void Btn_SpeechStop_Click(object sender, RoutedEventArgs e)
        {
            SpeechToText.recognizer.StopContinuousRecognitionAsync();
        }
    }
}
