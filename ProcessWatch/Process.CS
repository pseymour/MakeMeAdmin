// 
// Copyright © 2010-2019, Sinclair Community College
// Licensed under the GNU General Public License, version 3.
// See the LICENSE file in the project root for full license information.  
//
// This file is part of Make Me Admin.
//
// Make Me Admin is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, version 3.
//
// Make Me Admin is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with Make Me Admin. If not, see <http://www.gnu.org/licenses/>.
//

namespace WMI.Win32 {
using System;
    using System.ComponentModel;
    using System.Management;
    using System.Collections;
    using System.Globalization;
    
    
    // Functions ShouldSerialize<PropertyName> are functions used by VS property browser to check if a particular property has to be serialized. These functions are added for all ValueType properties ( properties of type Int32, BOOL etc.. which cannot be set to null). These functions use Is<PropertyName>Null function. These functions are also used in the TypeConverter implementation for the properties to check for NULL value of property so that an empty value can be shown in Property browser in case of Drag and Drop in Visual studio.
    // Functions Is<PropertyName>Null() are used to check if a property is NULL.
    // Functions Reset<PropertyName> are added for Nullable Read/Write properties. These functions are used by VS designer in property browser to set a property to NULL.
    // Every property added to the class for WMI property has attributes set to define its behavior in Visual Studio designer and also to define a TypeConverter to be used.
    // Datetime conversion functions ToDateTime and ToDmtfDateTime are added to the class to convert DMTF datetime to System.DateTime and vice-versa.
    // An Early Bound class generated for the WMI class.Win32_Process
    public class Process : System.ComponentModel.Component {
        
        // Private property to hold the WMI namespace in which the class resides.
        private static string CreatedWmiNamespace = "root\\cimv2";
        
        // Private property to hold the name of WMI class which created this class.
        private static string CreatedClassName = "Win32_Process";
        
        // Private member variable to hold the ManagementScope which is used by the various methods.
        private static System.Management.ManagementScope statMgmtScope = null;
        
        private ManagementSystemProperties PrivateSystemProperties;
        
        // Underlying lateBound WMI object.
        private System.Management.ManagementObject PrivateLateBoundObject;
        
        // Member variable to store the 'automatic commit' behavior for the class.
        private bool AutoCommitProp;
        
        // Private variable to hold the embedded property representing the instance.
        private System.Management.ManagementBaseObject embeddedObj;
        
        // The current WMI object used
        private System.Management.ManagementBaseObject curObj;
        
        // Flag to indicate if the instance is an embedded object.
        private bool isEmbedded;
        
        // Below are different overloads of constructors to initialize an instance of the class with a WMI object.
        public Process() {
            this.InitializeObject(null, null, null);
        }
        
        public Process(string keyHandle) {
            this.InitializeObject(null, new System.Management.ManagementPath(Process.ConstructPath(keyHandle)), null);
        }
        
        public Process(System.Management.ManagementScope mgmtScope, string keyHandle) {
            this.InitializeObject(((System.Management.ManagementScope)(mgmtScope)), new System.Management.ManagementPath(Process.ConstructPath(keyHandle)), null);
        }
        
        public Process(System.Management.ManagementPath path, System.Management.ObjectGetOptions getOptions) {
            this.InitializeObject(null, path, getOptions);
        }
        
        public Process(System.Management.ManagementScope mgmtScope, System.Management.ManagementPath path) {
            this.InitializeObject(mgmtScope, path, null);
        }
        
        public Process(System.Management.ManagementPath path) {
            this.InitializeObject(null, path, null);
        }
        
        public Process(System.Management.ManagementScope mgmtScope, System.Management.ManagementPath path, System.Management.ObjectGetOptions getOptions) {
            this.InitializeObject(mgmtScope, path, getOptions);
        }
        
        public Process(System.Management.ManagementObject theObject) {
            Initialize();
            if ((CheckIfProperClass(theObject) == true)) {
                PrivateLateBoundObject = theObject;
                PrivateSystemProperties = new ManagementSystemProperties(PrivateLateBoundObject);
                curObj = PrivateLateBoundObject;
            }
            else {
                throw new System.ArgumentException("Class name does not match.");
            }
        }
        
        public Process(System.Management.ManagementBaseObject theObject) {
            Initialize();
            if ((CheckIfProperClass(theObject) == true)) {
                embeddedObj = theObject;
                PrivateSystemProperties = new ManagementSystemProperties(theObject);
                curObj = embeddedObj;
                isEmbedded = true;
            }
            else {
                throw new System.ArgumentException("Class name does not match.");
            }
        }
        
