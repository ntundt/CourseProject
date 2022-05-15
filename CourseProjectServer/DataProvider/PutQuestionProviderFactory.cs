using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace CourseProjectServer.DataProvider
{
    public class PutQuestionProviderFactory : IValueProviderFactory
    {
        public async Task CreateValueProviderAsync(ValueProviderFactoryContext context)
        {
            return Task.CompletedTask;
        }
    }
}
