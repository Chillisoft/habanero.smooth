// ---------------------------------------------------------------------------------
//  Copyright (C) 2007-2010 Chillisoft Solutions
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
using System;
using Habanero.Base;
using Habanero.BO.Loaders;

namespace Habanero.Smooth
{
    ///<summary>
    /// Provides a <see cref="IClassDefsLoader"/> that will load a class Definition
    /// based on reflection and heuristics.
    ///</summary>
    public class ReflectionClassDefLoader : IClassDefsLoader
    {
        private ITypeSource Source { get; set; }

        ///<summary>
        /// constructor that takes a <see cref="ITypeSource"/> that wil be used to build the
        /// ClassDefinitions.
        ///</summary>
        ///<param name="source"></param>
        ///<exception cref="ArgumentNullException"></exception>
        public ReflectionClassDefLoader(ITypeSource source)
        {
            if (source == null) throw new ArgumentNullException("source");
            Source = source;
        }
        /// <summary>
        /// Thius is not implemented since with a Reflective ClassDefLoader you will never load from 
        /// a string xml def.
        /// </summary>
        /// <param name="classDefsXml"></param>
        /// <returns></returns>
        public ClassDefCol LoadClassDefs(string classDefsXml)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Loads one ClassDef for each Type in the <see cref="ITypeSource"/>.
        /// </summary>
        /// <returns></returns>
        public ClassDefCol LoadClassDefs()
        {
            AllClassesAutoMapper allClassesAutoMapper = new AllClassesAutoMapper(Source);
            return allClassesAutoMapper.Map();
        }
    }
}