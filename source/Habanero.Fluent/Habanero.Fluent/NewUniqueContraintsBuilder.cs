using System.Collections.Generic;
using Habanero.BO;

namespace Habanero.Fluent
{
    public class NewUniqueContraintsBuilder<T> where T : BusinessObject
    {
        private IList<KeyDefBuilder<T>> _newKeyDefBuilders;
        private ClassDefBuilder2<T> _classDefBuilder2;

        public NewUniqueContraintsBuilder(ClassDefBuilder2<T> classDefBuilder2, IList<KeyDefBuilder<T>> keyDefBuilders)
        {
            _classDefBuilder2 = classDefBuilder2;
            _newKeyDefBuilders = keyDefBuilders;
        }

        public KeyDefBuilder<T> UniqueConstraint(string keyName = "")
        {
            KeyDefBuilder<T> keyDefBuilder = new KeyDefBuilder<T>(this, keyName);
            _newKeyDefBuilders.Add(keyDefBuilder);
            return keyDefBuilder;
        }

        public ClassDefBuilder2<T> EndUniqueConstraints()
        {
            return _classDefBuilder2;
        }
    }
}