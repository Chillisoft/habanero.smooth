#region Licensing Header
// ---------------------------------------------------------------------------------
//  Copyright (C) 2007-2011 Chillisoft Solutions
//  
//  This file is part of the Habanero framework.
//  
//      Habanero is a free framework: you can redistribute it and/or modify
//      it under the terms of the GNU Lesser General Public License as published by
//      the Free Software Foundation, either version 3 of the License, or
//      (at your option) any later version.
//  
//      The Habanero framework is distributed in the hope that it will be useful,
//      but WITHOUT ANY WARRANTY; without even the implied warranty of
//      MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//      GNU Lesser General Public License for more details.
//  
//      You should have received a copy of the GNU Lesser General Public License
//      along with the Habanero framework.  If not, see <http://www.gnu.org/licenses/>.
// ---------------------------------------------------------------------------------
#endregion
using Habanero.BO;
using Habanero.Smooth.Test;

namespace Habanero.Fluent.Tests.TestStubs
{
    class SingleRelationshipDefBuilderSpy<T, TRelatedType> : SingleRelationshipDefBuilder<T, TRelatedType>
        where T : BusinessObject
        where TRelatedType : BusinessObject
    {
        public SingleRelationshipDefBuilderSpy()
            : base(new RelationshipsBuilderStub<T>(), GetRandomRelName())
        {
            /*                var relKeyDefBuilder = new SingleRelKeyDefBuilder<T, TRelatedType>(singleRelationshipDefBuilder);
                            this.SingleRelKeyDefBuilder = */
        }

        private static string GetRandomRelName()
        {
            return RandomValueGenerator.GetRandomString();
        }

        /*
                public SingleRelationshipDefBuilderSpy(RelationshipsBuilder<T> RelationshipsBuilder, Expression<Func<T, TRelatedType>> relationshipExpression) : base(RelationshipsBuilder, relationshipExpression)
                {
                }*/
    }


}