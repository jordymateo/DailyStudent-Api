using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DailyStudent.Api.Services.Security.UserContext
{
    public interface IUserContext
    {
        void UpdateData(SessionUser mainData);
        void Clear();
        SessionUser User { get; }
    }
}
