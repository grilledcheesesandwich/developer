﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18033
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by Microsoft.VSDesigner, Version 4.0.30319.18033.
// 
#pragma warning disable 1591

namespace ServiceTest.MultiService {
    using System;
    using System.Web.Services;
    using System.Diagnostics;
    using System.Web.Services.Protocols;
    using System.Xml.Serialization;
    using System.ComponentModel;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.17929")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name="QuerySoap", Namespace="http://tempuri.org/")]
    public partial class Query : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        private System.Threading.SendOrPostCallback GetJobsForDpkOperationCompleted;
        
        private System.Threading.SendOrPostCallback GetJobsForChangelistOperationCompleted;
        
        private System.Threading.SendOrPostCallback GetJobsFromWtqXmlOperationCompleted;
        
        private System.Threading.SendOrPostCallback GetJobsFromWtqOperationCompleted;
        
        private System.Threading.SendOrPostCallback GetJobNamesOperationCompleted;
        
        private System.Threading.SendOrPostCallback GetDpkNameFromDpkBytesOperationCompleted;
        
        private System.Threading.SendOrPostCallback GetDpkNameOperationCompleted;
        
        private bool useDefaultCredentialsSetExplicitly;
        
        /// <remarks/>
        public Query() {
            this.Url = global::ServiceTest.Properties.Settings.Default.ServiceTest_MultiService_Query;
            if ((this.IsLocalFileSystemWebService(this.Url) == true)) {
                this.UseDefaultCredentials = true;
                this.useDefaultCredentialsSetExplicitly = false;
            }
            else {
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        public new string Url {
            get {
                return base.Url;
            }
            set {
                if ((((this.IsLocalFileSystemWebService(base.Url) == true) 
                            && (this.useDefaultCredentialsSetExplicitly == false)) 
                            && (this.IsLocalFileSystemWebService(value) == false))) {
                    base.UseDefaultCredentials = false;
                }
                base.Url = value;
            }
        }
        
        public new bool UseDefaultCredentials {
            get {
                return base.UseDefaultCredentials;
            }
            set {
                base.UseDefaultCredentials = value;
                this.useDefaultCredentialsSetExplicitly = true;
            }
        }
        
        /// <remarks/>
        public event GetJobsForDpkCompletedEventHandler GetJobsForDpkCompleted;
        
        /// <remarks/>
        public event GetJobsForChangelistCompletedEventHandler GetJobsForChangelistCompleted;
        
        /// <remarks/>
        public event GetJobsFromWtqXmlCompletedEventHandler GetJobsFromWtqXmlCompleted;
        
        /// <remarks/>
        public event GetJobsFromWtqCompletedEventHandler GetJobsFromWtqCompleted;
        
        /// <remarks/>
        public event GetJobNamesCompletedEventHandler GetJobNamesCompleted;
        
        /// <remarks/>
        public event GetDpkNameFromDpkBytesCompletedEventHandler GetDpkNameFromDpkBytesCompleted;
        
        /// <remarks/>
        public event GetDpkNameCompletedEventHandler GetDpkNameCompleted;
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetJobsForDpk", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string[] GetJobsForDpk(string dpkPath) {
            object[] results = this.Invoke("GetJobsForDpk", new object[] {
                        dpkPath});
            return ((string[])(results[0]));
        }
        
        /// <remarks/>
        public void GetJobsForDpkAsync(string dpkPath) {
            this.GetJobsForDpkAsync(dpkPath, null);
        }
        
        /// <remarks/>
        public void GetJobsForDpkAsync(string dpkPath, object userState) {
            if ((this.GetJobsForDpkOperationCompleted == null)) {
                this.GetJobsForDpkOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetJobsForDpkOperationCompleted);
            }
            this.InvokeAsync("GetJobsForDpk", new object[] {
                        dpkPath}, this.GetJobsForDpkOperationCompleted, userState);
        }
        
