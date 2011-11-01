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
using System;
using System.Linq.Expressions;
using Habanero.Base;
using Habanero.BO;
using Habanero.Fluent;
using Habanero.Util;

public class MultipleRelKeyDefBuilder<TBo, TRelatedType>
    where TBo : BusinessObject
{
    private MultipleRelationshipDefBuilder<TBo, TRelatedType> _multipleRelationshipDefBuilder;
    private MultipleRelKeyBuilder<TBo, TRelatedType> _multipleRelKeyBuilder;

    public MultipleRelKeyDefBuilder(MultipleRelationshipDefBuilder<TBo, TRelatedType> multipleRelationshipDefBuilder)
    {
        _multipleRelationshipDefBuilder = multipleRelationshipDefBuilder;
        _multipleRelKeyBuilder = new MultipleRelKeyBuilder<TBo, TRelatedType>(_multipleRelationshipDefBuilder);
    }

    public MultipleRelationshipDefBuilder<TBo, TRelatedType> WithRelProp(string ownerPropName, string relatedPropName)
    {
        _multipleRelKeyBuilder.WithRelProp(ownerPropName, relatedPropName);
        return _multipleRelationshipDefBuilder;
    }

    public MultipleRelationshipDefBuilder<TBo, TRelatedType> WithRelProp<TReturn>(Expression<Func<TBo, TReturn>> ownerPropExpression, Expression<Func<TRelatedType, TReturn>> relatedPropExpression)
    {
        string ownerClassPropertyName = ReflectionUtilities.GetPropertyName(ownerPropExpression);
        string relatedPropName = ReflectionUtilities.GetPropertyName(relatedPropExpression);  // note the different expression
        return this.WithRelProp(ownerClassPropertyName, relatedPropName);
    }

    public MultipleRelKeyBuilder<TBo, TRelatedType> WithCompositeRelationshipKey()
    {
        return _multipleRelKeyBuilder;
    }

    public IRelKeyDef Build()
    {
        return _multipleRelKeyBuilder.Build();
    }

}