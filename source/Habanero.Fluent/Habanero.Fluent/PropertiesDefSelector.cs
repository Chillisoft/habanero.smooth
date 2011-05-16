using System.Collections.Generic;
using Habanero.BO;

namespace Habanero.Fluent
{
    public class PropertiesDefSelector<T> where T : BusinessObject
    {
        private readonly ClassDefBuilder<T> _classDefBuilder;
        private readonly SuperClassDefBuilder<T> _superClassDefBuilder;
        private readonly IList<string> _primaryKeyPropNames;
        private PropertiesDefBuilder<T> _propertiesDefBuilder;
        private List<PropDefBuilder<T>> _propDefBuilders;
        private ClassDefBuilder2<T> _classDefBuilder2;


        public PropertiesDefSelector(ClassDefBuilder<T> classDefBuilder, IList<string> primaryKeyPropNames)
        {
            _classDefBuilder = classDefBuilder;
            _primaryKeyPropNames = primaryKeyPropNames;
            Initialise();
        }

        public PropertiesDefSelector(ClassDefBuilder<T> classDefBuilder, SuperClassDefBuilder<T> superClassDefBuilder)
        {
            _classDefBuilder = classDefBuilder;
            _superClassDefBuilder = superClassDefBuilder;
            _primaryKeyPropNames = new List<string>();
            Initialise();
        }

        private void Initialise()
        {
            _propDefBuilders = new List<PropDefBuilder<T>>();
            if (_superClassDefBuilder==null)
            {
                _classDefBuilder2 = new ClassDefBuilder2<T>(_classDefBuilder, _propDefBuilders, _primaryKeyPropNames);
            }
            else
            {
                _classDefBuilder2 = new ClassDefBuilder2<T>(_classDefBuilder, _propDefBuilders, _primaryKeyPropNames, _superClassDefBuilder);
            }

            _propertiesDefBuilder = new PropertiesDefBuilder<T>(_classDefBuilder2, _propDefBuilders);
        }


        public PropertiesDefBuilder<T> WithProperties()
        {
            return _propertiesDefBuilder;
        }

        public RelationshipsBuilder<T> WithRelationships()
        {
            return _classDefBuilder2.WithRelationships();
        }

    }
}
