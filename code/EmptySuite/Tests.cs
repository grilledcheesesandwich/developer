//
// Copyright (c) Microsoft Corporation.  All rights reserved.
//
//
// Use of this source code is subject to the terms of the Microsoft
// premium shared source license agreement under which you licensed
// this source code. If you did not accept the terms of the license
// agreement, you are not authorized to use this source code.
// For the terms of the license, please see the license agreement
// signed by you and Microsoft.
// THE SOURCE CODE IS PROVIDED "AS IS", WITH NO WARRANTIES OR INDEMNITIES.
//
using System;
using System.IO;
using System.Data;
using System.Collections.Generic;
using Microsoft.WindowsCE.TuxNet;
using Microsoft.WindowsCE.TuxNet.Core;
using Datk = Microsoft.WindowsCE.Datk;
using Microsoft.WindowsCE.Logging;
using Utils = Microsoft.MobileDevices.Utils;
using Logger = Microsoft.MobileDevices.Utils.GlobalLogger;

namespace Microsoft.MobileDevices.AreaLibrary.Security.PolicyRule
{
    /// <summary>
    /// Test suite for the Security AreaLibrary
    /// </summary>
    public class Tests : TestSuite
    {
        
        #region Constructors
        /// <summary>
        /// Create new suite
        /// </summary>
        /// <param name="harness">IHarness interface passed in by the harness
        /// this allows access to the "User" param</param>
        public Tests(ITestHarness harness)
            : base(harness)
        {
        }
        #endregion //Constructors

        /// <summary>
        /// Empty test
        /// </summary>
        [TestCaseAttribute("Empty test", Type = TestType.Functional)]
        public LogResult EmptyTest()
        {
            return LogResult.Pass;
        }
    }
}
