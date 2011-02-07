using System.Collections.Generic;
using Habanero.BO;

namespace Habanero.Fluent
{
    public class NewUniqueContraintsBuilder<T> where T : BusinessObject
    {
        private IList<NewKeyDefBuilder<T>> _newKeyDefBuilders;
        private NewClassDefBuilder2<T> _classDefBuilder2;

        public NewUniqueContraintsBuilder(NewClassDefBuilder2<T> classDefBuilder2, IList<NewKeyDefBuilder<T>> keyDefBuilders)
        {
            _classDefBuilder2 = classDefBuilder2;
            _newKeyDefBuilders = keyDefBuilders;
        }

        public NewKeyDefBuilder<T> UniqueConstraint(string keyName = "")
        {
            NewKeyDefBuilder<T> keyDefBuilder = new NewKeyDefBuilder<T>(this, keyName);
            _newKeyDefBuilders.Add(keyDefBuilder);
            return keyDefBuilder;
        }

        public NewClassDefBuilder2<T> EndUniqueConstraints()
        {
            return _classDefBuilder2;
        }
    }
}