using System.Collections.Generic;
using System.Linq;

namespace Backup.CleaningAlgorithms
{
    public class CleaningByLength : ICleaningAlgorithm
    {

        private List<IPoints> _restorePoints;
        private int _length;
        
        public CleaningByLength(List<IPoints> restorePoints, int length)
        {
            _restorePoints = restorePoints;
            _length = length;
        }
        
        
        public List<IPoints> Cleaning()
        {
            List<IPoints> SaveToDelbyLen = new List<IPoints>();
            if (_restorePoints.Count > _length)
            {
                var FirstStepPointsToDeletebyLen =
                    from x in _restorePoints
                    where _restorePoints.IndexOf(x) < _restorePoints.Count - _length
                    select x;

                {
                    var SecondStepPointsToDeletebyLen =
                        from x in FirstStepPointsToDeletebyLen
                        where x is FullPoint && x.IndexOfDeltas == 0
                        select x;

                    foreach (var point in SecondStepPointsToDeletebyLen)
                    {
                        SaveToDelbyLen.Add(point);
                    }
                }

                var ThirdStepPointsToDeletebyLen =
                    from x in FirstStepPointsToDeletebyLen
                    where x is FullPoint && x.IndexOfDeltas > 0
                    select x;


                int count = 0;
                foreach (var point in ThirdStepPointsToDeletebyLen)
                {
                    for (int i = 1; i <= point.IndexOfDeltas; ++i)
                        if (_restorePoints.IndexOf(point) + i < _restorePoints.Count - _length)
                        {
                            SaveToDelbyLen.Add(_restorePoints[_restorePoints.IndexOf(point) + i]);
                            count++;
                        }

                    point.IndexOfDeltas -= count;

                    if (point.IndexOfDeltas == 0)
                    {
                        SaveToDelbyLen.Add(point);
                    }
                }
            }
            return SaveToDelbyLen;
            
        }
    }
}