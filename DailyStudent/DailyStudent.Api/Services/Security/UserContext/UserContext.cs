using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DailyStudent.Api.Services.Security.UserContext
{
    public class UserContext: IUserContext
    {
        public UserContext(SessionUser data = null)
        {
            this.User = data;
        }

        public void UpdateData(SessionUser mainData)
        {
            User = mainData;
        }

        public void Clear()
        {
            User = null;
        }

        public SessionUser User { get; private set; }
    }
}
