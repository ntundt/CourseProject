using CourseProjectClient.Exceptions;
using CourseProjectClient.MVVM.Model;
using CourseProjectClient.Services;
using DataTransferObject;
using GongSolutions.Wpf.DragDrop;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WPFUI.Common;

namespace CourseProjectClient.MVVM.ViewModel
{
    internal class TestEditViewModel : INotifyPropertyChanged, IDropTarget
    {
        public void DragOver(IDropInfo dropInfo) {
            Question question = dropInfo.TargetItem as Question;

            if (question != null)
            {
                dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
                dropInfo.Effects = DragDropEffects.Move;
            }
        }

        public void Drop(IDropInfo dropInfo) 
        {
            Question source = dropInfo.Data as Question;
            int oldIndex = Questions.IndexOf(source);

            // Если место, куда хотим вставить отличается от места, где вопрос был раньше
            if (oldIndex != dropInfo.InsertIndex && oldIndex + 1 != dropInfo.InsertIndex)
            {
                try
                {
                    // Сохранить изменение индекса вопроса на сервере
                    Task.Run(async () => await CommunicationService.SaveQuestion(Test.Id, source.Id, new PutQuestion
                    {
                        Index = oldIndex < dropInfo.InsertIndex ? dropInfo.InsertIndex : dropInfo.InsertIndex + 1
                    }));

                    // Обновить локальный список
                    SelectedQuestion = Questions.FirstOrDefault(x => x.Id != source.Id);

                    Questions.Remove(source);
                    try
                    {
                        Questions.Insert(oldIndex < dropInfo.InsertIndex ? dropInfo.InsertIndex - 1 : dropInfo.InsertIndex, source);
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        Questions.Add(source);
                    }

                    SelectedQuestion = source;

                    UpdateQuestionIndexes();
                }
                catch (AggregateException e) when (e.InnerException is DefaultException)
                {
                    // Если сервер вернул ошибку в ответ на запрос на изменение индекса вопроса, сообщить пользователю о ней
                    (e.InnerException as DefaultException).ShowSnackBar();
                }
            }
        }
        public void DragLeave(IDropInfo dropInfo) { }
        public void DragEnter(IDropInfo dropInfo) { }


        public ICommand AddAnswerOptionCommand { get; set; }
        public ICommand CreateQuestionCommand { get; set; }
        public ICommand DeleteQuestionCommand { get; set; }
        public ICommand SaveQuestionCommand { get; set; }
        public ICommand SaveCommand { get; set; }

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        List<EventHandler<PropertyChangedEventArgs>> changedHandlers =
            new List<EventHandler<PropertyChangedEventArgs>>();

