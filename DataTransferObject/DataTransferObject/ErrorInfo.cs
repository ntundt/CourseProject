using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransferObject
{
    public class ErrorInfo
    {
        // Код ошибки
        public int Code { get; set; }
        // Сообщение ошибки
        public string Message { get; set; }
    }
}