        // Property returns the namespace of the WMI class.
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string OriginatingNamespace {
            get {
                return "root\\cimv2";
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string ManagementClassName {
            get {
                string strRet = CreatedClassName;
                if ((curObj != null)) {
                    if ((curObj.ClassPath != null)) {
                        strRet = ((string)(curObj["__CLASS"]));
                        if (((strRet == null) 
                                    || (strRet == string.Empty))) {
                            strRet = CreatedClassName;
                        }
                    }
                }
                return strRet;
            }
        }
        
        // Property pointing to an embedded object to get System properties of the WMI object.
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ManagementSystemProperties SystemProperties {
            get {
                return PrivateSystemProperties;
            }
        }
        
        // Property returning the underlying lateBound object.
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public System.Management.ManagementBaseObject LateBoundObject {
            get {
                return curObj;
            }
        }
        
        // ManagementScope of the object.
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public System.Management.ManagementScope Scope {
            get {
                if ((isEmbedded == false)) {
                    return PrivateLateBoundObject.Scope;
                }
                else {
                    return null;
                }
            }
            set {
                if ((isEmbedded == false)) {
                    PrivateLateBoundObject.Scope = value;
                }
            }
        }
        
        // Property to show the commit behavior for the WMI object. If true, WMI object will be automatically saved after each property modification.(ie. Put() is called after modification of a property).
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool AutoCommit {
            get {
                return AutoCommitProp;
            }
            set {
                AutoCommitProp = value;
            }
        }
        
        // The ManagementPath of the underlying WMI object.
        [Browsable(true)]
        public System.Management.ManagementPath Path {
            get {
                if ((isEmbedded == false)) {
                    return PrivateLateBoundObject.Path;
                }
                else {
                    return null;
                }
            }
            set {
                if ((isEmbedded == false)) {
                    if ((CheckIfProperClass(null, value, null) != true)) {
                        throw new System.ArgumentException("Class name does not match.");
                    }
                    PrivateLateBoundObject.Path = value;
                }
            }
        }
        
        // Public static scope property which is used by the various methods.
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static System.Management.ManagementScope StaticScope {
            get {
                return statMgmtScope;
            }
            set {
                statMgmtScope = value;
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("The Caption property is a short textual description (one-line string) of the obje" +
            "ct.")]
        public string Caption {
            get {
                return ((string)(curObj["Caption"]));
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("The CommandLine property specifies the command line used to start a particular pr" +
            "ocess, if applicable.")]
        public string CommandLine {
            get {
                return ((string)(curObj["CommandLine"]));
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("CreationClassName indicates the name of the class or the subclass used in the cre" +
            "ation of an instance. When used with the other key properties of this class, thi" +
            "s property allows all instances of this class and its subclasses to be uniquely " +
            "identified.")]
        public string CreationClassName {
            get {
                return ((string)(curObj["CreationClassName"]));
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsCreationDateNull {
            get {
                if ((curObj["CreationDate"] == null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Time that the process began executing.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public System.DateTime CreationDate {
            get {
                if ((curObj["CreationDate"] != null)) {
                    return ToDateTime(((string)(curObj["CreationDate"])));
                }
                else {
                    return System.DateTime.MinValue;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("CSCreationClassName contains the scoping computer system\'s creation class name.")]
        public string CSCreationClassName {
            get {
                return ((string)(curObj["CSCreationClassName"]));
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("The scoping computer system\'s name.")]
        public string CSName {
            get {
                return ((string)(curObj["CSName"]));
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("The Description property provides a textual description of the object. ")]
        public string Description {
            get {
                return ((string)(curObj["Description"]));
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("The ExecutablePath property indicates the path to the executable file of the proc" +
            "ess.\nExample: C:\\WINDOWS\\EXPLORER.EXE")]
        public string ExecutablePath {
            get {
                return ((string)(curObj["ExecutablePath"]));
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsExecutionStateNull {
            get {
                if ((curObj["ExecutionState"] == null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Indicates the current operating condition of the process. Values include ready (2" +
            "), running (3), and blocked (4), among others.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public ExecutionStateValues ExecutionState {
            get {
                if ((curObj["ExecutionState"] == null)) {
                    return ((ExecutionStateValues)(System.Convert.ToInt32(10)));
                }
                return ((ExecutionStateValues)(System.Convert.ToInt32(curObj["ExecutionState"])));
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("A string used to identify the process. A process ID is a kind of process handle.")]
        public string Handle {
            get {
                return ((string)(curObj["Handle"]));
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsHandleCountNull {
            get {
                if ((curObj["HandleCount"] == null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description(@"The HandleCount property specifies the total number of handles currently open by this process. This number is the sum of the handles currently open by each thread in this process. A handle is used to examine or modify the system resources. Each handle has an entry in an internally maintained table. These entries contain the addresses of the resources and the means to identify the resource type.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public uint HandleCount {
            get {
                if ((curObj["HandleCount"] == null)) {
                    return System.Convert.ToUInt32(0);
                }
                return ((uint)(curObj["HandleCount"]));
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsInstallDateNull {
            get {
                if ((curObj["InstallDate"] == null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("The InstallDate property is datetime value indicating when the object was install" +
            "ed. A lack of a value does not indicate that the object is not installed.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public System.DateTime InstallDate {
            get {
                if ((curObj["InstallDate"] != null)) {
                    return ToDateTime(((string)(curObj["InstallDate"])));
                }
                else {
                    return System.DateTime.MinValue;
                }
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsKernelModeTimeNull {
            get {
                if ((curObj["KernelModeTime"] == null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Time in kernel mode, in 100 nanoseconds. If this information is not available, a " +
            "value of 0 should be used.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public ulong KernelModeTime {
            get {
                if ((curObj["KernelModeTime"] == null)) {
                    return System.Convert.ToUInt64(0);
                }
                return ((ulong)(curObj["KernelModeTime"]));
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsMaximumWorkingSetSizeNull {
            get {
                if ((curObj["MaximumWorkingSetSize"] == null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description(@"The MaximumWorkingSetSize property indicates the maximum working set size of the process. The working set of a process is the set of memory pages currently visible to the process in physical RAM. These pages are resident and available for an application to use without triggering a page fault.
Example: 1413120.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public uint MaximumWorkingSetSize {
            get {
                if ((curObj["MaximumWorkingSetSize"] == null)) {
                    return System.Convert.ToUInt32(0);
                }
                return ((uint)(curObj["MaximumWorkingSetSize"]));
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsMinimumWorkingSetSizeNull {
            get {
                if ((curObj["MinimumWorkingSetSize"] == null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description(@"The MinimumWorkingSetSize property indicates the minimum working set size of the process. The working set of a process is the set of memory pages currently visible to the process in physical RAM. These pages are resident and available for an application to use without triggering a page fault.
Example: 20480.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public uint MinimumWorkingSetSize {
            get {
                if ((curObj["MinimumWorkingSetSize"] == null)) {
                    return System.Convert.ToUInt32(0);
                }
                return ((uint)(curObj["MinimumWorkingSetSize"]));
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("The Name property defines the label by which the object is known. When subclassed" +
            ", the Name property can be overridden to be a Key property.")]
        public string Name {
            get {
                return ((string)(curObj["Name"]));
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("The scoping operating system\'s creation class name.")]
        public string OSCreationClassName {
            get {
                return ((string)(curObj["OSCreationClassName"]));
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("The scoping operating system\'s name.")]
        public string OSName {
            get {
                return ((string)(curObj["OSName"]));
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsOtherOperationCountNull {
            get {
                if ((curObj["OtherOperationCount"] == null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("The OtherOperationCount property specifies the number of I/O operations performed" +
            ", other than read and write operations.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public ulong OtherOperationCount {
            get {
                if ((curObj["OtherOperationCount"] == null)) {
                    return System.Convert.ToUInt64(0);
                }
                return ((ulong)(curObj["OtherOperationCount"]));
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsOtherTransferCountNull {
            get {
                if ((curObj["OtherTransferCount"] == null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("The OtherTransferCount property specifies the amount of data transferred during o" +
            "perations other than read and write operations.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public ulong OtherTransferCount {
            get {
                if ((curObj["OtherTransferCount"] == null)) {
                    return System.Convert.ToUInt64(0);
                }
                return ((ulong)(curObj["OtherTransferCount"]));
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsPageFaultsNull {
            get {
                if ((curObj["PageFaults"] == null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("The PageFaults property indicates the number of page faults generated by the proc" +
            "ess.\nExample: 10")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public uint PageFaults {
            get {
                if ((curObj["PageFaults"] == null)) {
                    return System.Convert.ToUInt32(0);
                }
                return ((uint)(curObj["PageFaults"]));
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsPageFileUsageNull {
            get {
                if ((curObj["PageFileUsage"] == null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("The PageFileUsage property indicates the amountof page file space currently being" +
            " used by the process.\nExample: 102435")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public uint PageFileUsage {
            get {
                if ((curObj["PageFileUsage"] == null)) {
                    return System.Convert.ToUInt32(0);
                }
                return ((uint)(curObj["PageFileUsage"]));
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsParentProcessIdNull {
            get {
                if ((curObj["ParentProcessId"] == null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description(@"The ParentProcessId property specifies the unique identifier of the process that created this process. Process identifier numbers are reused, so they only identify a process for the lifetime of that process. It is possible that the process identified by ParentProcessId has terminated, so ParentProcessId may not refer to an running process. It is also possible that ParentProcessId incorrectly refers to a process which re-used that process identifier. The CreationDate property can be used to determine whether the specified parent was created after this process was created.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public uint ParentProcessId {
            get {
                if ((curObj["ParentProcessId"] == null)) {
                    return System.Convert.ToUInt32(0);
                }
                return ((uint)(curObj["ParentProcessId"]));
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsPeakPageFileUsageNull {
            get {
                if ((curObj["PeakPageFileUsage"] == null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("The PeakPageFileUsage property indicates the maximum amount of page file space  u" +
            "sed during the life of the process.\nExample: 102367")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public uint PeakPageFileUsage {
            get {
                if ((curObj["PeakPageFileUsage"] == null)) {
                    return System.Convert.ToUInt32(0);
                }
                return ((uint)(curObj["PeakPageFileUsage"]));
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsPeakVirtualSizeNull {
            get {
                if ((curObj["PeakVirtualSize"] == null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description(@"The PeakVirtualSize property specifies the maximum virtual address space the process has used at any one time. Use of virtual address space does not necessarily imply corresponding use of either disk or main memory pages. However, virtual space is finite, and by using too much, the process might limit its ability to load libraries.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public ulong PeakVirtualSize {
            get {
                if ((curObj["PeakVirtualSize"] == null)) {
                    return System.Convert.ToUInt64(0);
                }
                return ((ulong)(curObj["PeakVirtualSize"]));
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsPeakWorkingSetSizeNull {
            get {
                if ((curObj["PeakWorkingSetSize"] == null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("The PeakWorkingSetSize property indicates the peak working set size of the proces" +
            "s.\nExample: 1413120")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public uint PeakWorkingSetSize {
            get {
                if ((curObj["PeakWorkingSetSize"] == null)) {
                    return System.Convert.ToUInt32(0);
                }
                return ((uint)(curObj["PeakWorkingSetSize"]));
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsPriorityNull {
            get {
                if ((curObj["Priority"] == null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("The Priority property indicates the scheduling priority of the process within the" +
            " operating system. The higher the value, the higher priority the process receive" +
            "s. Priority values can range from 0 (lowest priority) to 31 (highest priority).\n" +
            "Example: 7.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public uint Priority {
            get {
                if ((curObj["Priority"] == null)) {
                    return System.Convert.ToUInt32(0);
                }
                return ((uint)(curObj["Priority"]));
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsPrivatePageCountNull {
            get {
                if ((curObj["PrivatePageCount"] == null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("The PrivatePageCount property specifies the current number of pages allocated tha" +
            "t are accessible only to this process ")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public ulong PrivatePageCount {
            get {
                if ((curObj["PrivatePageCount"] == null)) {
                    return System.Convert.ToUInt64(0);
                }
                return ((ulong)(curObj["PrivatePageCount"]));
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsProcessIdNull {
            get {
                if ((curObj["ProcessId"] == null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("The ProcessId property contains the global process identifier that can be used to" +
            " identify a process. The value is valid from the creation of the process until t" +
            "he process is terminated.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public uint ProcessId {
            get {
                if ((curObj["ProcessId"] == null)) {
                    return System.Convert.ToUInt32(0);
                }
                return ((uint)(curObj["ProcessId"]));
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsQuotaNonPagedPoolUsageNull {
            get {
                if ((curObj["QuotaNonPagedPoolUsage"] == null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("The QuotaNonPagedPoolUsage property indicates the quota amount of non-paged pool " +
            "usage for the process.\nExample: 15")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public uint QuotaNonPagedPoolUsage {
            get {
                if ((curObj["QuotaNonPagedPoolUsage"] == null)) {
                    return System.Convert.ToUInt32(0);
                }
                return ((uint)(curObj["QuotaNonPagedPoolUsage"]));
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsQuotaPagedPoolUsageNull {
            get {
                if ((curObj["QuotaPagedPoolUsage"] == null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("The QuotaPagedPoolUsage property indicates the quota amount of paged pool usage f" +
            "or the process.\nExample: 22")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public uint QuotaPagedPoolUsage {
            get {
                if ((curObj["QuotaPagedPoolUsage"] == null)) {
                    return System.Convert.ToUInt32(0);
                }
                return ((uint)(curObj["QuotaPagedPoolUsage"]));
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsQuotaPeakNonPagedPoolUsageNull {
            get {
                if ((curObj["QuotaPeakNonPagedPoolUsage"] == null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("The QuotaPeakNonPagedPoolUsage property indicates the peak quota amount of non-pa" +
            "ged pool usage for the process.\nExample: 31")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public uint QuotaPeakNonPagedPoolUsage {
            get {
                if ((curObj["QuotaPeakNonPagedPoolUsage"] == null)) {
                    return System.Convert.ToUInt32(0);
                }
                return ((uint)(curObj["QuotaPeakNonPagedPoolUsage"]));
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsQuotaPeakPagedPoolUsageNull {
            get {
                if ((curObj["QuotaPeakPagedPoolUsage"] == null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("The QuotaPeakPagedPoolUsage property indicates the peak quota amount of paged poo" +
            "l usage for the process.\n Example: 31")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public uint QuotaPeakPagedPoolUsage {
            get {
                if ((curObj["QuotaPeakPagedPoolUsage"] == null)) {
                    return System.Convert.ToUInt32(0);
                }
                return ((uint)(curObj["QuotaPeakPagedPoolUsage"]));
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsReadOperationCountNull {
            get {
                if ((curObj["ReadOperationCount"] == null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("The ReadOperationCount property specifies the number of read operations performed" +
            ".")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public ulong ReadOperationCount {
            get {
                if ((curObj["ReadOperationCount"] == null)) {
                    return System.Convert.ToUInt64(0);
                }
                return ((ulong)(curObj["ReadOperationCount"]));
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsReadTransferCountNull {
            get {
                if ((curObj["ReadTransferCount"] == null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("The ReadTransferCount property specifies the amount of data read.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public ulong ReadTransferCount {
            get {
                if ((curObj["ReadTransferCount"] == null)) {
                    return System.Convert.ToUInt64(0);
                }
                return ((ulong)(curObj["ReadTransferCount"]));
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsSessionIdNull {
            get {
                if ((curObj["SessionId"] == null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("The SessionId property specifies the unique identifier that is generated by the o" +
            "perating system when the session is created. A session spans a period of time fr" +
            "om log in to log out on a particular system.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public uint SessionId {
            get {
                if ((curObj["SessionId"] == null)) {
                    return System.Convert.ToUInt32(0);
                }
                return ((uint)(curObj["SessionId"]));
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description(@"The Status property is a string indicating the current status of the object. Various operational and non-operational statuses can be defined. Operational statuses are ""OK"", ""Degraded"" and ""Pred Fail"". ""Pred Fail"" indicates that an element may be functioning properly but predicting a failure in the near future. An example is a SMART-enabled hard drive. Non-operational statuses can also be specified. These are ""Error"", ""Starting"", ""Stopping"" and ""Service"". The latter, ""Service"", could apply during mirror-resilvering of a disk, reload of a user permissions list, or other administrative work. Not all such work is on-line, yet the managed element is neither ""OK"" nor in one of the other states.")]
        public string Status {
            get {
                return ((string)(curObj["Status"]));
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsTerminationDateNull {
            get {
                if ((curObj["TerminationDate"] == null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Time that the process was stopped or terminated.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public System.DateTime TerminationDate {
            get {
                if ((curObj["TerminationDate"] != null)) {
                    return ToDateTime(((string)(curObj["TerminationDate"])));
                }
                else {
                    return System.DateTime.MinValue;
                }
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsThreadCountNull {
            get {
                if ((curObj["ThreadCount"] == null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description(@"The ThreadCount property specifies the number of active threads in this process. An instruction is the basic unit of execution in a processor, and a thread is the object that executes instructions. Every running process has at least one thread. This property is for computers running Windows NT only.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public uint ThreadCount {
            get {
                if ((curObj["ThreadCount"] == null)) {
                    return System.Convert.ToUInt32(0);
                }
                return ((uint)(curObj["ThreadCount"]));
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsUserModeTimeNull {
            get {
                if ((curObj["UserModeTime"] == null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("Time in user mode, in 100 nanoseconds. If this information is not available, a va" +
            "lue of 0 should be used.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public ulong UserModeTime {
            get {
                if ((curObj["UserModeTime"] == null)) {
                    return System.Convert.ToUInt64(0);
                }
                return ((ulong)(curObj["UserModeTime"]));
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsVirtualSizeNull {
            get {
                if ((curObj["VirtualSize"] == null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description(@"The VirtualSize property specifies the current size in bytes of the virtual address space the process is using. Use of virtual address space does not necessarily imply corresponding use of either disk or main memory pages. Virtual space is finite, and by using too much, the process can limit its ability to load libraries.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public ulong VirtualSize {
            get {
                if ((curObj["VirtualSize"] == null)) {
                    return System.Convert.ToUInt64(0);
                }
                return ((ulong)(curObj["VirtualSize"]));
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("The WindowsVersion property indicates the version of Windows in which the process" +
            " is running.\nExample: 4.0")]
        public string WindowsVersion {
            get {
                return ((string)(curObj["WindowsVersion"]));
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsWorkingSetSizeNull {
            get {
                if ((curObj["WorkingSetSize"] == null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description(@"The amount of memory in bytes that a process needs to execute efficiently, for an operating system that uses page-based memory management. If an insufficient amount of memory is available (< working set size), thrashing will occur. If this information is not known, NULL or 0 should be entered.  If this data is provided, it could be monitored to understand a process' changing memory requirements as execution proceeds.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public ulong WorkingSetSize {
            get {
                if ((curObj["WorkingSetSize"] == null)) {
                    return System.Convert.ToUInt64(0);
                }
                return ((ulong)(curObj["WorkingSetSize"]));
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsWriteOperationCountNull {
            get {
                if ((curObj["WriteOperationCount"] == null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("The WriteOperationCount property specifies the number of write operations perform" +
            "ed.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public ulong WriteOperationCount {
            get {
                if ((curObj["WriteOperationCount"] == null)) {
                    return System.Convert.ToUInt64(0);
                }
                return ((ulong)(curObj["WriteOperationCount"]));
            }
        }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public bool IsWriteTransferCountNull {
            get {
                if ((curObj["WriteTransferCount"] == null)) {
                    return true;
                }
                else {
                    return false;
                }
            }
        }
        
        [Browsable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Description("The WriteTransferCount property specifies the amount of data written.")]
        [TypeConverter(typeof(WMIValueTypeConverter))]
        public ulong WriteTransferCount {
            get {
                if ((curObj["WriteTransferCount"] == null)) {
                    return System.Convert.ToUInt64(0);
                }
                return ((ulong)(curObj["WriteTransferCount"]));
            }
        }
        
        private bool CheckIfProperClass(System.Management.ManagementScope mgmtScope, System.Management.ManagementPath path, System.Management.ObjectGetOptions OptionsParam) {
            if (((path != null) 
                        && (string.Compare(path.ClassName, this.ManagementClassName, true, System.Globalization.CultureInfo.InvariantCulture) == 0))) {
                return true;
            }
            else {
                return CheckIfProperClass(new System.Management.ManagementObject(mgmtScope, path, OptionsParam));
            }
        }
        
        private bool CheckIfProperClass(System.Management.ManagementBaseObject theObj) {
            if (((theObj != null) 
                        && (string.Compare(((string)(theObj["__CLASS"])), this.ManagementClassName, true, System.Globalization.CultureInfo.InvariantCulture) == 0))) {
                return true;
            }
            else {
                System.Array parentClasses = ((System.Array)(theObj["__DERIVATION"]));
                if ((parentClasses != null)) {
                    int count = 0;
                    for (count = 0; (count < parentClasses.Length); count = (count + 1)) {
                        if ((string.Compare(((string)(parentClasses.GetValue(count))), this.ManagementClassName, true, System.Globalization.CultureInfo.InvariantCulture) == 0)) {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        
        // Converts a given datetime in DMTF format to System.DateTime object.
        static System.DateTime ToDateTime(string dmtfDate) {
            System.DateTime initializer = System.DateTime.MinValue;
            int year = initializer.Year;
            int month = initializer.Month;
            int day = initializer.Day;
            int hour = initializer.Hour;
            int minute = initializer.Minute;
            int second = initializer.Second;
            long ticks = 0;
            string dmtf = dmtfDate;
            System.DateTime datetime = System.DateTime.MinValue;
            string tempString = string.Empty;
            if ((dmtf == null)) {
                throw new System.ArgumentOutOfRangeException();
            }
            if ((dmtf.Length == 0)) {
                throw new System.ArgumentOutOfRangeException();
            }
            if ((dmtf.Length != 25)) {
                throw new System.ArgumentOutOfRangeException();
            }
            try {
                tempString = dmtf.Substring(0, 4);
                if (("****" != tempString)) {
                    year = int.Parse(tempString);
                }
                tempString = dmtf.Substring(4, 2);
                if (("**" != tempString)) {
                    month = int.Parse(tempString);
                }
                tempString = dmtf.Substring(6, 2);
                if (("**" != tempString)) {
                    day = int.Parse(tempString);
                }
                tempString = dmtf.Substring(8, 2);
                if (("**" != tempString)) {
                    hour = int.Parse(tempString);
                }
                tempString = dmtf.Substring(10, 2);
                if (("**" != tempString)) {
                    minute = int.Parse(tempString);
                }
                tempString = dmtf.Substring(12, 2);
                if (("**" != tempString)) {
                    second = int.Parse(tempString);
                }
                tempString = dmtf.Substring(15, 6);
                if (("******" != tempString)) {
                    ticks = (long.Parse(tempString) * ((long)((System.TimeSpan.TicksPerMillisecond / 1000))));
                }
                if (((((((((year < 0) 
                            || (month < 0)) 
                            || (day < 0)) 
                            || (hour < 0)) 
                            || (minute < 0)) 
                            || (minute < 0)) 
                            || (second < 0)) 
                            || (ticks < 0))) {
                    throw new System.ArgumentOutOfRangeException();
                }
            }
            catch (System.Exception e) {
                throw new System.ArgumentOutOfRangeException(null, e.Message);
            }
            datetime = new System.DateTime(year, month, day, hour, minute, second, 0);
            datetime = datetime.AddTicks(ticks);
            System.TimeSpan tickOffset = System.TimeZone.CurrentTimeZone.GetUtcOffset(datetime);
            int UTCOffset = 0;
            int OffsetToBeAdjusted = 0;
            long OffsetMins = ((long)((tickOffset.Ticks / System.TimeSpan.TicksPerMinute)));
            tempString = dmtf.Substring(22, 3);
            if ((tempString != "******")) {
                tempString = dmtf.Substring(21, 4);
                try {
                    UTCOffset = int.Parse(tempString);
                }
                catch (System.Exception e) {
                    throw new System.ArgumentOutOfRangeException(null, e.Message);
                }
                OffsetToBeAdjusted = ((int)((OffsetMins - UTCOffset)));
                datetime = datetime.AddMinutes(((double)(OffsetToBeAdjusted)));
            }
            return datetime;
        }
        
        // Converts a given System.DateTime object to DMTF datetime format.
        static string ToDmtfDateTime(System.DateTime date) {
            string utcString = string.Empty;
            System.TimeSpan tickOffset = System.TimeZone.CurrentTimeZone.GetUtcOffset(date);
            long OffsetMins = ((long)((tickOffset.Ticks / System.TimeSpan.TicksPerMinute)));
            if ((System.Math.Abs(OffsetMins) > 999)) {
                date = date.ToUniversalTime();
                utcString = "+000";
            }
            else {
                if ((tickOffset.Ticks >= 0)) {
                    utcString = string.Concat("+", ((System.Int64 )((tickOffset.Ticks / System.TimeSpan.TicksPerMinute))).ToString().PadLeft(3, '0'));
                }
                else {
                    string strTemp = ((System.Int64 )(OffsetMins)).ToString();
                    utcString = string.Concat("-", strTemp.Substring(1, (strTemp.Length - 1)).PadLeft(3, '0'));
                }
            }
            string dmtfDateTime = ((System.Int32 )(date.Year)).ToString().PadLeft(4, '0');
            dmtfDateTime = string.Concat(dmtfDateTime, ((System.Int32 )(date.Month)).ToString().PadLeft(2, '0'));
            dmtfDateTime = string.Concat(dmtfDateTime, ((System.Int32 )(date.Day)).ToString().PadLeft(2, '0'));
            dmtfDateTime = string.Concat(dmtfDateTime, ((System.Int32 )(date.Hour)).ToString().PadLeft(2, '0'));
            dmtfDateTime = string.Concat(dmtfDateTime, ((System.Int32 )(date.Minute)).ToString().PadLeft(2, '0'));
            dmtfDateTime = string.Concat(dmtfDateTime, ((System.Int32 )(date.Second)).ToString().PadLeft(2, '0'));
            dmtfDateTime = string.Concat(dmtfDateTime, ".");
            System.DateTime dtTemp = new System.DateTime(date.Year, date.Month, date.Day, date.Hour, date.Minute, date.Second, 0);
            long microsec = ((long)((((date.Ticks - dtTemp.Ticks) 
                        * 1000) 
                        / System.TimeSpan.TicksPerMillisecond)));
            string strMicrosec = ((System.Int64 )(microsec)).ToString();
            if ((strMicrosec.Length > 6)) {
                strMicrosec = strMicrosec.Substring(0, 6);
            }
            dmtfDateTime = string.Concat(dmtfDateTime, strMicrosec.PadLeft(6, '0'));
            dmtfDateTime = string.Concat(dmtfDateTime, utcString);
            return dmtfDateTime;
        }
        
        private bool ShouldSerializeCreationDate() {
            if ((this.IsCreationDateNull == false)) {
                return true;
            }
            return false;
        }
        
        private bool ShouldSerializeExecutionState() {
            if ((this.IsExecutionStateNull == false)) {
                return true;
            }
            return false;
        }
        
        private bool ShouldSerializeHandleCount() {
            if ((this.IsHandleCountNull == false)) {
                return true;
            }
            return false;
        }
        
        private bool ShouldSerializeInstallDate() {
            if ((this.IsInstallDateNull == false)) {
                return true;
            }
            return false;
        }
        
        private bool ShouldSerializeKernelModeTime() {
            if ((this.IsKernelModeTimeNull == false)) {
                return true;
            }
            return false;
        }
        
        private bool ShouldSerializeMaximumWorkingSetSize() {
            if ((this.IsMaximumWorkingSetSizeNull == false)) {
                return true;
            }
            return false;
        }
        
        private bool ShouldSerializeMinimumWorkingSetSize() {
            if ((this.IsMinimumWorkingSetSizeNull == false)) {
                return true;
            }
            return false;
        }
        
        private bool ShouldSerializeOtherOperationCount() {
            if ((this.IsOtherOperationCountNull == false)) {
                return true;
            }
            return false;
        }
        
        private bool ShouldSerializeOtherTransferCount() {
            if ((this.IsOtherTransferCountNull == false)) {
                return true;
            }
            return false;
        }
        
        private bool ShouldSerializePageFaults() {
            if ((this.IsPageFaultsNull == false)) {
                return true;
            }
            return false;
        }
        
        private bool ShouldSerializePageFileUsage() {
            if ((this.IsPageFileUsageNull == false)) {
                return true;
            }
            return false;
        }
        
        private bool ShouldSerializeParentProcessId() {
            if ((this.IsParentProcessIdNull == false)) {
                return true;
            }
            return false;
        }
        
        private bool ShouldSerializePeakPageFileUsage() {
            if ((this.IsPeakPageFileUsageNull == false)) {
                return true;
            }
            return false;
        }
        
        private bool ShouldSerializePeakVirtualSize() {
            if ((this.IsPeakVirtualSizeNull == false)) {
                return true;
            }
            return false;
        }
        
        private bool ShouldSerializePeakWorkingSetSize() {
            if ((this.IsPeakWorkingSetSizeNull == false)) {
                return true;
            }
            return false;
        }
        
        private bool ShouldSerializePriority() {
            if ((this.IsPriorityNull == false)) {
                return true;
            }
            return false;
        }
        
        private bool ShouldSerializePrivatePageCount() {
            if ((this.IsPrivatePageCountNull == false)) {
                return true;
            }
            return false;
        }
        
        private bool ShouldSerializeProcessId() {
            if ((this.IsProcessIdNull == false)) {
                return true;
            }
            return false;
        }
        
        private bool ShouldSerializeQuotaNonPagedPoolUsage() {
            if ((this.IsQuotaNonPagedPoolUsageNull == false)) {
                return true;
            }
            return false;
        }
        
        private bool ShouldSerializeQuotaPagedPoolUsage() {
            if ((this.IsQuotaPagedPoolUsageNull == false)) {
                return true;
            }
            return false;
        }
        
        private bool ShouldSerializeQuotaPeakNonPagedPoolUsage() {
            if ((this.IsQuotaPeakNonPagedPoolUsageNull == false)) {
                return true;
            }
            return false;
        }
        
        private bool ShouldSerializeQuotaPeakPagedPoolUsage() {
            if ((this.IsQuotaPeakPagedPoolUsageNull == false)) {
                return true;
            }
            return false;
        }
        
        private bool ShouldSerializeReadOperationCount() {
            if ((this.IsReadOperationCountNull == false)) {
                return true;
            }
            return false;
        }
        
        private bool ShouldSerializeReadTransferCount() {
            if ((this.IsReadTransferCountNull == false)) {
                return true;
            }
            return false;
        }
        
        private bool ShouldSerializeSessionId() {
            if ((this.IsSessionIdNull == false)) {
                return true;
            }
            return false;
        }
        
        private bool ShouldSerializeTerminationDate() {
            if ((this.IsTerminationDateNull == false)) {
                return true;
            }
            return false;
        }
        
        private bool ShouldSerializeThreadCount() {
            if ((this.IsThreadCountNull == false)) {
                return true;
            }
            return false;
        }
        
        private bool ShouldSerializeUserModeTime() {
            if ((this.IsUserModeTimeNull == false)) {
                return true;
            }
            return false;
        }
        
        private bool ShouldSerializeVirtualSize() {
            if ((this.IsVirtualSizeNull == false)) {
                return true;
            }
            return false;
        }
        
        private bool ShouldSerializeWorkingSetSize() {
            if ((this.IsWorkingSetSizeNull == false)) {
                return true;
            }
            return false;
        }
        
        private bool ShouldSerializeWriteOperationCount() {
            if ((this.IsWriteOperationCountNull == false)) {
                return true;
            }
            return false;
        }
        
        private bool ShouldSerializeWriteTransferCount() {
            if ((this.IsWriteTransferCountNull == false)) {
                return true;
            }
            return false;
        }
        
        [Browsable(true)]
        public void CommitObject() {
            if ((isEmbedded == false)) {
                PrivateLateBoundObject.Put();
            }
        }
        
        [Browsable(true)]
        public void CommitObject(System.Management.PutOptions putOptions) {
            if ((isEmbedded == false)) {
                PrivateLateBoundObject.Put(putOptions);
            }
        }
        
        private void Initialize() {
            AutoCommitProp = true;
            isEmbedded = false;
        }
        
        private static string ConstructPath(string keyHandle) {
            string strPath = "root\\cimv2:Win32_Process";
            strPath = string.Concat(strPath, string.Concat(".Handle=", string.Concat("\"", string.Concat(keyHandle, "\""))));
            return strPath;
        }
        
        private void InitializeObject(System.Management.ManagementScope mgmtScope, System.Management.ManagementPath path, System.Management.ObjectGetOptions getOptions) {
            Initialize();
            if ((path != null)) {
                if ((CheckIfProperClass(mgmtScope, path, getOptions) != true)) {
                    throw new System.ArgumentException("Class name does not match.");
                }
            }
            PrivateLateBoundObject = new System.Management.ManagementObject(mgmtScope, path, getOptions);
            PrivateSystemProperties = new ManagementSystemProperties(PrivateLateBoundObject);
            curObj = PrivateLateBoundObject;
        }
        
        // Different overloads of GetInstances() help in enumerating instances of the WMI class.
        public static ProcessCollection GetInstances() {
            return GetInstances(null, null, null);
        }
        
        public static ProcessCollection GetInstances(string condition) {
            return GetInstances(null, condition, null);
        }
        
        public static ProcessCollection GetInstances(System.String [] selectedProperties) {
            return GetInstances(null, null, selectedProperties);
        }
        
        public static ProcessCollection GetInstances(string condition, System.String [] selectedProperties) {
            return GetInstances(null, condition, selectedProperties);
        }
        
        public static ProcessCollection GetInstances(System.Management.ManagementScope mgmtScope, System.Management.EnumerationOptions enumOptions) {
            if ((mgmtScope == null)) {
                if ((statMgmtScope == null)) {
                    mgmtScope = new System.Management.ManagementScope();
                    mgmtScope.Path.NamespacePath = "root\\cimv2";
                }
                else {
                    mgmtScope = statMgmtScope;
                }
            }
            System.Management.ManagementPath pathObj = new System.Management.ManagementPath();
            pathObj.ClassName = "Win32_Process";
            pathObj.NamespacePath = "root\\cimv2";
            System.Management.ManagementClass clsObject = new System.Management.ManagementClass(mgmtScope, pathObj, null);
            if ((enumOptions == null)) {
                enumOptions = new System.Management.EnumerationOptions();
                enumOptions.EnsureLocatable = true;
            }
            return new ProcessCollection(clsObject.GetInstances(enumOptions));
        }
        
        public static ProcessCollection GetInstances(System.Management.ManagementScope mgmtScope, string condition) {
            return GetInstances(mgmtScope, condition, null);
        }
        
        public static ProcessCollection GetInstances(System.Management.ManagementScope mgmtScope, System.String [] selectedProperties) {
            return GetInstances(mgmtScope, null, selectedProperties);
        }
        
        public static ProcessCollection GetInstances(System.Management.ManagementScope mgmtScope, string condition, System.String [] selectedProperties) {
            if ((mgmtScope == null)) {
                if ((statMgmtScope == null)) {
                    mgmtScope = new System.Management.ManagementScope();
                    mgmtScope.Path.NamespacePath = "root\\cimv2";
                }
                else {
                    mgmtScope = statMgmtScope;
                }
            }
            System.Management.ManagementObjectSearcher ObjectSearcher = new System.Management.ManagementObjectSearcher(mgmtScope, new SelectQuery("Win32_Process", condition, selectedProperties));
            System.Management.EnumerationOptions enumOptions = new System.Management.EnumerationOptions();
            enumOptions.EnsureLocatable = true;
            ObjectSearcher.Options = enumOptions;
            return new ProcessCollection(ObjectSearcher.Get());
        }
        
        [Browsable(true)]
        public static Process CreateInstance() {
            System.Management.ManagementScope mgmtScope = null;
            if ((statMgmtScope == null)) {
                mgmtScope = new System.Management.ManagementScope();
                mgmtScope.Path.NamespacePath = CreatedWmiNamespace;
            }
            else {
                mgmtScope = statMgmtScope;
            }
            System.Management.ManagementPath mgmtPath = new System.Management.ManagementPath(CreatedClassName);
            System.Management.ManagementClass tmpMgmtClass = new System.Management.ManagementClass(mgmtScope, mgmtPath, null);
            return new Process(tmpMgmtClass.CreateInstance());
        }
        
        [Browsable(true)]
        public void Delete() {
            PrivateLateBoundObject.Delete();
        }
        
        public uint AttachDebugger() {
            if ((isEmbedded == false)) {
                System.Management.ManagementBaseObject inParams = null;
                System.Management.ManagementBaseObject outParams = PrivateLateBoundObject.InvokeMethod("AttachDebugger", inParams, null);
                return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
            }
            else {
                return System.Convert.ToUInt32(0);
            }
        }
        
        public static uint Create(string CommandLine, string CurrentDirectory, System.Management.ManagementBaseObject ProcessStartupInformation, out uint ProcessId) {
            bool IsMethodStatic = true;
            if ((IsMethodStatic == true)) {
                System.Management.ManagementBaseObject inParams = null;
                System.Management.ManagementPath mgmtPath = new System.Management.ManagementPath(CreatedClassName);
                System.Management.ManagementClass classObj = new System.Management.ManagementClass(statMgmtScope, mgmtPath, null);
                bool EnablePrivileges = classObj.Scope.Options.EnablePrivileges;
                classObj.Scope.Options.EnablePrivileges = true;
                inParams = classObj.GetMethodParameters("Create");
                inParams["CommandLine"] = ((System.String )(CommandLine));
                inParams["CurrentDirectory"] = ((System.String )(CurrentDirectory));
                inParams["ProcessStartupInformation"] = ((System.Management.ManagementBaseObject )(ProcessStartupInformation));
                System.Management.ManagementBaseObject outParams = classObj.InvokeMethod("Create", inParams, null);
                ProcessId = System.Convert.ToUInt32(outParams.Properties["ProcessId"].Value);
                classObj.Scope.Options.EnablePrivileges = EnablePrivileges;
                return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
            }
            else {
                ProcessId = System.Convert.ToUInt32(0);
                return System.Convert.ToUInt32(0);
            }
        }
        
        public uint GetOwner(out string Domain, out string User) {
            if ((isEmbedded == false)) {
                System.Management.ManagementBaseObject inParams = null;
                bool EnablePrivileges = PrivateLateBoundObject.Scope.Options.EnablePrivileges;
                PrivateLateBoundObject.Scope.Options.EnablePrivileges = true;
                System.Management.ManagementBaseObject outParams = PrivateLateBoundObject.InvokeMethod("GetOwner", inParams, null);
                Domain = System.Convert.ToString(outParams.Properties["Domain"].Value);
                User = System.Convert.ToString(outParams.Properties["User"].Value);
                PrivateLateBoundObject.Scope.Options.EnablePrivileges = EnablePrivileges;
                return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
            }
            else {
                Domain = null;
                User = null;
                return System.Convert.ToUInt32(0);
            }
        }
        
        public uint GetOwnerSid(out string Sid) {
            if ((isEmbedded == false)) {
                System.Management.ManagementBaseObject inParams = null;
                bool EnablePrivileges = PrivateLateBoundObject.Scope.Options.EnablePrivileges;
                PrivateLateBoundObject.Scope.Options.EnablePrivileges = true;
                System.Management.ManagementBaseObject outParams = PrivateLateBoundObject.InvokeMethod("GetOwnerSid", inParams, null);
                Sid = System.Convert.ToString(outParams.Properties["Sid"].Value);
                PrivateLateBoundObject.Scope.Options.EnablePrivileges = EnablePrivileges;
                return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
            }
            else {
                Sid = null;
                return System.Convert.ToUInt32(0);
            }
        }
        
        public uint SetPriority(int Priority) {
            if ((isEmbedded == false)) {
                System.Management.ManagementBaseObject inParams = null;
                bool EnablePrivileges = PrivateLateBoundObject.Scope.Options.EnablePrivileges;
                PrivateLateBoundObject.Scope.Options.EnablePrivileges = true;
                inParams = PrivateLateBoundObject.GetMethodParameters("SetPriority");
                inParams["Priority"] = ((System.Int32 )(Priority));
                System.Management.ManagementBaseObject outParams = PrivateLateBoundObject.InvokeMethod("SetPriority", inParams, null);
                PrivateLateBoundObject.Scope.Options.EnablePrivileges = EnablePrivileges;
                return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
            }
            else {
                return System.Convert.ToUInt32(0);
            }
        }
        
        public uint Terminate(uint Reason) {
            if ((isEmbedded == false)) {
                System.Management.ManagementBaseObject inParams = null;
                bool EnablePrivileges = PrivateLateBoundObject.Scope.Options.EnablePrivileges;
                PrivateLateBoundObject.Scope.Options.EnablePrivileges = true;
                inParams = PrivateLateBoundObject.GetMethodParameters("Terminate");
                inParams["Reason"] = ((System.UInt32 )(Reason));
                System.Management.ManagementBaseObject outParams = PrivateLateBoundObject.InvokeMethod("Terminate", inParams, null);
                PrivateLateBoundObject.Scope.Options.EnablePrivileges = EnablePrivileges;
                return System.Convert.ToUInt32(outParams.Properties["ReturnValue"].Value);
            }
            else {
                return System.Convert.ToUInt32(0);
            }
        }
        
        public enum ExecutionStateValues {
            
            Unknown0 = 0,
            
            Other0 = 1,
            
            Ready = 2,
            
            Running = 3,
            
            Blocked = 4,
            
            Suspended_Blocked = 5,
            
            Suspended_Ready = 6,
            
            Terminated = 7,
            
            Stopped = 8,
            
            Growing = 9,
            
            NULL_ENUM_VALUE = 10,
        }
        
        // Enumerator implementation for enumerating instances of the class.
        public class ProcessCollection : object, ICollection {
            
            private ManagementObjectCollection privColObj;
            
            public ProcessCollection(ManagementObjectCollection objCollection) {
                privColObj = objCollection;
            }
            
            public virtual int Count {
                get {
                    return privColObj.Count;
                }
            }
            
            public virtual bool IsSynchronized {
                get {
                    return privColObj.IsSynchronized;
                }
            }
            
            public virtual object SyncRoot {
                get {
                    return this;
                }
            }
            
            public virtual void CopyTo(System.Array array, int index) {
                privColObj.CopyTo(array, index);
                int nCtr;
                for (nCtr = 0; (nCtr < array.Length); nCtr = (nCtr + 1)) {
                    array.SetValue(new Process(((System.Management.ManagementObject)(array.GetValue(nCtr)))), nCtr);
                }
            }
            
            public virtual System.Collections.IEnumerator GetEnumerator() {
                return new ProcessEnumerator(privColObj.GetEnumerator());
            }
            
            public class ProcessEnumerator : object, System.Collections.IEnumerator {
                
                private ManagementObjectCollection.ManagementObjectEnumerator privObjEnum;
                
                public ProcessEnumerator(ManagementObjectCollection.ManagementObjectEnumerator objEnum) {
                    privObjEnum = objEnum;
                }
                
                public virtual object Current {
                    get {
                        return new Process(((System.Management.ManagementObject)(privObjEnum.Current)));
                    }
                }
                
                public virtual bool MoveNext() {
                    return privObjEnum.MoveNext();
                }
                
                public virtual void Reset() {
                    privObjEnum.Reset();
                }
            }
        }
        
        // TypeConverter to handle null values for ValueType properties
        public class WMIValueTypeConverter : TypeConverter {
            
            private TypeConverter baseConverter;
            
            private System.Type baseType;
            
            public WMIValueTypeConverter(System.Type inBaseType) {
                baseConverter = TypeDescriptor.GetConverter(inBaseType);
                baseType = inBaseType;
            }
            
            public override bool CanConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Type srcType) {
                return baseConverter.CanConvertFrom(context, srcType);
            }
            
            public override bool CanConvertTo(System.ComponentModel.ITypeDescriptorContext context, System.Type destinationType) {
                return baseConverter.CanConvertTo(context, destinationType);
            }
            
            public override object ConvertFrom(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value) {
                return baseConverter.ConvertFrom(context, culture, value);
            }
            
            public override object CreateInstance(System.ComponentModel.ITypeDescriptorContext context, System.Collections.IDictionary dictionary) {
                return baseConverter.CreateInstance(context, dictionary);
            }
            
            public override bool GetCreateInstanceSupported(System.ComponentModel.ITypeDescriptorContext context) {
                return baseConverter.GetCreateInstanceSupported(context);
            }
            
            public override PropertyDescriptorCollection GetProperties(System.ComponentModel.ITypeDescriptorContext context, object value, System.Attribute[] attributeVar) {
                return baseConverter.GetProperties(context, value, attributeVar);
            }
            
            public override bool GetPropertiesSupported(System.ComponentModel.ITypeDescriptorContext context) {
                return baseConverter.GetPropertiesSupported(context);
            }
            
            public override System.ComponentModel.TypeConverter.StandardValuesCollection GetStandardValues(System.ComponentModel.ITypeDescriptorContext context) {
                return baseConverter.GetStandardValues(context);
            }
            
            public override bool GetStandardValuesExclusive(System.ComponentModel.ITypeDescriptorContext context) {
                return baseConverter.GetStandardValuesExclusive(context);
            }
            
            public override bool GetStandardValuesSupported(System.ComponentModel.ITypeDescriptorContext context) {
                return baseConverter.GetStandardValuesSupported(context);
            }
            
            public override object ConvertTo(System.ComponentModel.ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, System.Type destinationType) {
                if ((baseType.BaseType == typeof(System.Enum))) {
                    if ((value.GetType() == destinationType)) {
                        return value;
                    }
                    if ((((value == null) 
                                && (context != null)) 
                                && (context.PropertyDescriptor.ShouldSerializeValue(context.Instance) == false))) {
                        return  "NULL_ENUM_VALUE" ;
                    }
                    return baseConverter.ConvertTo(context, culture, value, destinationType);
                }
                if (((baseType == typeof(bool)) 
                            && (baseType.BaseType == typeof(System.ValueType)))) {
                    if ((((value == null) 
                                && (context != null)) 
                                && (context.PropertyDescriptor.ShouldSerializeValue(context.Instance) == false))) {
                        return "";
                    }
                    return baseConverter.ConvertTo(context, culture, value, destinationType);
                }
                if (((context != null) 
                            && (context.PropertyDescriptor.ShouldSerializeValue(context.Instance) == false))) {
                    return "";
                }
                return baseConverter.ConvertTo(context, culture, value, destinationType);
            }
        }
        
        // Embedded class to represent WMI system Properties.
        [TypeConverter(typeof(System.ComponentModel.ExpandableObjectConverter))]
        public class ManagementSystemProperties {
            
            private System.Management.ManagementBaseObject PrivateLateBoundObject;
            
            public ManagementSystemProperties(System.Management.ManagementBaseObject ManagedObject) {
                PrivateLateBoundObject = ManagedObject;
            }
            
            [Browsable(true)]
            public int GENUS {
                get {
                    return ((int)(PrivateLateBoundObject["__GENUS"]));
                }
            }
            
            [Browsable(true)]
            public string CLASS {
                get {
                    return ((string)(PrivateLateBoundObject["__CLASS"]));
                }
            }
            
            [Browsable(true)]
            public string SUPERCLASS {
                get {
                    return ((string)(PrivateLateBoundObject["__SUPERCLASS"]));
                }
            }
            
            [Browsable(true)]
            public string DYNASTY {
                get {
                    return ((string)(PrivateLateBoundObject["__DYNASTY"]));
                }
            }
            
            [Browsable(true)]
            public string RELPATH {
                get {
                    return ((string)(PrivateLateBoundObject["__RELPATH"]));
                }
            }
            
            [Browsable(true)]
            public int PROPERTY_COUNT {
                get {
                    return ((int)(PrivateLateBoundObject["__PROPERTY_COUNT"]));
                }
            }
            
            [Browsable(true)]
            public string[] DERIVATION {
                get {
                    return ((string[])(PrivateLateBoundObject["__DERIVATION"]));
                }
            }
            
            [Browsable(true)]
            public string SERVER {
                get {
                    return ((string)(PrivateLateBoundObject["__SERVER"]));
                }
            }
            
            [Browsable(true)]
            public string NAMESPACE {
                get {
                    return ((string)(PrivateLateBoundObject["__NAMESPACE"]));
                }
            }
            
            [Browsable(true)]
            public string PATH {
                get {
                    return ((string)(PrivateLateBoundObject["__PATH"]));
                }
            }
        }
    }
}