        private void OnGetJobsForDpkOperationCompleted(object arg) {
            if ((this.GetJobsForDpkCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetJobsForDpkCompleted(this, new GetJobsForDpkCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetJobsForChangelist", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string[] GetJobsForChangelist(int changelist, string depot) {
            object[] results = this.Invoke("GetJobsForChangelist", new object[] {
                        changelist,
                        depot});
            return ((string[])(results[0]));
        }
        
        /// <remarks/>
        public void GetJobsForChangelistAsync(int changelist, string depot) {
            this.GetJobsForChangelistAsync(changelist, depot, null);
        }
        
        /// <remarks/>
        public void GetJobsForChangelistAsync(int changelist, string depot, object userState) {
            if ((this.GetJobsForChangelistOperationCompleted == null)) {
                this.GetJobsForChangelistOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetJobsForChangelistOperationCompleted);
            }
            this.InvokeAsync("GetJobsForChangelist", new object[] {
                        changelist,
                        depot}, this.GetJobsForChangelistOperationCompleted, userState);
        }
        
        private void OnGetJobsForChangelistOperationCompleted(object arg) {
            if ((this.GetJobsForChangelistCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetJobsForChangelistCompleted(this, new GetJobsForChangelistCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetJobsFromWtqXml", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string[] GetJobsFromWtqXml(string wtqXml) {
            object[] results = this.Invoke("GetJobsFromWtqXml", new object[] {
                        wtqXml});
            return ((string[])(results[0]));
        }
        
        /// <remarks/>
        public void GetJobsFromWtqXmlAsync(string wtqXml) {
            this.GetJobsFromWtqXmlAsync(wtqXml, null);
        }
        
        /// <remarks/>
        public void GetJobsFromWtqXmlAsync(string wtqXml, object userState) {
            if ((this.GetJobsFromWtqXmlOperationCompleted == null)) {
                this.GetJobsFromWtqXmlOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetJobsFromWtqXmlOperationCompleted);
            }
            this.InvokeAsync("GetJobsFromWtqXml", new object[] {
                        wtqXml}, this.GetJobsFromWtqXmlOperationCompleted, userState);
        }
        
        private void OnGetJobsFromWtqXmlOperationCompleted(object arg) {
            if ((this.GetJobsFromWtqXmlCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetJobsFromWtqXmlCompleted(this, new GetJobsFromWtqXmlCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetJobsFromWtq", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string[] GetJobsFromWtq(string wtqPath) {
            object[] results = this.Invoke("GetJobsFromWtq", new object[] {
                        wtqPath});
            return ((string[])(results[0]));
        }
        
        /// <remarks/>
        public void GetJobsFromWtqAsync(string wtqPath) {
            this.GetJobsFromWtqAsync(wtqPath, null);
        }
        
        /// <remarks/>
        public void GetJobsFromWtqAsync(string wtqPath, object userState) {
            if ((this.GetJobsFromWtqOperationCompleted == null)) {
                this.GetJobsFromWtqOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetJobsFromWtqOperationCompleted);
            }
            this.InvokeAsync("GetJobsFromWtq", new object[] {
                        wtqPath}, this.GetJobsFromWtqOperationCompleted, userState);
        }
        
        private void OnGetJobsFromWtqOperationCompleted(object arg) {
            if ((this.GetJobsFromWtqCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetJobsFromWtqCompleted(this, new GetJobsFromWtqCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetJobNames", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string[] GetJobNames(string[] jobIDs) {
            object[] results = this.Invoke("GetJobNames", new object[] {
                        jobIDs});
            return ((string[])(results[0]));
        }
        
        /// <remarks/>
        public void GetJobNamesAsync(string[] jobIDs) {
            this.GetJobNamesAsync(jobIDs, null);
        }
        
        /// <remarks/>
        public void GetJobNamesAsync(string[] jobIDs, object userState) {
            if ((this.GetJobNamesOperationCompleted == null)) {
                this.GetJobNamesOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetJobNamesOperationCompleted);
            }
            this.InvokeAsync("GetJobNames", new object[] {
                        jobIDs}, this.GetJobNamesOperationCompleted, userState);
        }
        
        private void OnGetJobNamesOperationCompleted(object arg) {
            if ((this.GetJobNamesCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetJobNamesCompleted(this, new GetJobNamesCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetDpkNameFromDpkBytes", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string GetDpkNameFromDpkBytes([System.Xml.Serialization.XmlElementAttribute(DataType="base64Binary")] byte[] dpkBytes) {
            object[] results = this.Invoke("GetDpkNameFromDpkBytes", new object[] {
                        dpkBytes});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void GetDpkNameFromDpkBytesAsync(byte[] dpkBytes) {
            this.GetDpkNameFromDpkBytesAsync(dpkBytes, null);
        }
        
        /// <remarks/>
        public void GetDpkNameFromDpkBytesAsync(byte[] dpkBytes, object userState) {
            if ((this.GetDpkNameFromDpkBytesOperationCompleted == null)) {
                this.GetDpkNameFromDpkBytesOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetDpkNameFromDpkBytesOperationCompleted);
            }
            this.InvokeAsync("GetDpkNameFromDpkBytes", new object[] {
                        dpkBytes}, this.GetDpkNameFromDpkBytesOperationCompleted, userState);
        }
        
        private void OnGetDpkNameFromDpkBytesOperationCompleted(object arg) {
            if ((this.GetDpkNameFromDpkBytesCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetDpkNameFromDpkBytesCompleted(this, new GetDpkNameFromDpkBytesCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/GetDpkName", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        public string GetDpkName(string dpkPath) {
            object[] results = this.Invoke("GetDpkName", new object[] {
                        dpkPath});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void GetDpkNameAsync(string dpkPath) {
            this.GetDpkNameAsync(dpkPath, null);
        }
        
        /// <remarks/>
        public void GetDpkNameAsync(string dpkPath, object userState) {
            if ((this.GetDpkNameOperationCompleted == null)) {
                this.GetDpkNameOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetDpkNameOperationCompleted);
            }
            this.InvokeAsync("GetDpkName", new object[] {
                        dpkPath}, this.GetDpkNameOperationCompleted, userState);
        }
        
        private void OnGetDpkNameOperationCompleted(object arg) {
            if ((this.GetDpkNameCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetDpkNameCompleted(this, new GetDpkNameCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        public new void CancelAsync(object userState) {
            base.CancelAsync(userState);
        }
        
        private bool IsLocalFileSystemWebService(string url) {
            if (((url == null) 
                        || (url == string.Empty))) {
                return false;
            }
            System.Uri wsUri = new System.Uri(url);
            if (((wsUri.Port >= 1024) 
                        && (string.Compare(wsUri.Host, "localHost", System.StringComparison.OrdinalIgnoreCase) == 0))) {
                return true;
            }
            return false;
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.17929")]
    public delegate void GetJobsForDpkCompletedEventHandler(object sender, GetJobsForDpkCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.17929")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetJobsForDpkCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal GetJobsForDpkCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string[] Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string[])(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.17929")]
    public delegate void GetJobsForChangelistCompletedEventHandler(object sender, GetJobsForChangelistCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.17929")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetJobsForChangelistCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal GetJobsForChangelistCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string[] Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string[])(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.17929")]
    public delegate void GetJobsFromWtqXmlCompletedEventHandler(object sender, GetJobsFromWtqXmlCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.17929")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetJobsFromWtqXmlCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal GetJobsFromWtqXmlCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string[] Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string[])(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.17929")]
    public delegate void GetJobsFromWtqCompletedEventHandler(object sender, GetJobsFromWtqCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.17929")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetJobsFromWtqCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal GetJobsFromWtqCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string[] Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string[])(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.17929")]
    public delegate void GetJobNamesCompletedEventHandler(object sender, GetJobNamesCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.17929")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetJobNamesCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal GetJobNamesCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string[] Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string[])(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.17929")]
    public delegate void GetDpkNameFromDpkBytesCompletedEventHandler(object sender, GetDpkNameFromDpkBytesCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.17929")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetDpkNameFromDpkBytesCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal GetDpkNameFromDpkBytesCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.17929")]
    public delegate void GetDpkNameCompletedEventHandler(object sender, GetDpkNameCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Web.Services", "4.0.30319.17929")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetDpkNameCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal GetDpkNameCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
}

#pragma warning restore 1591