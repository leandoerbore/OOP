using System.Collections.Generic;
using System.Linq;

namespace Backup.CleaningAlgorithms
{
    public class CleaningBySize : ICleaningAlgorithm
    {
        private long _backupSize;
        private long _maxSize;
        private List<IPoints> _restorePoints;
        
        public CleaningBySize(long backupSize, long maxSize, List<IPoints> restorePoints )
        {
            _backupSize = backupSize;
            _maxSize = maxSize;
            _restorePoints = restorePoints;
        }

        public List<IPoints> Cleaning()
        {
            List<IPoints> SaveToDelbySize = new List<IPoints>();
            if (_backupSize < _maxSize)
                return SaveToDelbySize;
            var currentSize = _backupSize;

            int countPoints = 0;
            foreach (var point in _restorePoints)
            {
                currentSize -= point._size;
                ++countPoints;

                if (currentSize < _maxSize)
                    break;
            }

            var FirstStepPointsToDeletebySize =
                from x in _restorePoints
                where x is FullPoint && _restorePoints.IndexOf(x) < countPoints
                select x;

            {
                var SecondStepPointsToDeletebySize =
                    from x in FirstStepPointsToDeletebySize
                    where x.IndexOfDeltas == 0
                    select x;

                foreach (var point in SecondStepPointsToDeletebySize)
                {
                    SaveToDelbySize.Add(point);
                }
            }

            var ThirdStepPointsToDeletebySize =
                from x in FirstStepPointsToDeletebySize
                where x.IndexOfDeltas > 0
                select x;

            int count = 0;
            currentSize = _backupSize;
            foreach (var point in ThirdStepPointsToDeletebySize)
            {
                int indexOfFullPoint = _restorePoints.IndexOf(point);
                for (int i = 1; i <= point.IndexOfDeltas; ++i)
                {
                    if (currentSize - _restorePoints[indexOfFullPoint + i]._size > _backupSize)
                    {
                        SaveToDelbySize.Add(_restorePoints[indexOfFullPoint + i]);
                        count++;
                    }
                }

                point.IndexOfDeltas -= count;
                
            }

            return SaveToDelbySize;
        }
    }
}