using System;
using System.Collections.Generic;
using System.Linq;

namespace Backup.CleaningAlgorithms
{
    public class CleaningByDate : ICleaningAlgorithm
    {
        private List<IPoints> _restorePoints;
        private int _dateTimeSpan;

        public CleaningByDate(List<IPoints> restorePoints, int dateTimeSpan)
        {
            _restorePoints = restorePoints;
            _dateTimeSpan = dateTimeSpan;

        }
        public List<IPoints> Cleaning()
        {
            List<IPoints> SaveToDelbyDate = new List<IPoints>();
            var dateNow = DateTime.Now;

            var FirstStepPointsToDeletebyDate =
                from x in _restorePoints
                where (dateNow - x._date).Days > _dateTimeSpan
                select x;

            {
                var SecondStepPointsToDeletebyDate =
                    from x in FirstStepPointsToDeletebyDate
                    where x is FullPoint && x.IndexOfDeltas < 1
                    select x;

                foreach (var point in SecondStepPointsToDeletebyDate)
                {
                    SaveToDelbyDate.Add(point);
                }
            }

            var ThirdStepPointsToDeletebyDate =
                from x in FirstStepPointsToDeletebyDate
                where x is FullPoint && x.IndexOfDeltas > 0
                select x;

            int count = 0;
            foreach (var point in ThirdStepPointsToDeletebyDate)
            {
                int indexOfFullPoint = _restorePoints.IndexOf(point);
                for (int i = 1; i <= point.IndexOfDeltas; ++i)
                {
                    if ((dateNow - _restorePoints[indexOfFullPoint + i]._date).Days > _dateTimeSpan)
                    {
                        SaveToDelbyDate.Add(_restorePoints[indexOfFullPoint + i]);
                        count++;
                    }
                }

                point.IndexOfDeltas -= count;

                if (point.IndexOfDeltas == 0)
                {
                    SaveToDelbyDate.Add(point);
                }
            }

            return SaveToDelbyDate;
        }
    }
}