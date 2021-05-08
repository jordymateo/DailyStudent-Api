using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DailyStudent.Api.Exceptions
{
    public class Message
    {
        public short Code { get; set; }
        public string Description { get; set; }
    }

    public class Messages
    {
        private List<Message> _messages = new List<Message>
        {
            new Message { Code = 1, Description = "{0} requiere un valor." },
            new Message { Code = 2, Description = "El usuario en sesión no existe." },
            new Message { Code = 3, Description = "El país de id {0} no ha sido encontrado." },
            new Message { Code = 4, Description = "No se encontró valor para '{0}': {1}." },
            new Message { Code = 5, Description = "Acceso denegado. No cuenta con los permisos requeridos" },
            new Message { Code = 6, Description = "El registro de ({0}) que intenta modificar no existe." },
            new Message { Code = 7, Description = "Contraseña incorrecta" },
            new Message { Code = 8, Description = "Contraseñas no coinciden" },
            new Message { Code = 9, Description = "No se puede crear un periodo que inicie en la fecha ({0}) ya que interfiere con uno existente" },
            new Message { Code = 10, Description = "Ya existe una cuenta con el correo ingresado" },
            new Message { Code = 11, Description = "Error enviando el correo eléctronico" },
            new Message { Code = 12, Description = "Error de verificación de usuario" },
            new Message { Code = 13, Description = "Usuario inactivo actualmente, favor comunicarse con algún administrador" },
            new Message { Code = 14, Description = "No existe ninguna cuenta con el correo eléctronico ingresado" },
            new Message { Code = 15, Description = "Error al realizar cambio de contraseña" }
        };

        public List<Message> Get() => _messages;
        public Message Get(short code) => _messages.SingleOrDefault(x => x.Code == code);

    }
}
