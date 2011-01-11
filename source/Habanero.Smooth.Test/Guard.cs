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

namespace Habanero.Smooth.Test
{
    /// <summary>
    /// Performs common argument validation.
    /// </summary>
    public static class Guard
    {
        #region Methods

        /// <summary>
        /// Checks a string argument to ensure it isn't null or empty.
        /// </summary>
        /// <param name="argumentValue">The argument value to check.</param>
        /// <param name="argumentName">The name of the argument.</param>
        /// <exception cref="ArgumentNullException"><paramref name="argumentValue"/> is a null reference.</exception>
        /// <exception cref="ArgumentException"><paramref name="argumentValue"/> is <see cref="string.Empty"/>.</exception>
        public static void AgainstNullOrEmptyString(string argumentValue, string argumentName)
        {
            AgainstNull(argumentValue, argumentName);

            if (argumentValue.Length == 0)
            {
                throw new ArgumentException("String cannot be empty.", argumentName);
            }
        }


        /// <summary>
        /// Checks a string argument to ensure it isn't empty.
        /// </summary>
        /// <param name="argumentValue">The argument value to check.</param>
        /// <param name="argumentName">The name of the argument.</param>
        /// <exception cref="ArgumentException"><paramref name="argumentValue"/> is <see cref="string.Empty"/>.</exception>
        public static void AgainstEmptyString(string argumentValue, string argumentName)
        {
            if ((argumentValue != null) && (argumentValue.Length == 0))
            {
                throw new ArgumentException("String cannot be empty.", argumentName);
            }
        }


        /// <summary>
        /// Checks an argument to ensure it isn't null.
        /// </summary>
        /// <param name="argumentValue">The argument value to check.</param>
        /// <param name="argumentName">The name of the argument.</param>
        /// <exception cref="ArgumentNullException"><paramref name="argumentValue"/> is a null reference.</exception>
        public static void AgainstNull(object argumentValue, string argumentName)
        {
            if (argumentValue == null)
            {
                throw new ArgumentNullException(argumentName);
            }
        }

        #endregion
    }
}