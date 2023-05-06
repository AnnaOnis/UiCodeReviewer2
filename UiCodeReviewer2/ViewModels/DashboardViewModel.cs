using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using OpenAI.ChatGpt;
using OpenAI.ChatGpt.Models.ChatCompletion.Messaging;
using System;
using System.IO;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using UiCodeReviewer2.Helpers;
using Wpf.Ui.Common.Interfaces;
using Wpf.Ui.Mvvm.Interfaces;

namespace UiCodeReviewer2.ViewModels
{
    public partial class DashboardViewModel : ObservableObject, INavigationAware
    {
        
        
        private string? _key;
        private OpenAiClient? _client;

        private string _configureMessage =
            "Я хочу чтобы вы провели качественный анализ кода " +
            "как опытный программист и преподаватель по программированию." +
            "Во-первых, убедитесь, что вы понимаете основные концепции языка программирования, на котором написан код, " +
            "который необходимо проверить. Это позволит вам определить возможные ошибки в синтаксисе, " +
            "а также проблемы в логике программы." +
            "Во-вторых, обратите внимание на читабельность кода. Читабельность кода важна для того, чтобы другие программисты " +
            "могли легко понимать, что происходит в коде, и вносить изменения в него. Обратите внимание на использование пробелов, " +
            "отступов, именования переменных и функций." +
            "В-третьих, проверьте наличие комментариев в коде. " +
            "Хорошие комментарии могут помочь другим программистам быстрее понять код, особенно если он сложный." +
            "В-четвертых, обратите внимание на эффективность кода. Это включает в себя проверку на наличие неэффективных циклов, " +
            "повторяющегося кода и других мест, которые могут привести к увеличению времени выполнения программы." +
            "Наконец, убедитесь, что код соответствует стандартам программирования. Это включает в себя правильное форматирование, " +
            "использование соответствующих библиотек и подходящих алгоритмов." +
            "Отвечайте четко и понятно для студентов. Отвечайте на русском языке.";

        [ObservableProperty]
        private string? _answer;

        [ObservableProperty]
        private string? _question;

        [ObservableProperty]
        private bool _isChecked;

        [ObservableProperty]
        private bool _shouldScrollToEnd;

        public void OnNavigatedTo()
        {
            Question = "Введи в это поле свой код или перетащи файл... А можешь просто задать вопрос))";
            LoadAPIKey();
            try
            {
                _client = new OpenAiClient(_key);
            }
            catch (ArgumentException)
            {
                Application.Current.Shutdown();
            }
        }

        public void OnNavigatedFrom()
        {
        }

        [RelayCommand]
        private void OnSaveAnswerToTextFile()
        {
            if (string.IsNullOrEmpty(Answer)) { throw new ArgumentNullException(nameof(Answer)); }

            SaveFileDialog dialog = new()
            {
                Filter = "Text Files(*.txt)|*.txt|All(*.*)|*"
            };

            if (dialog.ShowDialog() == true)
            {
                File.WriteAllText(dialog.FileName, Answer);
            }
        }

        [RelayCommand]
        private async Task OnSendMessage()
        {         
            
            if (string.IsNullOrEmpty(Question) || Question == "Введи в это поле свой код или перетащи файл... А можешь просто задать вопрос))")
            {
                Answer = "Вы не ввели вопрос!!!";
            }
            else
            {
                if(IsChecked == true)
                {
                    var dialog = Dialog.StartAsSystem(_configureMessage).ThenUser(Question);
                    Answer += $"\n\n{Question}\n\n";
                    Question = "";
                    await foreach (string chunk in _client!.StreamChatCompletions(dialog, maxTokens: 1000))
                    {
                        ShouldScrollToEnd = false;
                        Answer += chunk;
                        ShouldScrollToEnd = true;
                    }
                }
                else
                {
                    Answer += $"\n\n{Question}\n\n";
                   
                    await foreach (string chunk in _client!.StreamChatCompletions((UserMessage)Question, maxTokens: 1000))
                    {
                        ShouldScrollToEnd = false;
                        Answer += chunk;
                        ShouldScrollToEnd = true;
                    }
                    Question = "";
                }
                Answer += "\n----------------------------\n";
            }
            IsChecked = false;

        }

        [RelayCommand]
        private void OnDragOver(DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.All;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
            e.Handled = false;
        }

        [RelayCommand]
        private void OnDrop(DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files != null && files.Length > 0)
                {
                    Question = File.ReadAllText(files[0]);
                    e.Handled = true;
                }
            }
        }


        private void LoadAPIKey()
        {           
            _key = Environment.GetEnvironmentVariable("openai_api_key");
            if (string.IsNullOrEmpty(_key))
            {
                MessageBox.Show("OpenAI API ключ не найден!\nЕсли у вас есть ключ для доступа к OpenAI API,\n" +
                "создайте для него переменную окружения - \"openai_api_key\".\n" +
                "Если у вас нет ключа, то получите его пройдя регистрацию на https://platform.openai.com/.", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
