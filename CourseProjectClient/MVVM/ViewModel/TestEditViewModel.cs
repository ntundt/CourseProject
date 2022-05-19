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

        public void DragEnter(IDropInfo dropInfo) { }

        public void DragOver(IDropInfo dropInfo) {
            Question question = dropInfo.TargetItem as Question;

            if (question != null)
            {
                dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
                dropInfo.Effects = DragDropEffects.Move;
            }
        }

        public void DragLeave(IDropInfo dropInfo) { }

        public void Drop(IDropInfo dropInfo) 
        {
            Question source = dropInfo.Data as Question;
            int oldIndex = Questions.IndexOf(source);
            if (oldIndex != dropInfo.InsertIndex)
            {
                SelectedQuestion = Questions.FirstOrDefault(x => x.Id != source.Id);

                Questions.Remove(source);
                try
                {
                    Questions.Insert(oldIndex < dropInfo.InsertIndex ? dropInfo.InsertIndex - 1: dropInfo.InsertIndex, source);
                } catch (ArgumentOutOfRangeException)
                {
                    Questions.Add(source);
                }

                SelectedQuestion = source;

                UpdateQuestionIndexes();

                try
                {
                    Task.Run(async () => await CommunicationService.SaveQuestion(Test.Id, SelectedQuestion.Id, new PutQuestion
                    {
                        Index = SelectedQuestion.Index
                    }));
                }
                catch (AggregateException e) when (e.InnerException is DefaultException)
                {
                    (e.InnerException as DefaultException).ShowSnackBar();
                }
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
                question.Index = index++;
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

        public bool AddAnswerVisible
        {
            get => _selectedQuestion.QuestionType == QuestionType.MultipleChoise
                || _selectedQuestion.QuestionType == QuestionType.SingleChoise;
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

        public TestEditViewModel()
        {
            Question question = new Question()
            {
                Id = 1,
                Text = "what is obamas last name",
                Index = 1,
                QuestionType = QuestionType.MultipleChoise,
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
                    SingleChoiseSelected = !SelectedQuestion.AnswerOptions.Any(x => x.SingleChoiseSelected)
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
                    AnswerOptions = _selectedQuestion.QuestionType == QuestionType.SingleChoise ? 
                        _selectedQuestion.AnswerOptions.Select(x => new PutAnswerOption
                        {
                            Text = x.Text,
                            Checked = x.SingleChoiseSelected
                        }).ToList()
                        : 
                        _selectedQuestion.QuestionType == QuestionType.MultipleChoise ?
                            _selectedQuestion.AnswerOptions.Select(x => new PutAnswerOption
                            {
                                Text = x.Text,
                                Checked = x.MultipleChoiseSelected
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
                NavigationMediator.SetRootViewModel(new TestListViewModel());
            });
        }
    }
}