        public void AddQuestion(Question question)
        {
            _questions.Add(question);
            question.PropertyChanged += SelectedQuestionChanged;
            changedHandlers.Add(SelectedQuestionChanged);
            PropertyChanged(this, new PropertyChangedEventArgs(nameof(Questions)));
        }
        public void SelectedQuestionChanged(object sender, PropertyChangedEventArgs e)
        {
            Question child = sender as Question;
            if (_questions.Contains(child))
            {
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(SelectedQuestion)));
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(RadioButtonsVisible)));
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(CheckboxesVisible)));
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(StringInputVisible)));
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(AddAnswerVisible)));
            }
        }

        public void SetSelectedQuestion(int index)
        {
            SelectedQuestion = _questions.First(x => x.Index == index);
        }


        private bool _hasTimeLimit;
        public bool HasTimeLimit
        {
            get => _hasTimeLimit;
            set
            {
                _hasTimeLimit = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(HasTimeLimit)));
            }
        }

        private int _timeLimit;
        public int TimeLimit
        {
            get => _timeLimit;
            set
            {
                _timeLimit = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(TimeLimit)));
            }
        }

        private Test _test;
        public Test Test
        {
            get => _test;
            set
            {
                _test = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(Test)));
            }
        }

        private ObservableCollection<Question> _questions = new ObservableCollection<Question>();
        public ObservableCollection<Question> Questions
        {
            get => _questions;
            set
            {
                _questions = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(Questions)));
            }
        }
        private void UpdateQuestionIndexes()
        {
            int index = 1;
            foreach (Question question in _questions)
            {
                bool temp = question.Unsaved;
                question.Index = index++;
                question.Unsaved = temp;
            }
            PropertyChanged(this, new PropertyChangedEventArgs(nameof(Questions)));
        }

        private Question _selectedQuestion;
        public Question SelectedQuestion
        {
            get => _selectedQuestion;
            set
            {
                _selectedQuestion = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(SelectedQuestion)));
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(AnswerOptions)));
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(RadioButtonsVisible)));
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(CheckboxesVisible)));
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(StringInputVisible)));
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(AddAnswerVisible)));
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(QuestionType)));
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(CheckAlgorithm)));
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(QuestionText)));
            }
        }

        public ObservableCollection<AnswerOption> AnswerOptions
        {
            get => _selectedQuestion.AnswerOptions;
            set
            {
                _selectedQuestion.AnswerOptions = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(AnswerOptions)));
            }
        }

        private string _answerOptionConstructed;
        
        private bool _hasAttemptLimit;
        public bool HasAttemptLimit
        {
            get => _hasAttemptLimit;
            set
            {
                _hasAttemptLimit = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(HasAttemptLimit)));
            }
        }

        private int _attemptLimit;
        public int AttemptLimit
        {
            get => _attemptLimit;
            set
            {
                _attemptLimit = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(HasAttemptLimit)));
            }
        }

        public string AnswerOptionConstructed
        {
            get => _answerOptionConstructed;
            set
            {
                _answerOptionConstructed = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(AnswerOptionConstructed)));
            }
        }

        public QuestionType QuestionType
        {
            get => _selectedQuestion.QuestionType;
            set
            {
                _selectedQuestion.QuestionType = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(QuestionType)));
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(RadioButtonsVisible)));
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(CheckboxesVisible)));
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(StringInputVisible)));
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(AddAnswerVisible)));
            }
        }

        public CheckAlgorithm? CheckAlgorithm
        {
            get => _selectedQuestion.CheckAlgorithm;
            set
            {
                _selectedQuestion.CheckAlgorithm = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(CheckAlgorithm)));
            }
        }

        public string QuestionText
        {
            get => _selectedQuestion.Text;
            set
            {
                _selectedQuestion.Text = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(QuestionText)));
            }
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

        public bool AddAnswerVisible
        {
            get => _selectedQuestion.QuestionType == QuestionType.MultipleChoice
                || _selectedQuestion.QuestionType == QuestionType.SingleChoice;
        }

        public void SetTestId(int id) {
            try
            {
                var test = Task.Run<TestInfo>(async () => await CommunicationService.GetTest(id)).Result;
                Test = new Test
                {
                    Id = test.Id,
                    AuthorId = test.AuthorId,
                    CanNotReviewQuestions = test.CanNotReviewQuestions,
                    Attempts = test.Attempts,
                    TimeLimit = new TimeSpan(0, 0, (int)test.TimeLimit),
                    Created = DateTimeOffset.FromUnixTimeSeconds(test.CreatedDate).DateTime,
                    PrivateUntil = DateTimeOffset.FromUnixTimeSeconds(test.PrivateUntil).DateTime,
                    PublicUntil = DateTimeOffset.FromUnixTimeSeconds(test.PublicUntil).DateTime
                };
            }
            catch (AggregateException e) when (e.InnerException is DefaultException)
            {
                (e.InnerException as DefaultException).ShowSnackBar();
            }

            RetrieveQuestions();
        }

        public void CreateQuestion()
        {
            Question newQuestion = new Question { };
            try
            {
                PostQuestionResult result = Task.Run<PostQuestionResult>(async () => await CommunicationService.CreateQuestion(_test.Id)).Result;
                newQuestion.Id = result.QuestionId;
                newQuestion.Index = result.Index;

                _questions.Add(newQuestion);
                SelectedQuestion = newQuestion;
            }
            catch (AggregateException e) when (e.InnerException is DefaultException)
            {
                (e.InnerException as DefaultException).ShowSnackBar();
            }
        }

        public void RetrieveQuestions()
        {
            try
            {
                var questionInfos = Task.Run<List<QuestionInfo>>(async () => await CommunicationService.GetTestQuestions(_test.Id)).Result;

                var questions = new ObservableCollection<Question>(questionInfos.Select(x => new Question
                {
                    Id = x.QuestionId,
                    Text = x.Text,
                    QuestionType = x.QuestionType,
                    CheckAlgorithm = x.CheckAlgorithm,
                    AnswerOptions = new ObservableCollection<AnswerOption>(x.AnswerOptions.Select(y => new AnswerOption
                    {
                        Id = y.AnswerId,
                        IsChecked = y.Checked ?? false,
                        Text = y.Text
                    }).ToList())
                }));

                Questions = questions;

                if (Questions.Count > 0)
                {
                    SelectedQuestion = questions[0];
                }
                else
                {
                    CreateQuestion();
                }
            }
            catch (AggregateException e) when (e.InnerException is DefaultException)
            {
                (e.InnerException as DefaultException).ShowSnackBar();
            }
        }

        private bool ValidateQuestions()
        {
            foreach (Question question in _questions)
            {
                if (question.AnswerOptions.Count == 0 && (question.StringInputAnswerOption.Text ?? "").Equals(""))
                {
                    SelectedQuestion = question;
                    new DefaultException(0, "Для вопроса не указан ответ").ShowSnackBar();
                    return false;
                }
                if (question.AnswerOptions.Count > 0 && !question.AnswerOptions.Any(x=> x.SingleChoiceSelected || x.MultipleChoiceSelected))
                {
                    SelectedQuestion = question;
                    new DefaultException(0, "Для вопроса не указан правильный ответ").ShowSnackBar();
                    return false;
                } 
            }
            return true;
        }

        public TestEditViewModel()
        {
            Question question = new Question()
            {
                Id = 1,
                Text = "what is obamas last name",
                Index = 1,
                QuestionType = QuestionType.MultipleChoice,
                AnswerOptions = new ObservableCollection<AnswerOption>()
                {
                    new AnswerOption()
                    {
                        Id = 1,
                        Text = "barack",
                        IsChecked = true
                    },
                    new AnswerOption()
                    {
                        Id = 1,
                        Text = "oabma"
                    }
                }
            };

            AddQuestion(question);

            _selectedQuestion = question;

            _answerOptionConstructed = "";

            AddAnswerOptionCommand = new RelayCommand(() =>
            {
                SelectedQuestion.AnswerOptions.Add(new AnswerOption { 
                    Text = AnswerOptionConstructed,
                    SingleChoiceSelected = !SelectedQuestion.AnswerOptions.Any(x => x.SingleChoiceSelected)
                });
                AnswerOptionConstructed = "";
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(AnswerOptions)));
            }, () => !_answerOptionConstructed.Equals(""));

            CreateQuestionCommand = new RelayCommand(() =>
            {
                CreateQuestion();
            });

            DeleteQuestionCommand = new RelayCommand(() =>
            {
                try
                {
                    Task.Run(async () => await CommunicationService.DeleteQuestion(Test.Id, SelectedQuestion.Id));
                    int index = _questions.IndexOf(_selectedQuestion);
                    var temp = _selectedQuestion;

                    if (index == Questions.Count - 1)
                    {
                        SelectedQuestion = Questions[index - 1];
                    }
                    else
                    {
                        SelectedQuestion = Questions[index + 1];
                    }

                    _questions.Remove(temp);
                    UpdateQuestionIndexes();
                }
                catch (AggregateException e) when (e.InnerException is DefaultException)
                {
                    (e.InnerException as DefaultException).ShowSnackBar();
                }
            }, () => _questions.Count > 1);

            SaveQuestionCommand = new RelayCommand(() =>
            {
                PutQuestion putQuestion = new PutQuestion {
                    Index = _selectedQuestion.Index,
                    QuestionType = (int)_selectedQuestion.QuestionType,
                    CheckAlgorithm = (int)_selectedQuestion.CheckAlgorithm,
                    Text = _selectedQuestion.Text,
                    AnswerOptions = _selectedQuestion.QuestionType == QuestionType.SingleChoice ? 
                        _selectedQuestion.AnswerOptions.Select(x => new PutAnswerOption
                        {
                            Text = x.Text,
                            Checked = x.SingleChoiceSelected
                        }).ToList()
                        : 
                        _selectedQuestion.QuestionType == QuestionType.MultipleChoice ?
                            _selectedQuestion.AnswerOptions.Select(x => new PutAnswerOption
                            {
                                Text = x.Text,
                                Checked = x.MultipleChoiceSelected
                            }).ToList()
                            :
                            new List<PutAnswerOption>()
                            {
                                new PutAnswerOption
                                {
                                    Text = _selectedQuestion.StringInputAnswerOption.Text,
                                    Checked = true
                                }
                            }
                };
                try
                {
                    Task.Run(async () => await CommunicationService.SaveQuestion(_test.Id, _selectedQuestion.Id, putQuestion));
                    SelectedQuestion.Unsaved = false;
                }
                catch (AggregateException e) when (e.InnerException is DefaultException)
                {
                    (e.InnerException as DefaultException).ShowSnackBar();
                }
            }, () => SelectedQuestion.Unsaved);

            SaveCommand = new RelayCommand(() =>
            {
                if (ValidateQuestions())
                {
                    try 
                    {
                        Task.Run<TestInfo>(async () => await CommunicationService.PutTest(this._test.Id, new TestInfoSetter
                        {
                            TimeLimit = _hasTimeLimit ? _timeLimit * 60 : 0,
                            Attempts = _hasAttemptLimit ? (int?)_attemptLimit : null
                        }));
                        NavigationMediator.SetRootViewModel(new TestListViewModel());
                    }
                    catch (AggregateException e) when (e.InnerException is DefaultException)
                    {
                        (e.InnerException as DefaultException).ShowSnackBar();
                    }
                }
            });
        }
    }
}
