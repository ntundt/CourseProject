using CourseProjectClient.Exceptions;
using CourseProjectClient.MVVM.Model;
using CourseProjectClient.Services;
using DataTransferObject;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using WPFUI.Common;

namespace CourseProjectClient.MVVM.ViewModel
{
    internal class TestViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        public ICommand SaveAnswer { get; set; }
        public ICommand EndAttempt { get; set; }

        private Attempt _attempt;
        public Attempt Attempt
        { 
            get => _attempt; 
            set
            {
                _attempt = value;
                RetrieveQuestions();
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(Attempt)));
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(Questions)));
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(HasTimeLimit)));
                if (HasTimeLimit)
                {
                    DispatcherTimer timer = new DispatcherTimer(DispatcherPriority.Normal);
                    timer.Tick += delegate
                    {
                        if (_attempt.Ended.Value - DateTime.Now + new TimeSpan(3, 0, 0) 
                            < new TimeSpan(0, 0, -1))
                        {
                            var result = new AttemptResultViewModel();
                            result.SetAttemptId(_attempt.Id);
                            NavigationMediator.SetRootViewModel(result);
                            timer.Stop();
                        }
                        PropertyChanged(this, new PropertyChangedEventArgs(nameof(TimeLeft)));
                    };
                    timer.Interval = new TimeSpan(0, 0, 1);
                    timer.Start();
                }
            } 
        }

        private ObservableCollection<Question> _questions;
        public ObservableCollection<Question> Questions 
        { 
            get => _questions; 
            set
            {
                _questions = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(Questions)));
            }
        }

        private Question _selectedQuestion;
        public Question SelectedQuestion 
        {
            get => _selectedQuestion;
            set
            {
                _selectedQuestion = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(SelectedQuestion)));
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(RadioButtonsVisible)));
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(CheckboxesVisible)));
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(StringInputVisible)));
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(SelectedQuestion)));
            } 
        }

        public string TimeLeft
        {
            get {
                if (_attempt.Ended == null) return "";
                TimeSpan timeLeft = _attempt.Ended.Value - DateTime.Now + new TimeSpan(3, 0, 0);
                return $"{timeLeft.Hours}:{timeLeft.Minutes.ToString().PadLeft(2, '0')}:{timeLeft.Seconds.ToString().PadLeft(2, '0')}";
            }
        }

        public bool HasTimeLimit
        {
            get => _attempt.Ended > _attempt.Started;
        }

        public bool RadioButtonsVisible
        {
            get => _selectedQuestion.QuestionType == QuestionType.SingleChoice;
        }

        public bool CheckboxesVisible
        {
            get => _selectedQuestion.QuestionType == QuestionType.MultipleChoice;
        }

        public bool StringInputVisible
        {
            get => _selectedQuestion.QuestionType == QuestionType.StringInput;
        }

        private void RetrieveQuestions()
        {
            try
            {
                var questionsInfo = Task.Run<List<QuestionInfo>>(async () => await CommunicationService.GetAttemptQuestions(_attempt.Id)).Result;

                ObservableCollection<Question> questions = new ObservableCollection<Question>(
                    questionsInfo.Select(info => new Question
                    {
                        Id = info.QuestionId,
                        Text = info.Text,
                        Index = info.Index,
                        QuestionType = info.QuestionType,
                        CheckAlgorithm = info.CheckAlgorithm,
                        AnswerOptions = new ObservableCollection<AnswerOption>(info.AnswerOptions.Select(x => new AnswerOption
                        {
                            Id = x.AnswerId,
                            Text = x.Text,
                            IsChecked = x.Checked ?? false
                        }).ToList())
                    }).ToList()
                );

                Questions = questions;

                SelectedQuestion = questions[0];
            }
            catch (AggregateException e) when (e.InnerException is DefaultException)
            {
                (e.InnerException as DefaultException).ShowSnackBar();
            }
        }

        public TestViewModel()
        {
            SaveAnswer = new RelayCommand(() =>
            {
                try
                {
                    var _question = SelectedQuestion;
                    Task.Run(async () => await CommunicationService.SaveAnswer(_attempt.Id, _question));
                    SelectedQuestion.Unsaved = false;
                    int index = Questions.IndexOf(SelectedQuestion) + 1;
                    SelectedQuestion = Questions.Count > index ? Questions[index] : SelectedQuestion;
                }
                catch (AggregateException e) when (e.InnerException is DefaultException)
                {
                    (e.InnerException as DefaultException).ShowSnackBar();
                }
            }, () =>
            {
                return SelectedQuestion.Unsaved;
            });

            EndAttempt = new RelayCommand(() =>
            {
                try
                {
                    Task.Run(async () => await CommunicationService.EndTest(_attempt.Id));
                    NavigationMediator.SetRootViewModel(new TestListViewModel());
                }
                catch (AggregateException e) when (e.InnerException is DefaultException)
                {
                    (e.InnerException as DefaultException).ShowSnackBar();
                }
            });
        }
    }
}
