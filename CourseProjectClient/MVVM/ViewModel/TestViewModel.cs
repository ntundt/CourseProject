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
                PropertyChanged(this, new PropertyChangedEventArgs("Attempt"));
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(Questions)));
            } 
        }

        private ObservableCollection<Question> _questions;
        public ObservableCollection<Question> Questions 
        { 
            get => _questions; 
            set
            {
                _questions = value;
                PropertyChanged(this, new PropertyChangedEventArgs("Questions"));
            }
        }

        private Question _selectedQuestion;
        public Question SelectedQuestion 
        {
            get => _selectedQuestion;
            set
            {
                _selectedQuestion = value;
                PropertyChanged(this, new PropertyChangedEventArgs("SelectedQuestion"));
            } 
        }

        public string TimeLeft
        {
            get {
                if (_attempt.Ended == null) return "";
                TimeSpan timeLeft = _attempt.Ended.Value - DateTime.Now;
                return $"{timeLeft.TotalHours}:{timeLeft.TotalMinutes}:{timeLeft.TotalSeconds}";
            }
        }

        public bool HasTimeLimit
        {
            get => _attempt.Ended <= _attempt.Started;
        }

        public bool RadioButtonsVisible
        {
            get => _selectedQuestion.QuestionType == QuestionType.SingleChoise;
        }

        public bool CheckboxesVisible
        {
            get => _selectedQuestion.QuestionType == QuestionType.MultipleChoise;
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
                    Task.Run(async () => await CommunicationService.SaveAnswer(_attempt.Id, SelectedQuestion));
                }
                catch (AggregateException e) when (e.InnerException is DefaultException)
                {
                    (e.InnerException as DefaultException).ShowSnackBar();
                }
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
