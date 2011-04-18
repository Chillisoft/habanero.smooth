using System.Collections.Generic;
using Habanero.BO;

namespace Habanero.Fluent
{
    public class PropertiesDefSelector<T> where T : BusinessObject
    {
        private readonly ClassDefBuilder2<T> _classDefBuilder2;
        private PropertiesDefBuilder<T> _propertiesDefBuilder;
        private IList<PropDefBuilder<T>> PropDefBuilders { get; set; }


        public PropertiesDefSelector(ClassDefBuilder2<T> classDefBuilder2)
        {
            _classDefBuilder2 = classDefBuilder2;
            Initialise();
        }

        private void Initialise()
        {
            _propertiesDefBuilder = new PropertiesDefBuilder<T>(_classDefBuilder2, PropDefBuilders);
        }


        public PropertiesDefBuilder<T> WithProperties()
        {
            return _propertiesDefBuilder;
        }

        public ClassDefBuilder2<T> EndProperties()
        {
            return _classDefBuilder2;
        }
    }
}
