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

public class SingleRelKeyDefBuilder<TBo, TRelatedType>
    where TBo : BusinessObject
    where TRelatedType : BusinessObject
{
    private SingleRelationshipDefBuilder<TBo, TRelatedType> _singleRelationshipDefBuilder;
    private SingleRelKeyBuilder<TBo, TRelatedType> _singleRelKeyBuilder;

    public SingleRelKeyDefBuilder(SingleRelationshipDefBuilder<TBo, TRelatedType> singleRelationshipDefBuilder)
    {
        _singleRelationshipDefBuilder = singleRelationshipDefBuilder;
        _singleRelKeyBuilder = new SingleRelKeyBuilder<TBo, TRelatedType>(_singleRelationshipDefBuilder);
    }

    public SingleRelationshipDefBuilder<TBo, TRelatedType> WithRelProp(string ownerPropName, string relatedPropName)
    {
        //_singleRelKeyBuilder = new SingleRelKeyBuilder<TBo, TRelatedType>(_singleRelationshipDefBuilder);
        _singleRelKeyBuilder.WithRelProp(ownerPropName, relatedPropName);
        return _singleRelationshipDefBuilder;
    }

    public SingleRelationshipDefBuilder<TBo, TRelatedType> WithRelProp<TReturn>(Expression<Func<TBo, TReturn>> ownerPropExpression, Expression<Func<TRelatedType, TReturn>> relatedPropExpression)
    {
        string ownerClassPropertyName = ReflectionUtilities.GetPropertyName(ownerPropExpression);
        string relatedPropName = ReflectionUtilities.GetPropertyName(relatedPropExpression);  // note the differente expressions
        return this.WithRelProp(ownerClassPropertyName, relatedPropName);
    }

    public SingleRelKeyBuilder<TBo, TRelatedType> WithCompositeRelationshipKey()
    {
        //_singleRelKeyBuilder = new SingleRelKeyBuilder<TBo, TRelatedType>(_singleRelationshipDefBuilder);
        return _singleRelKeyBuilder;
    }

    public IRelKeyDef Build()
    {
        return _singleRelKeyBuilder.Build();
    }

}