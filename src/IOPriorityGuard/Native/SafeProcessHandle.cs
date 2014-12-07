// ==++==
//
//   Copyright (c) Microsoft Corporation.  All rights reserved.
//
// ==--==
/*============================================================
**
** Class:  SafeProcessHandle
**
** A wrapper for a process handle
**
**
===========================================================*/

using System;
using System.Diagnostics;
using System.Security;
using System.Security.Permissions;
using Microsoft.Win32.SafeHandles;

namespace IOPriorityGuard.Native
{
    [HostProtection(MayLeakOnAbort = true)]
    [SuppressUnmanagedCodeSecurity]
    internal sealed class SafeProcessHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        internal static SafeProcessHandle InvalidHandle = new SafeProcessHandle(IntPtr.Zero);

        internal SafeProcessHandle() : base(true) { }

        [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
        internal SafeProcessHandle(IntPtr handle)
            : base(true)
        {
            SetHandle(handle);
        }

        internal void InitialSetHandle(IntPtr h)
        {
            Debug.Assert(IsInvalid, "Safe handle should only be set once");
            handle = h;
        }

        override protected bool ReleaseHandle()
        {
            return Win32.CloseHandle(handle);
        }
    }
}