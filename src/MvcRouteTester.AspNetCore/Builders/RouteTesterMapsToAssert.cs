﻿#region License
// Copyright (c) Niklas Wendel 2018
// 
// Licensed under the Apache License, Version 2.0 (the "License"); 
// you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at 
// 
// http://www.apache.org/licenses/LICENSE-2.0 
// 
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, 
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
// See the License for the specific language governing permissions and 
// limitations under the License.
#endregion
using System.Linq.Expressions;
using System.Net.Http;
using Newtonsoft.Json;
using Xunit;
using MvcRouteTester.AspNetCore.Internal;

namespace MvcRouteTester.AspNetCore.Builders
{

    /// <summary>
    /// 
    /// </summary>
    public class RouteTesterMapsToAssert :
        IRouteAssertMapsToBuilder,
        IRouteAssert
    {

        #region Fields

        private ActionInvokeInfo _expected;

        #endregion

        #region Constructor

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actionCallExpression"></param>
        public RouteTesterMapsToAssert(LambdaExpression actionCallExpression)
        {
            ParseActionCallExpression(actionCallExpression);
        }

        #endregion

        #region Parse Action Call Expression

        /// <summary>
        /// 
        /// </summary>
        /// <param name="actionCallExpression"></param>
        private void ParseActionCallExpression(LambdaExpression actionCallExpression)
        {
            var parser = new RouteExpressionParser();
            _expected = parser.Parse(actionCallExpression);
        }

        #endregion

        #region Ensure

        /// <summary>
        /// 
        /// </summary>
        /// <param name="responseMessage"></param>
        public void Ensure(HttpResponseMessage responseMessage)
        {
            responseMessage.EnsureSuccessStatusCode();

            var json = responseMessage.Content.ReadAsStringAsync().Result;
            var actionInvokeInfo = JsonConvert.DeserializeObject<ActionInvokeInfo>(json);

            // TODO: Rewrite!
            Assert.Equal(
                _expected.ActionInfo.ControllerTypeNameInfo.AssemblyQualifiedName, 
                actionInvokeInfo.ActionInfo.ControllerTypeNameInfo.AssemblyQualifiedName);
            Assert.Equal(
                _expected.ActionInfo.ActionMethodName, 
                actionInvokeInfo.ActionInfo.ActionMethodName);
            Assert.Equal(
                _expected.ActionInfo.ParameterTypeNameInfos.Length, 
                actionInvokeInfo.ActionInfo.ParameterTypeNameInfos.Length);
            for (var ix = 0; ix < _expected.ActionInfo.ParameterTypeNameInfos.Length; ix++)
            {
                Assert.Equal(
                    _expected.ActionInfo.ParameterTypeNameInfos[ix].AssemblyQualifiedName, 
                    actionInvokeInfo.ActionInfo.ParameterTypeNameInfos[ix].AssemblyQualifiedName);
            }
        }

        #endregion

    }

}
