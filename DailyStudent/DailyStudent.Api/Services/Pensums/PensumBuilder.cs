using DailyStudent.Api.DTOs.Pensum.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DailyStudent.Api.Services.Pensums
{
    public class PensumBuilder
    {
        private int _creditLimit;

        public List<Period> Build(List<Subject> input, int creditLimitPerPeriod)
        {
            if (creditLimitPerPeriod < 1)
                throw new Exception("El límite de créditos debe ser un número positivo.");

            _creditLimit = creditLimitPerPeriod;
            var periods = GroupSubjects(input);
            periods = RemoveCompletedSubjects(periods);
            periods = MoveSubjects(periods);

            return periods;
        }

        private List<Period> GroupSubjects(List<Subject> input)
        {
            int i = 1;
            var data = input.GroupBy(x => x.Period)
                .OrderBy(x => x.Key)
                .Select(x => new Period(i++)
                {
                    Name = "Periodo " + (i - 1).ToString(),
                    Subjects = x.ToList(),
                    Credits = x.ToList().Sum(x => x.Credits)
                })
                .ToList();

            return data;
        }

        private List<Period> RemoveCompletedSubjects(List<Period> input)
        {
            foreach(var period in input)
            {
                var subjectsCompleted = period.Subjects.Where(x => x.IsCompleted).ToList();
                foreach(var subject in subjectsCompleted)
                {
                    period.Credits -= subject.Credits;
                    period.Subjects.Remove(subject);
                }
            }

            return input;
        }

        private List<Period> MoveSubjects(List<Period> input)
        {
            List<Period> newPeriods = new List<Period>();
            foreach (var period in input)
            {
                var periodCopy = new Period(period.Code) 
                { 
                    Name = period.Name,
                    Credits = period.Credits,
                    Subjects = new List<Subject>(period.Subjects)
                };

                foreach (var subject in period.Subjects)
                {
                    if (newPeriods.Any(x => x.Subjects.Any(x => x.Code == subject.Code)))
                        continue;

                    var periodCanMove = SelectBestPeriod(subject, newPeriods);
                    if (periodCanMove != null)
                    {
                        periodCanMove.Subjects.Add(subject);
                        periodCanMove.Credits += subject.Credits;
                        periodCopy.Credits -= subject.Credits;
                        periodCopy.Subjects.Remove(periodCopy.Subjects.SingleOrDefault(x => x.Code == subject.Code));

                        if (subject.Corequisites.Count > 0)
                        {
                            foreach(var item in subject.Corequisites)
                            {
                                periodCanMove.Subjects.Add(item);
                                periodCanMove.Credits += item.Credits;
                                periodCopy.Credits -= item.Credits;
                                periodCopy.Subjects.Remove(periodCopy.Subjects.SingleOrDefault(x => x.Code == item.Code));
                            }
                        }
                    }
                }
                if (periodCopy.Subjects.Count > 0)
                {
                    periodCopy.Code = newPeriods.Count() + 1;
                    newPeriods.Add(periodCopy);
                }
            }
            return newPeriods;
        }

        private Period SelectBestPeriod(Subject subject, List<Period> periods)
        {
            if (periods.Count > 0)
            {
                int prerequisitePeriod = 0;
                if (subject.Prerequisites.Count > 0)
                {
                    var prerequisites = subject.Prerequisites.Select(x => x.Code).ToArray();
                    //prerequisitePeriod = periods.Where(x => x.Subjects.Any(x => prerequisites.Contains(x.Code))).Select(x => x.Code).Max();
                    var data = periods.Where(x => x.Subjects.Any(x => prerequisites.Contains(x.Code))).Select(x => x.Code).ToArray();
                    if (data.Count() > 0)
                        prerequisitePeriod = data.Max();
                }

                foreach (var period in periods)
                {
                    if (period.Code > prerequisitePeriod && period.Credits < _creditLimit)
                    {
                        var credits = subject.Credits;
                        if (subject.Corequisites.Count > 0)
                            credits += subject.Corequisites.Sum(x => x.Credits);

                        if (credits + period.Credits <= _creditLimit)
                            return period;
                    }
                }
            }
            return null;
        }

    }

}
