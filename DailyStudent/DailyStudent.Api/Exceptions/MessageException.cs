using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DailyStudent.Api.Exceptions
{
    public class MessageException : Exception
    {
        private string _Message;
        public override string Message { get => _Message; }

        public MessageException(short code) : base()
        {
            var messages = new Messages();
            _Message = messages.Get(code).Description;

            // TODO Implementar log de excepción.
        }

        public MessageException(short code, params string[] param) : base()
        {
            var messages = new Messages();
            _Message = string.Format(messages.Get(code).Description, param.ToArray());

            // TODO Implementar log de excepción.
        }
    }
}
